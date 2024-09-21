using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTeleport : MonoBehaviour
{
    public float teleportDelay = 0.5f; // Delay before teleporting to the ball
    public float destroyDelay = 2.0f; // Delay to destroy the ball if not on ground
    private bool hasLanded = false; // Check if the ball has landed
    private GameObject player; // Reference to the player
    private Rigidbody rb;

    public LayerMask groundLayer; // LayerMask for the ground

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player"); // Assumes player has the tag "Player"
        rb = GetComponent<Rigidbody>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!hasLanded)
        {
            hasLanded = true;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true; // Stop all physics interactions immediately on landing

            if (IsGround(collision.gameObject))
            {
                StartCoroutine(TeleportPlayerAfterDelay());
            }
            else
            {
                StartCoroutine(DestroyBallAfterDelay());
            }
        }
    }

    bool IsGround(GameObject obj)
    {
        return (groundLayer.value & (1 << obj.layer)) > 0;
    }

    IEnumerator TeleportPlayerAfterDelay()
    {
        yield return new WaitForSeconds(teleportDelay);
        if (player != null)
        {
            player.transform.position = transform.position;
        }

        Destroy(gameObject);
    }

    IEnumerator DestroyBallAfterDelay()
    {
        yield return new WaitForSeconds(destroyDelay);
        Destroy(gameObject);
    }
}