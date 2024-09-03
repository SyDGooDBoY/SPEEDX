using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeScoreSystem : MonoBehaviour
{
    public float maxTime = 300f; // 最大时间（例如5分钟）
    public float maxScore = 100f; // 初始最高分
    private float currentTime;
    private float currentScore;
    private bool isGameRunning = false;

    public float scoreMultiplier = 1f; // 时间扣分倍率，可在Unity中调整

    public Text timeText; // 用于显示时间的UI
    public Text scoreText; // 用于显示分数的UI

    void Start()
    {
        
            ResetTimer();
            InvokeRepeating("LogTimeAndScore", 0f, 3f); // 每3秒调用一次LogTimeAndScore方法
        
    }

    void Update()
    {
        if (isGameRunning)
        {
            UpdateTimer();
            UpdateScore();
            UpdateUI();
        }
    }

    // 初始化计时器
    public void ResetTimer()
    {
        currentTime = 0f;
        currentScore = maxScore;
        isGameRunning = true;
    }

    // 更新计时器
    private void UpdateTimer()
    {
        if (isGameRunning)
        {
            currentTime += Time.deltaTime;
            if (currentTime >= maxTime)
            {
                currentTime = maxTime;
                //EndGame();
            }
        }
    }

    // 更新分数（随着时间减少）
    private void UpdateScore()
    {
        currentScore = Mathf.Max(maxScore - (currentTime * scoreMultiplier), 0);
    }

    //结束游戏
    private void EndGame()
    {
        isGameRunning = false;
        CancelInvoke("LogTimeAndScore"); // 停止日志输出
        // 在这里添加游戏结束逻辑，如显示游戏结束屏幕等
    }

    // 暂停游戏
    public void PauseGame()
    {
        isGameRunning = false;
    }

    // 恢复游戏
    public void ResumeGame()
    {
        isGameRunning = true;
    }

    private void LogTimeAndScore()
    {
        Debug.Log($"Current Time: {currentTime:F2}, Current Score: {currentScore:F2}");
    }

    // 更新UI显示
    private void UpdateUI()
    {
        if (timeText != null)
        {
            timeText.text = $"Time: {currentTime:F2} / {maxTime:F2}";
        }

        if (scoreText != null)
        {
            scoreText.text = $"Score: {currentScore:F2}";
        }
    }
}
