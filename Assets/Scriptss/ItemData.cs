using UnityEngine;

public class ItemData : MonoBehaviour
{
    public GameObject physicalPrefab; // Drag your 3D GPU prefab here
    private Transform spawnPoint;

    void Start()
    {
        // Find the spawn point in the scene by name
        spawnPoint = GameObject.Find("SpawnPoint").transform;
    }

    public void BuyItem()
    {
        if (physicalPrefab != null && spawnPoint != null)
        {
            // This creates the 3D object at the spawn point's position and rotation
            Instantiate(physicalPrefab, spawnPoint.position, spawnPoint.rotation);

            Debug.Log("Purchased " + physicalPrefab.name);
        }
        else
        {
            Debug.LogError("Missing Prefab or SpawnPoint!");
        }
    }
}