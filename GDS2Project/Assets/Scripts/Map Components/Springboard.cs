using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Springboard : MonoBehaviour
{
    public float jumpForce = 40f;
    public AudioSource audioSource;
    public AudioClip springSound;

    private void Start()
    {
        if (GetComponent<AudioSource>() == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        else
        {
            audioSource = GetComponent<AudioSource>();
        }

        springSound = Resources.Load<AudioClip>("Sound/NEW SOUNDS/JUMP PAD");
    }

    private void OnTriggerEnter(Collider other)
    {
        // 检查是否是玩家碰到了跳板
        if (other.CompareTag("Player"))
        {
            Rigidbody playerRb = other.GetComponent<Rigidbody>(); // 获取玩家的Rigidbody组件

            if (playerRb != null)
            {
                // 先清除玩家的垂直速度（避免在掉落时施加力造成累积）
                playerRb.velocity = new Vector3(playerRb.velocity.x, 0f, playerRb.velocity.z);

                // 向玩家施加一个向上的力
                playerRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                audioSource.PlayOneShot(springSound);

                // 输出调试信息
                Debug.Log("Player hit the springboard and jumped!");
            }
        }
    }
}