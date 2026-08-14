using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections; // Added for sorting

public class LevelLoader : MonoBehaviour
{
    public GameObject buttonPrefab; 
    public Transform buttonParent; 
    private List<LevelObject> cachedLevels = new List<LevelObject>();
    public SceneTransitionManager sceneTransitionManager;
    public ScrollRect scrollRect; // Assign your Scroll View here
    public CustomButton leftArrowBtn;
    public CustomButton rightArrowBtn;
    public Image leftArrowBtnImage;
    public Image rightArrowBtnImage;
    public Color activeColor = Color.white;
    public Color inactiveColor = Color.grey;
    public int itemsThatFitOnScreen;
    private bool isInitialized, isLoading = false;

    void Start()
    {
        Application.targetFrameRate = 60;
        AllLevelData(); 
        SetLevelData(); 
        GenerateButtons();
        SetInitialArrowStates();
        scrollRect.onValueChanged.AddListener((vec) => UpdateArrowStates());
    }
    void Update()
    {
        if(Keyboard.current != null && Keyboard.current.oKey.isPressed)
        {
            SetCurrentLevel(1);
        }
    }

    void GenerateButtons()
    {
        int CurrentLevel = GetCurrentLevel();
        Debug.Log("GenerateButtons Called");
        foreach (LevelObject level in cachedLevels)
        {
            GameObject newButton = Instantiate(buttonPrefab, buttonParent);
            LevelButtonUI btnUI = newButton.GetComponent<LevelButtonUI>();
            btnUI.sceneTransitionManager = sceneTransitionManager;
            
            // Check completion to potentially grey out or highlight the button
            bool isCompleted = IsLevelCompleted(level.levelNumber);
             
            if(isCompleted || level.levelNumber != CurrentLevel) btnUI.Setup(level, isCompleted, false);
            else btnUI.Setup(level, false, true);
        }
        
    }

    void AllLevelData()
    {
        cachedLevels = Resources.LoadAll<LevelObject>("Level").ToList();
        Debug.Log(cachedLevels.Count);
    }

    void SetLevelData()
    {
        cachedLevels = cachedLevels.OrderBy(l => l.levelNumber).ToList();
    }

    // Capture the result after a level is finished
    public void MarkLevelComplete(int levelNumber)
    {
        if (!Wallet.data.completedLevelNumbers.Contains(levelNumber))
        {
            Wallet.data.completedLevelNumbers.Add(levelNumber);
            Wallet.Save();
        }
    }

    public bool IsLevelCompleted(int levelNumber)
    {
        return Wallet.data.completedLevelNumbers.Contains(levelNumber);
    }

    public int GetCurrentLevel() => Wallet.data.currentLevel;

    public void SetCurrentLevel(int levelNumber)
    {
        Wallet.data.currentLevel = levelNumber;
        Wallet.Save();
    }
    

    // Access specific data from the cached list
    public LevelObject GetLevelData(int levelNumber)
    {
        return cachedLevels.FirstOrDefault(l => l.levelNumber == levelNumber);
    }

    void OnScrollChanged(Vector2 scrollPos)
    {
        UpdateArrowStates();
    }

    public void SetInitialArrowStates()
    {
        // Get the total number of level buttons
        int totalLevels = cachedLevels.Count;

        // Check if we need to scroll at all
        bool needsScrolling = totalLevels > itemsThatFitOnScreen;

        if (!needsScrolling)
        {
            // If everything fits, hide/disable both arrows
            SetButtonState(leftArrowBtn, leftArrowBtnImage, false);
            SetButtonState(rightArrowBtn, rightArrowBtnImage, false);
        }
        else
        {
            // If we have more than enough, Right is enabled (can move), Left is disabled (at start)
            SetButtonState(leftArrowBtn, leftArrowBtnImage, false);
            SetButtonState(rightArrowBtn, rightArrowBtnImage, true);
        }
    }

    public void UpdateArrowStates()
    {
        if (scrollRect == null) return;

        // 1. Get raw dimensions
        float viewWidth = scrollRect.viewport.rect.width;
        float contentWidth = scrollRect.content.rect.width;
        float currentX = scrollRect.content.anchoredPosition.x;

        // 2. Logic: The content is at 0 (far left) or at -(contentWidth - viewWidth) (far right)
        // We use Mathf.Abs to compare absolute distances from the edges
        float maxScroll = Mathf.Max(0, contentWidth - viewWidth);
        
        // We are at the start if currentX is very close to 0
        bool atStart = currentX > -10f; 
        
        // We are at the end if currentX is very close to -maxScroll
        // We use 10f as a tolerance buffer to stop the "flickering"
        bool atEnd = currentX < -(maxScroll - 10f);

        // 3. Set states
        // Left arrow is active only if we are NOT at the start AND content is scrollable
        bool canGoLeft = !atStart && (contentWidth > viewWidth);
        
        // Right arrow is active only if we are NOT at the end AND content is scrollable
        bool canGoRight = !atEnd && (contentWidth > viewWidth);

        SetButtonState(leftArrowBtn, leftArrowBtnImage, canGoLeft);
        SetButtonState(rightArrowBtn, rightArrowBtnImage, canGoRight);
    }

    // Helper to keep code clean
    void SetButtonState(CustomButton btn, Image btnImage, bool isActive)
    {
        btn.interactable = isActive;
        btnImage.color = isActive ? activeColor : inactiveColor;
    }

    public void ScrollLeft()
    {
        scrollRect.horizontalNormalizedPosition = Mathf.Clamp01(scrollRect.horizontalNormalizedPosition - 0.3f)-0.001f;
        UpdateArrowStates();
    }

    public void ScrollRight()
    {
        scrollRect.horizontalNormalizedPosition = Mathf.Clamp01(scrollRect.horizontalNormalizedPosition + 0.3f) + 0.001f;
        UpdateArrowStates();
    }

}