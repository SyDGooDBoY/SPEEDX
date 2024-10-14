using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

// Script to manage player swinging mechanics similar to a grappling hook or swing system.
public class PlayerSwinging : MonoBehaviour
{
    [Header("References")]
    // LineRenderer component to visually represent the swing rope.
    public LineRenderer lr;

    // Point from which the swing/grapple is shot.
    public Transform gunTip;

    // The player's camera transform for direction and raycasting.
    public Transform cam;

    // The player's main transform.
    public Transform player;

    // LayerMask to define what objects the player can grapple to.
    public LayerMask whatIsGrappleable;

    // Reference to PlayerMovement script to handle movement states.
    public PlayerMovement pm;

    [Header("Swinging")]
    // Maximum distance the swing can reach.
    private float maxSwingDistance = 25f;

    // The current point where the swing is attached.
    private Vector3 swingPoint;

    // The physics joint used to create swing mechanics.
    private SpringJoint joint;

    [Header("OdmGear")]
    // Reference for calculating directional forces.
    public Transform orientation;

    // Rigidbody of the player for applying forces.
    public Rigidbody rb;

    // Force applied horizontally during swinging.
    public float horizontalThrustForce;

    // Forward force applied to the player when adjusting the rope length.
    public float forwardThrustForce;

    // Speed at which the swing cable extends or retracts.
    public float extendCableSpeed;

    [Header("Prediction")]
    // Stores result of raycast for swing prediction.
    public RaycastHit predictionHit;

    // Radius for sphere casting to predict grapple points.
    public float predictionSphereCastRadius;

    // Visual marker for where the swing point might be.
    public Transform predictionPoint;

    [Header("Input")]
    // Key binding to initiate swinging.
    public KeyCode swingKey = KeyCode.Mouse0;

    public PlayerShootTeleport playerShootTeleport;

    [FormerlySerializedAs("grappleSound")]
    [Header("Sound")]
    public AudioClip swingSound;

    private AudioSource audioSource;

    private void Start()
    {
        playerShootTeleport = GetComponent<PlayerShootTeleport>();
        cam = GameObject.Find("Camera").transform;
        gunTip = GameObject.Find("shooting point").transform;
        if (GetComponent<AudioSource>() == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        else
        {
            audioSource = GetComponent<AudioSource>();
        }

        swingSound = Resources.Load<AudioClip>("Sound/NEW SOUNDS/SWINGING");
    }

    private void Update()
    {
        if (playerShootTeleport.GetCurrentShootPhase() != 0) return; // Prevent swinging during aiming or shooting
        // Handle input for starting and stopping the swing.
        if (Input.GetKeyDown(swingKey)) StartSwing();
        if (Input.GetKeyUp(swingKey)) StopSwing();

        // Continuously check for potential swing points.
        CheckForSwingPoints();

        // Handle movement adjustments while swinging.
        if (joint != null) OdmGearMovement();
    }

    private void LateUpdate()
    {
        // Draw the visual representation of the rope.
        DrawRope();
    }

    private void CheckForSwingPoints()
    {
        // Return early if already swinging.
        if (joint != null) return;

        RaycastHit sphereCastHit;
        // Sphere cast to predict swing points at a distance.
        Physics.SphereCast(cam.position, predictionSphereCastRadius, cam.forward,
            out sphereCastHit, maxSwingDistance, whatIsGrappleable);

        RaycastHit raycastHit;
        // Direct raycast to determine exact swing points.
        Physics.Raycast(cam.position, cam.forward,
            out raycastHit, maxSwingDistance, whatIsGrappleable);

        Vector3 realHitPoint;

        // Determine the most accurate swing point based on raycasts.
        if (raycastHit.point != Vector3.zero)
            realHitPoint = raycastHit.point; // Direct hit
        else if (sphereCastHit.point != Vector3.zero)
            realHitPoint = sphereCastHit.point; // Indirect (predicted) hit
        else
            realHitPoint = Vector3.zero; // Miss

        // Update the prediction point marker based on results.
        if (realHitPoint != Vector3.zero)
        {
            predictionPoint.gameObject.SetActive(true);
            predictionPoint.position = realHitPoint;
        }
        else
        {
            predictionPoint.gameObject.SetActive(false);
        }

        // Store the most suitable hit for use when starting a swing.
        predictionHit = raycastHit.point == Vector3.zero ? sphereCastHit : raycastHit;
    }

    private void StartSwing()
    {
        // Exit function if no valid swing point was detected.
        if (predictionHit.point == Vector3.zero) return;

        // Deactivate any active grapple to start swinging.
        if (GetComponent<PlayerGrappling>() != null)
            GetComponent<PlayerGrappling>().StopGrapple();
        pm.ResetRestrictions();

        // Set swinging state and configure the physics joint.
        pm.swinging = true;
        swingPoint = predictionHit.point;
        joint = player.gameObject.AddComponent<SpringJoint>();
        joint.autoConfigureConnectedAnchor = false;
        joint.connectedAnchor = swingPoint;

        float distanceFromPoint = Vector3.Distance(player.position, swingPoint);
        joint.maxDistance = distanceFromPoint * 0.8f;
        joint.minDistance = distanceFromPoint * 0.25f;

        // Spring physics parameters for the swinging motion.
        joint.spring = 4.5f;
        joint.damper = 7f;
        joint.massScale = 4.5f;

        lr.positionCount = 2;
        currentGrapplePosition = gunTip.position;
        if (swingSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(swingSound);
        }
    }

    public void StopSwing()
    {
        // Reset swinging state and visual elements.
        pm.swinging = false;
        lr.positionCount = 0;
        Destroy(joint);
    }

    private void OdmGearMovement()
    {
        // Apply forces based on player input for movement control during swing.
        if (Input.GetKey(KeyCode.D)) rb.AddForce(orientation.right * horizontalThrustForce * Time.deltaTime);
        if (Input.GetKey(KeyCode.A)) rb.AddForce(-orientation.right * horizontalThrustForce * Time.deltaTime);
        if (Input.GetKey(KeyCode.W)) rb.AddForce(orientation.forward * forwardThrustForce * Time.deltaTime);
        if (Input.GetKey(KeyCode.S)) rb.AddForce(-orientation.forward * forwardThrustForce * Time.deltaTime);

        // Adjust cable length based on input.
        // if (Input.GetKey(KeyCode.Space)) AdjustCableLength(-extendCableSpeed); // Shorten
        // if (Input.GetKey(KeyCode.S)) AdjustCableLength(extendCableSpeed); // Extend
    }

    private void AdjustCableLength(float speed)
    {
        // Calculate new distances based on current position and adjust joint limits.
        float adjustedDistance = Vector3.Distance(transform.position, swingPoint) + speed;
        joint.maxDistance = adjustedDistance * 0.8f;
        joint.minDistance = adjustedDistance * 0.25f;
    }

    private Vector3 currentGrapplePosition;

    private void DrawRope()
    {
        // Only draw rope if swinging is active.
        if (!joint) return;

        // Smoothly transition the rope's endpoint to the swing point for visual effect.
        currentGrapplePosition = Vector3.Lerp(currentGrapplePosition, swingPoint, Time.deltaTime * 8f);

        lr.SetPosition(0, gunTip.position);
        lr.SetPosition(1, currentGrapplePosition);
    }
}