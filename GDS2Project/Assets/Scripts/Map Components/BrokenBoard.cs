using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrokenBoard : MonoBehaviour
{
    public string playerTag = "Player"; // ��ұ�ǩ
    public float brokenVel = 11f; // ������Ҫ����С�ٶ� 

    public AudioClip destructionSound;  // ����ʱ����Ч
    public GameObject destructionParticles; // ������ЧԤ�Ƽ�

    // ���ڵ�����������λ�õ����ƫ����
    public Vector3 particleOffset = Vector3.zero; // ������ Unity �е�����ƫ����
    private AudioSource audioSource;    // ��Ч�������

    private void Start()
    {
        // ��ʼ����Ч���
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            Debug.Log("Player has entered the trigger area.");
            Rigidbody playerRigidbody = other.GetComponent<Rigidbody>();

            if (playerRigidbody != null)
            {
                // ��ȡ����ٶ�
                Vector3 playerVelocity = playerRigidbody.velocity;

                // �����ҵ��ٶ�
                Debug.Log("Player velocity: " + playerVelocity.magnitude);

                if (playerVelocity.magnitude > brokenVel)
                {
                    // ������Ч������������
                    if (destructionSound != null)
                    {
                        // ����һ���µ� GameObject ��������Ч
                        GameObject tempAudioPlayer = new GameObject("TempAudioPlayer");
                        AudioSource tempAudioSource = tempAudioPlayer.AddComponent<AudioSource>();
                        tempAudioSource.clip = destructionSound;
                        tempAudioSource.Play();

                        // �Զ�������Ч���Ŷ��󣬵���Ч���Ž�����
                        Destroy(tempAudioPlayer, destructionSound.length);
                    }

                    // ��������Ч��
                    if (destructionParticles != null)
                    {
                        // ������������λ�ã�ǽ��λ�� + ƫ������
                        Vector3 particleSpawnPosition = transform.position + particleOffset;

                        // ʵ����������Ч
                        GameObject particles = Instantiate(destructionParticles, particleSpawnPosition, Quaternion.identity);

                        // ��ȡ����ϵͳ�ĳ���ʱ�䣬Ȼ���ڳ���ʱ��������������Ӷ���
                        ParticleSystem ps = particles.GetComponent<ParticleSystem>();
                        if (ps != null)
                        {
                            Destroy(particles, ps.main.duration);
                        }
                        else
                        {
                            // �������ϵͳû���ҵ���ֱ�Ӹ�һ��Ĭ�ϵ���ʱ����
                            Destroy(particles, 2f);
                        }
                    }

                    // ���ٸ�����
                    Destroy(transform.parent.gameObject); // ��������Cube
                }
            }
        }
    }
}
