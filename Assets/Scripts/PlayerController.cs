using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 3f;
    public float mouseSensitivity = 2f;

    [Header("References")]
    public Camera playerCamera;
    public Animator handAnimator;
    public Transform grabPoint;
    public float grabDistance = 2.5f;

    private CharacterController controller;
    private float verticalRotation = 0f;
    private GameObject heldObject;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleMovement();
        HandleMouseLook();
        HandleGrab();
    }

    void HandleMovement()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x +
                       transform.forward * z;
        controller.Move(move * walkSpeed * Time.deltaTime);

        // Apply gravity
        controller.Move(Vector3.down * 9.81f * Time.deltaTime);

        // Walking animation
        bool isWalking = move.magnitude > 0.1f;
        if (handAnimator != null)
            handAnimator.SetBool("isWalking", isWalking);
    }

    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -80f, 80f);

        playerCamera.transform.localRotation =
            Quaternion.Euler(verticalRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    void HandleGrab()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (heldObject == null)
                TryGrab();
            else
                Drop();
        }
    }

    void TryGrab()
    {
        RaycastHit hit;
        if (Physics.Raycast(
            playerCamera.transform.position,
            playerCamera.transform.forward,
            out hit, grabDistance))
        {
            if (hit.collider.CompareTag("Grabbable"))
            {
                heldObject = hit.collider.gameObject;
                heldObject.GetComponent<Rigidbody>().isKinematic = true;
                heldObject.transform.SetParent(grabPoint);
                heldObject.transform.localPosition = Vector3.zero;

                if (handAnimator != null)
                    handAnimator.SetBool("isGrabbing", true);

                Debug.Log("Grabbed: " + heldObject.name);
            }
        }
    }

    void Drop()
    {
        if (heldObject != null)
        {
            heldObject.transform.SetParent(null);
            heldObject.GetComponent<Rigidbody>().isKinematic = false;
            heldObject = null;

            if (handAnimator != null)
                handAnimator.SetBool("isGrabbing", false);

            Debug.Log("Dropped object");
        }
    }
}
