using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BestTime : MonoBehaviour
{
    public TextMeshProUGUI bestTimeText;

    // Start is called before the first frame update
    void Start()
    {
        //display best time in 2 decimal places, if no best time, display --:--
        float bestTime = SaveManager.Instance.GetBestTime("Level 1");
        if (bestTime <= 0)
        {
            bestTimeText.text = "Level 1 Best time" + "\n" + "--:--";
        }
        else
        {
            bestTimeText.text = "Level 1 Best time" + "\n" + bestTime.ToString("F2");
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
}