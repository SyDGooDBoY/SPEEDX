using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartUI : MonoBehaviour
{
    public void StartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }

    public void Options()
    {
        Debug.LogWarning("Options function is not implemented yet.");
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        Debug.Log("Exit game in editor"); //Quit the game function for editor
#else
        Application.Quit();//Quit the game function for build
#endif
    }
}