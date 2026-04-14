using UnityEngine;
using UnityEngine.UI;

public class CanvasController : MonoBehaviour
{
    public GameObject canvasToHide; 
    public Button closeButton;

    [Header("Drag Crosshair here")]
    public GameObject crosshairObject;

    void Start()
    {
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(HideCanvas);
        }
    }

    public void HideCanvas()
    {
        // 1. Close the shop
        if (canvasToHide != null)
            canvasToHide.SetActive(false);

        // 2. Lock and hide the hardware mouse pointer
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // 3. Force the crosshair to stay active
        if (crosshairObject != null)
        {
            crosshairObject.SetActive(true);
            Image img = crosshairObject.GetComponentInChildren<Image>();
            if (img != null) img.enabled = true;
        }

        Debug.Log("Shop closed. Crosshair forced to Active.");
    }
}