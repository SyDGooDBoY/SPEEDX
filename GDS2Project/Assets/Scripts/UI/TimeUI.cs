using TMPro;
using UnityEngine;

public class TimeUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timeText;

    private float passTime;

    // Start is called before the first frame update
    void Start()
    {
        timeText = transform.Find("Timer").GetComponent<TextMeshProUGUI>();
        passTime = 0;
        
    }

    void Update()
    {
        passTime += Time.deltaTime;
        var minutes = Mathf.FloorToInt(passTime / 60F);
        var seconds = Mathf.FloorToInt(passTime - minutes * 60);
        var milliseconds = Mathf.FloorToInt((passTime * 100) % 100); // Two digits for milliseconds
        timeText.text = $"<mspace=0.6em>{minutes:00}:{seconds:00}.{milliseconds:00}</mspace>";
    }
}