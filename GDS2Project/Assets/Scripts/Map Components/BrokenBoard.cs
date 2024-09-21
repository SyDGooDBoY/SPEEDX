using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrokenBoard : MonoBehaviour
{
    public string playerTag = "Player"; //Player Tag
    public float brokenVel = 11f; //Broken need speed 

    public AudioClip destructionSound;  // 预留的销毁时的音效
    private AudioSource audioSource;    // 音效播放组件

    private void Start()
    {
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
                // Get player velocity
                Vector3 playerVelocity = playerRigidbody.velocity;

                // Show player speed
                Debug.Log("Player velocity: " + playerVelocity.magnitude);

                if (playerVelocity.magnitude > brokenVel)
                {
                    //Debug.Log("Player velocity exceeded brokenVel, trigger object will be destroyed.");

                    if (transform.parent != null)
                    {
                        if (destructionSound != null)
                        {
                            audioSource.PlayOneShot(destructionSound);
                        }
                        Destroy(transform.parent.gameObject); // destory cube
                    }
                }
            }

        }
    }

}
