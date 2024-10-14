using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BestTime : MonoBehaviour
{
    public TextMeshProUGUI bestTimeText1;

    public TextMeshProUGUI bestTimeText2;
    public TextMeshProUGUI bestTimeText3;


    // Start is called before the first frame update
    void Start()
    {
        //display best time in 2 decimal places, if no best time, display --:--
        var level1BestTime = SaveManager.Instance.GetBestTime("Level 1");
        var level2BestTime = SaveManager.Instance.GetBestTime("Level 2");
        var level3BestTime = SaveManager.Instance.GetBestTime("Level 3");

        if (level1BestTime == null)
        {
            bestTimeText1.text = "Level 1\n--:--:--";
        }
        else
        {
            var timeSpan = TimeSpan.FromSeconds(level1BestTime.Value);
            bestTimeText1.text =
                "Level 1\n" + $"{timeSpan.Minutes:00}:{timeSpan.Seconds:00}:{timeSpan.Milliseconds / 10:00}";
        }

        if (level2BestTime == null)
        {
            bestTimeText2.text = "Level 2\n--:--:--";
        }
        else
        {
            var timeSpan = TimeSpan.FromSeconds(level2BestTime.Value);
            bestTimeText2.text =
                "Level 2\n" + $"{timeSpan.Minutes:00}:{timeSpan.Seconds:00}:{timeSpan.Milliseconds / 10:00}";
        }

        if (level3BestTime == null)
        {
            bestTimeText3.text = "Level 3\n--:--:--";
        }
        else
        {
            var timeSpan = TimeSpan.FromSeconds(level3BestTime.Value);
            bestTimeText3.text =
                "Level 3\n" + $"{timeSpan.Minutes:00}:{timeSpan.Seconds:00}:{timeSpan.Milliseconds / 10:00}";
        }
    }
}