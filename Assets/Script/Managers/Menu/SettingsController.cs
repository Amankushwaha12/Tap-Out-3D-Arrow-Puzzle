using UnityEngine;
using UnityEngine.UI;
using System;
using DG.Tweening;
using GooglePlayGames;
using GooglePlayGames.BasicApi; // Add this one
using GooglePlayGames.BasicApi.SavedGame; // And keep this one

public class SettingsController : MonoBehaviour
{
    public static event Action OnSettingsChanged;

    public Slider musicSlider;
    public Slider soundSlider;
    public Slider vibrationSlider;

    void Start()
    {
        // Load initial state from JSON
        musicSlider.value = Wallet.data.musicEnabled ? 1 : 0;
        soundSlider.value = Wallet.data.soundEnabled ? 1 : 0;
        vibrationSlider.value = Wallet.data.vibrationEnabled ? 1 : 0;

        musicSlider.onValueChanged.AddListener((v) => SaveSetting("Music", v));
        soundSlider.onValueChanged.AddListener((v) => SaveSetting("Sound", v));
        vibrationSlider.onValueChanged.AddListener((v) => SaveSetting("Vibe", v));
    }

    private void SaveSetting(string type, float value)
    {
        bool enabled = value == 1;

        switch (type)
        {
            case "Music": Wallet.data.musicEnabled = enabled; break;
            case "Sound": Wallet.data.soundEnabled = enabled; break;
            case "Vibe":  Wallet.data.vibrationEnabled = enabled; break;
        }

        Wallet.Save(); // Save everything to JSON
        OnSettingsChanged?.Invoke();
    }

    public void ToggleMusic() => FlipState(musicSlider, "Music");
    public void ToggleSound() => FlipState(soundSlider, "Sound");
    public void ToggleVibration() => FlipState(vibrationSlider, "Vibe");

    private void FlipState(Slider s, string type)
    {
        Debug.Log("Button Called");
        float targetValue = (s.value == 1) ? 0 : 1;
        s.DOKill();
        s.DOValue(targetValue, 0.4f).SetEase(Ease.OutCubic);
        SaveSetting(type, targetValue);
    }

    public void OnRestorePurchaseClicked()
    {
        // 1. UI Feedback: Start the process
        if (PopupManager.Instance != null)
            PopupManager.Instance.Show("Connecting to Cloud...");

        // 2. Check if the platform is initialized
        if (PlayGamesPlatform.Instance == null)
        {
            Debug.LogWarning("Google Play Platform is not initialized yet!");
            PopupManager.Instance?.Show("Error: Cloud not initialized.");
            return; 
        }

        // 3. Check if the user is authenticated
        if (!PlayGamesPlatform.Instance.IsAuthenticated())
        {
            Debug.Log("User not authenticated. Attempting login...");
            PlayGamesPlatform.Instance.Authenticate((success) => 
            {
                if (success == SignInStatus.Success) OnRestorePurchaseClicked(); // Retry
                else PopupManager.Instance?.Show("Login failed.");
            });
            return;
        }

        ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;

        savedGameClient.OpenWithAutomaticConflictResolution("wallet_save", 
            DataSource.ReadCacheOrNetwork, 
            ConflictResolutionStrategy.UseLongestPlaytime, 
            (status, game) => 
            {
                if (status == SavedGameRequestStatus.Success)
                {
                    savedGameClient.ReadBinaryData(game, (readStatus, data) => 
                    {
                        if (readStatus == SavedGameRequestStatus.Success)
                        {
                            if (data != null && data.Length > 0)
                            {
                                string jsonFromCloud = System.Text.Encoding.UTF8.GetString(data);
                                Wallet.data = JsonUtility.FromJson<GameData>(jsonFromCloud);
                                Wallet.Save();
                                
                                // SUCCESS MESSAGE
                                Debug.Log("Sync Complete!");
                                PopupManager.Instance?.Show("Purchases Restored!");
                            }
                            else
                            {
                                Debug.LogWarning("Cloud save file was empty.");
                                PopupManager.Instance?.Show("No save data found.");
                            }
                        }
                        else
                        {
                            Debug.LogError("Failed to read binary state: " + readStatus);
                            PopupManager.Instance?.Show("Restore failed (Read error).");
                        }
                    });
                }
                else
                {
                    Debug.LogError("Failed to open save file: " + status);
                    PopupManager.Instance?.Show("Restore failed (Connection error).");
                }
            });
    }
}