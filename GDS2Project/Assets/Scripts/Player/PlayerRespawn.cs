using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    public float fallThreshold = -10f; 
    private Rigidbody rb;
    private CharacterController controller;

    [SerializeField] string bottomObjectTag = "Bottom";

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Respawn();
        }

        if (transform.position.y < fallThreshold)
        {
            Respawn();
        }
    }

    void Respawn()
    {
        if (CheckpointManager.respawnPosition != null)
        {
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
            transform.GetComponent<EnergySystem>().SetEnergy(AbilityManager.LOW_THRESHOLD / 2);
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
