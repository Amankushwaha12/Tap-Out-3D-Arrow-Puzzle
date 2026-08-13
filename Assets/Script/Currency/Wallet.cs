using UnityEngine;
using System.IO;
using System;
using GooglePlayGames;
using GooglePlayGames.BasicApi; // Add this one
using GooglePlayGames.BasicApi.SavedGame; // And keep this one

public static class Wallet
{
    private static string filePath = Path.Combine(Application.persistentDataPath, "wallet.json");
    public static GameData data = new GameData();
    
    public static event Action OnCoinsChanged;

    // Static constructor loads the data immediately when the class is first accessed
    static Wallet()
    {
        Load();
    }

    #region Coin Realted Stuffs
    // This property keeps the exact same name for your other scripts
    public static int Coins
    {
        get => data.coins;
        set
        {
            data.coins = value;
            Save();
            OnCoinsChanged?.Invoke();
        }
    }
    public static bool TryPurchase(int cost)
    {
        if (Coins >= cost)
        {
            Coins -= cost;
            return true;
        }
        return false;
    }
    #endregion
    #region Basic Load And Save
    public static void Save()
    {
        data.PrepareForSave(); // Convert Dictionary to Lists
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(filePath, json);
    }
    private static void Load()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            data = JsonUtility.FromJson<GameData>(json);
            data.PrepareAfterLoad();
            Debug.Log("Wallet Loaded. Coins found: " + data.coins); // ADD THIS
            Debug.Log("--- Printing All Wallet Keys ---");
            foreach (string key in data.activeItems.Keys)
            {
                int value = data.activeItems[key];
                Debug.Log($"Key: '{key}' | Value (ID): {value}");
            }
            Debug.Log("--- End of Keys ---"); // ADD THIS
        }
        else
        {
            data = new GameData();
            Save();
            Debug.Log("New Wallet Created."); // ADD THIS
        }
    }
    #endregion
    #region  Cloud 
    public static void RestorePurchasesFromCloud(string jsonFromCloud)
    {
        try
        {
            // 1. Safety Check: Ensure string is not empty
            if (string.IsNullOrEmpty(jsonFromCloud))
            {
                Debug.LogError("Cloud JSON is null or empty!");
                PopupManager.Instance?.Show("Restore failed: No data found.");
                return;
            }

            // 2. Parse the incoming cloud JSON
            GameData cloudData = JsonUtility.FromJson<GameData>(jsonFromCloud);
            
            // 3. Overwrite local data
            data = cloudData;

            // 4. Save to local storage
            Save();

            // 5. Trigger event so all UI components refresh
            OnCoinsChanged?.Invoke(); 
            
            Debug.Log("Purchases and progress restored from cloud.");

            // 6. SHOW SUCCESS POPUP
            if (PopupManager.Instance != null)
            {
                PopupManager.Instance.Show("Purchases Restored Successfully!");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error restoring cloud data: " + e.Message);
            
            // SHOW ERROR POPUP
            if (PopupManager.Instance != null)
            {
                PopupManager.Instance.Show("Restore failed: Data error.");
            }
        }
    }
    public static void SaveToCloud()
    {
        // 1. Safety Check: Is the platform even active?
        if (PlayGamesPlatform.Instance == null)
        {
            Debug.LogWarning("PlayGamesPlatform is not initialized!");
            return;
        }

        // 2. Safety Check: Is the user logged in?
        if (!PlayGamesPlatform.Instance.IsAuthenticated())
        {
            Debug.LogWarning("User is not authenticated. Cannot save to cloud.");
            return;
        }

        string jsonToSave = JsonUtility.ToJson(data);
        byte[] dataToSave = System.Text.Encoding.UTF8.GetBytes(jsonToSave);

        var savedGameClient = PlayGamesPlatform.Instance.SavedGame;
        
        savedGameClient.OpenWithAutomaticConflictResolution("wallet_save", 
            DataSource.ReadCacheOrNetwork, 
            ConflictResolutionStrategy.UseLongestPlaytime, 
            (status, game) => 
            {
                if (status == SavedGameRequestStatus.Success)
                {
                    // CORRECT SYNTAX: Use 'new' to instantiate the builder
                    SavedGameMetadataUpdate.Builder builder = new SavedGameMetadataUpdate.Builder();
                    
                    // You can add metadata updates if you like
                    builder.WithUpdatedDescription("Saved at " + System.DateTime.Now);
                    
                    SavedGameMetadataUpdate updatedMetadata = builder.Build();

                    // Now commit the update
                    savedGameClient.CommitUpdate(game, updatedMetadata, dataToSave, (saveStatus, metadata) => 
                    {
                        if (saveStatus == SavedGameRequestStatus.Success)
                            Debug.Log("Cloud Save Successful!");
                        else
                            Debug.LogError("Cloud Save Failed: " + saveStatus);
                    });
                }
                else
                {
                    Debug.LogError("Failed to open save file for cloud update: " + status);
                }
            });
    }
    #endregion
    #region Level Info
    public static void SaveBestTime(int levelNumber, float timeTaken)
    {
        int index = data.bestTimeLevelNumbers.IndexOf(levelNumber);

        if (index == -1)
        {
            // Level not found: add new record
            data.bestTimeLevelNumbers.Add(levelNumber);
            data.bestTimes.Add(timeTaken);
        }
        else if (timeTaken < data.bestTimes[index])
        {
            // Level found: update if the new time is faster
            data.bestTimes[index] = timeTaken;
        }

        Save(); // Assuming your Save method is inside Wallet
    }
    public static float GetBestTime(int levelNumber)
    {
        int index = data.bestTimeLevelNumbers.IndexOf(levelNumber);
        return (index != -1) ? data.bestTimes[index] : -1f;
    }
    public static void PrintAllLevelScores()
    {
        Debug.Log("--- Level Best Times Report ---");
        
        if (data.bestTimeLevelNumbers.Count == 0)
        {
            Debug.Log("No scores recorded yet.");
            return;
        }

        for (int i = 0; i < data.bestTimeLevelNumbers.Count; i++)
        {
            int levelNum = data.bestTimeLevelNumbers[i];
            float time = data.bestTimes[i];
            Debug.Log($"Level {levelNum}: {time:F2} seconds");
        }
    }
    #endregion
}