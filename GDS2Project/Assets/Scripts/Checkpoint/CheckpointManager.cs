using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static Vector3 respawnPosition; 
    public Vector3 defaultRespawnPosition; 

    void Start()
    {
        respawnPosition = defaultRespawnPosition;
    }

    public static void UpdateCheckpoint(Vector3 newRespawnPosition)
    {
        respawnPosition = newRespawnPosition;
    }

    public static void RespawnPlayer(GameObject player)
    {
        player.transform.position = respawnPosition;
    }
}
