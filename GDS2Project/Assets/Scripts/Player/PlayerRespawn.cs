using UnityEngine;
using UnityEngine.Serialization;

public class PlayerRespawn : MonoBehaviour
{
    public float fallThreshold = -10f;
    private Rigidbody rb;

    [SerializeField]
    string bottomObjectTag = "Bottom";

    [FormerlySerializedAs("grappleSound")]
    [Header("Sound")]
    public AudioClip respawnSound;

    private AudioSource audioSource;
    private PlayerShootTeleport playerShootTeleport;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (GetComponent<AudioSource>() == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        else
        {
            audioSource = GetComponent<AudioSource>();
        }

        respawnSound = Resources.Load<AudioClip>("Sound/respawn_teleport sound");
        playerShootTeleport = GetComponent<PlayerShootTeleport>();
    }

    void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Respawn();
        }

        // if (transform.position.y < fallThreshold)
        // {
        //     Respawn();
        // }
    }

    void Respawn()
    {
        if (CheckpointManager.respawnPosition != null)
        {
            if (audioSource != null)
            {
                audioSource.PlayOneShot(respawnSound);
            }

            if (rb != null)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                //rb.position = CheckpointManager.respawnPosition;
                CheckpointManager.RespawnPlayer(gameObject);
                Debug.Log("Player respawned at (using Rigidbody): " + CheckpointManager.respawnPosition);
            }
            else
            {
                CheckpointManager.RespawnPlayer(gameObject);
            }

            //reset Energy State
            EnergySystem playerEnergySystem = transform.GetComponent<EnergySystem>();
            playerEnergySystem.SetEnergy(AbilityManager.LOW_THRESHOLD / 2);
            playerEnergySystem.ExitBoost();
            if (playerShootTeleport.currentBall != null)
            {
                StartCoroutine(playerShootTeleport.DestroyBallNextFrame());
            }
        }
        else
        {
            Debug.LogWarning("Respawn position is not set. Ensure a checkpoint has been reached.");
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(bottomObjectTag))
        {
            Respawn();
        }
    }
}