using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class ComponentManager : MonoBehaviour
{
    // Define available component categories
    public enum ComponentCategory
    {
        CPU,
        RAM,
        GPU,
        Motherboard,
        Storage,
        PSU,
        Cooler,
        Fan,
        Peripherals
    }

    [System.Serializable]
    public class CategoryButton
    {
        public ComponentCategory category;
        public Button button;
    }

    [Header("Category Buttons")]
    public List<CategoryButton> categoryButtons;

    // Current selected category
    private ComponentCategory currentCategory;

    // Events for other scripts to subscribe to
    public event Action<ComponentCategory> OnCategorySelected;

    void Start()
    {
        // Set up button listeners
        foreach (CategoryButton catButton in categoryButtons)
        {
            if (catButton.button != null)
            {
                ComponentCategory category = catButton.category; // Capture for lambda
                catButton.button.onClick.AddListener(() => SelectCategory(category));
            }
        }

        // Optionally select first category by default
        if (categoryButtons.Count > 0)
        {
            SelectCategory(categoryButtons[0].category);
        }
    }

    public void SelectCategory(ComponentCategory category)
    {
        currentCategory = category;
        Debug.Log("Category selected: " + category);

        // Notify all listening scripts
        OnCategorySelected?.Invoke(currentCategory);
    }

    public ComponentCategory GetCurrentCategory()
    {
        return currentCategory;
    }
}