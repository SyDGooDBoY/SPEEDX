using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public Dictionary<string, float> LevelBestTimes = new Dictionary<string, float>(); // Best time for each level
    public List<string> UnlockedLevels = new List<string>(); // unlocked level
    public List<string> UnlockedWeapons = new List<string>(); // unlocked weapon
}

public class SaveManager : MonoBehaviour
{
    private static SaveManager instance;
    private string savePath;
    private GameData gameData;

    public static SaveManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<SaveManager>();
                if (instance == null)
                {
                    GameObject go = new GameObject("SaveManager");
                    instance = go.AddComponent<SaveManager>();
                }
            }

            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Initialize Path
        savePath = Path.Combine(Application.persistentDataPath, "gameData.json");
        Debug.Log("Save path: " + savePath);

        // Load
        LoadGame();
    }

    public void SaveGame()
    {
        string json = JsonUtility.ToJson(gameData, true); // save as JSON
        File.WriteAllText(savePath, json); // write
        Debug.Log("Game data saved to " + savePath);
    }

    public void LoadGame()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath); // read file
            gameData = JsonUtility.FromJson<GameData>(json); // Deserialize to gamedata
            Debug.Log("Game data loaded from " + savePath);
        }
        else
        {
            gameData = new GameData();
            gameData.UnlockedLevels.Add("Level 1");
            Debug.LogWarning("No save file found, creating new game data.");
            SaveGame();
        }
    }

    // Update best time for level
    public void UpdateBestTime(string levelID, float time)
    {
        if (gameData.LevelBestTimes.ContainsKey(levelID))
        {
            if (time < gameData.LevelBestTimes[levelID])
            {
                gameData.LevelBestTimes[levelID] = time;
                Debug.Log($"New best time for {levelID}: {time}");
            }
        }
        else
        {
            gameData.LevelBestTimes[levelID] = time;
            Debug.Log($"Set best time for new level {levelID}: {time}");
        }

        SaveGame();
    }

    // unlock level
    public void UnlockLevel(string levelID)
    {
        if (!gameData.UnlockedLevels.Contains(levelID))
        {
            gameData.UnlockedLevels.Add(levelID);
            Debug.Log($"Unlocked level: {levelID}");
            SaveGame();
        }
    }

    // unlock weapon
    public void UnlockWeapon(string weaponID)
    {
        if (!gameData.UnlockedWeapons.Contains(weaponID))
        {
            gameData.UnlockedWeapons.Add(weaponID); 
            Debug.Log($"Unlocked weapon: {weaponID}");
            SaveGame();
        }
    }

    // get best time for UI
    public float GetBestTime(string levelID)
    {
        return gameData.LevelBestTimes.ContainsKey(levelID) ? gameData.LevelBestTimes[levelID] : float.MaxValue;
    }

    // check function
    public bool IsLevelUnlocked(string levelID)
    {
        return gameData.UnlockedLevels.Contains(levelID);
    }

    public bool IsWeaponUnlocked(string weaponID)
    {
        return gameData.UnlockedWeapons.Contains(weaponID);
    }

    // Display fastest time for all levels
    public void DisplayBestTimes()
    {
        foreach (var entry in gameData.LevelBestTimes)
        {
            Debug.Log($"Level: {entry.Key}, Best Time: {entry.Value}");
        }
    }

    public void DisplayUnlockedItems()
    {
        Debug.Log("Unlocked Levels: " + string.Join(", ", gameData.UnlockedLevels));
        Debug.Log("Unlocked Weapons: " + string.Join(", ", gameData.UnlockedWeapons));
    }
}