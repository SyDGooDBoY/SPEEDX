using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Environment : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    public float speed = 1.0f;
    private float t = 0.0f;
    private Vector3 lastPosition;
    private Vector3 velocity;

    void Start()
    {
        lastPosition = transform.position;
    }

    void FixedUpdate()
    {
        t += Time.deltaTime * speed;
        Vector3 nextPosition = Vector3.Lerp(pointA.position, pointB.position, Mathf.PingPong(t, 1.0f));
        velocity = (nextPosition - transform.position) / Time.fixedDeltaTime;
        transform.position = nextPosition;
    }


    public Vector3 GetVelocity()
    {
        return velocity;
    }
}