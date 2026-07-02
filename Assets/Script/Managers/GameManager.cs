using UnityEngine;
using UnityEngine.UI;
using TMPro; // For TextMeshPro

public class GameManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI levelText;
    public GameObject gameOverPanel;
    public GameObject levelCompletePanel;
    
    [Header("System References")]
    public LevelManager levelManager; // Drag your LevelManager component here in the inspector
    public static GameManager Instance;
    public LineManager lineManager;

    
    [Header("Mobile")]
    public float left;
    public float right, top, bottom;

    void Awake()
    {
        Instance = this;
        Vector3 min = Camera.main.ViewportToWorldPoint(new Vector3(0, 0));
        Vector3 max = Camera.main.ViewportToWorldPoint(new Vector3(1, 1));

        left = min.x;
        right = max.x;
        bottom = min.y;
        top = max.y;
    }

    private void Start()
    {
        // Hide screens on boot
        gameOverPanel.SetActive(false);
        levelCompletePanel.SetActive(false);

        // Subscribe to Level Manager events
        if (levelManager != null)
        {
            levelManager.OnLevelStarted += UpdateLevelUI;
            levelManager.OnLevelCompleted += ShowLevelCompleteScreen;
            levelManager.OnLevelFailed += ShowGameOverScreen;
        }
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
        gameOverPanel.SetActive(false);
        levelCompletePanel.SetActive(false);
        
        if (levelText != null)
        {
            levelText.text = "LEVEL " + currentLevelNumber;
        }
    }

    private void ShowLevelCompleteScreen()
    {
        levelCompletePanel.SetActive(true);
    }

    private void ShowGameOverScreen()
    {
        gameOverPanel.SetActive(true);
    }

    // --- BUTTON CLICKS ---
    // Link these public methods to your UI Buttons' OnClick() events in the Inspector!

    public void OnNextLevelButtonClicked()
    {
        levelCompletePanel.SetActive(false);
    }

    public void OnRetryButtonClicked()
    {
        gameOverPanel.SetActive(false);
        if (levelManager != null)
        {
            levelManager.RetryCurrentLevel();
        }
    }
    
}