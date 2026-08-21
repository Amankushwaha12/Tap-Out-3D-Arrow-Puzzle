using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SceneTransitionGame : MonoBehaviour
{    
    [Header("UI Panels")]
    public PanelAnimator victoryPanel;
    public PanelAnimator defeatPanel;
    public PanelAnimator pausePanel;
    public PanelAnimator playerPanel;
    public PanelAnimator loadingPanel;

    
    [Header("Loading")]
    public Slider progressSlider;
    public TextMeshProUGUI progressText;

    
    public PanelAnimator currentPanel;

    public void AtStart()
    {
        Application.targetFrameRate = 60;

        if(BootstrapManager.Instance) BootstrapManager.Instance.RegisterUI(progressSlider, progressText, loadingPanel, playerPanel);

        loadingPanel.gameObject.SetActive(true);
        pausePanel.gameObject.SetActive(false);
        victoryPanel.gameObject.SetActive(false);
        defeatPanel.gameObject.SetActive(false);
        loadingPanel.gameObject.SetActive(false);
        loadingPanel.HidePanel();
        playerPanel.ShowPanel();

        StartCoroutine(HideLoadingAfterDelay());
    }
    
    

    IEnumerator HideLoadingAfterDelay()
    {
        yield return new WaitForSeconds(.30f); // Or wait for Ad/Data callbacks
        OnLoadingComplete();
    }

    
    public void OnLoadingComplete()
    {
        // 1. Fade out the loading screen
        loadingPanel.HidePanel();

        // 2. Fade in the main menu
        playerPanel.ShowPanel();
        currentPanel = playerPanel;
    }

    
    public void ShowScreen(PanelAnimator newPanel)
    {
        // 1. Hide the old panel
        if (currentPanel != null)
        {
            currentPanel.HidePanel();
        }

        // 2. Show the new panel
        currentPanel = newPanel;
        currentPanel.ShowPanel();
    }
    
    public void ShowOverScreen(PanelAnimator newPanel)
    {
        currentPanel = newPanel;
        currentPanel.ShowPanel();
    }

    
    public void SceneChange(string sceneName)
    {
        BootstrapManager.Instance.RequestSceneLoad(sceneName);
    }

}
