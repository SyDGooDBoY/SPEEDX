using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UnlockAll : MonoBehaviour
{
    public void UnlockLevel()
    {
        SaveManager.Instance.UnlockLevel("Level 2");
        SaveManager.Instance.UnlockLevel("Level 3");
        SaveManager.Instance.UnlockLevel("Level 4");
        SaveManager.Instance.UnlockLevel("Level 5");

        SceneManager.LoadScene("StartScene");

        Debug.Log("Unlocked Level 2");
    }

    public void Reload()
    {
        SaveManager.Instance.LoadGame();
        SceneManager.LoadScene("StartScene");
    }

    public void Clear()
    {
        SaveManager.Instance.ClearGameData();
        SceneManager.LoadScene("StartScene");
    }
}