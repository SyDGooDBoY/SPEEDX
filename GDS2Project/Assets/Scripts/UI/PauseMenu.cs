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
    public GameObject mainMenu;
    public GameObject settingsMenu;
private AudioSource playerAudioSource;

    private void Start()
    {
        rotationSpeedSlider.value = playerCam.rotationSpeed;
        rotationSpeedSlider.onValueChanged.AddListener(HandleSliderChange);
        mainMenu.SetActive(false);
        settingsMenu.SetActive(false);
        playerAudioSource = GameObject.Find("Player").GetComponent<AudioSource>();
        
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
        // AudioListener.pause = true;

        GameIsPaused = true;
        pauseMenuUI.SetActive(true);
        mainMenu.SetActive(true);
        Time.timeScale = 0f;
        Debug.Log("Pause Game");
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ResumeGame()
    {
        // AudioListener.pause = false;
        GameIsPaused = false;
        crosshair.SetActive(true);
        pauseMenuUI.SetActive(false);
        settingsMenu.SetActive(false);
        Time.timeScale = 1f;
        Debug.Log("Resume Game");
        EventSystem.current.SetSelectedGameObject(null);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void SettingsMenu()
    {
        settingsMenu.SetActive(true);
        mainMenu.SetActive(false);
    }
    public void BackToMainMenu()
    {
        settingsMenu.SetActive(false);
        mainMenu.SetActive(true);
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