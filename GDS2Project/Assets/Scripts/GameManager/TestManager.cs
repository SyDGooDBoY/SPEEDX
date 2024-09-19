using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestManager : MonoBehaviour
{
    // just for test
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            UpdateTime();
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            UnlockLevel();
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            UnlockWeapon();
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            DisplayBestTimes();
        }
    }

    // random update fastest time
    private void UpdateTime()
    {
        float newTime = Random.Range(50.0f, 200.0f);  
        SaveManager.Instance.UpdateBestTime("Level1", newTime); 
        Debug.Log($"Updated Level1 time to: {newTime}");
    }


    private void UnlockLevel()
    {
        SaveManager.Instance.UnlockLevel("Level2"); 
        Debug.Log("Unlocked Level2");
    }


    private void UnlockWeapon()
    {
        SaveManager.Instance.UnlockWeapon("Sword"); 
        Debug.Log("Unlocked weapon: Sword");
    }


    private void DisplayBestTimes()
    {
        SaveManager.Instance.DisplayBestTimes(); 
    }
}
