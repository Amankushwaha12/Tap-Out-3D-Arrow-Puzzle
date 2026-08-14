using UnityEngine;
using DG.Tweening; 

[RequireComponent(typeof(CanvasGroup))]
public class PanelAnimator : MonoBehaviour
{
    private CanvasGroup canvasGroup;
    public float fadeDuration = 0.5f;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void ShowPanel(bool instant = false)
    {
        gameObject.SetActive(true);
        canvasGroup.alpha = 0;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        // Added .SetUpdate(true)
        canvasGroup.DOFade(1, instant ? 0 : fadeDuration)
            .SetUpdate(true)
            .OnComplete(() => gameObject.SetActive(true));
    }

    public void HidePanel(bool instant = false)
    {
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        // Added .SetUpdate(true)
        canvasGroup.DOFade(0, instant ? 0 : fadeDuration)
            .SetUpdate(true) 
            .OnComplete(() => gameObject.SetActive(false));
    }
}