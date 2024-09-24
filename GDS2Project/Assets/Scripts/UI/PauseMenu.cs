using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public bool GameIsPaused = false;
    public GameObject pauseMenuUI;

    public GameObject crosshair;
    public Slider rotationSpeedSlider;
    public PlayerCam playerCam;

    private void Start()
    {
        rotationSpeedSlider.value = playerCam.rotationSpeed;
        rotationSpeedSlider.onValueChanged.AddListener(HandleSliderChange);
    }

    void HandleSliderChange(float value)
    {
        playerCam.rotationSpeed = value;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                crosshair.SetActive(true);
                ResumeGame();
            }
            else
            {
                crosshair.SetActive(false);
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        GameIsPaused = true;
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        Debug.Log("Pause Game");
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ResumeGame()
    {
        GameIsPaused = false;
        crosshair.SetActive(true);
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        Debug.Log("Resume Game");
        EventSystem.current.SetSelectedGameObject(null);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void RestartGame()
    {
        GameIsPaused = false;
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Invoke("LockCursor", 0.1f);
    }

    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void HomeGame()
    {
        SceneManager.LoadScene(0);
        Time.timeScale = 1f;
        // Debug.LogWarning("Home function is not implemented yet.");
    }

    public void LevelSelection()
    {
        SceneManager.LoadScene(1);
        Time.timeScale = 1f;
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        Debug.Log("Exit game in editor"); //Quit the game function for editor
#else
        Application.Quit(); //Quit the game function for build
#endif
    }
}