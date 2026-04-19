using UnityEngine;

public class BenchmarkTestTrigger : MonoBehaviour
{
    void Update()
    {
        // Press B key to show benchmark
        if (Input.GetKeyDown(KeyCode.B))
        {
            BenchmarkUI benchmarkUI = FindFirstObjectByType<BenchmarkUI>();
            if (benchmarkUI != null)
            {
                benchmarkUI.ShowBenchmark();
                Debug.Log("Benchmark triggered!");
            }
            else
            {
                Debug.LogError("BenchmarkUI not found in scene!");
            }
        }
    }
}