using UnityEngine;
using UnityEngine.InputSystem;

public class ModelRotation : MonoBehaviour
{
    [Header("Rotation Settings")]
    public float rotationSpeed = 1f;
    public bool invertX = false;
    public bool invertY = false;

    private Vector2 lastMousePosition;
    private bool isDragging = false;

    void Update()
    {
        Mouse mouse = Mouse.current;
        if (mouse == null) return;

        // Check if left mouse button is pressed
        if (mouse.leftButton.wasPressedThisFrame)
        {
            isDragging = true;
            lastMousePosition = mouse.position.ReadValue();
        }

        // Check if left mouse button was released
        if (mouse.leftButton.wasReleasedThisFrame)
        {
            isDragging = false;
        }

        // Rotate while dragging
        if (isDragging)
        {
            Vector2 currentMousePosition = mouse.position.ReadValue();
            Vector2 delta = currentMousePosition - lastMousePosition;

            float xRotation = delta.y * rotationSpeed * (invertY ? -1 : 1);
            float yRotation = -delta.x * rotationSpeed * (invertX ? -1 : 1);

            transform.Rotate(Vector3.right, xRotation, Space.World);
            transform.Rotate(Vector3.up, yRotation, Space.World);

            lastMousePosition = currentMousePosition;
        }
    }
}