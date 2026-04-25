using UnityEngine;
using TMPro;
using System;

public class PlayerBalance : MonoBehaviour
{
    [Header("Balance Settings")]
    public int startingBalance = 1500;
    public int currentBalance;

    [Header("UI Reference (Optional)")]
    public TextMeshProUGUI balanceDisplayText;

    // Event that triggers when balance changes
    public event Action<int> OnBalanceChanged;

    void Start()
    {
        currentBalance = startingBalance;
        UpdateBalanceDisplay();
    }

    public bool CanAfford(int price)
    {
        return currentBalance >= price;
    }

    public bool DeductFunds(int amount)
    {
        if (CanAfford(amount))
        {
            currentBalance -= amount;
            UpdateBalanceDisplay();
            OnBalanceChanged?.Invoke(currentBalance);
            Debug.Log($"Funds deducted: {amount}. New balance: {currentBalance}");
            return true;
        }
        else
        {
            Debug.Log($"Insufficient funds. Need: {amount}, Have: {currentBalance}");
            return false;
        }
    }

    public void AddFunds(int amount)
    {
        currentBalance += amount;
        UpdateBalanceDisplay();
        OnBalanceChanged?.Invoke(currentBalance);
        Debug.Log($"Funds added: {amount}. New balance: {currentBalance}");
    }

    public void SetBalance(int newBalance)
    {
        currentBalance = newBalance;
        UpdateBalanceDisplay();
        OnBalanceChanged?.Invoke(currentBalance);
    }

    private void UpdateBalanceDisplay()
    {
        if (balanceDisplayText != null)
        {
            balanceDisplayText.text = $"${currentBalance}";
        }
    }

    public int GetCurrentBalance()
    {
        return currentBalance;
    }
}