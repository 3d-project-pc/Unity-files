using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public GameObject shopPanel;
    public GameObject crosshair; 
    private bool isShopOpen = false;

    void Start()
    {
        shopPanel.SetActive(false);
        crosshair.SetActive(true); // Ensure crosshair is visible at start
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            ToggleShop();
        }
    }

    public void ToggleShop()
    {
        isShopOpen = !isShopOpen;
        shopPanel.SetActive(isShopOpen);

        // This line toggles the crosshair to the opposite of the shop state
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
}