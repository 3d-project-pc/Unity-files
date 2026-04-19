using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class BenchmarkUI : MonoBehaviour
{
    [Header("UI References")]
    public GameObject benchmarkPanel;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI tierText;
    public TextMeshProUGUI tierDescriptionText;
    public TextMeshProUGUI fpsText;
    public TextMeshProUGUI renderTimeText;
    public TextMeshProUGUI powerText;
    public TextMeshProUGUI temperatureText;
    public TextMeshProUGUI bootTimeText;
    public TextMeshProUGUI noiseText;
    public TextMeshProUGUI totalScoreText;
    public TextMeshProUGUI warningText;
    
    [Header("Section Headers")]
    public TextMeshProUGUI performanceHeader;
    public TextMeshProUGUI systemHeader;
    public TextMeshProUGUI warningsHeader;
    
    [Header("References")]
    public WorldSpawner worldSpawner;
    public BenchmarkCalculator calculator;
    
    [Header("Animation")]
    public Animator panelAnimator;
    public string showTrigger = "Show";
    
    void Start()
    {
        if (calculator == null)
            calculator = gameObject.AddComponent<BenchmarkCalculator>();
        
        if (benchmarkPanel != null)
            benchmarkPanel.SetActive(false);
    }
    
    public void ShowBenchmark()
    {
        if (worldSpawner == null)
        {
            worldSpawner = FindFirstObjectByType<WorldSpawner>();
            if (worldSpawner == null)
            {
                Debug.LogError("WorldSpawner not found!");
                return;
            }
        }
        
        List<ComponentTag> allComponents = worldSpawner.GetAllSpawnedComponents();
        
        if (allComponents.Count == 0)
        {
            Debug.LogWarning("No components found to benchmark!");
            return;
        }
        
        var result = calculator.CalculateBenchmark(allComponents);
        UpdateUI(result);
        
        benchmarkPanel.SetActive(true);
        if (panelAnimator != null)
            panelAnimator.SetTrigger(showTrigger);
    }
    
    public void HideBenchmark()
    {
        benchmarkPanel.SetActive(false);
    }
    
    private void UpdateUI(BenchmarkCalculator.BenchmarkResult result)
    {
        // Title
        if (titleText != null)
            titleText.text = "SYSTEM MONITOR";
        
        // Performance Section
        if (performanceHeader != null)
            performanceHeader.text = "══ PERFORMANCE ══";
        
        if (tierText != null)
            tierText.text = $"TIER {result.tier}";
        
        if (tierDescriptionText != null)
            tierDescriptionText.text = result.tierDescription;
        
        if (totalScoreText != null)
            totalScoreText.text = $"Score: {Mathf.RoundToInt(result.totalScore)} pts";
        
        // System Stats Section
        if (systemHeader != null)
            systemHeader.text = "══ SYSTEM STATS ══";
        
        if (fpsText != null)
            fpsText.text = $"FPS: {Mathf.RoundToInt(result.fps)}";
        
        if (renderTimeText != null)
            renderTimeText.text = $"Render Time: {result.renderTimeMs} ms";
        
        if (powerText != null)
            powerText.text = $"Power: {Mathf.RoundToInt(result.totalPower)} W";
        
        if (temperatureText != null)
            temperatureText.text = $"Temperature: {Mathf.RoundToInt(result.peakTemperature)}°C";
        
        if (bootTimeText != null)
            bootTimeText.text = $"Boot Time: {result.bootTime:F1} s";
        
        if (noiseText != null)
            noiseText.text = $"Noise: {Mathf.RoundToInt(result.noiseLevel)} dB";
        
        // Warnings Section
        if (warningsHeader != null)
            warningsHeader.text = "══ WARNINGS ══";
        
        if (warningText != null)
        {
            if (result.hasWarning)
            {
                warningText.gameObject.SetActive(true);
                warningText.text = FormatWarnings(result.warningMessage);
                warningText.color = new Color(1f, 0.7f, 0.2f); // Orange color
                warningText.fontSize = 20;
                warningText.enableWordWrapping = true;
            }
            else
            {
                warningText.gameObject.SetActive(true);
                warningText.text = "✓ All systems nominal";
                warningText.color = Color.green;
                warningText.fontSize = 20;
            }
        }
        // Color code based on values
if (fpsText != null)
{
    fpsText.text = $"FPS: {Mathf.RoundToInt(result.fps)}";
    if (result.fps >= 120)
        fpsText.color = Color.green;
    else if (result.fps >= 60)
        fpsText.color = Color.yellow;
    else
        fpsText.color = Color.red;
}

if (temperatureText != null)
{
    temperatureText.text = $"Temperature: {Mathf.RoundToInt(result.peakTemperature)}°C";
    if (result.peakTemperature <= 60)
        temperatureText.color = Color.green;
    else if (result.peakTemperature <= 75)
        temperatureText.color = Color.yellow;
    else
        temperatureText.color = Color.red;
}
    }
    
    private string FormatWarnings(string warnings)
    {
        // Replace bullet points and clean up formatting
        string formatted = warnings.Replace("⚠", "•");
        return formatted;
    }
}