using UnityEngine;

public class PlayerAvatarMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private CharacterController controller;
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 10f;
    [SerializeField] private float gravity = -19.62f;
    [SerializeField] private float groundedGravity = -2f;
    
    [Header("Input")]
    [SerializeField] private string horizontalInput = "Horizontal";
    [SerializeField] private string verticalInput = "Vertical";
    
    [Header("Animation")]
    [SerializeField] private string walkingParam = "isWalking";
    [SerializeField] private string runningParam = "isRunning";
    
    [Header("Avatar References")]
    [SerializeField] private GameObject femaleModel;
    [SerializeField] private GameObject maleModel;
    
    // Input state
    private Vector2 currentMovementInput;
    private bool isMovementPressed;
    private bool isRunPressed;
    
    // Physics state
    private Vector3 velocity;
    
    // Animation hash IDs for performance
    private int isWalkingHash;
    private int isRunningHash;
    
    // Reference to active animator
    private Animator activeAnimator;
    
    private void Awake()
    {
        // Auto-find CharacterController if not assigned
        if (controller == null)
            controller = GetComponent<CharacterController>();
        
        // Get animator parameter hashes
        isWalkingHash = Animator.StringToHash(walkingParam);
        isRunningHash = Animator.StringToHash(runningParam);
        
        // Find the active animator based on which avatar is active
        FindActiveAnimator();
    }
    
    private void FindActiveAnimator()
    {
        // Check which avatar is active and get its Animator
        if (femaleModel != null && femaleModel.activeSelf)
        {
            activeAnimator = femaleModel.GetComponent<Animator>();
        }
        else if (maleModel != null && maleModel.activeSelf)
        {
            activeAnimator = maleModel.GetComponent<Animator>();
        }
        
        // If no animator found, try to find one on children
        if (activeAnimator == null)
        {
            activeAnimator = GetComponentInChildren<Animator>();
        }
        
        if (activeAnimator == null)
        {
            Debug.LogWarning("No Animator found on active avatar!");
        }
    }
    
    private void Update()
    {
        HandleInput();
        HandleMovement();
        HandleAnimation();
        HandleGravity();
        if (Input.GetKeyDown(KeyCode.E))
        {
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
            if (Physics.Raycast(ray, out RaycastHit hit, 12f))
            {
                // Try to find the Door script on the object we hit or its parent
                Door door = hit.collider.GetComponentInParent<Door>();
                if (door != null)
                {
                    door.ToggleDoor();
                }
            }
        }
    }
    
    private void HandleInput()
    {
        // Get movement input
        float moveX = Input.GetAxis(horizontalInput);
        float moveZ = Input.GetAxis(verticalInput);
        
        currentMovementInput = new Vector2(moveX, moveZ);
        isMovementPressed = moveX != 0 || moveZ != 0;
        
        // Get run input (Left Shift)
        isRunPressed = Input.GetKey(KeyCode.LeftShift);
    }
    
    private void HandleMovement()
    {
        // Calculate movement direction relative to player's orientation
        // In first-person, we use transform.forward/right which is controlled by the camera/mouse
        Vector3 moveDirection = transform.right * currentMovementInput.x + 
                                transform.forward * currentMovementInput.y;
        
        // Normalize diagonal movement to maintain consistent speed
        if (moveDirection.magnitude > 1f)
            moveDirection.Normalize();
        
        // Determine current speed based on run state
        float currentSpeed = (isRunPressed && isMovementPressed) ? runSpeed : walkSpeed;
        
        // Apply movement
        Vector3 movement = moveDirection * (currentSpeed * Time.deltaTime);
        
        // Add vertical movement from gravity
        movement.y = velocity.y * Time.deltaTime;
        
        controller.Move(movement);
    }
    
    private void HandleAnimation()
    {
        // Skip if no active animator found
        if (activeAnimator == null) return;
        
        bool isWalking = activeAnimator.GetBool(isWalkingHash);
        bool isRunning = activeAnimator.GetBool(isRunningHash);
        
        // Handle walking animation
        if (isMovementPressed && !isRunPressed && !isWalking)
        {
            activeAnimator.SetBool(isWalkingHash, true);
            activeAnimator.SetBool(isRunningHash, false);
        }
        // Handle running animation
        else if (isMovementPressed && isRunPressed && !isRunning)
        {
            activeAnimator.SetBool(isRunningHash, true);
            activeAnimator.SetBool(isWalkingHash, false);
        }
        // Handle stopping animations
        else if (!isMovementPressed && (isWalking || isRunning))
        {
            activeAnimator.SetBool(isWalkingHash, false);
            activeAnimator.SetBool(isRunningHash, false);
        }
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
    }
    
    // Call this method if avatar switches during gameplay
    public void OnAvatarSwitched()
    {
        FindActiveAnimator();
    }
}