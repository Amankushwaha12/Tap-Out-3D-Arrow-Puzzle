using UnityEngine;
using UnityEngine.UI;
using TMPro; 
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI levelText;
    public SceneTransitionGame sceneTransitionGame;
    
    [Header("System References")]
    public LevelManager levelManager; // Drag your LevelManager component here in the inspector
    public static GameManager Instance;
    public LineManager lineManager;
    public LivesManager livesManager;

    [Header("Screen Manger")]
    public bool isCompleted;

    [Header("Mobile")]
    public float left;
    public float right, top, bottom;

    


    void Awake()
    {
        Instance = this;
        livesManager = GetComponent<LivesManager>();
        Vector3 min = Camera.main.ViewportToWorldPoint(new Vector3(0, 0));
        Vector3 max = Camera.main.ViewportToWorldPoint(new Vector3(1, 1));

        // left = min.x;
        // right = max.x;
        // bottom = min.y;
        // top = max.y;
    }

    private void Start()
    {
        sceneTransitionGame.AtStart();

        // Subscribe to Level Manager events
        if (levelManager != null)
        {
            levelManager.OnLevelStarted += UpdateLevelUI;
            levelManager.OnLevelCompleted += ShowLevelCompleteScreen;
            levelManager.OnLevelFailed += ShowGameOverScreen;
        }

        livesManager.GetExtraLives += GetExtrLives;
    }
    public void GetExtrLives()
    {
        sceneTransitionGame.ShowScreen(sceneTransitionGame.playerPanel);
    }

    private void OnDestroy()
    {
        if (levelManager != null)
        {
            levelManager.OnLevelStarted -= UpdateLevelUI;
            levelManager.OnLevelCompleted -= ShowLevelCompleteScreen;
            levelManager.OnLevelFailed -= ShowGameOverScreen;
        }
    }

    // --- Checking Zone ---
    public bool IsHeadOutsideScreen(Vector3 p)
    {
        return p.x < left || p.x > right ||
            p.y < bottom || p.y > top;
    }

    // --- UI EVENT RESPONSES ---

    private void UpdateLevelUI(int currentLevelNumber)
    {
        // Make sure panels are hidden when a new level starts        
        if (levelText != null)
        {
            levelText.text = "LEVEL " + currentLevelNumber;
        }
    }

    private void ShowLevelCompleteScreen()
    {
        sceneTransitionGame.ShowScreen(sceneTransitionGame.victoryPanel);
    }

    private void ShowGameOverScreen()
    {
        sceneTransitionGame.ShowScreen(sceneTransitionGame.defeatPanel);
    }

    // --- BUTTON CLICKS ---
    // Link these public methods to your UI Buttons' OnClick() events in the Inspector!

    public void OnNextLevelButtonClicked()
    {
        sceneTransitionGame.ShowScreen(sceneTransitionGame.playerPanel);
    }

    public void OnRetryButtonClicked()
    {
        sceneTransitionGame.ShowScreen(sceneTransitionGame.playerPanel);
        if (levelManager != null)
        {
            levelManager.RetryCurrentLevel();
        }
    }
    
    public void Menu()
    {
        if(BootstrapManager.Instance) BootstrapManager.Instance.RequestSceneLoad("Menu");
        else Debug.LogWarning("Bootstrap Manger Not found.!!!");
    }
}