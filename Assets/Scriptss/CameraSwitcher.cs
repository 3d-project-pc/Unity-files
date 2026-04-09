using UnityEngine;
using UnityEngine.EventSystems;

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
    public GameObject roomViewUI;
    public GameObject playerHUD;

    [Header("Player Settings")]
    public MonoBehaviour playerMovementScript;

    // ── FIX: helper to detect if player is typing ─────────────────────────
    private bool IsTyping()
    {
        return EventSystem.current != null &&
               EventSystem.current.currentSelectedGameObject != null &&
               EventSystem.current.currentSelectedGameObject
                   .GetComponent<TMPro.TMP_InputField>() != null;
    }
    // ──────────────────────────────────────────────────────────────────────

    void Start()
    {
        ReturnToPlayer();
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (roomViewUI != null) roomViewUI.SetActive(false);
    }

    void Update()
    {
        // ── FIX: block ALL key shortcuts while typing ──────────────────────
        if (IsTyping()) return;
        // ──────────────────────────────────────────────────────────────────

        if (Input.GetKeyDown(KeyCode.V)) ReturnToPlayer();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (settingsPanel != null && settingsPanel.activeSelf)
                settingsPanel.SetActive(false);
            else if (roomCamera.activeSelf)
                ReturnToPlayer();
            else
                EnterCameraMode(roomCamera, true);
        }

        if (Input.GetKeyDown(KeyCode.L)) EnterCameraMode(pcCamera1, false);
        if (Input.GetKeyDown(KeyCode.J)) EnterCameraMode(pcCamera2, false);
    }

    void EnterCameraMode(GameObject targetCam, bool isRoomView)
    {
        if (targetCam == null) return;

        playerCamera.SetActive(false);
        roomCamera.SetActive(false);
        pcCamera1.SetActive(false);
        pcCamera2.SetActive(false);

        targetCam.SetActive(true);

        if (playerMovementScript != null) playerMovementScript.enabled = false;

        if (isRoomView)
        {
            roomViewUI.SetActive(true);
            if (playerHUD != null) playerHUD.SetActive(false);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            roomViewUI.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        if (crosshair != null) crosshair.SetActive(false);
    }

    public void ReturnToPlayer()
    {
        roomCamera.SetActive(false);
        pcCamera1.SetActive(false);
        pcCamera2.SetActive(false);
        roomViewUI.SetActive(false);

        playerCamera.SetActive(true);
        if (playerHUD != null) playerHUD.SetActive(true);

        if (playerMovementScript != null) playerMovementScript.enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (crosshair != null) crosshair.SetActive(true);
    }
}