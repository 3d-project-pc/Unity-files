using UnityEngine;

public class PlayerAvatarMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private CharacterController controller;
    [SerializeField] private float speed = 8f;
    [SerializeField] private float gravity = -19.62f;
    [SerializeField] private float groundedGravity = -2f;
    
    [Header("Input")]
    [SerializeField] private string horizontalInput = "Horizontal";
    [SerializeField] private string verticalInput = "Vertical";
    
    // Physics state
    private Vector3 velocity;
    
    private void Awake()
    {
        // Auto-find CharacterController if not assigned
        if (controller == null)
            controller = GetComponent<CharacterController>();
    }
    
    private void Update()
    {
        HandleMovement();
        HandleGravity();
    }
    
    private void HandleMovement()
    {
        // Get input
        float moveX = Input.GetAxis(horizontalInput);
        float moveZ = Input.GetAxis(verticalInput);
        
        // Calculate movement direction relative to player's orientation
        Vector3 moveDirection = transform.right * moveX + transform.forward * moveZ;
        
        // Normalize diagonal movement to maintain consistent speed
        if (moveDirection.magnitude > 1f)
            moveDirection.Normalize();
        
        // Apply movement
        Vector3 movement = moveDirection * (speed * Time.deltaTime);
        controller.Move(movement);
    }
    
    private void HandleGravity()
    {
        // Reset velocity when grounded to prevent constant downward acceleration
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = groundedGravity;
        }
        
        // Apply gravity
        velocity.y += gravity * Time.deltaTime;
        
        // Apply vertical movement
        controller.Move(velocity * Time.deltaTime);
    }
}