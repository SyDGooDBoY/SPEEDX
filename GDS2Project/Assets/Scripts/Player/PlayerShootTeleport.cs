using UnityEngine;
using UnityEngine.Serialization;

public class PlayerShootTeleport : MonoBehaviour
{
    // Public variables for configuration
    public KeyCode launchKey = KeyCode.Q; // Key to start shooting
    public KeyCode cancelShoot = KeyCode.Mouse1; // Key to cancel shooting
    public Transform shootingPoint; // The point from which the ball is shot
    public GameObject ballPrefab; // The ball (projectile) prefab to be instantiated
    public LineRenderer trajectoryLine; // Line renderer to display the projectile trajectory
    public int trajectorySegments = 20; // Number of segments in the trajectory line
    public float ballSpeed = 10.0f; // Speed of the projectile
    public float cooldown = 2.0f; // Cooldown time between teleports
    public float destroyTime = 3.0f; // Time after which the projectile is destroyed
    public float ballX = 0.05f; // Offset for the projectile's X-axis direction
    public float ballY = 0.5f; // Offset for the projectile's Y-axis direction
    private Camera cam; // Camera used for raycasting
    private GameObject crossHair; // Crosshair for aiming
    private float lastShootTime = 0; // Time of the last teleport

    private GameObject currentBall; // Currently instantiated projectile
    private int shootPhase = 0; // Shooting phase: 0 = Aim, 1 = Shoot, 2 = Teleport
    private PlayerMovement pm;
    private float remainingTimeToDestroy;

    [FormerlySerializedAs("grappleSound")]
    [Header("Sound")]
    public AudioClip shootSound;

    public AudioClip teleportSound;
    private AudioSource audioSource;

    public GradientColor gradientColor;

    void Start()
    {
        lastShootTime = -cooldown; // Initialize cooldown to allow immediate shooting at start
        trajectoryLine.positionCount = trajectorySegments; // Initialize the number of trajectory segments
        trajectoryLine.enabled = false; // Initially disable the trajectory line
        crossHair = GameObject.Find("Crosshair"); // Find the crosshair object in the scene
        cam = GameObject.Find("Camera").GetComponent<Camera>(); // Find the camera object in the scene
        pm = GetComponent<PlayerMovement>(); // Get the PlayerMovement component
        shootingPoint = GameObject.Find("shooting point").transform;
        audioSource = GetComponent<AudioSource>();
        shootSound = Resources.Load<AudioClip>("Sound/NEW SOUNDS/NEW GUN");
        teleportSound = Resources.Load<AudioClip>("Sound/NEW SOUNDS/NEW TELEPORT");

        // gradientColor = GameObject.Find("TeleportTime").GetComponent<GradientColor>();
    }

    void Update()
    {
        if (pm.swinging || pm.state == PlayerMovement.MoveState.grappling) return;
        if (currentBall != null)
        {
            gradientColor.EnableUI();
            gradientColor.SetMaxValue(destroyTime);
            remainingTimeToDestroy -= Time.deltaTime; // 减去上一帧所花的时间
            gradientColor.UpdateValue(remainingTimeToDestroy);
            if (remainingTimeToDestroy == 0)
            {
                currentBall = null;
                remainingTimeToDestroy = destroyTime; // 重置时间
                gradientColor.DisableUI();
            }
        }
        else if (currentBall == null)
        {
            remainingTimeToDestroy = destroyTime;
            gradientColor.DisableUI();
        }

        // Check if cooldown has passed before allowing aim or shoot
        if (Time.time - lastShootTime >= cooldown)
        {
            // In aim or shoot phase, update the trajectory
            if (shootPhase == 0 || shootPhase == 1)
            {
                UpdateTrajectoryBasedOnMouse();
            }

            // Press 'Q' to enter shooting mode, only if cooldown has passed
            if (Input.GetKeyDown(launchKey) && shootPhase == 0)
            {
                trajectoryLine.enabled = true; // Enable the trajectory line
                shootPhase = 1; // Switch to shooting phase
                crossHair.SetActive(false); // Hide the crosshair
            }

            // Release 'Q' to shoot the ball
            if (Input.GetKeyUp(launchKey) && shootPhase == 1)
            {
                Shoot(); // Perform shooting
                trajectoryLine.enabled = false; // Disable the trajectory line
                shootPhase = 2; // Switch to teleport phase
            }

            // Press 'Q' again to teleport to the ball
            if (Input.GetKeyDown(launchKey) && shootPhase == 2)
            {
                TeleportToBall(); // Perform teleportation
                lastShootTime = Time.time; // Update the time of the last teleport
                shootPhase = 0; // Reset to aim phase
                crossHair.SetActive(true); // Show the crosshair again
            }
        }

        if (shootPhase == 2 && currentBall == null)
        {
            shootPhase = 0; // Reset the phase to 0
            crossHair.SetActive(true); // Show the crosshair again
        }

        // Right mouse button cancels the shot at any stage
        if (Input.GetKeyDown(cancelShoot))
        {
            CancelShooting(); // Cancel the shooting process
        }
    }

    // Cancel shooting and destroy the current projectile
    void CancelShooting()
    {
        trajectoryLine.enabled = false; // Disable the trajectory line
        shootPhase = 0; // Reset the shooting phase
        if (currentBall != null)
        {
            Destroy(currentBall); // Destroy the current projectile
            currentBall = null; // Reset the projectile reference
        }

        crossHair.SetActive(true); // Show the crosshair again
    }

    // Update the projectile trajectory based on the mouse position
    void UpdateTrajectoryBasedOnMouse()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 10.0f; // Set the distance from the camera
        Vector3 worldPoint = cam.ScreenToWorldPoint(mousePos); // Convert mouse position to world space
        Vector3 direction = (worldPoint - shootingPoint.position).normalized; // Calculate the shooting direction
        direction.y += ballY; // Adjust the Y-axis direction for a parabolic curve
        ShowTrajectory(direction); // Show the calculated trajectory
    }

    // Display the trajectory of the projectile
    void ShowTrajectory(Vector3 direction)
    {
        Vector3[] points = new Vector3[trajectorySegments]; // Create an array for trajectory points
        Vector3 startingPosition = shootingPoint.position; // The starting position is the shooting point
        Vector3 startingVelocity = direction * ballSpeed; // Initial velocity of the projectile

        // Calculate the position of each point in the trajectory
        for (int i = 0; i < trajectorySegments; i++)
        {
            float time = i * 0.1f; // Time step for each segment
            Vector3 pointPosition = startingPosition + startingVelocity * time + Physics.gravity * time * time / 2f;
            points[i] = pointPosition; // Set the position of each point

            // Perform raycasting from the second segment onward to detect collisions
            if (i > 0)
            {
                RaycastHit hit;
                if (Physics.Raycast(points[i - 1], points[i] - points[i - 1], out hit,
                        Vector3.Distance(points[i - 1], points[i])))
                {
                    points[i] = hit.point; // Set the hit point as the trajectory endpoint
                    trajectoryLine.positionCount = i + 1; // Adjust the number of segments to display
                    break; // Stop calculating trajectory after collision
                }
            }
        }

        trajectoryLine.SetPositions(points); // Set positions for the trajectory line
    }

    // Shoot the projectile
    void Shoot()
    {
        if (audioSource != null)
        {
            audioSource.PlayOneShot(shootSound);
        }

        remainingTimeToDestroy = destroyTime;

        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 10.0f; // Set the distance from the camera
        Vector3 worldPoint = cam.ScreenToWorldPoint(mousePos); // Convert mouse position to world space
        Vector3 direction =
            (worldPoint - shootingPoint.position).normalized +
            new Vector3(ballX, ballY, 0); // Calculate shooting direction

        currentBall =
            Instantiate(ballPrefab, shootingPoint.position, Quaternion.identity); // Instantiate the projectile
        var rb = currentBall.GetComponent<Rigidbody>();
        rb.AddForce(direction * ballSpeed, ForceMode.VelocityChange); // Apply force to the projectile
        Destroy(currentBall, destroyTime); // Destroy the projectile after a specified time
    }

    // Teleport the player to the projectile's position
    void TeleportToBall()
    {
        if (currentBall != null)
        {
            if (audioSource != null)
            {
                audioSource.PlayOneShot(teleportSound);
            }
            transform.position = currentBall.transform.position; // Set player position to the projectile's position
            Destroy(currentBall); // Destroy the projectile
            currentBall = null; // Reset the projectile reference
        }
    }

    public int GetCurrentShootPhase()
    {
        return shootPhase;
    }
}