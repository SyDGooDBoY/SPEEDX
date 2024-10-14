using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    //public Transform camPos;  // �� Unity Inspector ������ CamPos ����
    // ����Э��
    public IEnumerator Shake(float duration, float magnitude)
    {
        // ��¼����ĳ�ʼ��תֵ
        Quaternion originalRotation = transform.localRotation;
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            // ����ƽ���� Z ��ڶ��������� Sin ������
            float zOffset = Mathf.Sin(elapsed * Mathf.PI * 2 * magnitude) * magnitude;

            // Ӧ�� Z ����תƫ��
            transform.localRotation = originalRotation * Quaternion.Euler(0, 0, zOffset);

            elapsed += Time.deltaTime;

            yield return null; // �ȴ���һ֡
        }

        // �ָ������ԭʼ��ת
        transform.localRotation = originalRotation;
    }
}
