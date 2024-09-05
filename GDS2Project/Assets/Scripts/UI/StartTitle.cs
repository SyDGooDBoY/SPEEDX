using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartTitle : MonoBehaviour
{
    public RectTransform titleRect; // UI元素的RectTransform
    public float duration = 2.0f; // 移动持续时间
    private Vector3 startPos = new Vector3(-113, 356, 32550); // 起始位置
    private Vector3 endPos = new Vector3(-113, 356, 0); // 结束位置
    private float elapsedTime = 0f; // 已过时间

    void Start()
    {
        // 设置初始位置
        titleRect.position = startPos;
    }

    void Update()
    {
        // 如果动画尚未完成
        if (elapsedTime < duration)
        {
            // 更新已过时间
            elapsedTime += Time.deltaTime;
            // 计算新的位置
            Vector3 newPos = Vector3.Lerp(startPos, endPos, elapsedTime / duration);
            // 设置UI元素的新位置
            titleRect.position = newPos;
        }
        else if (titleRect.position != endPos) // 确保在动画结束后精确设置位置
        {
            // 动画完成后，确保精确设置最终位置
            titleRect.position = endPos;
        }
    }
}