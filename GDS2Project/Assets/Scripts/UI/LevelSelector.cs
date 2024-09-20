using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelector : MonoBehaviour
{
    public int level; // Level number
    public TextMeshProUGUI levelText;
    public Button levelButton; // Reference to the Button component

    void Start()
    {
        levelText.text = "Level " + level;
        CheckLevelUnlocked();
    }

    void CheckLevelUnlocked()
    {
        // Check if the level is unlocked using SaveManager
        if (SaveManager.Instance.IsLevelUnlocked("Level " + level))
        {
            levelButton.interactable = true;
        }
        else
        {
            levelButton.interactable = false;
            // Optionally set a different text or color to indicate it's locked
            levelText.color = Color.gray; // Change color to gray if locked
        }
    }

    public void OpenLevel()
    {
        if (levelButton.interactable)
        {
            SceneManager.LoadScene("Level " + level);
        }
    }
}