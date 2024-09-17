using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script to handle player dashing mechanics.
public class PlayerDash : MonoBehaviour
{
    [Header("References")]
    // Orientation is generally the player's body direction
    public Transform orientation;

    // Reference to the camera to adjust the field of view during a dash
    public Transform playerCam;
    private Rigidbody rb; // Rigidbody component of the player for physics calculations
    private PlayerMovement pm; // Reference to the PlayerMovement script

    [Header("Dashing")]
    // Force applied when dashing
    public float dashForce = 50f;

    // Multiplier for dash force when in air
    public float inAirDashMultiplier = 0.3f;

    // Additional upward force applied during dashing
    public float dashUpwardForce;

    // Maximum vertical speed player can reach while dashing
    public float maxDashYSpeed;

    // Duration of the dash effect
    public float dashDuration;

    [Header("CameraEffects")]
    // Reference to the PlayerCam script to manage camera effects
    public PlayerCam cam;

    // Default and dashing field of view settings for the camera
    public float defaultFov;
    public float dashFov;

    [Header("Settings")]
    // If true, dash direction follows the camera's forward vector
    public bool useCameraForward = true;

    // Allows dashing in the direction of horizontal inputs
    public bool allowAllDirections = true;

    // Toggle gravity during dash
    public bool disableGravity = false;

    // Reset velocity before applying dash force
    public bool resetVel = true;

    [Header("Cooldown")]
    // Cooldown duration between dashes
    public float dashCd;

    // Cooldown timer to track dash availability
    private float dashCdTimer;

    [Header("Input")]
    // Key used to trigger a dash
    public KeyCode dashKey = KeyCode.E;

    private void Start()
    {
        // Getting the necessary components and setting the default field of view
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<PlayerMovement>();
        defaultFov = cam.GetComponent<Camera>().fieldOfView;
        cam.DoFov(defaultFov);
    }

    private void Update()
    {
        // Listening for the dash key input
        if (Input.GetKeyDown(dashKey))
        {
            Dash();
            Debug.Log("Dash" + pm.moveSpeed);
        }

        // Countdown the dash cooldown timer
        if (dashCdTimer > 0)
            dashCdTimer -= Time.deltaTime;
    }

    private void Dash()
    {
        // Check if the cooldown is active or player is not grounded
        if (dashCdTimer > 0 || !pm.isGrounded) return;
        else dashCdTimer = dashCd;

        pm.dashing = true;

        // Adjust the camera's field of view for the dash effect
        cam.DoFov(dashFov);

        Transform forwardT;

        // Determine the direction based on camera or orientation
        if (useCameraForward)
            forwardT = playerCam; // Direction you're looking
        else
            forwardT = orientation; // Direction you're facing (no up or down tilt)

        Vector3 direction = GetDirection(forwardT);

        // Modify dash force if in air
        if (!pm.isGrounded)
        {
            dashForce *= inAirDashMultiplier;
        }

        Vector3 forceToApply = direction * dashForce + orientation.up * dashUpwardForce;

        // Toggle gravity based on setting
        if (disableGravity)
            rb.useGravity = false;

        delayedForceToApply = forceToApply;
        Invoke(nameof(DelayedDashForce), 0.025f);

        // Reset dash effects after duration
        Invoke(nameof(ResetDash), dashDuration);
    }

    private Vector3 delayedForceToApply;

    private void DelayedDashForce()
    {
        // Reset velocity if enabled
        if (resetVel)
            rb.velocity = Vector3.zero;

        // Apply the calculated dash force
        rb.AddForce(delayedForceToApply, ForceMode.Impulse);
    }

    private void ResetDash()
    {
        // Resetting states after the dash is complete
        pm.dashing = false;
        cam.DoFov(defaultFov);
        if (disableGravity)
            rb.useGravity = true;
    }

    // Calculate the direction vector for the dash
    private Vector3 GetDirection(Transform forwardT)
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3();

        // Allow directional input if enabled, otherwise, use the forward direction
        if (allowAllDirections)
            direction = forwardT.forward * verticalInput + forwardT.right * horizontalInput;
        else
            direction = forwardT.forward;

        // Default to forward direction if no input is detected
        if (verticalInput == 0 && horizontalInput == 0)
            direction = forwardT.forward;

        return direction.normalized;
    }
}