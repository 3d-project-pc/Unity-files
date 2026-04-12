using UnityEngine;

public class DoorController : MonoBehaviour
{
    public bool isOpen = false;
    public float openAngle = 90f;
    public float smooth = 3f;

    private Quaternion targetRotation;
    private Quaternion closedRotation;

    void Start()
    {
        closedRotation = transform.rotation;
    }

    void Update()
    {
        if (isOpen)
        {
            targetRotation = closedRotation * Quaternion.Euler(0, openAngle, 0);
        }
        else
        {
            targetRotation = closedRotation;
        }

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * smooth);
    }

    public void ToggleDoor()
    {
        isOpen = !isOpen;
    }
}