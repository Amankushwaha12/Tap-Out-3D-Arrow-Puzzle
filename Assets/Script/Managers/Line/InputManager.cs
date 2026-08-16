using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (Pointer.current != null && Pointer.current.press.wasPressedThisFrame)
        {
            Vector2 mousePos = Pointer.current.position.ReadValue();
            Debug.Log(mousePos);

            Vector2 worldPos = mainCamera.ScreenToWorldPoint(mousePos);

            RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);

            if (hit.collider != null)
            {
                Debug.Log($"Hit Object: {hit.collider.gameObject.name}");

                if (hit.collider.TryGetComponent<LineController>(out var line))
                {
                    line.StartForward();
                }
                else
                {
                    LineController parentLine =
                        hit.collider.GetComponentInParent<LineController>();

                    if (parentLine != null)
                    {
                        parentLine.StartForward();
                    }
                }
            }
            else
            {
                Debug.Log("No object hit.");
            }
        }
    }
}