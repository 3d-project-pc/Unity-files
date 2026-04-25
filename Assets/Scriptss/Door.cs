using UnityEngine;
public class Door : MonoBehaviour

{
    public bool isOpen = false;
    public void ToggleDoor()

    {

        isOpen = !isOpen;

        // If open, rotate 90 degrees. If closed, back to 0.

        float angle = isOpen ? 90f : 12f;

        transform.localRotation = Quaternion.Euler(0, angle, 0);

    }
}