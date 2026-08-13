using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class BootstrapManager : MonoBehaviour
{
    // 1. Singleton Instance
    public static BootstrapManager Instance { get; private set; }
    [Header("UI Elements")]
    public Slider loadingBar;
    public TextMeshProUGUI statusText;
    public string sceneName;
    private bool isLoading = false;
    public PanelAnimator loadingScreen;
    public PanelAnimator currentScreen;

    // UI references that will be updated by the active scene
    private Slider _loadingBar;
    private TextMeshProUGUI _statusText;
    private PanelAnimator _loadingScreen;
    private PanelAnimator _currentScreen;

    private void Awake()
    {
        // 2. Singleton Setup
        if (Instance == null)
        {
            Instance = this;
            // Only use DontDestroyOnLoad if this BootstrapManager 
            // lives in a "Loading Scene" that stays alive.
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Start()
    {
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        // Start loading the scene in the background
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

        // Prevent the scene from activating immediately when it reaches 90%
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            // progress is 0 to 0.9. Map it to 0 to 1 for the slider.
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            
            loadingBar.value = progress;
            statusText.text = $"{(progress * 100):F0}%";

            // If loading is nearly finished
            if (operation.progress >= 0.9f)
            {                
                // Allow the scene to finish loading
                operation.allowSceneActivation = true;
            }

            yield return null;
        }
        
        statusText.text = $"{(100):F0}%";
    }


    // Public method to trigger the loading from ANY script
    public void RequestSceneLoad(string sceneName)
    {
        if (isLoading) return;
        StartCoroutine(LoadSceneWithProgress(sceneName));
    }
    public void RegisterUI(Slider bar, TextMeshProUGUI text, PanelAnimator loader, PanelAnimator current)
    {
        _loadingBar = bar;
        _statusText = text;
        _loadingScreen = loader;
        _currentScreen = current;
    }

    // Now your loading coroutine uses these cached references
    private IEnumerator LoadSceneWithProgress(string sceneName)
    {
        isLoading = true;
        
        // Use the cached references safely
        if (_currentScreen != null) _currentScreen.HidePanel(true);
        if (_loadingScreen != null) _loadingScreen.ShowPanel(true);
        
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            if (_loadingBar != null) _loadingBar.value = progress;
            if (_statusText != null) _statusText.text = $"{(progress * 100):F0}%";

            if (operation.progress >= 0.9f) operation.allowSceneActivation = true;
            yield return null;
        }
        isLoading = false;
    }

    
}