using UnityEngine;
using DG.Tweening; // Add this line
using TMPro;

public class PopupManager : MonoBehaviour
{
    public static PopupManager Instance;

    public CanvasGroup canvasGroup;
    public TextMeshProUGUI statusText; 

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public void Show(string message)
    {
        statusText.text = message;
        gameObject.SetActive(true);

        // Kill any existing tween so they don't conflict
        canvasGroup.DOKill(); 

        // Fade in over 0.5s
        canvasGroup.DOFade(1f, 0.5f).OnComplete(() => {
            // Wait 2 seconds then fade out
            canvasGroup.DOFade(0f, 0.5f).SetDelay(2.0f).OnComplete(() => {
                gameObject.SetActive(false);
            });
        });
    }
}