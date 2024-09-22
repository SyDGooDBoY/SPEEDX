using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Springboard : MonoBehaviour
{
    public float jumpForce = 40f;
    private void OnTriggerEnter(Collider other)
    {
        // ����Ƿ����������������
        if (other.CompareTag("Player"))
        {
            Rigidbody playerRb = other.GetComponent<Rigidbody>(); // ��ȡ��ҵ�Rigidbody���

            if (playerRb != null)
            {
                // �������ҵĴ�ֱ�ٶȣ������ڵ���ʱʩ��������ۻ���
                playerRb.velocity = new Vector3(playerRb.velocity.x, 0f, playerRb.velocity.z);

                // �����ʩ��һ�����ϵ���
                playerRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

                // ���������Ϣ
                Debug.Log("Player hit the springboard and jumped!");
            }
        }
    }
}
