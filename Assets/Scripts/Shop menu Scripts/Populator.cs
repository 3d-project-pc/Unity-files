using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.IO;

public class OptionsListPopulator : MonoBehaviour
{
    [System.Serializable]
    public class ComponentOption
    {
        public string optionName;
        public ComponentManager.ComponentCategory category;
        public GameObject spawnPrefab;
        public GameObject presentationPrefab;
        public string detailedDescription;
        public string frequency;
        public string wattage;
        public string additionalSpecs;
        public int price;
        
        // Benchmark & Compatibility Fields
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
    }

    [Header("Folder Settings")]
    public string prefabsFolderPath = "Assets/Resources/Models";
    public bool scanOnStart = true;

    [Header("UI References")]
    public Transform optionsContainer;
    public GameObject optionButtonPrefab;

    [Header("Selected Component Display")]
    public TextMeshProUGUI selectedPriceText;

    [Header("References to Other Scripts")]
    public ModelSwapper modelSwapper;
    public DescriptionUpdater descriptionUpdater;

    public ComponentOption currentSelectedComponent { get; private set; }

    private List<ComponentOption> allComponents = new List<ComponentOption>();
    private ComponentManager.ComponentCategory currentCategory;

    void Start()
    {
        if (scanOnStart)
        {
            ScanForComponents();
        }

        ComponentManager componentManager = FindFirstObjectByType<ComponentManager>();
        if (componentManager != null)
        {
            componentManager.OnCategorySelected += OnCategoryChanged;
        }
        else
        {
            Debug.LogError("ComponentManager not found in the scene!");
        }

        UpdateSelectedPriceDisplay();
    }

    void ScanForComponents()
    {
        allComponents.Clear();

        string[] categoryNames = System.Enum.GetNames(typeof(ComponentManager.ComponentCategory));

        foreach (string categoryName in categoryNames)
        {
            string categoryFolder = Path.Combine(prefabsFolderPath, categoryName);
            
            if (Directory.Exists(categoryFolder))
            {
                string[] jsonFiles = Directory.GetFiles(categoryFolder, "*.json", SearchOption.TopDirectoryOnly);

                foreach (string jsonPath in jsonFiles)
                {
                    LoadComponentFromJson(jsonPath, categoryName);
                }
            }
        }
    }

    void LoadComponentFromJson(string jsonPath, string categoryName)
    {
        string jsonContent = File.ReadAllText(jsonPath);
        
        ComponentJsonData jsonData = JsonUtility.FromJson<ComponentJsonData>(jsonContent);
        
        if (jsonData == null)
        {
            Debug.LogWarning($"Failed to parse JSON: {jsonPath}");
            return;
        }

        string categoryFolder = Path.Combine(prefabsFolderPath, categoryName);
        
        // Load spawn prefab from SpawnPrefabs folder
        string spawnPrefabPath = Path.Combine(categoryFolder, "SpawnPrefabs", jsonData.modelPrefab);
        string spawnRelativePath = spawnPrefabPath.Replace("Assets/Resources/", "").Replace(".prefab", "");
        GameObject spawnPrefab = Resources.Load<GameObject>(spawnRelativePath);

        if (spawnPrefab == null)
        {
            Debug.LogError($"Spawn prefab not found: {jsonData.modelPrefab} in {categoryFolder}/SpawnPrefabs/");
            return;
        }

        // Load presentation prefab from PresentationPrefabs folder
        GameObject presentationPrefab = null;
        
        if (!string.IsNullOrEmpty(jsonData.modelPrefabForScaling))
        {
            string presentationPrefabPath = Path.Combine(categoryFolder, "PresentationPrefabs", jsonData.modelPrefabForScaling);
            string presentationRelativePath = presentationPrefabPath.Replace("Assets/Resources/", "").Replace(".prefab", "");
            presentationPrefab = Resources.Load<GameObject>(presentationRelativePath);

            if (presentationPrefab == null)
            {
                Debug.LogError($"Presentation prefab not found: {jsonData.modelPrefabForScaling} in {categoryFolder}/PresentationPrefabs/");
                return;
            }
        }
        else
        {
            Debug.LogWarning($"No modelPrefabForScaling specified in {jsonPath}. Using spawn prefab for presentation.");
            presentationPrefab = spawnPrefab;
        }

        ComponentOption option = new ComponentOption();
        option.optionName = jsonData.optionName;
        option.category = (ComponentManager.ComponentCategory)System.Enum.Parse(typeof(ComponentManager.ComponentCategory), categoryName);
        option.spawnPrefab = spawnPrefab;
        option.presentationPrefab = presentationPrefab;
        option.detailedDescription = jsonData.detailedDescription;
        option.frequency = jsonData.frequency;
        option.wattage = jsonData.wattage;
        option.additionalSpecs = jsonData.additionalSpecs;
        option.price = jsonData.price;
        
        // Populate benchmark & compatibility fields
        option.powerDraw = jsonData.powerDraw;
        option.socket = jsonData.socket ?? "";
        option.ramType = jsonData.ramType ?? "";
        option.thermalOutput = jsonData.thermalOutput;
        option.coolerPerformance = jsonData.coolerPerformance;
        option.fpsContribution = jsonData.fpsContribution;
        option.renderContribution = jsonData.renderContribution;
        option.bootTimeReduction = jsonData.bootTimeReduction;
        option.bootTimeSec = jsonData.bootTimeSec;
        option.singleCoreScore = jsonData.singleCoreScore;
        option.multiCoreScore = jsonData.multiCoreScore;
        option.maxWattage = jsonData.maxWattage;
        option.efficiency = jsonData.efficiency;
        option.noiseLevelDb = jsonData.noiseLevelDb;

        allComponents.Add(option);
        Debug.Log($"Loaded component: {option.optionName} (Price: {option.price})");
    }

    [System.Serializable]
    class ComponentJsonData
    {
        public string optionName;
        public string modelPrefab;
        public string modelPrefabForScaling;
        public string detailedDescription;
        public string frequency;
        public string wattage;
        public string additionalSpecs;
        public int price;
        
        // Benchmark & Compatibility Fields
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
    }

    void OnCategoryChanged(ComponentManager.ComponentCategory newCategory)
    {
        currentCategory = newCategory;
        PopulateOptions();
    }

    void PopulateOptions()
    {
        foreach (Transform child in optionsContainer)
        {
            Destroy(child.gameObject);
        }

        List<ComponentOption> filteredOptions = allComponents.FindAll(option => option.category == currentCategory);

        foreach (ComponentOption option in filteredOptions)
        {
            GameObject buttonObj = Instantiate(optionButtonPrefab, optionsContainer);
            Button button = buttonObj.GetComponent<Button>();
            TextMeshProUGUI buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();

            if (buttonText != null)
            {
                buttonText.text = $"{option.optionName} - ${option.price}";
            }

            ComponentOption capturedOption = option;
            button.onClick.AddListener(() => OnOptionSelected(capturedOption));
        }

        Debug.Log($"Populated {filteredOptions.Count} options for category: {currentCategory}");
    }

    void OnOptionSelected(ComponentOption selectedOption)
    {
        Debug.Log($"Selected: {selectedOption.optionName} (Price: ${selectedOption.price})");
        
        currentSelectedComponent = selectedOption;
        
        UpdateSelectedPriceDisplay();

        if (modelSwapper != null)
        {
            modelSwapper.SwapModel(selectedOption.presentationPrefab);
        }

        if (descriptionUpdater != null)
        {
            descriptionUpdater.UpdateDetails(selectedOption);
        }
    }

    private void UpdateSelectedPriceDisplay()
    {
        if (selectedPriceText != null)
        {
            if (currentSelectedComponent != null)
            {
                selectedPriceText.text = $"Price: ${currentSelectedComponent.price}";
            }
            else
            {
                selectedPriceText.text = "Price: --";
            }
        }
    }

    public void RefreshComponents()
    {
        ScanForComponents();
        if (descriptionUpdater != null)
        {
            OnCategoryChanged(currentCategory);
        }
    }
}