using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static Vector3 respawnPosition;
    public static Vector3 targetLookAtPosition;

    public Vector3 defaultRespawnPosition;
    public static Vector3 defaultTargetLookAtPosition;

    public static PlayerCam cam;


    void Start()
    {
        respawnPosition = defaultRespawnPosition;
        targetLookAtPosition = defaultTargetLookAtPosition;
        cam = GameObject.FindWithTag("Cam").GetComponent<PlayerCam>();
    }

    public static void UpdateCheckpoint(Vector3 newRespawnPosition, Vector3 targetPosition)
    {
        respawnPosition = newRespawnPosition;
        targetLookAtPosition = targetPosition;
    }

    public static void RespawnPlayer(GameObject player)
    {
        player.transform.position = respawnPosition;
        cam.SetCameraToLookAtTarget(targetLookAtPosition);
    }
}
