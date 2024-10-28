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

    // public int loadIndex;

    // public float fallSpeed = 2.0f;
    public GameObject fadePanel;
    public float fadeDuration = 3.0f;
    public GameObject player;
    public TimeScoreSystem timeScoreSystem;

    private CanvasGroup fadeCanvasGroup;

    // private bool isFalling = false; // Track if the platform has started falling
    public TextMeshProUGUI time;
    public AudioSource audioSource;
    public AudioClip winSound;
    public AudioSource bgm;

    void Start()

    {
        fadeCanvasGroup = fadePanel.GetComponent<CanvasGroup>();
        if (fadeCanvasGroup == null)
        {
            fadeCanvasGroup = fadePanel.GameObject().AddComponent<CanvasGroup>();
        }

        fadeCanvasGroup.alpha = 0; // Ensure initial alpha is 0
        if (GetComponent<AudioSource>() == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        else
        {
            audioSource = GetComponent<AudioSource>();
        }

        bgm = GameObject.Find("BGM").GetComponent<AudioSource>();
        winSound = Resources.Load<AudioClip>("Sound/NEW SOUNDS/WIN");
    }

    private void OnCollisionEnter(Collision collision)
    {
        //if current scene called level 1 run this code
        if (SceneManager.GetActiveScene().name == "Level 1" || SceneManager.GetActiveScene().name == "Level 2" ||
            SceneManager.GetActiveScene().name == "Level 3" || SceneManager.GetActiveScene().name == "Level 4" &&
            collision.gameObject.CompareTag("Player"))
        {
            timeScoreSystem.isGameRunning = false;
            bgm.Stop();
            audioSource.PlayOneShot(winSound);
            fadePanel.SetActive(true);
            player.GetComponent<PlayerMovement>().inputEnabled = false;
            StartCoroutine(Level1EndFade());
            int currentLevelIndex = SceneManager.GetActiveScene().buildIndex;
            string currentLevelID = SceneManager.GetSceneByBuildIndex(currentLevelIndex).name;
            SaveManager.Instance.UpdateBestTime(currentLevelID, timeSys.currentTime);

            int nextLevelIndex = currentLevelIndex + 1;

            if (nextLevelIndex < SceneManager.sceneCountInBuildSettings)
            {
                SaveManager.Instance.UnlockLevel("Level " + currentLevelIndex);
            }
        }

        if (SceneManager.GetActiveScene().name == "Level 5" && collision.gameObject.CompareTag("Player"))
        {
            timeScoreSystem.isGameRunning = false;
            bgm.Stop();
            audioSource.PlayOneShot(winSound);
            fadePanel.SetActive(true);
            player.GetComponent<PlayerMovement>().inputEnabled = false;
            StartCoroutine(Level2EndFade());
            int currentLevelIndex = SceneManager.GetActiveScene().buildIndex;
            string currentLevelID = SceneManager.GetSceneByBuildIndex(currentLevelIndex).name;
            SaveManager.Instance.UpdateBestTime(currentLevelID, timeSys.currentTime);
        }

        if (SceneManager.GetActiveScene().name == "TUT" && collision.gameObject.CompareTag("Player"))
        {
            timeScoreSystem.isGameRunning = false;
            bgm.Stop();
            audioSource.PlayOneShot(winSound);
            fadePanel.SetActive(true);
            player.GetComponent<PlayerMovement>().inputEnabled = false;
            StartCoroutine(TUTEndFade());
            // int currentLevelIndex = SceneManager.GetActiveScene().buildIndex;
            // string currentLevelID = SceneManager.GetSceneByBuildIndex(currentLevelIndex).name;
            // SaveManager.Instance.UpdateBestTime(currentLevelID, timeSys.currentTime);
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

    IEnumerator TUTEndFade()
    {
        GameObject timer = GameObject.Find("Timer");
        // timer.SetActive(false);
        fadePanel.SetActive(true);

        // 获取并格式化时间
        // float passTime = timeScoreSystem.currentTime;
        // string formattedTime = FormatTime(passTime);
        // time.text = $"<mspace=0.6em>{formattedTime}</mspace>";

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