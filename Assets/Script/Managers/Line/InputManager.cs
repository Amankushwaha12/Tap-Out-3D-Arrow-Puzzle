using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{
    private Camera mainCamera;

    [Header("Setup")]
    [Tooltip("Layer containing clickable Line objects.")]
    [SerializeField] private LayerMask clickableLayer;

    [Header("Settings")]
    [SerializeField] private bool ignoreUI = true;

    [Header("Debug")]
    [SerializeField] private bool enableLogging = true;

    private void Awake()
    {
        mainCamera = Camera.main;

        if (mainCamera == null)
        {
            LogError("Main Camera not found!");
        }
        else
        {
            Log("Main Camera assigned successfully.");
        }
    }

    private void Update()
    {
        if (mainCamera == null)
            return;

        // Detect mouse click / touch
        if (Pointer.current == null ||
            !Pointer.current.press.wasPressedThisFrame)
            return;

        // Ignore clicks on UI
        if (ignoreUI &&
            EventSystem.current != null &&
            EventSystem.current.IsPointerOverGameObject())
        {
            Log("Click ignored because it is over UI.");
            return;
        }

        Vector2 screenPosition = Pointer.current.position.ReadValue();

        Log($"Tap detected at Screen Position: {screenPosition}");

        // Convert screen position to world position.
        Vector3 screenPoint = new Vector3(
            screenPosition.x,
            screenPosition.y,
            Mathf.Abs(mainCamera.transform.position.z)
        );

        Vector3 worldPosition =
            mainCamera.ScreenToWorldPoint(screenPoint);

        Vector2 worldPosition2D = new Vector2(
            worldPosition.x,
            worldPosition.y
        );

        Log($"World Position: {worldPosition2D}");

        // Find clickable collider.
        Collider2D hitCollider = Physics2D.OverlapPoint(
            worldPosition2D,
            clickableLayer
        );

        if (hitCollider == null)
        {
            Log($"Miss! No clickable object found at {worldPosition2D}.");
            return;
        }

        Log($"Hit: {hitCollider.gameObject.name}");

        // Check the clicked object itself.
        if (hitCollider.TryGetComponent<LineController>(
                out LineController line))
        {
            Log(
                $"LineController found on " +
                $"{hitCollider.gameObject.name}. Starting movement."
            );

            line.StartForward();
            return;
        }

        // Check parent objects.
        LineController parentLine =
            hitCollider.GetComponentInParent<LineController>();

        if (parentLine != null)
        {
            Log(
                $"LineController found on parent: " +
                $"{parentLine.gameObject.name}. Starting movement."
            );

            parentLine.StartForward();
            return;
        }

        LogWarning(
            $"Hit {hitCollider.gameObject.name}, " +
            "but no LineController was found on it or its parents."
        );
    }

    // ---------------------------------------------------------
    // Logging
    // ---------------------------------------------------------

    private void Log(string message)
    {
        if (!enableLogging)
            return;

        Debug.Log($"[InputManager] {message}");
    }

    private void LogWarning(string message)
    {
        if (!enableLogging)
            return;

        Debug.LogWarning($"[InputManager] {message}");
    }

    private void LogError(string message)
    {
        if (!enableLogging)
            return;

        Debug.LogError($"[InputManager] {message}");
    }
}