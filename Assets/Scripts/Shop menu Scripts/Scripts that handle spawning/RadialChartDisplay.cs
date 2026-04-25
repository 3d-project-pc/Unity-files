using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System.Collections;

// ============================================================
// RadialChartDisplay.cs
// Place this script on a GameObject called "RadialChartManager"
// in your scene. Then assign all chart references in the Inspector.
// ============================================================

public class RadialChartDisplay : MonoBehaviour
{
    // ============================================================
    // DATA STRUCTURE — matches your BenchmarkCalculator output
    // ============================================================
    [System.Serializable]
    public class BenchmarkData
    {
        public float fps;
        public float bootTime;       // in seconds
        public float totalPower;     // in watts
        public float bottleneckPercent; // 0-100
    }

    // ============================================================
    // ONE CHART — holds all references for a single radial chart
    // ============================================================
    [System.Serializable]
    public class RadialChart
    {
        [Header("UI Components")]
        public Image fillImage;          // The circular fill image (Filled type, Radial360)
        public TextMeshProUGUI valueText; // Center label e.g. "144 FPS"
        public TextMeshProUGUI nameText;  // Label below e.g. "FPS"

        [Header("Thresholds — set in Inspector")]
        public float maxValue = 100f;
        public bool invertFill = false;  // true for Boot Time (lower = better)

        // Color thresholds
        public float goodThreshold = 0.33f;   // below this = good (green)
        public float warnThreshold = 0.66f;   // below this = warn (orange)
                                               // above warnThreshold = red

        [Header("Colors")]
        public Color colorGood    = new Color(0.2f, 0.85f, 0.4f);
        public Color colorWarning = new Color(1f,   0.6f,  0.1f);
        public Color colorCritical= new Color(0.95f,0.2f,  0.2f);

        [HideInInspector] public float currentValue;

        // ---- Called by RadialChartDisplay to update this chart ----
        public void UpdateChart(float newValue)
        {
            currentValue = newValue;

            // --- 1. Calculate fill amount (0..1) ---
            float ratio = Mathf.Clamp01(newValue / maxValue);
            float fillAmount = invertFill ? (1f - ratio) : ratio;

            if (fillImage != null)
                fillImage.fillAmount = fillAmount;

            // --- 2. Choose color based on ratio (always uses raw ratio) ---
            Color chosenColor;
            if (invertFill)
            {
                // For inverted (boot time): LOW ratio = good, HIGH ratio = bad
                if (ratio <= goodThreshold)
                    chosenColor = colorGood;
                else if (ratio <= warnThreshold)
                    chosenColor = colorWarning;
                else
                    chosenColor = colorCritical;
            }
            else
            {
                // Normal: HIGH ratio = good
                if (ratio >= (1f - goodThreshold))
                    chosenColor = colorGood;
                else if (ratio >= (1f - warnThreshold))
                    chosenColor = colorWarning;
                else
                    chosenColor = colorCritical;
            }

            if (fillImage != null)
                fillImage.color = chosenColor;
        }

        // ---- Call this AFTER UpdateChart to set the label text ----
        public void SetLabel(string formatted)
        {
            if (valueText != null)
                valueText.text = formatted;
        }
    }

    // ============================================================
    // INSPECTOR FIELDS — drag your UI objects here
    // ============================================================
    [Header("=== FOUR CHARTS — assign in Inspector ===")]
    public RadialChart fpsChart;
    public RadialChart bootTimeChart;
    public RadialChart powerChart;
    public RadialChart bottleneckChart;

    [Header("=== JSON File Path ===")]
    [Tooltip("Path relative to Application.streamingAssetsPath")]
    public string jsonFileName = "benchmark_data.json";

    [Header("=== Auto-Refresh ===")]
    [Tooltip("Seconds between auto-refresh checks (0 = disabled)")]
    public float refreshInterval = 2f;

    // ============================================================
    // INTERNAL STATE
    // ============================================================
    private string jsonFilePath;
    private float lastModifiedTime = -1f;
    private Coroutine autoRefreshCoroutine;

    // ============================================================
    // UNITY LIFECYCLE
    // ============================================================
    void Start()
    {
        // Build the full path to the JSON file
        jsonFilePath = Path.Combine(Application.streamingAssetsPath, jsonFileName);

        // Set default labels
        SetDefaultLabels();

        // Load data immediately if file exists
        if (File.Exists(jsonFilePath))
        {
            LoadAndDisplay();
        }
        else
        {
            Debug.LogWarning($"[RadialChartDisplay] JSON not found at: {jsonFilePath}\n" +
                             $"Create the file at Assets/StreamingAssets/{jsonFileName}");
        }

        // Start auto-refresh if enabled
        if (refreshInterval > 0f)
        {
            autoRefreshCoroutine = StartCoroutine(AutoRefreshCoroutine());
        }
    }

    // ============================================================
    // PUBLIC: Call this from a UI Button's OnClick event
    // ============================================================
    public void RefreshCharts()
    {
        LoadAndDisplay();
        Debug.Log("[RadialChartDisplay] Manual refresh triggered.");
    }

    // ============================================================
    // LOAD JSON AND UPDATE ALL CHARTS
    // ============================================================
    void LoadAndDisplay()
    {
        if (!File.Exists(jsonFilePath))
        {
            Debug.LogError($"[RadialChartDisplay] File not found: {jsonFilePath}");
            return;
        }

        try
        {
            string json = File.ReadAllText(jsonFilePath);
            BenchmarkData data = JsonUtility.FromJson<BenchmarkData>(json);

            if (data == null)
            {
                Debug.LogError("[RadialChartDisplay] Failed to parse JSON. Check file format.");
                return;
            }

            // --- Update each chart ---
            UpdateFPS(data.fps);
            UpdateBootTime(data.bootTime);
            UpdatePower(data.totalPower);
            UpdateBottleneck(data.bottleneckPercent);

            Debug.Log($"[RadialChartDisplay] Charts updated — FPS:{data.fps} Boot:{data.bootTime}s " +
                      $"Power:{data.totalPower}W Bottleneck:{data.bottleneckPercent}%");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[RadialChartDisplay] Error reading JSON: {e.Message}");
        }
    }

    // ============================================================
    // INDIVIDUAL CHART UPDATERS
    // ============================================================
    void UpdateFPS(float fps)
    {
        fpsChart.UpdateChart(fps);
        fpsChart.SetLabel($"{Mathf.RoundToInt(fps)}\nFPS");
    }

    void UpdateBootTime(float seconds)
    {
        bootTimeChart.UpdateChart(seconds);
        bootTimeChart.SetLabel($"{seconds:F1}s\nBoot");
    }

    void UpdatePower(float watts)
    {
        powerChart.UpdateChart(watts);
        powerChart.SetLabel($"{Mathf.RoundToInt(watts)}W\nPower");
    }

    void UpdateBottleneck(float percent)
    {
        bottleneckChart.UpdateChart(percent);
        bottleneckChart.SetLabel($"{Mathf.RoundToInt(percent)}%\nBottleneck");
    }

    // ============================================================
    // AUTO-REFRESH COROUTINE — watches for file changes
    // ============================================================
    IEnumerator AutoRefreshCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(refreshInterval);

            if (File.Exists(jsonFilePath))
            {
                float modTime = (float)File.GetLastWriteTime(jsonFilePath)
                                    .Subtract(new System.DateTime(1970, 1, 1))
                                    .TotalSeconds;

                if (modTime != lastModifiedTime)
                {
                    lastModifiedTime = modTime;
                    LoadAndDisplay();
                    Debug.Log("[RadialChartDisplay] File changed — auto-refreshed.");
                }
            }
        }
    }

    // ============================================================
    // HELPER: Set placeholder labels on startup
    // ============================================================
    void SetDefaultLabels()
    {
        if (fpsChart.nameText != null)       fpsChart.nameText.text       = "FPS";
        if (bootTimeChart.nameText != null)  bootTimeChart.nameText.text  = "BOOT TIME";
        if (powerChart.nameText != null)     powerChart.nameText.text     = "POWER";
        if (bottleneckChart.nameText != null)bottleneckChart.nameText.text= "BOTTLENECK";

        if (fpsChart.valueText != null)       fpsChart.valueText.text       = "—";
        if (bootTimeChart.valueText != null)  bootTimeChart.valueText.text  = "—";
        if (powerChart.valueText != null)     powerChart.valueText.text     = "—";
        if (bottleneckChart.valueText != null)bottleneckChart.valueText.text= "—";
    }

    // ============================================================
    // INTEGRATION: Call this directly from BenchmarkUI.cs
    // instead of (or alongside) writing to JSON
    // ============================================================
    public void UpdateFromBenchmarkResult(BenchmarkCalculator.BenchmarkResult result)
    {
        // Calculate bottleneck: if any major component is missing, 100%
        float bottleneck = 0f;
        // Simple heuristic: if fps is very low relative to what power suggests, there's a bottleneck
        // You can replace this with your own formula
        bottleneck = Mathf.Clamp((800f - result.totalPower) / 8f, 0f, 100f);
        bottleneck = 100f - bottleneck; // invert: high power usage = more "used" = lower bottleneck %

        UpdateFPS(result.fps);
        UpdateBootTime(result.bootTime);
        UpdatePower(result.totalPower);
        UpdateBottleneck(bottleneck);
    }

    void OnDestroy()
    {
        if (autoRefreshCoroutine != null)
            StopCoroutine(autoRefreshCoroutine);
    }
}
