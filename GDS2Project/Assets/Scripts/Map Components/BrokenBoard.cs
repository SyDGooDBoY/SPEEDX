using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrokenBoard : MonoBehaviour
{
    public string playerTag = "Player"; // 玩家标签
    public float brokenVel = 11f; // 破碎需要的最小速度 

    public AudioClip destructionSound; // 销毁时的音效
    public GameObject destructionParticles; // 粒子特效预制件

    // 用于调整粒子生成位置的相对偏移量
    public Vector3 particleOffset = Vector3.zero; // 可以在 Unity 中调整的偏移量
    private AudioSource audioSource; // 音效播放组件
    private PlayerDash pd;
    private PlayerCameraShake pcs;

    //public GameObject objectWithShake;  // 拖入含有 ObjectShake 组件的物体
    //private ObjectShake objectShakeScript;  // 保存 ObjectShake 组件的引用

    private void Start()
    {
        // 初始化音效组件
        audioSource = gameObject.AddComponent<AudioSource>();

        //objectShakeScript = objectWithShake.GetComponent<ObjectShake>();
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag(playerTag))
        {
            Debug.Log("Player has entered the trigger area.");
            Rigidbody playerRigidbody = other.gameObject.GetComponent<Rigidbody>();
            pd = other.gameObject.GetComponent<PlayerDash>();
            pcs = other.gameObject.GetComponent<PlayerCameraShake>();

            if (playerRigidbody != null)
            {
                // 获取玩家速度
                Vector3 playerVelocity = playerRigidbody.velocity;

                // 输出玩家的速度
                Debug.Log("Player velocity: " + playerVelocity.magnitude);

                if (playerVelocity.magnitude > brokenVel)
                {
                    pcs.shakeCamera();
                    // 播放音效并且销毁物体
                    if (destructionSound != null)
                    {
                        // 创建一个新的 GameObject 来播放音效
                        GameObject tempAudioPlayer = new GameObject("TempAudioPlayer");
                        AudioSource tempAudioSource = tempAudioPlayer.AddComponent<AudioSource>();
                        tempAudioSource.clip = destructionSound;
                        tempAudioSource.Play();

                        // 自动销毁音效播放对象，当音效播放结束后
                        Destroy(tempAudioPlayer, destructionSound.length);
                    }

                    // 生成粒子效果
                    if (destructionParticles != null)
                    {
                        // 计算粒子生成位置（墙面位置 + 偏移量）
                        Vector3 particleSpawnPosition = transform.position + particleOffset;

                        // 实例化粒子特效
                        GameObject particles = Instantiate(destructionParticles, particleSpawnPosition,
                            Quaternion.identity);

                        // 获取粒子系统的持续时间，然后在持续时间结束后销毁粒子对象
                        ParticleSystem ps = particles.GetComponent<ParticleSystem>();
                        if (ps != null)
                        {
                            ps.Play();
                            Destroy(particles, ps.main.duration);
                        }
                        else
                        {
                            // 如果粒子系统没有找到，直接给一个默认的延时销毁
                            Destroy(particles, 2f);
                        }
                    }
                    /*if (objectShakeScript != null)
                     {
                         // 启用 ObjectShake 脚本
                         objectShakeScript.enabled = true;
                     }*/


                    pd.dashCdTimer = 0;

                    // 销毁父物体
                    if (transform.parent != null)
                        Destroy(transform.parent.gameObject); // 立即销毁Cube
                }
            }
        }
    }
}