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
        if (optionsListPopulator == null || optionsListPopulator.currentSelectedComponent == null)
        {
            ShowMessage("Please select a component first!");
            return;
        }

        OptionsListPopulator.ComponentOption selectedComponent = optionsListPopulator.currentSelectedComponent;
        int price = selectedComponent.price;

        if (playerBalance != null && playerBalance.CanAfford(price))
        {
            playerBalance.DeductFunds(price);

            if (worldSpawner != null)
            {
                // UPDATED: Use spawnPrefab instead of modelPrefab
                worldSpawner.SpawnModel(selectedComponent.spawnPrefab);
            }
            else
            {
                Debug.LogError("WorldSpawner reference is missing!");
            }

            ShowMessage("Purchased!");
            Debug.Log($"Purchased: {selectedComponent.optionName} for ${price}");
        }
        else
        {
            ShowMessage("Not enough coins!");
            Debug.Log($"Insufficient funds for: {selectedComponent.optionName} (${price})");
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
}