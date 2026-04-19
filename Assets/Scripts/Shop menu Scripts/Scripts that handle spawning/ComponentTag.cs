using UnityEngine;

// ===== ENUM DEFINITION =====
public enum ComponentType
{
    CPU,
    GPU,
    RAM,
    Storage,
    PSU,
    Cooler,
    Fan,
    Motherboard,
    Case,
    Unknown
}
// ===========================

public class ComponentTag : MonoBehaviour
{
    [Header("Basic Info")]
    public string componentName;
    public ComponentType componentType;  // Now using the enum!
    public string spawnPointID;
    public int price;
    
    [Header("Existing JSON Fields")]
    public string optionName;
    public string modelPrefab;
    public string modelPrefabForScaling;
    public string detailedDescription;
    public string frequency;
    public string wattage;
    public string additionalSpecs;
    
    [Header("Installation State")]
    public bool isInstalled = false;
    public GameObject currentSlot;
    
    [Header("Benchmark Stats")]
    public float singleCoreScore;
    public float multiCoreScore;
    public float fpsContribution;
    public float bootTimeReduction;
    public float powerDraw;
    public float baseTemp;
    public float coolingCapacity;
    public float noiseLevel;
    public string socket;
    public int maxWattage;
    public float efficiency;
    public int vramGB;
    public string chipsetTier;
    public float renderContribution;
    public float thermalOutput;
    public float coolerPerformance;
    public float bootTimeSec;
    public string ramType;
    
    // Initialize from ComponentOption
    public void InitializeFromComponentOption(OptionsListPopulator.ComponentOption optionData, ComponentType type, string spawnID)
    {
        componentName = optionData.optionName;
        componentType = type;
        spawnPointID = spawnID;
        price = optionData.price;
        
        optionName = optionData.optionName;
        modelPrefab = optionData.spawnPrefab != null ? optionData.spawnPrefab.name : "";
        modelPrefabForScaling = optionData.presentationPrefab != null ? optionData.presentationPrefab.name : "";
        detailedDescription = optionData.detailedDescription;
        frequency = optionData.frequency;
        wattage = optionData.wattage;
        additionalSpecs = optionData.additionalSpecs;
        
        singleCoreScore = optionData.singleCoreScore;
        multiCoreScore = optionData.multiCoreScore;
        fpsContribution = optionData.fpsContribution;
        bootTimeReduction = optionData.bootTimeReduction;
        powerDraw = optionData.powerDraw;
        baseTemp = optionData.thermalOutput;
        coolingCapacity = optionData.coolerPerformance;
        noiseLevel = optionData.noiseLevelDb;
        socket = optionData.socket;
        maxWattage = optionData.maxWattage;
        efficiency = optionData.efficiency;
        renderContribution = optionData.renderContribution;
        thermalOutput = optionData.thermalOutput;
        coolerPerformance = optionData.coolerPerformance;
        bootTimeSec = optionData.bootTimeSec;
        ramType = optionData.ramType;
    }
    
    // Helper methods for benchmark
    public float GetPowerDraw() => powerDraw;
    public float GetSingleCoreScore() => componentType == ComponentType.CPU ? singleCoreScore : 0;
    public float GetMultiCoreScore() => componentType == ComponentType.CPU ? multiCoreScore : 0;
    public float GetFPSContribution() => (componentType == ComponentType.GPU || componentType == ComponentType.RAM) ? fpsContribution : 0;
    public float GetBootTimeReduction() => componentType == ComponentType.Storage ? bootTimeReduction : 0;
    public float GetBaseTemp() => (componentType == ComponentType.CPU || componentType == ComponentType.GPU) ? thermalOutput : 0;
    public float GetCoolingCapacity() => componentType == ComponentType.Cooler ? coolerPerformance : 0;
    public float GetNoiseLevel() => (componentType == ComponentType.Cooler || componentType == ComponentType.Fan) ? noiseLevel : 0;
    public int GetMaxWattage() => componentType == ComponentType.PSU ? maxWattage : 0;
}