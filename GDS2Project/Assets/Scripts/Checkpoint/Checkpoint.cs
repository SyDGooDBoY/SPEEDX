using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CheckpointManager.UpdateCheckpoint(transform.position);
            Debug.Log("Checkpoint reached.");
        }
    }
}

