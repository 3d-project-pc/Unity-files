using UnityEngine;
using System.IO;

// ============================================================
// BenchmarkDataExporter.cs
// Attach this to the same GameObject as BenchmarkCalculator.
// It writes benchmark_data.json to StreamingAssets so
// RadialChartDisplay can read it.
// ============================================================

public class BenchmarkDataExporter : MonoBehaviour
{
    [System.Serializable]
    private class ExportData
    {
        public float fps;
        public float bootTime;
        public float totalPower;
        public float bottleneckPercent;
        public float temperature;
        public float noiseLevel;
        public int   totalScore;
        public string tier;
    }

    // Call this from BenchmarkUI.ShowBenchmark() after you get the result
    public void ExportToJSON(BenchmarkCalculator.BenchmarkResult result, float bottleneckPercent = 0f)
    {
        ExportData data = new ExportData
        {
            fps               = result.fps,
            bootTime          = result.bootTime,
            totalPower        = result.totalPower,
            bottleneckPercent = bottleneckPercent,
            temperature       = result.peakTemperature,
            noiseLevel        = result.noiseLevel,
            totalScore        = Mathf.RoundToInt(result.totalScore),
            tier              = result.tier
        };

        string json     = JsonUtility.ToJson(data, prettyPrint: true);
        string filePath = Path.Combine(Application.streamingAssetsPath, "benchmark_data.json");

        try
        {
            // Make sure the StreamingAssets folder exists
            if (!Directory.Exists(Application.streamingAssetsPath))
                Directory.CreateDirectory(Application.streamingAssetsPath);

            File.WriteAllText(filePath, json);
            Debug.Log($"[BenchmarkDataExporter] Saved to: {filePath}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[BenchmarkDataExporter] Failed to save JSON: {e.Message}");
        }
    }
}
