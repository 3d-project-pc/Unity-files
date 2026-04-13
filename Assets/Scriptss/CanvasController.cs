using UnityEngine;
using UnityEngine.UI; // Required for the Button component

public class CanvasController : MonoBehaviour
{
    public GameObject canvasToHide;
    public Button closeButton;

    void Start()
    {
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(HideCanvas);
        }
    }

    public void HideCanvas()
    {
        if (canvasToHide != null)
        {
            // SetActive(false) makes the object and all its children disappear
            canvasToHide.SetActive(false);
            Debug.Log("Canvas has been hidden.");
        }
    }
}