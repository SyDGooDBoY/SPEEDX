using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerMovement : MonoBehaviour
{
    [Header("Air Control")]
    public bool canControlInAir = false;

    [Header("Movement Speed")]
    public float moveSpeed = 10f; // Temporary speed variable

    // public float gravity = -5f;
    public float swingSpeed;
    public float walkSpeed = 10f; // Walking speed
    public float runSpeed = 25f; // Running speed
    public float wallRunSpeed = 10f; // Wall run speed
    public float slideSpeed = 15f; // Sliding speed
    public float climbSpeed = 5f; // Climbing speed
    public float dashSpeed; // Dash speed
    public float maxYSpeed;

    [Header("Grappling sens")]
    public float grapXZvalue = 2f; // Grappling XZ

    [Header("Friction Settings")]
    public float groundDrag = 5f; // Ground friction

    [Header("Jump Settings")]
    // public bool canDoubleJump = false; // Double jump state
    public float jumpForce = 12f; // Jump force

    // public float doubleJumpForce = 10f; // Double jump force
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

    public float slopeSpeedFactor = 20f;

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
    public float camFov;

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
    public bool sliding;

    [Header("Energy Recovery")]
    public float wallSlideRecoveryRate = 20f; // recovery rate through wall sliding

    private EnergySystem energySystem;
    private AbilityManager abilityManager;

    [Header("Player Input control")]
    public bool inputEnabled = true;

    [Header("Audio")]
    public AudioSource audioSource;

    public AudioClip jumpSound;

    // public AudioClip walkSound;

    public PauseMenu pauseMenu;

    // Movement states
    public enum MoveState
    {
        dashing,
        sliding,
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
        else if (sliding)
        {
            state = MoveState.sliding;

            if (OnSlope() && rb.velocity.y < 0.1f)
                moveSpeed = slideSpeed * abilityManager.GetAbilityMultiplier();

            else
                moveSpeed = runSpeed * abilityManager.GetAbilityMultiplier();
        }

        // Crouching state
        // else if (Input.GetKey(crouchKey))
        // {
        //     state = MoveState.crouching;
        //     moveSpeed = crouchMoveSpeed * abilityManager.GetAbilityMultiplier();
        // }

// // Running state
//         else if (isGrounded && Input.GetKey(runKey))
//         {
//             state = MoveState.Running;
//             moveSpeed = runSpeed * abilityManager.GetAbilityMultiplier();
//         }
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
            //moveSpeed *= abilityManager.GetAbilityMultiplier();
        }
    }

// Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        readyToJump = true;
        startYscale = transform.localScale.y;
        cam = GameObject.Find("Camera").GetComponent<PlayerCam>();

        camFov = cam.GetComponent<Camera>().fieldOfView;
        cam.DoFov(camFov);

        energySystem = GetComponent<EnergySystem>();
        abilityManager = GetComponent<AbilityManager>();
        if (GetComponent<AudioSource>() == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        else
        {
            audioSource = GetComponent<AudioSource>();
        }

        jumpSound = Resources.Load<AudioClip>("Sound/NEW SOUNDS/JUMP");
        // walkSound = Resources.Load<AudioClip>("Sound/NEW SOUNDS/FOOTSTEP B");
        pauseMenu = GameObject.Find("UIManager").GetComponent<PauseMenu>();
    }

// Update is called once per frame
    void Update()
    {
        if (!inputEnabled) return;
        if (pauseMenu.GameIsPaused == true)
        {
            audioSource.Pause();
            return;
        }

        // Ground check
        float sphereRadius = 0.4f; // Adjust the radius as needed
        float groundDistance = 0.6f; // Adjust the distance as needed
        isGrounded = Physics.SphereCast(transform.position, sphereRadius, Vector3.down, out RaycastHit hit,
            groundDistance, groundMask);

        if (isGrounded)
        {
            hasJumpedInAir = false; // Reset air jump when grounded
        }

        coyoteTimeCounter = isGrounded ? coyoteTime : coyoteTimeCounter - Time.deltaTime;
        PlayerInput();
        SpeedControl();
        StateHandle();

        // Ground friction handling
        if (isGrounded && !activeGrapple)
        {
            rb.drag = groundDrag;
            canControlInAir = false;
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
            energySystem.stopMoveTimer = 0f;
        }

        // if (rb.velocity.magnitude < 30f && isMoving())
        // {
        //     if (!audioSource.isPlaying)
        //     {
        //         audioSource.clip = walkSound;
        //         audioSource.Play();
        //     }
        // }

        // Debug.Log("PlayerSpeed: " + rb.velocity.magnitude);
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

    private bool hasJumpedInAir = false;

// Player input handling
    private void PlayerInput()
    {
        horizontalMovement = Input.GetAxisRaw("Horizontal");
        verticalMovement = Input.GetAxisRaw("Vertical");

        // Jumping
        if (Input.GetKeyDown(jumpKey))
        {
            if (isGrounded && readyToJump && coyoteTimeCounter > 0) // First jump, affected by coyote time
            {
                readyToJump = false;
                Jump();
                Invoke(nameof(ResetJump), jumpCooldown);
            }
            // else if (!isGrounded && !canDoubleJump) // Double jump, only allowed when not grounded
            // {
            //     DoubleJump();
            //     canDoubleJump = true; // Disable further double jumps until grounded again
            // }
            else if (!isGrounded && !hasJumpedInAir) // Allow one jump in air
            {
                hasJumpedInAir = true;
                Jump(true);
                Invoke(nameof(ResetJump), jumpCooldown);
            }

            if (activeGrapple || swinging || wallRunning || climbing)
            {
                // canDoubleJump = false; // Allow one double jump after these actions
                hasJumpedInAir = false;
                readyToJump = true; // Ready to jump again
            }
        }

        // Crouching
        // if (Input.GetKeyDown(crouchKey))
        // {
        //     transform.localScale = new Vector3(transform.localScale.x, crouchYscale, transform.localScale.z);
        //     rb.AddForce(Vector3.down * crouchSpeed, ForceMode.Impulse);
        // }
        // // Stand up
        // else if (Input.GetKeyUp(crouchKey))
        // {
        //     transform.localScale = new Vector3(transform.localScale.x, startYscale, transform.localScale.z);
        // }
    }

// Double jump
    // private void DoubleJump()
    // {
    //     rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z); // Reset vertical velocity
    //     rb.AddForce(transform.up * doubleJumpForce * 0.8f,
    //         ForceMode.Impulse); // Slightly less force than the initial jump
    //     canDoubleJump = true;
    // }

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
        // Apply movement forces only if grounded
        if (isGrounded)
        {
            // Slope handling
            if (OnSlope() && !exitingSlope)
            {
                rb.AddForce(GetSlopeMoveDirection(moveDirection) * moveSpeed * slopeSpeedFactor, ForceMode.Force);

                if (rb.velocity.y > 0)
                    rb.AddForce(Vector3.down * 80f, ForceMode.Force);
            }
            else
            {
                // Ground movement
                rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
            }
        }
        else if (!isGrounded)
        {
            if (canControlInAir)
            {
                rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airDrag, ForceMode.Force);
            }
            else
            {
                // Apply reduced movement forces in the air (e.g., 10% of normal)
                rb.AddForce(moveDirection.normalized * moveSpeed * 1f, ForceMode.Force);
            }
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

        if (maxYSpeed != 0 && rb.velocity.y > maxYSpeed)
            rb.velocity = new Vector3(rb.velocity.x, maxYSpeed, rb.velocity.z);
    }

// Jumping
    private void Jump(bool isDoubleJump = false)
    {
        audioSource.PlayOneShot(jumpSound);
        exitingSlope = true;
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        if (isDoubleJump)
        {
            Vector3 doubleJumpDirection =
                orientation.forward * verticalMovement + orientation.right * horizontalMovement;
            rb.AddForce((transform.up + doubleJumpDirection).normalized * jumpForce, ForceMode.Impulse);
        }
        else
        {
            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse); // Perform a vertical impulse jump
        }

        canControlInAir = false;
        // canDoubleJump = false;
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
        // canDoubleJump = false;
    }

// Check if on a slope
    public bool OnSlope()
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
    public Vector3 GetSlopeMoveDirection(Vector3 direction)
    {
        return Vector3.ProjectOnPlane(direction, slopeHit.normal).normalized;
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

    public void StopMovement()
    {
        rb.velocity = Vector3.zero; // Stop all movement
        rb.isKinematic = true; // Disable physics interactions
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
            StopMovement();
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
    public bool isMoving()
    {
        return rb.velocity.magnitude > 0.1f; // speed threshold
    }
}