using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public GameObject camLookAt;
    private Transform camLookAtTrans;
    private MeshRenderer mesh;
    private AudioSource audioSource;
    private AudioClip checkpointSound;
    private bool checkpointReached;

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

        mesh = GetComponent<MeshRenderer>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
        else
        {
            audioSource = GetComponent<AudioSource>();
        }

        checkpointSound = Resources.Load<AudioClip>("Sound/save");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !checkpointReached)
        {
            audioSource.PlayOneShot(checkpointSound);
            checkpointReached = true;
            CheckpointManager.UpdateCheckpoint(transform.position, camLookAtTrans);
            mesh.enabled = false;
            Debug.Log("Checkpoint reached.");
        }
    }
}