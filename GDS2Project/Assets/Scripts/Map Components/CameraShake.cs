using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    //public Transform camPos;  // 在 Unity Inspector 中拖入 CamPos 对象
    // 抖动协程
    public IEnumerator Shake(float duration, float magnitude)
    {
        // 记录物体的初始旋转值
        Quaternion originalRotation = transform.localRotation;
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            // 生成平滑的 Z 轴摆动（比如用 Sin 函数）
            float zOffset = Mathf.Sin(elapsed * Mathf.PI * 2 * magnitude) * magnitude;

            // 应用 Z 轴旋转偏移
            transform.localRotation = originalRotation * Quaternion.Euler(0, 0, zOffset);

            elapsed += Time.deltaTime;

            yield return null; // 等待下一帧
        }

        // 恢复物体的原始旋转
        transform.localRotation = originalRotation;
    }
}
