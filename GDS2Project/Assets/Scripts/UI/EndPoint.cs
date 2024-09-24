using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

public class Endpoint : MonoBehaviour
{
    public TimeScoreSystem timeSys;

    public int loadIndex;

    // public float fallSpeed = 2.0f;
    public GameObject fadePanel;
    public float fadeDuration = 3.0f;
    public GameObject player;
    public TimeScoreSystem timeScoreSystem;

    private CanvasGroup fadeCanvasGroup;

    // private bool isFalling = false; // Track if the platform has started falling
    public TextMeshProUGUI time;

    void Start()

    {
        fadeCanvasGroup = fadePanel.GetComponent<CanvasGroup>();
        if (fadeCanvasGroup == null)
        {
            fadeCanvasGroup = fadePanel.GameObject().AddComponent<CanvasGroup>();
        }

        fadeCanvasGroup.alpha = 0; // Ensure initial alpha is 0
    }

    private void OnCollisionEnter(Collision collision)
    {
        //if current scene called level 1 run this code
        if (SceneManager.GetActiveScene().name == "Level 1" && collision.gameObject.CompareTag("Player"))
        {
            timeScoreSystem.isGameRunning = false;
            // Time.timeScale = 0;
            fadePanel.SetActive(true);
            player.GetComponent<PlayerMovement>().inputEnabled = false;
            StartCoroutine(Level1EndFade());
            // get index and ID(scene name) of current level 
            int currentLevelIndex = SceneManager.GetActiveScene().buildIndex;
            string currentLevelID = SceneManager.GetSceneByBuildIndex(currentLevelIndex).name;
            // update best time
            SaveManager.Instance.UpdateBestTime(currentLevelID, timeSys.currentTime);

            // get index and ID(scene name) of next level
            int nextLevelIndex = currentLevelIndex + 1;

            // make sure the index is in the valid range
            if (nextLevelIndex < SceneManager.sceneCountInBuildSettings)
            {
                // Unlock next level and save
                SaveManager.Instance.UnlockLevel("Level " + currentLevelIndex);
            }

            // // Load Level Selection scene
            // SceneManager.LoadScene(loadIndex);
        }

        if (SceneManager.GetActiveScene().name == "Level 2" && collision.gameObject.CompareTag("Player"))
        {
            timeScoreSystem.isGameRunning = false;
            // Time.timeScale = 0;
            fadePanel.SetActive(true);
            player.GetComponent<PlayerMovement>().inputEnabled = false;
            StartCoroutine(Level2EndFade());
            // get index and ID(scene name) of current level 
            int currentLevelIndex = SceneManager.GetActiveScene().buildIndex;
            string currentLevelID = SceneManager.GetSceneByBuildIndex(currentLevelIndex).name;
            // update best time
            SaveManager.Instance.UpdateBestTime(currentLevelID, timeSys.currentTime);
        }
    }

    private string FormatTime(float timeInSeconds)
    {
        var minutes = Mathf.FloorToInt(timeInSeconds / 60F);
        var seconds = Mathf.FloorToInt(timeInSeconds - minutes * 60);
        var milliseconds = Mathf.FloorToInt((timeInSeconds * 100) % 100);
        return $"{minutes:00}:{seconds:00}:{milliseconds:00}";
    }

    IEnumerator Level1EndFade()
    {
        GameObject timer = GameObject.Find("Timer");
        timer.SetActive(false);
        fadePanel.SetActive(true);

        // 获取并格式化时间
        float passTime = timeScoreSystem.currentTime;
        string formattedTime = FormatTime(passTime);
        time.text = "Your time\n" + $"<mspace=0.6em>{formattedTime}</mspace>";

        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
            yield return null;
        }

        fadeCanvasGroup.alpha = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    IEnumerator Level2EndFade()
    {
        GameObject timer = GameObject.Find("Timer");
        timer.SetActive(false);
        fadePanel.SetActive(true);

        // 获取并格式化时间
        float passTime = timeScoreSystem.currentTime;
        string formattedTime = FormatTime(passTime);
        time.text = $"<mspace=0.6em>{formattedTime}</mspace>";

        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
            yield return null;
        }

        fadeCanvasGroup.alpha = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}