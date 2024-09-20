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
        var bullet = Instantiate(ballPrefab, shootingPoint.position, shootingPoint.rotation);
        bullet.GetComponent<Rigidbody>().velocity = shootingPoint.forward * ballSpeed;
    }
}