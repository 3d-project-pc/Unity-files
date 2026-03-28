using UnityEngine;
using System.Collections.Generic;

public class ShopFilter : MonoBehaviour
{
    public Transform container;
    public GameObject menuPanel;
    public Transform tableSpawnPoint;
    private GameObject currentObjectOnTable;

    void Start()
    {
        ShowAllItems();

        if (menuPanel != null)
        {
            menuPanel.SetActive(false);
        }

        Debug.Log("=== SHOP FILTER STARTED ===");
        Debug.Log("Table Spawn Point: " + (tableSpawnPoint != null ? tableSpawnPoint.name : "NOT ASSIGNED!"));
        Debug.Log("Container: " + (container != null ? container.name : "NOT ASSIGNED!"));
    }

    public void ToggleMenu()
    {
        if (menuPanel != null)
        {
            menuPanel.SetActive(!menuPanel.activeSelf);
            Debug.Log("Menu toggled: " + menuPanel.activeSelf);
        }
        else
        {
            Debug.LogWarning("Menu Panel is not assigned!");
        }
    }

    public void FilterItems(string categoryName)
    {
        if (container == null)
        {
            Debug.LogError("Container is not assigned in ShopFilter!");
            return;
        }

        Debug.Log("Filtering items: " + categoryName);
        int visibleCount = 0;

        foreach (Transform child in container)
        {
            ItemData item = child.GetComponent<ItemData>();
            if (item != null)
            {
                if (categoryName == "All")
                {
                    child.gameObject.SetActive(true);
                    visibleCount++;
                }
                else if (item.category == categoryName)
                {
                    child.gameObject.SetActive(true);
                    visibleCount++;
                }
                else
                {
                    child.gameObject.SetActive(false);
                }
            }
        }

        Debug.Log("Visible items after filter: " + visibleCount);
    }

    public void ShowAllItems()
    {
        if (container == null) return;

        foreach (Transform child in container)
        {
            ItemData item = child.GetComponent<ItemData>();
            if (item != null)
            {
                child.gameObject.SetActive(true);
            }
        }
    }

    public void SpawnItemOnTable(ItemData item)
    {
        Debug.Log("=== SPAWN ITEM ON TABLE ===");

        // Check if we have everything we need
        if (item == null)
        {
            Debug.LogError("❌ ItemData is null!");
            return;
        }

        Debug.Log("Item: " + item.itemName);

        if (item.physicalPrefab == null)
        {
            Debug.LogError("❌ Physical Prefab is missing for: " + item.itemName);
            return;
        }

        Debug.Log("Physical Prefab: " + item.physicalPrefab.name);

        if (tableSpawnPoint == null)
        {
            Debug.LogError("❌ Table Spawn Point is not assigned in ShopFilter!");
            Debug.LogError("Please assign a Transform (empty GameObject) to the Table Spawn Point field in the Inspector.");
            return;
        }

        Debug.Log("Table Spawn Point position: " + tableSpawnPoint.position);

        // Delete existing object
        if (currentObjectOnTable != null)
        {
            Debug.Log("Removing old object: " + currentObjectOnTable.name);
            Destroy(currentObjectOnTable);
            currentObjectOnTable = null;
        }

        // Spawn new object
        Debug.Log("Instantiating new object...");
        currentObjectOnTable = Instantiate(item.physicalPrefab, tableSpawnPoint.position, tableSpawnPoint.rotation);
        currentObjectOnTable.transform.SetParent(tableSpawnPoint);

        // Apply scale if custom scale is set
        if (item.customScale != Vector3.zero)
        {
            currentObjectOnTable.transform.localScale = item.customScale;
            Debug.Log("Applied custom scale: " + item.customScale);
        }

        Debug.Log("✅ SUCCESS! Spawned: " + item.itemName + " on table");
        Debug.Log("Object position: " + currentObjectOnTable.transform.position);
    }

    public void DeleteCurrentObject()
    {
        if (currentObjectOnTable != null)
        {
            Destroy(currentObjectOnTable);
            currentObjectOnTable = null;
            Debug.Log("Deleted object from table");
        }
        else
        {
            Debug.Log("No object to delete");
        }
    }
}