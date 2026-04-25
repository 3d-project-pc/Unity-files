using UnityEngine;
using System.Collections.Generic;

public class WorldSpawner : MonoBehaviour
{
    [Header("Spawn Points")]
    public Transform spawnPoint_CPU;
    public Transform spawnPoint_Cooler;
    public Transform spawnPoint_GPU;
    public Transform spawnPoint_Motherboard;
    public Transform spawnPoint_PSU;
    public Transform spawnPoint_Storage;

    [Header("RAM Spawn Points (2 sticks)")]
    public Transform[] spawnPoints_RAM;  // Size = 2

    [Header("Fan Spawn Points (3 fans)")]
    public Transform[] spawnPoints_Fans;  // Size = 3

    [Header("Spawn Settings")]
    public bool destroyPreviousOnReplace = true;

    // Track which spawn points are currently occupied
    private Dictionary<Transform, GameObject> occupiedSpawnPoints = new Dictionary<Transform, GameObject>();

    // Track spawned components by type for replacement    private List<GameObject> spawnedCPUs = new List<GameObject>();
    private List<GameObject> spawnedCoolers = new List<GameObject>();
    private List<GameObject> spawnedGPUs = new List<GameObject>();
    private List<GameObject> spawnedMotherboards = new List<GameObject>();
    private List<GameObject> spawnedPSUs = new List<GameObject>();
    private List<GameObject> spawnedStorages = new List<GameObject>();
    private List<GameObject> spawnedRAMs = new List<GameObject>();
    private List<GameObject> spawnedFans = new List<GameObject>();

    // Store the original pair price for RAM and fans (for refund calculations)
    private int lastRAMPairPrice = 0;
    private int lastFanSetPrice = 0;

    // Public properties to access spawned components
    public List<GameObject> SpawnedRAMs => spawnedRAMs;
    public List<GameObject> SpawnedFans => spawnedFans;
    public GameObject SpawnedCPU => spawnedCPUs.Count > 0 ? spawnedCPUs[0] : null;
    public GameObject SpawnedCooler => spawnedCoolers.Count > 0 ? spawnedCoolers[0] : null;
    public GameObject SpawnedGPU => spawnedGPUs.Count > 0 ? spawnedGPUs[0] : null;
    public GameObject SpawnedMotherboard => spawnedMotherboards.Count > 0 ? spawnedMotherboards[0] : null;
    public GameObject SpawnedPSU => spawnedPSUs.Count > 0 ? spawnedPSUs[0] : null;
    public GameObject SpawnedStorage => spawnedStorages.Count > 0 ? spawnedStorages[0] : null;

    // Public property to get all spawned components for benchmark
    public List<ComponentTag> GetAllSpawnedComponents()
    {
        List<ComponentTag> allComponents = new List<ComponentTag>();

        AddComponentsToList(spawnedCPUs, allComponents);
        AddComponentsToList(spawnedCoolers, allComponents);
        AddComponentsToList(spawnedGPUs, allComponents);
        AddComponentsToList(spawnedMotherboards, allComponents);
        AddComponentsToList(spawnedPSUs, allComponents);
        AddComponentsToList(spawnedStorages, allComponents);
        AddComponentsToList(spawnedRAMs, allComponents);
        AddComponentsToList(spawnedFans, allComponents);

        return allComponents;
    }

    private void AddComponentsToList(List<GameObject> source, List<ComponentTag> target)
    {
        foreach (GameObject obj in source)
        {
            if (obj != null)
            {
                ComponentTag tag = obj.GetComponent<ComponentTag>();
                if (tag != null)
                    target.Add(tag);
            }
        }
    }

    void Start()
    {
        // Validate spawn point arrays
        if (spawnPoints_RAM == null || spawnPoints_RAM.Length != 2)
            Debug.LogError("RAM spawn points array must have exactly 2 entries!");

        if (spawnPoints_Fans == null || spawnPoints_Fans.Length != 3)
            Debug.LogError("Fan spawn points array must have exactly 3 entries!");
    }

    // Main spawn method - receives ComponentOption directly
    public void SpawnModel(OptionsListPopulator.ComponentOption optionData, ComponentType componentType)
    {
        if (optionData == null || optionData.spawnPrefab == null)
        {
            Debug.LogWarning("Attempted to spawn with null optionData or spawnPrefab");
            return;
        }

        switch (componentType)
        {
            case ComponentType.RAM:
                SpawnRAMPair(optionData);
                break;
            case ComponentType.Fan:
                SpawnFanSet(optionData);
                break;
            default:
                SpawnSingleComponent(optionData, componentType);
                break;
        }
        AutoUpdateBenchmark();
    }

    private void AutoUpdateBenchmark()
    {
        BenchmarkUI benchmarkUI = FindFirstObjectByType<BenchmarkUI>();
	if (benchmarkUI != null && benchmarkUI.benchmarkPanel != null && benchmarkUI.benchmarkPanel.activeSelf)
        {
            // Only refresh when the benchmark panel is already visible
            benchmarkUI.RefreshBenchmark();
            Debug.Log("Benchmark auto-refreshed after component spawn");
        }
    }
    // Spawn single component (CPU, GPU, Cooler, Motherboard, PSU, Storage)
    private void SpawnSingleComponent(OptionsListPopulator.ComponentOption optionData, ComponentType componentType)
    {
        Transform spawnPoint = GetSpawnPointForType(componentType);
        if (spawnPoint == null)
        {
            Debug.LogError($"No spawn point assigned for {componentType}");
            return;
        }

        // Clear existing component if any
        ClearExistingComponent(componentType);

        // Spawn new component
        GameObject newComponent = InstantiateComponent(optionData, spawnPoint, componentType, "");

        // Add to tracking list
        AddToTrackingList(newComponent, componentType);

        // Mark spawn point as occupied
        occupiedSpawnPoints[spawnPoint] = newComponent;

        Debug.Log($"Spawned {componentType}: {optionData.optionName} at {spawnPoint.name}");
    }

    // Spawn 2 RAM sticks - splits the price between both sticks
    private void SpawnRAMPair(OptionsListPopulator.ComponentOption optionData)
    {
        // Store the original pair price for refund calculations
        lastRAMPairPrice = optionData.price;

        // Calculate price per stick (half of the pair price, rounded)
        int pricePerStick = Mathf.RoundToInt(optionData.price / 2f);

        // Clear existing RAMs
        ClearExistingComponents(spawnedRAMs);

        // Create a copy of optionData for individual sticks with modified price
        OptionsListPopulator.ComponentOption stickOption = new OptionsListPopulator.ComponentOption();
        CopyComponentOption(optionData, stickOption);
        stickOption.price = pricePerStick;

        // Spawn 2 new RAM sticks
        for (int i = 0; i < spawnPoints_RAM.Length; i++)
        {
            Transform spawnPoint = spawnPoints_RAM[i];
            if (spawnPoint == null)
            {
                Debug.LogError($"RAM spawn point {i} is not assigned!");
                continue;
            }

            string spawnID = $"RAM_{i + 1}";
            GameObject ramStick = InstantiateComponent(stickOption, spawnPoint, ComponentType.RAM, spawnID);

            spawnedRAMs.Add(ramStick);
            occupiedSpawnPoints[spawnPoint] = ramStick;
        }

        Debug.Log($"Spawned {spawnedRAMs.Count} RAM sticks (Pair price: ${lastRAMPairPrice}, Each: ${pricePerStick})");
    }

    // Spawn 3 fans - splits the price between all fans
    private void SpawnFanSet(OptionsListPopulator.ComponentOption optionData)
    {
        // Store the original set price for refund calculations
        lastFanSetPrice = optionData.price;

        // Calculate price per fan (third of the set price, rounded)
        int pricePerFan = Mathf.RoundToInt(optionData.price / 3f);

        // Clear existing fans
        ClearExistingComponents(spawnedFans);

        // Create a copy of optionData for individual fans with modified price
        OptionsListPopulator.ComponentOption fanOption = new OptionsListPopulator.ComponentOption();
        CopyComponentOption(optionData, fanOption);
        fanOption.price = pricePerFan;

        // Spawn 3 new fans
        for (int i = 0; i < spawnPoints_Fans.Length; i++)
        {
            Transform spawnPoint = spawnPoints_Fans[i];
            if (spawnPoint == null)
            {
                Debug.LogError($"Fan spawn point {i} is not assigned!");
                continue;
            }

            string spawnID = $"Fan_{i + 1}";
            GameObject fan = InstantiateComponent(fanOption, spawnPoint, ComponentType.Fan, spawnID);

            spawnedFans.Add(fan);
            occupiedSpawnPoints[spawnPoint] = fan;
        }

        Debug.Log($"Spawned {spawnedFans.Count} fans (Set price: ${lastFanSetPrice}, Each: ${pricePerFan})");
    }

    // Helper method to copy ComponentOption data
    private void CopyComponentOption(OptionsListPopulator.ComponentOption source, OptionsListPopulator.ComponentOption destination)
    {
        destination.optionName = source.optionName;
        destination.category = source.category;
        destination.spawnPrefab = source.spawnPrefab;
        destination.presentationPrefab = source.presentationPrefab;
        destination.detailedDescription = source.detailedDescription;
        destination.frequency = source.frequency;
        destination.wattage = source.wattage;
        destination.additionalSpecs = source.additionalSpecs;
        destination.price = source.price;

        // Copy benchmark & compatibility fields
        destination.powerDraw = source.powerDraw;
        destination.socket = source.socket;
        destination.ramType = source.ramType;
        destination.thermalOutput = source.thermalOutput;
        destination.coolerPerformance = source.coolerPerformance;
        destination.fpsContribution = source.fpsContribution;
        destination.renderContribution = source.renderContribution;
        destination.bootTimeReduction = source.bootTimeReduction;
        destination.bootTimeSec = source.bootTimeSec;
        destination.singleCoreScore = source.singleCoreScore;
        destination.multiCoreScore = source.multiCoreScore;
        destination.maxWattage = source.maxWattage;
        destination.efficiency = source.efficiency;
        destination.noiseLevelDb = source.noiseLevelDb;
    }

    // Instantiate a component and populate its ComponentTag WITHOUT modifying transform
    private GameObject InstantiateComponent(OptionsListPopulator.ComponentOption optionData, Transform spawnPoint, ComponentType componentType, string spawnID)
    {
        // Instantiate at spawn point position and rotation WITHOUT any modifications
        GameObject newComponent = Instantiate(optionData.spawnPrefab, spawnPoint.position, optionData.spawnPrefab.transform.rotation);

        // DO NOT apply custom scale or rotation - keep prefab's original transform

        // Get or add ComponentTag
        ComponentTag tag = newComponent.GetComponent<ComponentTag>();
        if (tag == null)
            tag = newComponent.AddComponent<ComponentTag>();

        // Populate ComponentTag
        tag.InitializeFromComponentOption(optionData, componentType, spawnID);

        return newComponent;
    }

    // Get spawn point for single-component types
    private Transform GetSpawnPointForType(ComponentType type)
    {
        switch (type)
        {
            case ComponentType.CPU: return spawnPoint_CPU;
            case ComponentType.Cooler: return spawnPoint_Cooler;
            case ComponentType.GPU: return spawnPoint_GPU;
            case ComponentType.Motherboard: return spawnPoint_Motherboard;
            case ComponentType.PSU: return spawnPoint_PSU;
            case ComponentType.Storage: return spawnPoint_Storage;
            default: return null;
        }
    }

    // Clear a single existing component (for replacement)
    private void ClearExistingComponent(ComponentType type)
    {
        List<GameObject> targetList = GetTrackingListForType(type);
        ClearExistingComponents(targetList);
    }

    // Clear a list of components
    private void ClearExistingComponents(List<GameObject> componentList)
    {
        foreach (GameObject component in componentList)
        {
            if (component != null)
            {
                // Remove from occupied spawn points
                ComponentTag tag = component.GetComponent<ComponentTag>();
                if (tag != null && !string.IsNullOrEmpty(tag.spawnPointID))
                {
                    Transform spawnPoint = FindSpawnPointByID(tag.spawnPointID);
                    if (spawnPoint != null)
                        occupiedSpawnPoints.Remove(spawnPoint);
                }

                Destroy(component);
            }
        }
        componentList.Clear();
    }

    // Add component to tracking list
    private void AddToTrackingList(GameObject component, ComponentType type)
    {
        switch (type)
        {
            case ComponentType.CPU: spawnedCPUs.Add(component); break;
            case ComponentType.Cooler: spawnedCoolers.Add(component); break;
            case ComponentType.GPU: spawnedGPUs.Add(component); break;
            case ComponentType.Motherboard: spawnedMotherboards.Add(component); break;
            case ComponentType.PSU: spawnedPSUs.Add(component); break;
            case ComponentType.Storage: spawnedStorages.Add(component); break;
                // RAM and Fan are handled separately in their own methods
        }
    }

    // Get tracking list for a component type
    private List<GameObject> GetTrackingListForType(ComponentType type)
    {
        switch (type)
        {
            case ComponentType.CPU: return spawnedCPUs;
            case ComponentType.Cooler: return spawnedCoolers;
            case ComponentType.GPU: return spawnedGPUs;
            case ComponentType.Motherboard: return spawnedMotherboards;
            case ComponentType.PSU: return spawnedPSUs;
            case ComponentType.Storage: return spawnedStorages;
            case ComponentType.RAM: return spawnedRAMs;
            case ComponentType.Fan: return spawnedFans;
            default: return null;
        }
    }

    // Find spawn point by its ID string (for RAM and fans)
    private Transform FindSpawnPointByID(string id)
    {
        if (id.StartsWith("RAM_"))
        {
            int index = int.Parse(id.Substring(4)) - 1;
            if (index >= 0 && index < spawnPoints_RAM.Length)
                return spawnPoints_RAM[index];
        }
        else if (id.StartsWith("Fan_"))
        {
            int index = int.Parse(id.Substring(4)) - 1;
            if (index >= 0 && index < spawnPoints_Fans.Length)
                return spawnPoints_Fans[index];
        }
        return null;
    }

    // Check if a component type is already owned
    public bool IsComponentOwned(ComponentType type)
    {
        List<GameObject> list = GetTrackingListForType(type);
        return list != null && list.Count > 0;
    }

    // Get total refund amount for a component type
    public int GetRefundAmountForType(ComponentType type)
    {
        List<GameObject> list = GetTrackingListForType(type);
        int total = 0;
        foreach (GameObject component in list)
        {
            if (component != null)
            {
                ComponentTag tag = component.GetComponent<ComponentTag>();
                if (tag != null)
                    total += tag.price;
            }
        }
        return total;
    }

    // Clear all spawned components (for reset)
    public void ClearAllSpawnedComponents()
    {
        ClearExistingComponents(spawnedCPUs);
        ClearExistingComponents(spawnedCoolers);
        ClearExistingComponents(spawnedGPUs);
        ClearExistingComponents(spawnedMotherboards);
        ClearExistingComponents(spawnedPSUs);
        ClearExistingComponents(spawnedStorages);
        ClearExistingComponents(spawnedRAMs);
        ClearExistingComponents(spawnedFans);
        occupiedSpawnPoints.Clear();
        lastRAMPairPrice = 0;
        lastFanSetPrice = 0;
    }
}
