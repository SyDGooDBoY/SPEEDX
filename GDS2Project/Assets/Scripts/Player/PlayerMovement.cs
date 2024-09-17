using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Speed")]
    public float moveSpeed = 10f; // Temporary speed variable

    // public float gravity = -5f;
    public float swingSpeed;
    public float walkSpeed = 10f; // Walking speed
    public float runSpeed = 20f; // Running speed
    public float wallRunSpeed = 10f; // Wall run speed
    public float climbSpeed = 5f; // Climbing speed
    public float dashSpeed; // Dash speed

    [Header("Grappling sens")]
    public float grapXZvalue = 2f; // Grappling XZ

    [Header("Friction Settings")]
    public float groundDrag = 5f; // Ground friction

    [Header("Jump Settings")]
    public float jumpForce = 12f; // Jump force

    public float downForce = 5f; // Downward force
    public float coyoteTime = 0.2f; // Coyote time duration in seconds
    private float coyoteTimeCounter;

    public float jumpCooldown = 0.25f; // Jump cooldown

    public float airDrag = 0.3f; // Air resistance

    bool readyToJump;

    [Header("Crouch")]
    [Tooltip("Speed while crouching")]
    public float crouchMoveSpeed = 5f; // Crouch movement speed

    public float crouchSpeed = 5f; // Crouch speed
    public float crouchYscale = 0.5f; // Crouch height scale
    private float startYscale; // Initial height scale


    [Header("Control Keys")]
    public KeyCode jumpKey = KeyCode.Space;

    public KeyCode runKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;


    [Header("Ground Check")]
    // Grounded state
    public bool isGrounded;

    public float playerHeight; // Player height

    public LayerMask groundMask; // Ground mask

    [Header("Slope Movement")]
    public float maxSlopeAngle = 40f; // Maximum slope angle

    private RaycastHit slopeHit; // Slope hit detection
    private bool exitingSlope; // Exiting slope state

    [Header("Orientation Reference")]
    [Tooltip("Orientation transform used to determine movement direction")]
    public Transform orientation; // Orientation transform used to determine movement direction

    public PlayerClimb pc;

    // Movement vectors
    float horizontalMovement;
    float verticalMovement;
    Vector3 moveDirection;
    Rigidbody rb;

    [Header("Camera Effects")]
    public PlayerCam cam;

    public float grappleFOV = 120f;
    private float camFov;

    [Header("Movement State")]
    public MoveState state;

    public bool wallRunning;
    public bool climbing;
    public bool freeze;
    public bool unlimited;
    public bool restricted;
    public bool activeGrapple;
    public bool swinging;
    public bool dashing;

    [Header("Energy Recovery")]
    public float wallSlideRecoveryRate = 20f; // recovery rate through wall sliding

    private EnergySystem energySystem;
    private AbilityManager abilityManager;

    [Header("Player Input control")]
    public bool inputEnabled = true;

    // Movement states
    public enum MoveState
    {
        dashing,
        swinging,
        freeze,
        grappling,
        unlimited,
        Walking,
        Running,
        wallRunning,
        climbing,
        crouching,
        Jumping
    }

    // State handling
    private void StateHandle()
    {
        // Freeze state
        if (freeze)
        {
            state = MoveState.freeze;
            moveSpeed = 0;
            rb.velocity = Vector3.zero;
        }

        else if (dashing)
        {
            state = MoveState.dashing;
            moveSpeed = dashSpeed;
        }

        else if (swinging)
        {
            state = MoveState.swinging;
            moveSpeed = swingSpeed;
        }

        else if (activeGrapple)
        {
            state = MoveState.grappling;
            moveSpeed = runSpeed;
        }

        // Unlimited speed state
        else if (unlimited)
        {
            state = MoveState.unlimited;
            moveSpeed = 999f;
            return;
        }

        // Climbing state
        else if (climbing)
        {
            state = MoveState.climbing;
            moveSpeed = climbSpeed * abilityManager.GetAbilityMultiplier();
        }

        // Wall running state
        else if (wallRunning)
        {
            state = MoveState.wallRunning;
            moveSpeed = wallRunSpeed * abilityManager.GetAbilityMultiplier();
            energySystem.RecoverEnergyThroughSpecialAction(wallSlideRecoveryRate);
        }

        // Crouching state
        else if (Input.GetKey(crouchKey))
        {
            state = MoveState.crouching;
            moveSpeed = crouchMoveSpeed * abilityManager.GetAbilityMultiplier();
        }

// Running state
        else if (isGrounded && Input.GetKey(runKey))
        {
            state = MoveState.Running;
            moveSpeed = runSpeed * abilityManager.GetAbilityMultiplier();
        }
// Walking state
        else if (isGrounded)
        {
            state = MoveState.Walking;
            moveSpeed = walkSpeed * abilityManager.GetAbilityMultiplier();
        }
// Jumping state
        else
        {
            state = MoveState.Jumping;
        }
    }

// Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        readyToJump = true;
        startYscale = transform.localScale.y;
        camFov = cam.GetComponent<Camera>().fieldOfView;
        cam.DoFov(camFov);

        energySystem = GetComponent<EnergySystem>();
        abilityManager = GetComponent<AbilityManager>();
    }

// Update is called once per frame
    void Update()
    {
        if (!inputEnabled) return;
        // Ground check
        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, groundMask);
        coyoteTimeCounter = isGrounded ? coyoteTime : coyoteTimeCounter - Time.deltaTime;
        PlayerInput();
        SpeedControl();
        StateHandle();

        // Ground friction handling
        if (isGrounded && !activeGrapple)
        {
            rb.drag = groundDrag;
        }
        else
        {
            rb.drag = 0;
        }

        // Energy is restored only while maintaining a special movement state
        if (!isMoving())
        {
            energySystem.StopRecovery(); // quickly decreasing
        }
        else
        {
            energySystem.StartRecovery(); // recover 
        }
    }

    private void FixedUpdate()
    {
        PlayerMove();
        ApplyDownForce();
        if (transform.parent != null && transform.parent.CompareTag("MovingPlatform"))
        {
            Vector3 platformVelocity = transform.parent.GetComponent<Environment>().GetVelocity();
            rb.MovePosition(rb.position + platformVelocity * Time.fixedDeltaTime);
        }
    }

// Player input handling
    private void PlayerInput()
    {
        horizontalMovement = Input.GetAxisRaw("Horizontal");
        verticalMovement = Input.GetAxisRaw("Vertical");

        // Jumping
        if (Input.GetKeyDown(jumpKey) && readyToJump && coyoteTimeCounter > 0)
        {
            //if (energySystem.UseEnergy(jumpConsumption)) // Check and consume the energy required to jump
            //{
            //    readyToJump = false;
            //    Jump();
            //    Invoke(nameof(ResetJump), jumpCooldown);
            //}
            //else
            //{
            //    Debug.Log("Not enough energy to jump!");
            //}
            readyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }

        // Crouching
        if (Input.GetKeyDown(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, crouchYscale, transform.localScale.z);
            rb.AddForce(Vector3.down * crouchSpeed, ForceMode.Impulse);
        }
        // Stand up
        else if (Input.GetKeyUp(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, startYscale, transform.localScale.z);
        }
    }


// Player movement
    private void PlayerMove()
    {
        if (state == MoveState.dashing) return;

        // Skip movement if grappling
        if (activeGrapple)
        {
            return;
        }

        // Skip movement if restricted
        if (restricted)
        {
            return;
        }

        // Skip movement if exiting a wall
        if (pc.exitingWall)
        {
            return;
        }

        // Calculate movement direction
        moveDirection = orientation.forward * verticalMovement + orientation.right * horizontalMovement;
        // Slope handling
        if (OnSlope() && !exitingSlope)
        {
            rb.AddForce(GetSlopeMoveDirection() * moveSpeed * 20f, ForceMode.Force);

            if (rb.velocity.y > 0)
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
        }

        // Ground movement
        else if (isGrounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        }

        // Air movement
        else if (!isGrounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airDrag, ForceMode.Force);
        }

        if (!wallRunning)
        {
            rb.useGravity = !OnSlope(); // Disable gravity on slopes
        }

        //// Consume energy over time while moving
        //if (moveDirection.magnitude > 0)
        //{
        //    energySystem.ConsumeEnergyOverTime(moveConsumptionRate);
        //}
    }

// Control movement speed under different conditions
    private void SpeedControl()
    {
        // Skip speed control if grappling
        if (activeGrapple)
        {
            return;
        }

        // Slope movement speed control
        if (OnSlope() && !exitingSlope)
        {
            if (rb.velocity.magnitude > moveSpeed)
                rb.velocity = rb.velocity.normalized * moveSpeed;
        }
        // Flat surface speed control
        else
        {
            Vector3 flatVelocity = new Vector3(rb.velocity.x, 0, rb.velocity.z); // Flat velocity
            // Control movement speed
            if (flatVelocity.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVelocity.normalized * moveSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }
        }
    }

// Jumping
    private void Jump()
    {
        exitingSlope = true;
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce,
            ForceMode.Impulse); // Perform a vertical impulse jump
    }

// Apply downward force
    private void ApplyDownForce()
    {
        if (!isGrounded)
        {
            rb.AddForce(Vector3.down * downForce, ForceMode.Force);
        }
    }

// Reset jumping state
    private void ResetJump()
    {
        exitingSlope = false;
        readyToJump = true;
    }

// Check if on a slope
    private bool OnSlope()
    {
        if (!isGrounded) return false;
        // Cast a ray downwards to detect the ground

        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            // Calculate the angle between the ground normal and the up vector
            float slopeAngle = Vector3.Angle(slopeHit.normal, Vector3.up);

            // Only treat it as a slope if the angle is greater than 0 (not flat ground) and less than the max slope angle
            if (slopeAngle > 0 && slopeAngle < maxSlopeAngle)
            {
                Debug.Log("Slope detected" + slopeAngle);
                return true; // It's a slope
            }
        }

        // Return false if no slope is detected or it's flat ground
        return false;
    }


// Get slope movement direction
    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
    }

// Calculate jump velocity for a given trajectory
    public Vector3 CalculateJumpVelocity(Vector3 startPoint, Vector3 endPoint, float trajectoryHeight)
    {
        float gravity = Physics.gravity.y;
        float displacementY = endPoint.y - startPoint.y;
        Vector3 displacementXZ = new Vector3(endPoint.x - startPoint.x, 0f, endPoint.z - startPoint.z);

        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * trajectoryHeight);
        Vector3 velocityXZ = displacementXZ / (Mathf.Sqrt(-2 * trajectoryHeight / gravity)
                                               + Mathf.Sqrt(2 * (displacementY - trajectoryHeight) / gravity));

        return grapXZvalue * velocityXZ + velocityY;
    }

// Jump to a specified position
    public void JumpToPosition(Vector3 targetPosition, float trajectoryHeight)
    {
        activeGrapple = true;

        velocityToSet = CalculateJumpVelocity(transform.position, targetPosition, trajectoryHeight);
        cam.DoFov(grappleFOV);

        Invoke(nameof(SetVelocity), 0.1f);

        Invoke(nameof(ResetRestrictions), 3f);
    }

    private Vector3 velocityToSet;


    private bool enableMovementOnNextTouch;

// Set movement velocity
    private void SetVelocity()
    {
        enableMovementOnNextTouch = true;
        rb.velocity = velocityToSet;
        cam.DoFov(grappleFOV);
    }

// Reset movement restrictions
    public void ResetRestrictions()
    {
        activeGrapple = false;
        cam.DoFov(camFov);
    }


// Restore movement
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("MovingPlatform"))
        {
            transform.parent = collision.transform;
        }

        if (collision.gameObject.CompareTag("EndPoint"))
        {
            transform.parent = collision.transform;
        }

        if (enableMovementOnNextTouch)
        {
            enableMovementOnNextTouch = false;
            ResetRestrictions();
            GetComponent<PlayerGrappling>().StopGrapple();
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("MovingPlatform"))
        {
            transform.parent = null;
        }

        if (collision.gameObject.CompareTag("EndPoint"))
        {
            transform.parent = null;
        }
    }

// detect whether player is moving
    private bool isMoving()
    {
        return rb.velocity.magnitude > 0.1f; // speed threshold
    }
}