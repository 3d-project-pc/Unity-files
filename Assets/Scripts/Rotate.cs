using UnityEngine;

public class SimpleRotator : MonoBehaviour
{
    [Header("Rotation Settings")]
    [SerializeField] private Vector3 rotationSpeed = new Vector3(0, 90f, 0); // Rotates 90 degrees per second on Y-axis
    [SerializeField] private Space rotationSpace = Space.Self; // Use Self for local rotation, World for world rotation

    void Update()
    {
        // Apply rotation based on speed and time
        transform.Rotate(rotationSpeed * Time.deltaTime, rotationSpace);
    }
}