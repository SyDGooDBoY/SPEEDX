using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreUI : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI scoreText;

    private float score;
    public TimeScoreSystem timeScoreSystem;

    // Start is called before the first frame update
    void Start()
    {
        score = timeScoreSystem.currentScore;
    }

    // Update is called once per frame
    void Update()
    {
        score = timeScoreSystem.currentScore;
        scoreText.text = $"Score: {score:F2}";
    }
}