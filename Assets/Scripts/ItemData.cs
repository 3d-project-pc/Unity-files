using UnityEngine;
using TMPro;

public class ItemData : MonoBehaviour
{
    [Header("Item Information")]
    public string itemName = "Item Name";
    public float price = 0;
    public string category = "CPU";
    public Vector3 customScale = new Vector3(0.3f, 0.3f, 0.3f);

    [Header("References")]
    public GameObject physicalPrefab;

    [Header("UI References (Optional)")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI priceText;

    void Start()
    {
        if (nameText != null)
        {
            nameText.text = itemName;
        }

        if (priceText != null)
        {
            priceText.text = "$" + price.ToString();
        }

        Debug.Log("ItemData initialized: " + itemName);
        Debug.Log("  - Category: " + category);
        Debug.Log("  - Physical Prefab: " + (physicalPrefab != null ? physicalPrefab.name : "NOT ASSIGNED!"));
    }

    public void BuyItem()
    {
        Debug.Log("=== BUY BUTTON CLICKED ===");
        Debug.Log("Item: " + itemName);
        Debug.Log("Price: $" + price);

        // Check if physicalPrefab is assigned
        if (physicalPrefab == null)
        {
            Debug.LogError("❌ PHYSICAL PREFAB IS NOT ASSIGNED for " + itemName + "!");
            Debug.LogError("Please drag a 3D model prefab to the Physical Prefab field in the Inspector.");
            return;
        }

        Debug.Log("✓ Physical Prefab is assigned: " + physicalPrefab.name);

        // Find ShopFilter
        ShopFilter shopFilter = FindObjectOfType<ShopFilter>();

        if (shopFilter == null)
        {
            Debug.LogError("❌ SHOP FILTER NOT FOUND in the scene!");
            Debug.LogError("Please add a GameObject with ShopFilter script to your scene.");
            return;
        }

        Debug.Log("✓ ShopFilter found: " + shopFilter.gameObject.name);

        // Check Table Spawn Point
        if (shopFilter.tableSpawnPoint == null)
        {
            Debug.LogError("❌ TABLE SPAWN POINT is not assigned in ShopFilter!");
            Debug.LogError("Please create an empty GameObject on your table and assign it to the Table Spawn Point field.");
            return;
        }

        Debug.Log("✓ Table Spawn Point found at: " + shopFilter.tableSpawnPoint.position);

        // Try to spawn
        Debug.Log("Attempting to spawn item...");
        shopFilter.SpawnItemOnTable(this);
    }
}