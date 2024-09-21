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
            SaveManager.Instance.DisplayUnlockedItems();
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            DisplayBestTimes();
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            SaveManager.Instance.ClearGameData();
        }
    }

    // random update fastest time
    private void UpdateTime()
    {
        float newTime = Random.Range(50.0f, 200.0f);  
        SaveManager.Instance.UpdateBestTime("Level 1", newTime); 
        Debug.Log($"Updated Level 1 time to: {newTime}");
    }


    private void UnlockLevel()
    {
        SaveManager.Instance.UnlockLevel("Level 2"); 
        Debug.Log("Unlocked Level 2");
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
