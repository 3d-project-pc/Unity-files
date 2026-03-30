using UnityEngine;

public class WorldSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public bool destroyPreviousModel = true;
    
    [Header("Rotation")]
    public Vector3 spawnRotation = Vector3.zero;
    
    [Header("Scale")]
    public bool useCustomScale = false;           // If false, keeps prefab's original scale
    public Vector3 customScale = Vector3.one;     // Only used if useCustomScale is true

    private GameObject currentSpawnedModel;

    private Vector3 SpawnPosition => transform.position;

    public void SpawnModel(GameObject modelPrefab)
    {
        if (modelPrefab == null)
        {
            Debug.LogWarning("Attempted to spawn a null model prefab");
            return;
        }

        if (destroyPreviousModel && currentSpawnedModel != null)
        {
            Destroy(currentSpawnedModel);
            Debug.Log("Previous model destroyed");
        }

        currentSpawnedModel = Instantiate(modelPrefab, SpawnPosition, Quaternion.Euler(spawnRotation));
        
        // Apply scale based on setting
        if (useCustomScale)
        {
            currentSpawnedModel.transform.localScale = customScale;
            Debug.Log($"Applied custom scale: {customScale}");
        }
        else
        {
            // Keep the prefab's original scale
            Debug.Log($"Using prefab's original scale: {currentSpawnedModel.transform.localScale}");
        }
        
        Debug.Log($"Spawned model: {modelPrefab.name} at {SpawnPosition}");
    }

    public void ClearSpawnedModel()
    {
        if (currentSpawnedModel != null)
        {
            Destroy(currentSpawnedModel);
            currentSpawnedModel = null;
            Debug.Log("Spawned model cleared");
        }
    }

    public GameObject GetCurrentSpawnedModel()
    {
        return currentSpawnedModel;
    }
}