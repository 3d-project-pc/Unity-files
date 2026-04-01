using UnityEngine;

public class AvatarSwitcher : MonoBehaviour
{
    public GameObject femaleModel;
    public GameObject maleModel;
    public RuntimeAnimatorController animatorController; // Drag your Animator Controller here
    
    [Header("Camera Height Settings")]
    public Transform playerCamera; // Drag your camera here
    [SerializeField] private float femaleCameraHeight = 0.5f; // Exact Y position for female avatar
    [SerializeField] private float maleCameraHeight = 0.7f;   // Exact Y position for male avatar
    
    private PlayerAvatarMovement movementScript;
    private Vector3 originalCameraPosition;
    
    void Start()
    {
        movementScript = GetComponent<PlayerAvatarMovement>();
        
        // Store original camera position to preserve X and Z values
        if (playerCamera != null)
        {
            originalCameraPosition = playerCamera.localPosition;
        }
        
        // Check the memory to see who was picked
        if (CharacterData.SelectedAvatarIndex == 1)
        {
            femaleModel.SetActive(true);
            maleModel.SetActive(false);
            SetupAnimator(femaleModel);
            AdjustCameraY(femaleCameraHeight);
        }
        else
        {
            femaleModel.SetActive(false);
            maleModel.SetActive(true);
            SetupAnimator(maleModel);
            AdjustCameraY(maleCameraHeight);
        }
        
        // Notify movement script about the avatar change
        if (movementScript != null)
        {
            movementScript.OnAvatarSwitched();
        }
    }
    
    private void SetupAnimator(GameObject avatar)
    {
        Animator anim = avatar.GetComponent<Animator>();
        if (anim != null && animatorController != null)
        {
            anim.runtimeAnimatorController = animatorController;
        }
        else if (anim == null)
        {
            Debug.LogError($"No Animator component on {avatar.name}!");
        }
        else if (animatorController == null)
        {
            Debug.LogError("Animator Controller not assigned in AvatarSwitcher!");
        }
    }
    
    private void AdjustCameraY(float newYValue)
    {
        if (playerCamera != null)
        {
            // Keep existing X and Z values, only change Y
            Vector3 currentPosition = playerCamera.localPosition;
            currentPosition.y = newYValue;
            playerCamera.localPosition = currentPosition;
            
            Debug.Log($"Camera Y adjusted to: {newYValue} (X={currentPosition.x}, Z={currentPosition.z} unchanged)");
        }
        else
        {
            Debug.LogWarning("Player Camera not assigned in AvatarSwitcher!");
        }
    }
    
    // Optional: Call this if you need to switch avatars during gameplay
    public void SwitchAvatar(int avatarIndex)
    {
        if (avatarIndex == 1) // Female
        {
            femaleModel.SetActive(true);
            maleModel.SetActive(false);
            SetupAnimator(femaleModel);
            AdjustCameraY(femaleCameraHeight);
        }
        else // Male
        {
            femaleModel.SetActive(false);
            maleModel.SetActive(true);
            SetupAnimator(maleModel);
            AdjustCameraY(maleCameraHeight);
        }
        
        // Notify movement script about the avatar change
        if (movementScript != null)
        {
            movementScript.OnAvatarSwitched();
        }
    }
    
    // Optional: Reset camera to original position (preserves original X and Z)
    public void ResetCameraY()
    {
        if (playerCamera != null && originalCameraPosition != null)
        {
            playerCamera.localPosition = originalCameraPosition;
            Debug.Log($"Camera reset to original position: {originalCameraPosition}");
        }
    }
}