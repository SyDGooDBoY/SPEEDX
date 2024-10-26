using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public GameObject camLookAt;
    private Transform camLookAtTrans;

    private void Start()
    {
        if (camLookAt == null)
        {
            camLookAtTrans = transform;
        }
        else
        {
            camLookAtTrans = camLookAt.transform;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CheckpointManager.UpdateCheckpoint(transform.position, camLookAtTrans);
            Debug.Log("Checkpoint reached.");
        }
    }
}

