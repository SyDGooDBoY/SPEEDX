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
    private void OnCollisionEnter(Collision collision)
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // get index and ID(scene name) of next level
        int nextLevelIndex = SceneManager.GetActiveScene().buildIndex + 1;
        string nextLevelID = SceneManager.GetSceneByBuildIndex(nextLevelIndex).name;

        // Unlock next level and save
        SaveManager.Instance.UnlockLevel(nextLevelID);
        SaveManager.Instance.SaveGame();

        SceneManager.LoadScene(1);
    }

    // public GameObject platform; // Reference to the platform GameObject
    //
    // // public float fallSpeed = 2.0f;
    // public GameObject fadePanel;
    // public GameObject messageText;
    // public GameObject scoreText;
    // public float fadeDuration = 5.0f;
    // public GameObject button;
    // public GameObject player;
    // public TimeScoreSystem timeScoreSystem;
    // private CanvasGroup fadeCanvasGroup;
    // // private bool isFalling = false; // Track if the platform has started falling
    // public TextMeshProUGUI time;
    //
    // void Start()
    // {
    //     fadeCanvasGroup = fadePanel.GetComponent<CanvasGroup>();
    //     if (fadeCanvasGroup == null)
    //     {
    //         fadeCanvasGroup = fadePanel.GameObject().AddComponent<CanvasGroup>();
    //     }
    //
    //     fadeCanvasGroup.alpha = 0; // Ensure initial alpha is 0
    // }
    //
    // void OnCollisionEnter(Collision collision)
    // {
    //     timeScoreSystem.isGameRunning = false;
    //     // Time.timeScale = 0;
    //     fadePanel.GameObject().SetActive(true);
    //     player.GetComponent<PlayerMovement>().inputEnabled = false; // ������ҿ���
    //     StartCoroutine(FallAndFade());
    // }
    //
    // IEnumerator FallAndFade()
    // {
    //     // Get initial position and calculate the end position
    //     Vector3 startPosition = platform.transform.position;
    //     Vector3 endPosition = new Vector3(startPosition.x, startPosition.y - 10, startPosition.z); // Lower by 10 units
    //
    //     float elapsedTime = 0;
    //     while (elapsedTime < fadeDuration)
    //     {
    //         // Move the platform down
    //         platform.transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / fadeDuration);
    //         // Increase the fadePanel alpha
    //         fadeCanvasGroup.alpha = Mathf.Lerp(0, 1, elapsedTime / fadeDuration);
    //         elapsedTime += Time.deltaTime;
    //         yield return null;
    //     }
    //
    //     // Ensure the platform is exactly at the end position and fadePanel alpha is 1
    //     platform.transform.position = endPosition;
    //     fadeCanvasGroup.alpha = 1;
    //     messageText.GameObject().SetActive(true);
    //     scoreText.GameObject().SetActive(true);
    //     button.SetActive(true);
    //     
    //     Cursor.lockState = CursorLockMode.None;
    //     Cursor.visible = true;
    //     Time.timeScale = 0;
    // }
}