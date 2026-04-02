using UnityEngine;

public class BedInteraction : MonoBehaviour
{
    [Header("References")]
    public GameObject player;
    public Transform bedTransform;
    
    [Header("Settings")]
    public float interactionRange = 2f;
    public KeyCode sleepKey = KeyCode.B;
    
    [Header("Sleep Positioning")]
    public Vector3 lieDownRotation = new Vector3(90f, 0f, 0f);
    public Vector3 lieDownOffset = new Vector3(0f, 0.5f, 0f);
    public float transitionSpeed = 5f;
    
    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip sleepingSound;  // Your sound file
    public float soundVolume = 0.5f;
    
    [Header("Visual Feedback")]
    public GameObject interactionPrompt;
    
    private bool isPlayerInRange = false;
    private bool isSleeping = false;
    private Vector3 originalPlayerPosition;
    private Quaternion originalPlayerRotation;
    private Quaternion targetRotation;
    private Vector3 targetPosition;
    private CharacterController characterController;
    private MonoBehaviour playerMovementScript;
    
    void Start()
    {
        // Find player if not assigned
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player");
            
        if (player != null)
        {
            characterController = player.GetComponent<CharacterController>();
            playerMovementScript = player.GetComponent<MonoBehaviour>();
        }
        
        // Setup audio source if not assigned
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
                audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        // Configure audio source for looping
        audioSource.loop = true;
        audioSource.playOnAwake = false;
        
        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);
    }
    
    void Update()
    {
        if (player != null && Vector3.Distance(transform.position, player.transform.position) <= interactionRange)
        {
            if (!isPlayerInRange)
            {
                isPlayerInRange = true;
                ShowPrompt(true);
            }
            
            if (Input.GetKeyDown(sleepKey))
            {
                if (!isSleeping)
                    StartSleep();
                else
                    WakeUp();
            }
        }
        else
        {
            if (isPlayerInRange)
            {
                isPlayerInRange = false;
                ShowPrompt(false);
            }
            
            if (isSleeping)
                WakeUp();
        }
        
        // Smooth transition while sleeping
        if (isSleeping)
        {
            player.transform.position = Vector3.Lerp(player.transform.position, targetPosition, Time.deltaTime * transitionSpeed);
            player.transform.rotation = Quaternion.Lerp(player.transform.rotation, targetRotation, Time.deltaTime * transitionSpeed);
        }
    }
    
    void StartSleep()
    {
        isSleeping = true;
        
        // Store original position and rotation
        originalPlayerPosition = player.transform.position;
        originalPlayerRotation = player.transform.rotation;
        
        // Disable Character Controller and movement
        if (characterController != null)
            characterController.enabled = false;
        if (playerMovementScript != null)
            playerMovementScript.enabled = false;
        
        // Calculate target position and rotation for lying down
        if (bedTransform != null)
        {
            targetPosition = bedTransform.position + lieDownOffset;
            targetRotation = bedTransform.rotation * Quaternion.Euler(lieDownRotation);
        }
        else
        {
            targetPosition = transform.position + lieDownOffset;
            targetRotation = Quaternion.Euler(lieDownRotation);
        }
        
        // Set player position and rotation
        player.transform.position = targetPosition;
        player.transform.rotation = targetRotation;
        
        // Play sleeping sound
        if (sleepingSound != null && audioSource != null)
        {
            audioSource.clip = sleepingSound;
            audioSource.volume = soundVolume;
            audioSource.Play();
        }
        
        Debug.Log("Player is sleeping");
    }
    
    void WakeUp()
    {
        // Stop sleeping sound
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
        
        isSleeping = false;
        
        // Restore player position and rotation
        player.transform.position = originalPlayerPosition;
        player.transform.rotation = originalPlayerRotation;
        
        // Re-enable Character Controller and movement
        if (characterController != null)
            characterController.enabled = true;
        if (playerMovementScript != null)
            playerMovementScript.enabled = true;
        
        Debug.Log("Player woke up");
    }
    
    void ShowPrompt(bool show)
    {
        if (interactionPrompt != null)
            interactionPrompt.SetActive(show);
    }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
        
        Gizmos.color = Color.cyan;
        Vector3 sleepPos = bedTransform != null ? bedTransform.position + lieDownOffset : transform.position + lieDownOffset;
        Gizmos.DrawWireCube(sleepPos, Vector3.one * 0.5f);
    }
}