using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeScoreSystem : MonoBehaviour
{
    public float maxTime = 300f; // ���ʱ�䣨����5���ӣ�
    public float maxScore = 100f; // ��ʼ��߷�
    private float currentTime;
    private float currentScore;
    private bool isGameRunning = false;

    public float scoreMultiplier = 1f; // ʱ��۷ֱ��ʣ�����Unity�е���

    public Text timeText; // ������ʾʱ���UI
    public Text scoreText; // ������ʾ������UI

    void Start()
    {
        
            ResetTimer();
            InvokeRepeating("LogTimeAndScore", 0f, 3f); // ÿ3�����һ��LogTimeAndScore����
        
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

    // ��ʼ����ʱ��
    public void ResetTimer()
    {
        currentTime = 0f;
        currentScore = maxScore;
        isGameRunning = true;
    }

    // ���¼�ʱ��
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

    // ���·���������ʱ����٣�
    private void UpdateScore()
    {
        currentScore = Mathf.Max(maxScore - (currentTime * scoreMultiplier), 0);
    }

    //������Ϸ
    private void EndGame()
    {
        isGameRunning = false;
        CancelInvoke("LogTimeAndScore"); // ֹͣ��־���
        // �����������Ϸ�����߼�������ʾ��Ϸ������Ļ��
    }

    // ��ͣ��Ϸ
    public void PauseGame()
    {
        isGameRunning = false;
    }

    // �ָ���Ϸ
    public void ResumeGame()
    {
        isGameRunning = true;
    }

    private void LogTimeAndScore()
    {
        Debug.Log($"Current Time: {currentTime:F2}, Current Score: {currentScore:F2}");
    }

    // ����UI��ʾ
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
