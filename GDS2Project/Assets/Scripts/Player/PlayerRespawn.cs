using UnityEngine;
using UnityEngine.Serialization;

public class PlayerRespawn : MonoBehaviour
{
    public float fallThreshold = -10f;
    private Rigidbody rb;
    private CharacterController controller;

    [SerializeField]
    string bottomObjectTag = "Bottom";

    [FormerlySerializedAs("grappleSound")]
    [Header("Sound")]
    public AudioClip respawnSound;

    private AudioSource audioSource;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        controller = GetComponent<CharacterController>();
        audioSource = GetComponent<AudioSource>();
        respawnSound = Resources.Load<AudioClip>("Sound/respawn_teleport sound");
    }

    void Update()
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
                rb.position = CheckpointManager.respawnPosition;
                Debug.Log("Player respawned at (using Rigidbody): " + CheckpointManager.respawnPosition);
            }
            else if (controller != null)
            {
                controller.enabled = false;
                transform.position = CheckpointManager.respawnPosition;
                controller.enabled = true;
            }
            else
            {
                transform.position = CheckpointManager.respawnPosition;
            }

            //reset Energy State
            EnergySystem playerEnergySystem = transform.GetComponent<EnergySystem>();
            playerEnergySystem.SetEnergy(AbilityManager.LOW_THRESHOLD / 2);
            playerEnergySystem.ExitBoost();
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