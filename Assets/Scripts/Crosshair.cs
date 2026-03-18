using UnityEngine;
using UnityEngine.UI;

public class Crosshair : MonoBehaviour
{
    [Header("Sizing (Pixels)")]
    public float idleSize = 4f;      // The normal dot size
    public float maxSize = 8f;     // The size when clicking
    public float speed = 15f;       // Animation speed

    private RectTransform rect;
    private float currentSize;

    void Start()
    {
        rect = GetComponent<RectTransform>();
        currentSize = idleSize;

        // Safety: Force the scale to 1 so it doesn't get blurry/huge
        rect.localScale = Vector3.one;
    }

    void Update()
    {
        // 1. Determine target based on mouse click
        float target = Input.GetMouseButton(0) ? maxSize : idleSize;

        // 2. Smoothly move toward that target
        currentSize = Mathf.Lerp(currentSize, target, Time.deltaTime * speed);

        // 3. HARD LIMIT: Never let it go above 20 pixels regardless of settings
        currentSize = Mathf.Clamp(currentSize, 0.1f, 8f);

        // 4. Apply to the UI
        rect.sizeDelta = new Vector2(currentSize, currentSize);
    }
}