using System;
using System.Collections.Generic;

[System.Serializable]
public class LevelData
{
    public int levelNumber;
    public float bestTime;
}

[Serializable]
public class GameData
{
    public int coins = 0;
    public List<int> ownedItemIds = new List<int>();

    // 1. Dictionaries are not serializable by JsonUtility. 
    // We keep them for runtime, but mark them NonSerialized.
    [System.NonSerialized] 
    public Dictionary<string, int> activeItems = new Dictionary<string, int>()
    {
        { "Ball", 0 },  // Default ID
        { "Paint", 50 }, // Default ID
        { "Theme", 100 }  // Default ID
    };

    // 2. These two parallel lists ARE serializable and will store our data to disk.
    public List<string> activeItemKeys = new List<string>();
    public List<int> activeItemValues = new List<int>();

    // 3. Call this BEFORE saving to disk
    public void PrepareForSave()
    {
        activeItemKeys.Clear();
        activeItemValues.Clear();
        foreach (var pair in activeItems)
        {
            activeItemKeys.Add(pair.Key);
            activeItemValues.Add(pair.Value);
        }
    }

    // 4. Call this AFTER loading from disk
    public void PrepareAfterLoad()
    {
        activeItems.Clear();
        for (int i = 0; i < activeItemKeys.Count; i++)
        {
            activeItems[activeItemKeys[i]] = activeItemValues[i];
        }
    }

    // ... your other variables (musicEnabled, etc.) remain as they are
    public bool musicEnabled = true;
    public bool soundEnabled = true;
    public bool vibrationEnabled = true;
    public int currentLevel = 1;
    public List<int> completedLevelNumbers = new List<int>();
    public List<int> bestTimeLevelNumbers = new List<int>();
    public List<float> bestTimes = new List<float>();
}