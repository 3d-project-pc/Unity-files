using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class DescriptionUpdater : MonoBehaviour
{
    [Header("UI Text References")]
    public TextMeshProUGUI generalDescriptionText;
    public TextMeshProUGUI detailedDescriptionText;
    public TextMeshProUGUI specsText;

    [Header("Default Texts")]
    [TextArea(2, 3)]
    public string defaultGeneralText = "Select a component category to learn what it does.";
    [TextArea(2, 3)]
    public string defaultDetailedText = "Select a component to view details.";
    public string defaultSpecsText = "";

    [Header("Category Descriptions")]
    public List<CategoryDescription> categoryDescriptions;

    [System.Serializable]
    public class CategoryDescription
    {
        public ComponentManager.ComponentCategory category;
        [TextArea(3, 5)]
        public string description;
    }

    private ComponentManager.ComponentCategory currentCategory;

    void Start()
    {
        // Set default texts on start
        SetDefaultTexts();

        ComponentManager componentManager = FindFirstObjectByType<ComponentManager>();
        if (componentManager != null)
        {
            componentManager.OnCategorySelected += OnCategoryChanged;
        }
        else
        {
            Debug.LogError("ComponentManager not found in the scene!");
        }
    }

    void SetDefaultTexts()
    {
        if (generalDescriptionText != null)
        {
            generalDescriptionText.text = defaultGeneralText;
        }

        if (detailedDescriptionText != null)
        {
            detailedDescriptionText.text = defaultDetailedText;
        }

        if (specsText != null)
        {
            specsText.text = defaultSpecsText;
        }
    }

    void OnCategoryChanged(ComponentManager.ComponentCategory newCategory)
    {
        currentCategory = newCategory;
        UpdateGeneralDescription();
    }

    void UpdateGeneralDescription()
    {
        CategoryDescription found = categoryDescriptions.Find(cd => cd.category == currentCategory);

        if (found != null && generalDescriptionText != null)
        {
            generalDescriptionText.text = found.description;
        }
        else if (generalDescriptionText != null)
        {
            generalDescriptionText.text = defaultGeneralText;
        }
    }

    public void UpdateDetails(OptionsListPopulator.ComponentOption selectedOption)
    {
        if (detailedDescriptionText != null)
        {
            string details = selectedOption.detailedDescription;

            if (!string.IsNullOrEmpty(selectedOption.frequency) ||
                !string.IsNullOrEmpty(selectedOption.wattage) ||
                !string.IsNullOrEmpty(selectedOption.additionalSpecs))
            {
                details += "\n\n--- Specifications ---\n";
                if (!string.IsNullOrEmpty(selectedOption.frequency))
                    details += $"\nFrequency: {selectedOption.frequency}";
                if (!string.IsNullOrEmpty(selectedOption.wattage))
                    details += $"\nWattage: {selectedOption.wattage}";
                if (!string.IsNullOrEmpty(selectedOption.additionalSpecs))
                    details += $"\n{selectedOption.additionalSpecs}";
            }

            detailedDescriptionText.text = details;
        }

        if (specsText != null)
        {
            string specs = "";
            if (!string.IsNullOrEmpty(selectedOption.frequency))
                specs += $"Frequency: {selectedOption.frequency}\n";
            if (!string.IsNullOrEmpty(selectedOption.wattage))
                specs += $"Wattage: {selectedOption.wattage}\n";
            if (!string.IsNullOrEmpty(selectedOption.additionalSpecs))
                specs += selectedOption.additionalSpecs;
            specsText.text = specs;
        }

        Debug.Log($"Updated details for: {selectedOption.optionName}");
    }

    public void ClearDetails()
    {
        if (detailedDescriptionText != null)
            detailedDescriptionText.text = defaultDetailedText;
        if (specsText != null)
            specsText.text = defaultSpecsText;
    }
}