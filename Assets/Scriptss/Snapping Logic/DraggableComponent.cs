using UnityEngine;

public class DraggableComponent : MonoBehaviour
{
    private Vector3 mOffset;
    private float mZCoord;
    private Vector3 startPosition;

    [HideInInspector]
    public bool isSnapped = false;

    [Header("Rotation Settings")]
    public float rotationAmount = 90f;
    private PartIdentityV2 myData;

    void Start()
    {
        myData = GetComponent<PartIdentityV2>();
    }

    void OnMouseDown()
    {
        if (myData != null && !myData.isInstalled && !isSnapped)
        {
            startPosition = transform.position;
            mZCoord = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
            mOffset = gameObject.transform.position - GetMouseWorldPos();
            
            // Show the guide message when picked up
            if (!string.IsNullOrEmpty(myData.guideMessage))
            {
                InstallationGuide.Instance.ShowSuccessMessage(myData.guideMessage);
            }
        }
    }

    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = mZCoord;
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }

    void OnMouseDrag()
    {
        if (myData != null && !myData.isInstalled && !isSnapped)
        {
            transform.position = GetMouseWorldPos() + mOffset;

            if (Input.GetKeyDown(KeyCode.R)) RotateObject(Vector3.up);
            if (Input.GetKeyDown(KeyCode.T)) RotateObject(Vector3.right);
        }
    }

    void OnMouseUp()
    {
        // Clear the message when released (whether snapped or not)
        if (myData != null)
        {
            InstallationGuide.Instance.ClearMessage();
        }
        
        // Give socket time to process snap
        Invoke("CheckIfSnapped", 0.05f);
    }

    void CheckIfSnapped()
    {
        if (!isSnapped && transform.parent == null && myData != null && !myData.isInstalled)
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