using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    public float speed = 2.0f;

    private Vector3 target;

    void Start()
    {
        // Set initial target to pointB
        target = pointB.position;
    }

    void Update()
    {
        // Move the platform towards the target
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

        // If the platform reaches the target, switch the target
        if (Vector3.Distance(transform.position, target) < 0.1f)
        {
            target = (target == pointA.position) ? pointB.position : pointA.position;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Parent the player to the platform when they collide
            collision.gameObject.transform.SetParent(transform);
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Unparent the player when they exit the collision
            collision.gameObject.transform.SetParent(null);
        }
    }
}
