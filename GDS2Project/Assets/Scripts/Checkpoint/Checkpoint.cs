using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public GameObject camLookAt;
    private Vector3 camLookAtPos;

    private void Start()
    {
        if (camLookAt == null)
        {
            camLookAtPos = transform.position;
        }
        else
        {
            camLookAtPos = camLookAt.transform.position;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CheckpointManager.UpdateCheckpoint(transform.position, camLookAtPos);
            Debug.Log("Checkpoint reached.");
        }
    }
}

