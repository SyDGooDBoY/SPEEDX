using System.Collections;
using UnityEngine;

public class ObjectShake : MonoBehaviour
{
    public float shakeDuration = 2.0f;  // ��������ʱ��
    public float initialShakeMagnitude = 0.2f; // ��ʼ��������
    public float dampingSpeed = 1.0f;   // ˥���ٶ�

    private Vector3 originalPosition;   // ��¼����ĳ�ʼλ��

    private void OnEnable()
    {
        // ��¼����ĳ�ʼλ��
        originalPosition = transform.localPosition;
        // ��ʼ����
        StartCoroutine(ShakeObject());
    }

    public IEnumerator ShakeObject()
    {
        float elapsed = 0.0f;

        while (elapsed < shakeDuration)
        {
            // ����ʱ��˥����������
            float currentMagnitude = Mathf.Lerp(initialShakeMagnitude, 0, elapsed / shakeDuration);

            // ���������X��Yƫ����
            float offsetX = Random.Range(-1f, 1f) * currentMagnitude;
            float offsetY = Random.Range(-1f, 1f) * currentMagnitude;

            // Ӧ��X��Y���������ƫ����
            transform.localPosition = originalPosition + new Vector3(offsetX, offsetY, 0);

            elapsed += Time.deltaTime * dampingSpeed; // ���¾�����ʱ�䣬����˥���ٶ�
            yield return null; // �ȴ���һ֡
        }

        // �ָ�����ĳ�ʼλ��
        transform.localPosition = originalPosition;

        // �����������������ű�
        this.enabled = false;
    }
}
