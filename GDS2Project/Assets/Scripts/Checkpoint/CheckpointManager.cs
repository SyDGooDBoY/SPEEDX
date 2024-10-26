using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static Vector3 respawnPosition;
    public static Transform targetLookAt;

    public Vector3 defaultRespawnPosition;
    public static Transform defaultTargetLookAt;

    public static PlayerCam cam;


    void Start()
    {
        respawnPosition = defaultRespawnPosition;
        targetLookAt = defaultTargetLookAt;
        cam = GameObject.FindWithTag("Cam").GetComponent<PlayerCam>();
    }

    public static void UpdateCheckpoint(Vector3 newRespawnPosition, Transform target)
    {
        respawnPosition = newRespawnPosition;
        SetTargetLookAt(target);
        Debug.Log(targetLookAt.transform.position + "99999999");
    }

    public static void SetTargetLookAt(Transform target)
    {
        targetLookAt = target;
    }

    public static void RespawnPlayer(GameObject player)
    {
        player.transform.position = respawnPosition;
        if (targetLookAt != null)
            cam.SetCameraToLookAtTarget(targetLookAt);
    }
}