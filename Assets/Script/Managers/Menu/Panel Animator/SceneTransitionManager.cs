using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SceneTransitionManager : MonoBehaviour
{
    public PanelAnimator loadingScreen;
    public PanelAnimator mainMenu;
    public PanelAnimator settings;
    public PanelAnimator shop;
    public PanelAnimator levels;
    public Slider loadingBar;
    public TextMeshProUGUI statusText;

    public PanelAnimator currentPanel;

    void Start()
    {
        Application.targetFrameRate = 60;
        // 1. Start fully opaque
        loadingScreen.ShowPanel(true);

        // 2. Wait for Bootstrap logic (Ads/Data) to complete
        // Then fade out the loading screen
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
        loadingScreen.HidePanel();

        // 2. Fade in the main menu
        mainMenu.ShowPanel();
        currentPanel = mainMenu;
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
    public void LoadLevel(string sceneName)
    {
        mainMenu.gameObject.SetActive(false);
        settings.gameObject.SetActive(false);
        shop.gameObject.SetActive(false);
        levels.gameObject.SetActive(false);
        BootstrapManager.Instance.RequestSceneLoad(sceneName);
    }
}