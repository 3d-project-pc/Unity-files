using UnityEngine;

public class PlayerAvatarMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public CharacterController controller;
    public float speed = 8f;
    public float gravity = -19.62f;

    [Header("Animation")]
    public Animator playerAnimator;

    [Header("Grab Settings")]
    public Camera playerCamera;
    public Transform grabPoint;
    public float grabDistance = 2.5f;

    [Header("Crouch Settings")]
    public float standHeight = 2f;
    public float crouchHeight = 1f;
    public float crouchSpeed = 4f;
    public float crouchTransitionSpeed = 8f;
    private bool isCrouching = false;

    [Header("Physics")]
    private Vector3 velocity;
    private GameObject heldObject;

    void Update()
    {
        HandleMovement();
        HandleCrouch();
        HandleGrab();
    }

    void HandleMovement()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        if (move.magnitude > 1) move.Normalize();

        float currentSpeed = isCrouching ? crouchSpeed : speed;
        controller.Move(move * currentSpeed * Time.deltaTime);

        if (playerAnimator != null)
        {
            bool isWalking = move.magnitude > 0.1f;
            playerAnimator.SetBool("isWalking", isWalking);
        }

        if (controller.isGrounded && velocity.y < 0)
            velocity.y = -2f;

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void HandleCrouch()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            isCrouching = !isCrouching;

            if (playerAnimator != null)
                playerAnimator.SetBool("isCrouching", isCrouching);
        }

        float targetHeight = isCrouching ? crouchHeight : standHeight;
        controller.height = Mathf.Lerp(controller.height, targetHeight,
                            crouchTransitionSpeed * Time.deltaTime);
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

                if (playerAnimator != null)
                    playerAnimator.SetBool("isGrabbing", true);
            }
        }
    }

    void Drop()
    {
        if (heldObject != null)
        {
            heldObject.transform.SetParent(null);
            Rigidbody rb = heldObject.GetComponent<Rigidbody>();
            if (rb != null) rb.isKinematic = false;
            heldObject = null;

            if (playerAnimator != null)
                playerAnimator.SetBool("isGrabbing", false);
        }
    }
}