using TMPro;
using UnityEngine;

public class TimeUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timeText;
    public TimeScoreSystem timeScoreSystem;
    private float passTime;

    // Start is called before the first frame update
    void Start()
    {
        // timeText = transform.Find("Timer").GetComponent<TextMeshProUGUI>();
        // timeScoreSystem = transform.Find("TimeScoreSystem").GetComponent<TimeScoreSystem>();
        passTime = 0;
    }

    void Update()
    {
        passTime = timeScoreSystem.currentTime;
        var minutes = Mathf.FloorToInt(passTime / 60F);
        var seconds = Mathf.FloorToInt(passTime - minutes * 60);
        var milliseconds = Mathf.FloorToInt((passTime * 100) % 100); // Two digits for milliseconds
        timeText.text = $"<mspace=0.6em>{minutes:00}:{seconds:00}.{milliseconds:00}</mspace>";
    }
}