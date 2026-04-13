using UnityEngine;

public class DraggableComponent : MonoBehaviour
{
    private Vector3 mOffset;
    private float mZCoord;
    private Vector3 startPosition;

    [HideInInspector]
    public bool isSnapped = false; // Controlled by the SnapSocket script

    [Header("Rotation Settings")]
    public float rotationAmount = 90f;
    private ComponentTag myData;

    void Start()
    {
        myData = GetComponent<ComponentTag>();
    }

    void OnMouseDown()
    {
        // Save where we picked it up from (the table)
        startPosition = transform.position;
        isSnapped = false;

        mZCoord = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
        mOffset = gameObject.transform.position - GetMouseWorldPos();
    }

    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = mZCoord;
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }

    void OnMouseDrag()
    {
        // Only move if not already installed
        if (myData != null && !myData.isInstalled)
        {
            transform.position = GetMouseWorldPos() + mOffset;

            // Manual rotation while dragging
            if (Input.GetKeyDown(KeyCode.R)) RotateObject(Vector3.up);
            if (Input.GetKeyDown(KeyCode.T)) RotateObject(Vector3.right);
        }
    }

    void OnMouseUp()
    {
        // Give the SnapSocket a tiny moment to process parenting
        Invoke("CheckIfSnapped", 0.05f);
    }

    void CheckIfSnapped()
    {
        // If the socket didn't grab us, teleport back to the table
        if (!isSnapped && transform.parent == null)
        {
            transform.position = startPosition;
            Debug.Log("Missed the socket! Returning to table.");
        }
    }

    void RotateObject(Vector3 axis)
    {
        transform.Rotate(axis, rotationAmount, Space.Self);
    }
}