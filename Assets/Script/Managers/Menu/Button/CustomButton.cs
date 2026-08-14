using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class CustomButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    [Header("State")]
    public bool interactable = true;
    public Color disabledColor = new Color(0.5f, 0.5f, 0.5f, 1f);

    [Header("Actions")]
    public UnityEvent onClick;

    [Header("Settings")]
    public Vector2 pressedPositionOffset;
    public float pressedScale = 0.9f;
    public float animationDuration = 0.1f;
    public Color pressedColor = new Color(0.8f, 0.8f, 0.8f);

    private Vector2 restingAnchoredPosition;
    private Vector3 restingScale;
    private Color originalColor;
    private RectTransform rectTransform;
    private Image buttonImage;
    public ObjectTransformManager manager;

    // Added state tracker to prevent double-firing
    private bool isPressed = false;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        restingAnchoredPosition = rectTransform.anchoredPosition;
        restingScale = transform.localScale;
        buttonImage = GetComponent<Image>();
        if (buttonImage != null) originalColor = buttonImage.color;
        UpdateVisuals();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!interactable) return;
        
        isPressed = true;
        
        Animate(restingAnchoredPosition + pressedPositionOffset, new Vector3(pressedScale, pressedScale, pressedScale), pressedColor);
        if (manager != null) manager.OnPressed();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // Only execute if the button is currently in a pressed state
        if (!interactable || !isPressed) return;
        
        isPressed = false;
        
        Animate(restingAnchoredPosition, restingScale, originalColor);
        if (manager != null) manager.OnReleased();
        onClick.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // If the pointer slides off while pressed, cancel the press visually 
        // but DO NOT invoke the click event.
        if (!interactable || !isPressed) return;
        
        isPressed = false;
        
        Animate(restingAnchoredPosition, restingScale, originalColor);
        if (manager != null) manager.OnReleased();
    }

    private void Animate(Vector2 pos, Vector3 scale, Color color)
    {
        rectTransform.DOKill();
        transform.DOKill();
        
        rectTransform.DOAnchorPos(pos, animationDuration).SetUpdate(true);
        transform.DOScale(scale, animationDuration).SetUpdate(true);
        if (buttonImage != null) buttonImage.DOColor(color, animationDuration).SetUpdate(true);
    }

    private void OnDisable()
    {
        rectTransform.DOKill();
        transform.DOKill();
        rectTransform.anchoredPosition = restingAnchoredPosition;
        transform.localScale = restingScale;
        if (buttonImage != null) buttonImage.color = originalColor;
        
        isPressed = false; // Reset state on disable
    }

    public void UpdateVisuals()
    {
        if (buttonImage != null)
            buttonImage.DOColor(interactable ? originalColor : disabledColor, 0.2f).SetUpdate(true);
    }
}