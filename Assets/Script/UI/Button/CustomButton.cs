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
        // Wrap the float in a new Vector3(scale, scale, scale)
        Animate(restingAnchoredPosition + pressedPositionOffset, new Vector3(pressedScale, pressedScale, pressedScale), pressedColor);
        if (manager != null) manager.OnPressed();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!interactable) return;
        // Use your original Vector3 variable 'restingScale'
        Animate(restingAnchoredPosition, restingScale, originalColor);
        if (manager != null) manager.OnReleased();
        onClick.Invoke();
    }

    // Safety: If the finger slides off the button while pressed, reset state
    public void OnPointerExit(PointerEventData eventData) => OnPointerUp(eventData);

    // Update the method signature to accept Vector3 for scale
    private void Animate(Vector2 pos, Vector3 scale, Color color)
    {
        rectTransform.DOKill();
        transform.DOKill();
        
        rectTransform.DOAnchorPos(pos, animationDuration).SetUpdate(true);
        transform.DOScale(scale, animationDuration).SetUpdate(true); // Now this works with Vector3
        if (buttonImage != null) buttonImage.DOColor(color, animationDuration).SetUpdate(true);
    }

    private void OnDisable()
    {
        // Force reset visuals if the object is disabled while clicked
        rectTransform.DOKill();
        transform.DOKill();
        rectTransform.anchoredPosition = restingAnchoredPosition;
        transform.localScale = restingScale;
        if (buttonImage != null) buttonImage.color = originalColor;
    }

    public void UpdateVisuals()
    {
        if (buttonImage != null)
            buttonImage.DOColor(interactable ? originalColor : disabledColor, 0.2f).SetUpdate(true);
    }
}