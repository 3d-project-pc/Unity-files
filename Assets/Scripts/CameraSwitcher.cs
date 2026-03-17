using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    [Header("Camera References")]
    public GameObject playerCamera;
    public GameObject pcCamera1;
    public GameObject pcCamera2;

    [Header("Player Settings")]
    public MonoBehaviour playerMovementScript;

    void Update()
    {
        // RETURN TO PLAYER (V)
        if (Input.GetKeyDown(KeyCode.V))
        {
            ReturnToPlayer();
        }

        // VIEW PC CAMERA 1 (K)
        if (Input.GetKeyDown(KeyCode.K))
        {
            EnterCameraMode(pcCamera1);
        }

        // VIEW PC CAMERA 2 (L)
        if (Input.GetKeyDown(KeyCode.L))
        {
            EnterCameraMode(pcCamera2);
        }
    }

    void EnterCameraMode(GameObject targetCam)
    {
        // Safety check: make sure the camera actually exists in the slot
        if (targetCam == null)
        {
            Debug.LogError("Camera slot is empty! Drag a camera into the GameManager inspector.");
            return;
        }

        // 1. Turn off all cameras
        playerCamera.SetActive(false);
        pcCamera1.SetActive(false);
        pcCamera2.SetActive(false);

        // 2. Turn on the chosen camera
        targetCam.SetActive(true);

        // 3. Stop player movement so you don't walk away
        if (playerMovementScript != null)
            playerMovementScript.enabled = false;

        Debug.Log("Switched to: " + targetCam.name);
    }

    void ReturnToPlayer()
    {
        // 1. Reset cameras
        pcCamera1.SetActive(false);
        pcCamera2.SetActive(false);
        playerCamera.SetActive(true);

        // 2. Re-enable movement
        if (playerMovementScript != null)
            playerMovementScript.enabled = true;

        Debug.Log("Returned to Player view.");
    }
}