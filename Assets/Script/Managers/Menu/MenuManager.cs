using UnityEngine;
using UnityEngine.InputSystem;

public class MenuManager : MonoBehaviour
{
    
    // The static instance for global access
    public static MenuManager Instance { get; private set; }

    public SceneTransitionManager sceneTransitionManager;
    public LevelLoader levelLoader;
    public ShopElementLoader shopElementLoader;
    public SettingsController settingsController;
    public CoinDisplay coinDisplay;

    [Header("Game Settings")]
    public float gameSpeed = 1f;
    public string Level;

    private void Awake()
    {
        // Check if an instance already exists
        if (Instance != null && Instance != this)
        {
            // Destroy this instance because one already exists
            Destroy(gameObject);
            return;
        }

        // Set this as the singleton instance
        Instance = this;
        
        // Note: We intentionally DO NOT call DontDestroyOnLoad(gameObject);
        // This ensures the manager is destroyed when the scene unloads.
    }
    void Start()
    {
        Application.targetFrameRate = 60;
        coinDisplay.CoinTextUpdate();
        BootstrapManager.Instance.RegisterUI(sceneTransitionManager.loadingBar, sceneTransitionManager.statusText, sceneTransitionManager.loadingScreen, sceneTransitionManager.currentPanel);
        // if (AudioManager.Instance != null)
        // {
        //     AudioManager.Instance.musicSource.volume = 1f; // Or fade it in:
        //     AudioManager.Instance.FadeMusic(1f, 1.0f);
        // }

        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            // Call the Restore method we wrote earlier
            settingsController.OnRestorePurchaseClicked(); 
        }
        // AdManager.Instance.HideBanner();
    }
    void Update()
    {
        // Check if the Keyboard is connected and the 'P' key is pressed
        if (Keyboard.current != null && Keyboard.current.pKey.wasPressedThisFrame)
        {
            Wallet.Coins += 100;
            Debug.Log("Coins added! Current balance: " + Wallet.Coins);
            coinDisplay.CoinTextUpdate();
        }
        // Triggering the score report with the 'T' key
        if (Keyboard.current != null && Keyboard.current.tKey.wasPressedThisFrame)
        {
            Wallet.PrintAllLevelScores();
        }
    }

    public void Play()
    {
        string lastLevel = levelLoader.GetCurrentLevel().ToString();
        string secondLastLevel = (int.Parse(lastLevel)-1).ToString();
        // if(!LevelController.DoesSceneExist(lastLevel))
        //     if(LevelController.DoesSceneExist(secondLastLevel))
        //         sceneTransitionManager.LoadLevel(secondLastLevel);
        //     else
        //         Debug.Log("lastLevel: " + lastLevel);
        // else sceneTransitionManager.LoadLevel(lastLevel);
        sceneTransitionManager.LoadLevel(Level);
        
    }
    public void Quit()
    {
        Debug.Log("[AppManager] Quitting game...");
#if UNITY_EDITOR
        // This stops the game in the Unity Editor
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // This closes the app in a real build
        Application.Quit();
#endif
    }

    public void PlayClick()
    {
        // if (AudioManager.Instance != null)
        // {
        //     AudioManager.Instance.PlayButtonClick();
        // }
    }
}
