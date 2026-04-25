using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ShopBuyButton : MonoBehaviour
{
    [Header("References")]
    public OptionsListPopulator optionsListPopulator;
    public PlayerBalance playerBalance;
    public WorldSpawner worldSpawner;

    [Header("UI Message")]
    public TextMeshProUGUI messageText;
    public float messageDisplayTime = 2f;

    [Header("Assembly State")]
    public bool isAssemblyStarted = false;

    private Button buyButton;

    void Start()
    {
        buyButton = GetComponent<Button>();
        if (buyButton != null)
        {
            buyButton.onClick.AddListener(OnBuyButtonClicked);
        }

        if (messageText != null)
        {
            messageText.text = "";
        }
    }

    void OnBuyButtonClicked()
    {
        if (isAssemblyStarted)
        {
            ShowMessage("Cannot buy — assembly already in progress!");
            return;
        }

        if (optionsListPopulator == null || optionsListPopulator.currentSelectedComponent == null)
        {
            ShowMessage("Please select a component first!");
            return;
        }

        OptionsListPopulator.ComponentOption selectedComponent = optionsListPopulator.currentSelectedComponent;
        ComponentType componentType = GetComponentTypeFromCategory(selectedComponent.category);
        int price = selectedComponent.price;

        if (worldSpawner.IsComponentOwned(componentType))
        {
            HandleReplacement(selectedComponent, componentType, price);
        }
        else
        {
            HandleNormalPurchase(selectedComponent, componentType, price);
        }
    }

    void HandleNormalPurchase(OptionsListPopulator.ComponentOption selectedComponent, ComponentType componentType, int price)
    {
        if (playerBalance != null && playerBalance.CanAfford(price))
        {
            playerBalance.DeductFunds(price);
            worldSpawner.SpawnModel(selectedComponent, componentType);
            ShowMessage("Purchased!");
            Debug.Log($"Purchased: {selectedComponent.optionName} for ${price}");
        }
        else
        {
            ShowMessage("Not enough coins!");
        }
    }

    void HandleReplacement(OptionsListPopulator.ComponentOption selectedComponent, ComponentType componentType, int newPrice)
    {
        int refundAmount = worldSpawner.GetRefundAmountForType(componentType);
        int netCost = newPrice - refundAmount;

        if (playerBalance != null && playerBalance.CanAfford(netCost))
        {
            if (refundAmount > 0)
            {
                playerBalance.AddFunds(refundAmount);
                Debug.Log($"Refunded ${refundAmount} for old {componentType}");
            }
            
            playerBalance.DeductFunds(newPrice);
            worldSpawner.SpawnModel(selectedComponent, componentType);
            
            ShowMessage($"Replaced! Net cost: ${netCost}");
            Debug.Log($"Replaced {componentType}: Net ${netCost}");
        }
        else
        {
            int needed = netCost - playerBalance.GetCurrentBalance();
            ShowMessage($"Not enough coins! Need ${needed} more after refund");
        }
    }

    ComponentType GetComponentTypeFromCategory(ComponentManager.ComponentCategory category)
    {
        switch (category)
        {
            case ComponentManager.ComponentCategory.CPU: return ComponentType.CPU;
            case ComponentManager.ComponentCategory.RAM: return ComponentType.RAM;
            case ComponentManager.ComponentCategory.GPU: return ComponentType.GPU;
            case ComponentManager.ComponentCategory.Motherboard: return ComponentType.Motherboard;
            case ComponentManager.ComponentCategory.Storage: return ComponentType.Storage;
            case ComponentManager.ComponentCategory.PSU: return ComponentType.PSU;
            case ComponentManager.ComponentCategory.Fan: return ComponentType.Fan;
            case ComponentManager.ComponentCategory.Cooler: return ComponentType.Cooler;
            default: return ComponentType.CPU;
        }
    }

    void ShowMessage(string message)
    {
        if (messageText != null)
        {
            StopAllCoroutines();
            messageText.text = message;
            StartCoroutine(ClearMessageAfterDelay());
        }
    }

    IEnumerator ClearMessageAfterDelay()
    {
        yield return new WaitForSeconds(messageDisplayTime);
        if (messageText != null)
        {
            messageText.text = "";
        }
    }

    public void LockPurchases()
    {
        isAssemblyStarted = true;
        if (buyButton != null)
        {
            buyButton.interactable = false;
        }
        Debug.Log("Purchases locked — assembly has started");
    }
}