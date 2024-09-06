using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] Transform playerCamera;
    [SerializeField] [Range(0.0f, 0.5f)] float mouseSmoothTime = 0.03f;
    [SerializeField] bool cursorLock = true;

    [SerializeField] float mouseSensitivity = 3.5f;
    [SerializeField] [Range(0.0f, 0.5f)] float moveSmoothTime = 0.3f;
    [SerializeField] float gravity = -30f;
    [SerializeField] Transform groundCheck;
    [SerializeField] LayerMask ground;

    [SerializeField] float moveConsumption = 5f; // move consumption per half second
    [SerializeField] float jumpConsumption = 10f;


    private AbilityManager abilityManager;
    private EnergySystem energySystem;

    float velocityY;
    bool isGrounded;
    private bool isMoving = false; // used to check player state

    float cameraCap;
    Vector2 currentMouseDelta;
    Vector2 currentMouseDeltaVelocity;

    CharacterController controller;
    Vector2 currentDir;
    Vector2 currentDirVelocity;
    Vector3 velocity;

    // ensure accuracy in detecting whether the player is moving
    private const float movementThreshold = 0.1f; 
    private const float inputThreshold = 0.1f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        abilityManager = GetComponent<AbilityManager>();
        energySystem = GetComponent<EnergySystem>();

        if (cursorLock)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = true;
        }
    }

    void Update()
    {
        if (!PauseMenu.GameIsPaused)
        {
            UpdateMouse();
            UpdateMove();
        }
    }

    void UpdateMouse()
    {
        Vector2 targetMouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        currentMouseDelta = Vector2.SmoothDamp(currentMouseDelta, targetMouseDelta, ref currentMouseDeltaVelocity,
            mouseSmoothTime);

        cameraCap -= currentMouseDelta.y * mouseSensitivity;
        cameraCap = Mathf.Clamp(cameraCap, -90.0f, 90.0f);

        playerCamera.localEulerAngles = Vector3.right * cameraCap;
        transform.Rotate(Vector3.up * currentMouseDelta.x * mouseSensitivity);
    }

    void UpdateMove()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, 0.2f, ground);

        Vector2 targetDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        targetDir.Normalize();

        currentDir = Vector2.SmoothDamp(currentDir, targetDir, ref currentDirVelocity, moveSmoothTime);
        velocityY += gravity * 2f * Time.deltaTime;

        // get speed and jump height based on Energy State
        float speed = abilityManager.GetCurrentSpeed();
        float jumpHeight = abilityManager.GetCurrentJumpHeight();

        // Check whether player is moving
        isMoving = IsMoving();

        // If the energy is 0, stop all operations
        if (energySystem.GetCurrentEnergy() <= 0)
        {
            Debug.Log("No energy left!");
            // Should have some SFV to Notice Player
            return;
        }

        MoveConsumptionCheck();
     
        Vector3 velocity = (transform.forward * currentDir.y + transform.right * currentDir.x) * speed +
                           Vector3.up * velocityY;
        controller.Move(velocity * Time.deltaTime);

        // Jump Logic
        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            if (energySystem.UseEnergy(jumpConsumption)) 
            {
                velocityY = Mathf.Sqrt(jumpHeight * -2f * gravity); 
                Debug.Log("PlayerSpeed" + speed);
                Debug.Log("PlayerJumpHeight" + jumpHeight);
            }
            else
            {
                Debug.Log("Not enough energy for jump, setting energy to zero.");
            }
        }

        if (!isGrounded && controller.velocity.y < -1f)
        {
            velocityY = -8f;
        }
    }

    void MoveConsumptionCheck ()
    {
        // if player moving 
        if (isMoving)
        {
            energySystem.StopRecovery();
            ConsumeEnergyOverTime(moveConsumption);
        }
        else
        {
            energySystem.TryStartRecovery(); // Try to start energy recovery
        }
    }

    private void ConsumeEnergyOverTime(float consumptionRate)
    {
        float energyToConsume = consumptionRate * Time.deltaTime;
        energySystem.UseEnergy(energyToConsume);
    }

    private bool IsMoving()
    {
        bool hasInput = currentDir.magnitude > inputThreshold;
        bool hasVelocity = controller.velocity.magnitude > movementThreshold;

        return hasInput && hasVelocity; // Both are required
    }
}