using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelector : MonoBehaviour
{
    //public int level;

    //public TextMeshProUGUI levelText;

    //// Start is called before the first frame update
    //void Start()
    //{
    //    levelText.text = "Level " + level.ToString();
    //}

    //// Update is called once per frame
    //public void OpenLevel()
    //{
    //    SceneManager.LoadScene("Level " + level.ToString());
    //}


    //NOT SURE IF THE CODE BELOW IS RIGHT OR NOT


    public int level; // Level number
    public TextMeshProUGUI levelText;
    public Image levelImage;
    public Button levelButton; // Reference to the Button component

    void Start()
    {
        levelText.text = level.ToString();
        levelImage = GetComponent<Image>();
        CheckLevelUnlocked();
    }

    void CheckLevelUnlocked()
    {
        // Check if the level is unlocked using SaveManager
        if (SaveManager.Instance.IsLevelUnlocked("Level " + level))
        {
            // levelButton.GetComponent<EventTrigger>().enabled = true;

            levelButton.interactable = true;
        }
        else
        {
            levelButton.interactable = false;
            // Disable EventTrigger component
            // levelButton.GetComponent<EventTrigger>().enabled = false;
            // Optionally set a different text or color to indicate it's locked
            levelText.color = Color.gray; // Change color to gray if locked
            levelImage.color = Color.gray; // Change color to gray if locked
        }
    }

    public void OpenLevel()
    {
        if (levelButton.interactable)
        {
            SceneManager.LoadScene("Level " + level);
        }
    }

    public void OpenTutorial()
    {
        SceneManager.LoadScene("TUT");
    }
}