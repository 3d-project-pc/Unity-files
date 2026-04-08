using UnityEngine;

public class ComponentTag : MonoBehaviour
{
    [Header("Component Identification")]
    public ComponentType componentType;
    public string componentName;
    public int price;
    
    [Header("Spawn Tracking")]
    public string spawnPointID;
    
    [Header("Performance & Compatibility Stats")]
    public int powerDraw;
    public string socket;
    public string ramType;
    public int thermalOutput;
    public float coolerPerformance;
    public int fpsContribution;
    public int renderContribution;
    public int bootTimeReduction;
    public int bootTimeSec;
    public int singleCoreScore;
    public int multiCoreScore;
    public int maxWattage;
    public float efficiency;
    public int noiseLevelDb;
    
    [Header("Installation State")]
    public bool isInstalled;
    
    // Populate all fields from ComponentOption
    public void InitializeFromComponentOption(OptionsListPopulator.ComponentOption option, ComponentType type, string spawnID)
    {
        componentType = type;
        componentName = option.optionName;
        price = option.price;
        spawnPointID = spawnID;
        
        powerDraw = option.powerDraw;
        socket = option.socket;
        ramType = option.ramType;
        thermalOutput = option.thermalOutput;
        coolerPerformance = option.coolerPerformance;
        fpsContribution = option.fpsContribution;
        renderContribution = option.renderContribution;
        bootTimeReduction = option.bootTimeReduction;
        bootTimeSec = option.bootTimeSec;
        singleCoreScore = option.singleCoreScore;
        multiCoreScore = option.multiCoreScore;
        maxWattage = option.maxWattage;
        efficiency = option.efficiency;
        noiseLevelDb = option.noiseLevelDb;
        
        isInstalled = false;
    }
}

public enum ComponentType
{
    CPU,
    RAM,
    GPU,
    Cooler,
    Motherboard,
    PSU,
    Storage,
    Fan
}