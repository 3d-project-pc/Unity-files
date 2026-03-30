using UnityEngine;
using System.Collections.Generic;

public class ShopFilter : MonoBehaviour
{
    // Drag your 'Content' object (the one holding the items) here
    public Transform container;

    public void FilterItems(string categoryName)
    {
        // Loop through every item in the shop container
        foreach (Transform child in container)
        {
            ShopItem item = child.GetComponent<ShopItem>();

            if (item != null)
            {
                // If "All" is selected, show everything
                if (categoryName == "All")
                {
                    child.gameObject.SetActive(true);
                }
                // Otherwise, only show if the category matches the button name
                else if (item.category.ToString() == categoryName)
                {
                    child.gameObject.SetActive(true);
                }
                else
                {
                    child.gameObject.SetActive(false);
                }
            }
        }
    }
}