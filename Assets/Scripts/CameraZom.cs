using UnityEngine;
using UnityEngine.InputSystem;

public class CameraZoom : MonoBehaviour
{
    [Header("Zoom Settings")]
    public float zoomSpeed = 2f;

    private Camera mainCam;
    private InputAction scrollAction;

    void Awake()
    {
        mainCam = Camera.main;

        // Create an InputAction for scroll events
        scrollAction = new InputAction(
            type: InputActionType.PassThrough,
            binding: "<Mouse>/scroll"
        );

        // Subscribe to scroll input
        scrollAction.performed += OnScroll;
    }

    void OnEnable()
    {
        scrollAction.Enable();
    }

    void OnDisable()
    {
        scrollAction.Disable();
    }

    private void OnScroll(InputAction.CallbackContext context)
    {
        // Get scroll delta (Vector2, but we use Y)
        Vector2 scrollDelta = context.ReadValue<Vector2>();
        mainCam.orthographicSize += scrollDelta.y * zoomSpeed * Time.deltaTime;
    }
}
