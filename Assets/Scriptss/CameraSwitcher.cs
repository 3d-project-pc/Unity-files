using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    [Header("Camera References")]
    public GameObject crosshair;
    public GameObject playerCamera;
    public GameObject roomCamera;
    public GameObject pcCamera1;
    public GameObject pcCamera2;
    public GameObject settingsPanel;

    [Header("UI References")]
    public GameObject roomViewUI; // Drag your new Room Canvas/Panel here
    public GameObject playerHUD;  // Drag your crosshair/game UI here

    [Header("Player Settings")]
    public MonoBehaviour playerMovementScript;

    void Start()
    {
        // Always start as the player
        ReturnToPlayer();
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);

        }
        if (roomViewUI != null)
        {
            roomViewUI.SetActive(false);
        }
    }

    void Update()
    {
        // V - RETURN TO PLAYER
        if (Input.GetKeyDown(KeyCode.V)) ReturnToPlayer();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (settingsPanel != null && settingsPanel.activeSelf)
            {
                settingsPanel.SetActive(false);
            }
            else if (roomCamera.activeSelf)
            {
                ReturnToPlayer();
            }
            else
            {
                EnterCameraMode(roomCamera, true);
            }
        }

        // L - PC VIEW 1 (No UI/No Mouse - Camera only)
        if (Input.GetKeyDown(KeyCode.L)) EnterCameraMode(pcCamera1, false);

        // J - PC VIEW 2 (No UI/No Mouse - Camera only)
        if (Input.GetKeyDown(KeyCode.J)) EnterCameraMode(pcCamera2, false);
    }

    void EnterCameraMode(GameObject targetCam, bool isRoomView)
    {
        if (targetCam == null) return;

        // 1. Disable all cameras
        playerCamera.SetActive(false);
        roomCamera.SetActive(false);
        pcCamera1.SetActive(false);
        pcCamera2.SetActive(false);

        // 2. Enable the target
        targetCam.SetActive(true);

        // 3. Stop movement
        if (playerMovementScript != null) playerMovementScript.enabled = false;

        // 4. Handle UI and Mouse ONLY if it's the Room View (K)
        if (isRoomView)
        {
            roomViewUI.SetActive(true);
            if (playerHUD != null) playerHUD.SetActive(false);

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            // For PC views (L/J), keep UI off and mouse locked
            roomViewUI.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        if (crosshair != null) crosshair.SetActive(false);
    }

    public void ReturnToPlayer()
    {
        // Turn off all extra cameras and UI
        roomCamera.SetActive(false);
        pcCamera1.SetActive(false);
        pcCamera2.SetActive(false);
        roomViewUI.SetActive(false);

        // Turn on Player stuff
        playerCamera.SetActive(true);
        if (playerHUD != null) playerHUD.SetActive(true);

        // Enable movement and lock mouse
        if (playerMovementScript != null) playerMovementScript.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        if (crosshair != null) crosshair.SetActive(true);
    }
}