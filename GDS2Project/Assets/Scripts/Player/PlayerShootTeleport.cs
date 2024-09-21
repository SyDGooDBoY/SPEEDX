using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerShootTeleport : MonoBehaviour
{
    public Transform shootingPoint;
    public GameObject ballPrefab;

    public float ballSpeed = 10.0f;

    public float cooldown = 2.0f;

    private float lastShootTime = 0;
    public float targetY = 0.5f; // 用于调整抛物线的高度

    public float targetX = 0.05f; // 用于调整抛物线的水平偏移

    // Start is called before the first frame update
    void Start()
    {
        lastShootTime = -cooldown;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && Time.time - lastShootTime >= cooldown)
        {
            Shoot();
            lastShootTime = Time.time;
        }
    }

    void Shoot()
    {
        var bullet = Instantiate(ballPrefab, shootingPoint.position, Quaternion.identity);
        var rb = bullet.GetComponent<Rigidbody>();


        Vector3 targetDir = shootingPoint.forward;
        targetDir.y += targetY; // 给一个向上的初始偏移，让抛物线弯曲更明显
        targetDir.x += targetX;

        rb.AddForce(targetDir.normalized * ballSpeed, ForceMode.VelocityChange);
    }
}