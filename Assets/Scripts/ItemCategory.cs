using UnityEngine;
using TMPro;

public enum ItemCategory { CPU, GPU, Motherboard, RAM, Disk, Fan, Peripheral }

public class ShopItem : MonoBehaviour
{
    public ItemCategory category;

    // You can also link your UI elements here to fill them via code later
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI priceText;
}