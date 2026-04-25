using UnityEngine;
using UnityEngine.EventSystems;

public class ShopManager : MonoBehaviour
{
    public GameObject shopPanel;
    public GameObject crosshair;
    private bool isShopOpen = false;

    void Start()
    {
        shopPanel.SetActive(false);
        crosshair.SetActive(true);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            // ── FIX: ignore X key if the player is typing in any input field ──
            if (EventSystem.current != null &&
                EventSystem.current.currentSelectedGameObject != null &&
                EventSystem.current.currentSelectedGameObject
                    .GetComponent<TMPro.TMP_InputField>() != null)
                return;
            // ──────────────────────────────────────────────────────────────────

            ToggleShop();
        }
    }

    public void ToggleShop()
    {
        isShopOpen = !isShopOpen;
        shopPanel.SetActive(isShopOpen);
        crosshair.SetActive(!isShopOpen);

        if (isShopOpen)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
    public void CloseShop()
    {
        if (!isShopOpen) return; // only close if open
        ToggleShop();
    }
}