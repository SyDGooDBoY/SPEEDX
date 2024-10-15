using System.Collections;
using UnityEngine;

public class ObjectShake : MonoBehaviour
{
    public float shakeDuration = 2.0f;  // 抖动持续时间
    public float initialShakeMagnitude = 0.2f; // 初始抖动幅度
    public float dampingSpeed = 1.0f;   // 衰减速度

    private Vector3 originalPosition;   // 记录物体的初始位置

    private void OnEnable()
    {
        // 记录物体的初始位置
        originalPosition = transform.localPosition;
        // 开始抖动
        StartCoroutine(ShakeObject());
    }

    public IEnumerator ShakeObject()
    {
        float elapsed = 0.0f;

        while (elapsed < shakeDuration)
        {
            // 根据时间衰减抖动幅度
            float currentMagnitude = Mathf.Lerp(initialShakeMagnitude, 0, elapsed / shakeDuration);

            // 生成随机的X和Y偏移量
            float offsetX = Random.Range(-1f, 1f) * currentMagnitude;
            float offsetY = Random.Range(-1f, 1f) * currentMagnitude;

            // 应用X和Y的随机抖动偏移量
            transform.localPosition = originalPosition + new Vector3(offsetX, offsetY, 0);

            elapsed += Time.deltaTime * dampingSpeed; // 更新经过的时间，乘以衰减速度
            yield return null; // 等待下一帧
        }

        // 恢复物体的初始位置
        transform.localPosition = originalPosition;

        // 抖动结束后禁用这个脚本
        this.enabled = false;
    }
}
