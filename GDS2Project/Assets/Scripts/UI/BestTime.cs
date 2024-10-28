using System;
using TMPro;
using UnityEngine;

public class BestTime : MonoBehaviour
{
    // Array to hold references to your TextMeshPro elements
    public TextMeshProUGUI[] bestTimeTexts;

    // Start is called before the first frame update
    void Start()
    {
        DisplayBestTimes();
    }

    void DisplayBestTimes()
    {
        // Assuming levels are named consistently as "Level 1", "Level 2", etc.
        for (int i = 0; i < bestTimeTexts.Length; i++)
        {
            var levelName = $"Level {i + 1}";
            var bestTime = SaveManager.Instance.GetBestTime(levelName);
            bestTimeTexts[i].text = FormatBestTime(levelName, bestTime);
        }
    }

    string FormatBestTime(string levelName, float? bestTime)
    {
        if (bestTime == null)
        {
            return $"{levelName}\n--:--:--";
        }
        else
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(bestTime.Value);
            return $"{levelName}\n{timeSpan.Minutes:00}:{timeSpan.Seconds:00}:{timeSpan.Milliseconds / 10:00}";
        }
    }
}