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
        public GameObject modelPrefab;
        public string detailedDescription;
        public string frequency;
        public string wattage;
        public string additionalSpecs;
        public int price;
    }

    [Header("Folder Settings")]
    public string prefabsFolderPath = "Assets/Resources/Models";
    public bool scanOnStart = true;

    [Header("UI References")]
    public Transform optionsContainer;
    public GameObject optionButtonPrefab;

    [Header("Selected Component Display")]
    public TextMeshProUGUI selectedPriceText;  // NEW: TMP to show price of selected component

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

        // Clear selected price display at start
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

        string prefabName = jsonData.modelPrefab;
        string prefabPath = Path.Combine(prefabsFolderPath, categoryName, prefabName);
        string relativePath = prefabPath.Replace("Assets/Resources/", "").Replace(".prefab", "");
        GameObject prefab = Resources.Load<GameObject>(relativePath);

        if (prefab == null)
        {
            Debug.LogWarning($"Model prefab not found: {prefabName} in {categoryName}");
            return;
        }

        ComponentOption option = new ComponentOption();
        option.optionName = jsonData.optionName;
        option.category = (ComponentManager.ComponentCategory)System.Enum.Parse(typeof(ComponentManager.ComponentCategory), categoryName);
        option.modelPrefab = prefab;
        option.detailedDescription = jsonData.detailedDescription;
        option.frequency = jsonData.frequency;
        option.wattage = jsonData.wattage;
        option.additionalSpecs = jsonData.additionalSpecs;
        option.price = jsonData.price;

        allComponents.Add(option);
        Debug.Log($"Loaded component: {option.optionName} (Price: {option.price}) from {jsonPath}");
    }

    [System.Serializable]
    class ComponentJsonData
    {
        public string optionName;
        public string modelPrefab;
        public string detailedDescription;
        public string frequency;
        public string wattage;
        public string additionalSpecs;
        public int price;
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
                buttonText.text = $"{option.optionName}";
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
        
        // Update the selected price display
        UpdateSelectedPriceDisplay();

        if (modelSwapper != null)
        {
            modelSwapper.SwapModel(selectedOption.modelPrefab);
        }

        if (descriptionUpdater != null)
        {
            descriptionUpdater.UpdateDetails(selectedOption);
        }
    }

    // NEW: Updates the TMP text showing the price of the selected component
    private void UpdateSelectedPriceDisplay()
    {
        if (selectedPriceText != null)
        {
            if (currentSelectedComponent != null)
            {
                selectedPriceText.text = $"${currentSelectedComponent.price}";
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