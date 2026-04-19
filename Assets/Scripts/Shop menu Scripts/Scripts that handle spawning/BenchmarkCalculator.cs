using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class BenchmarkCalculator : MonoBehaviour
{
    [System.Serializable]
    public class BenchmarkResult
    {
        public float totalScore;
        public string tier;
        public string tierDescription;
        public float fps;
        public float renderTimeMs;
        public float totalPower;
        public float peakTemperature;
        public float bootTime;
        public float noiseLevel;
        public bool hasWarning;
        public string warningMessage;
        public List<string> allWarnings = new List<string>();
    }
    
    private readonly int S_TIER_MIN = 4500;
    private readonly int A_TIER_MIN = 3500;
    private readonly int B_TIER_MIN = 2500;
    private readonly int C_TIER_MIN = 1500;
    
    public BenchmarkResult CalculateBenchmark(List<ComponentTag> installedComponents)
    {
        BenchmarkResult result = new BenchmarkResult();
        result.allWarnings = new List<string>();
        
        var cpu = installedComponents.FirstOrDefault(c => c.componentType == ComponentType.CPU);
        var gpu = installedComponents.FirstOrDefault(c => c.componentType == ComponentType.GPU);
        var ramSticks = installedComponents.Where(c => c.componentType == ComponentType.RAM).ToList();
        var storage = installedComponents.FirstOrDefault(c => c.componentType == ComponentType.Storage);
        var psu = installedComponents.FirstOrDefault(c => c.componentType == ComponentType.PSU);
        var coolers = installedComponents.Where(c => c.componentType == ComponentType.Cooler).ToList();
        var fans = installedComponents.Where(c => c.componentType == ComponentType.Fan).ToList();
        var motherboard = installedComponents.FirstOrDefault(c => c.componentType == ComponentType.Motherboard);
        
        // ========== WARNING 1: Missing Cooler ==========
        if (cpu != null && coolers.Count == 0)
        {
            result.allWarnings.Add("⚠ No CPU cooler installed! CPU may overheat.");
        }
        
        // ========== WARNING 2: CPU-Cooler Mismatch ==========
        if (cpu != null && coolers.Count > 0)
        {
            float cpuHeat = cpu.thermalOutput;
            float totalCooling = coolers.Sum(c => c.coolingCapacity);
            
            if (cpuHeat > totalCooling + 20)
            {
                result.allWarnings.Add($"⚠ CPU cooler insufficient! CPU needs {cpuHeat}W cooling, have {totalCooling}W.");
            }
        }
        
        // ========== WARNING 3: Socket Mismatch ==========
        if (cpu != null && motherboard != null)
        {
            if (!string.IsNullOrEmpty(cpu.socket) && !string.IsNullOrEmpty(motherboard.socket))
            {
                if (cpu.socket != motherboard.socket)
                {
                    result.allWarnings.Add($"⚠ Socket mismatch! CPU ({cpu.socket}) not compatible with Motherboard ({motherboard.socket}).");
                }
            }
        }
        
        // ========== WARNING 4: RAM Compatibility ==========
        if (ramSticks.Count > 0 && motherboard != null)
        {
            string motherboardRamType = motherboard.ramType;
            if (!string.IsNullOrEmpty(motherboardRamType))
            {
                foreach (var ram in ramSticks)
                {
                    if (!string.IsNullOrEmpty(ram.ramType) && ram.ramType != motherboardRamType)
                    {
                        result.allWarnings.Add($"⚠ RAM type mismatch! {ram.ramType} not compatible with Motherboard ({motherboardRamType}).");
                        break;
                    }
                }
            }
        }
        
        // ========== WARNING 5: Missing RAM ==========
        if (ramSticks.Count == 0)
        {
            result.allWarnings.Add("⚠ No RAM installed! System cannot boot.");
        }
        
        // ========== WARNING 6: Missing Storage ==========
        if (storage == null)
        {
            result.allWarnings.Add("⚠ No storage device installed! Cannot install OS.");
        }
        
        // ========== WARNING 7: Missing PSU ==========
        if (psu == null)
        {
            result.allWarnings.Add("⚠ No power supply installed! System cannot power on.");
        }
        
        // ========== 1. Total Score ==========
        float totalScore = 0;
        
        if (cpu != null)
        {
            totalScore += cpu.singleCoreScore * 2;
            totalScore += cpu.multiCoreScore;
        }
        
        if (gpu != null)
        {
            totalScore += gpu.fpsContribution * 10;
        }
        
        foreach (var ram in ramSticks)
        {
            totalScore += ram.fpsContribution * 5;
        }
        
        if (storage != null)
        {
            totalScore += storage.bootTimeReduction * 50;
        }
        
        result.totalScore = totalScore;
        
        // ========== 2. Tier ==========
        if (totalScore >= S_TIER_MIN)
        {
            result.tier = "S";
            result.tierDescription = "Elite — Top 1%";
        }
        else if (totalScore >= A_TIER_MIN)
        {
            result.tier = "A";
            result.tierDescription = "Great — Top 15%";
        }
        else if (totalScore >= B_TIER_MIN)
        {
            result.tier = "B";
            result.tierDescription = "Good — Above Average";
        }
        else if (totalScore >= C_TIER_MIN)
        {
            result.tier = "C";
            result.tierDescription = "Fair — Entry Level";
        }
        else
        {
            result.tier = "D";
            result.tierDescription = "Poor — Consider Upgrading";
        }
        
        // ========== WARNING 8: Low Performance ==========
        if (result.tier == "D")
        {
            result.allWarnings.Add("⚠ Low performance tier! Consider upgrading components.");
        }
        
        // ========== 3. FPS ==========
        float fps = 30;
        if (gpu != null) fps += gpu.fpsContribution;
        foreach (var ram in ramSticks) fps += ram.fpsContribution * 0.3f;
        if (cpu != null) fps += cpu.singleCoreScore / 100;
        result.fps = Mathf.Clamp(fps, 15, 300);
        result.renderTimeMs = Mathf.Round(1000f / result.fps);
        
        // ========== WARNING 9: Low FPS ==========
        if (result.fps < 30)
        {
            result.allWarnings.Add("⚠ Low FPS (<30)! Gaming experience may be poor.");
        }
        
        // ========== 4. Total Power ==========
        result.totalPower = 0;
        foreach (var comp in installedComponents)
        {
            result.totalPower += comp.powerDraw;
        }
        
        // ========== WARNING 10: PSU Insufficient ==========
        if (psu != null && result.totalPower > psu.maxWattage)
        {
            result.allWarnings.Add($"⚠ PSU insufficient! System needs {result.totalPower}W, PSU provides {psu.maxWattage}W.");
        }
        
        // ========== WARNING 11: Power Spike Risk ==========
        if (psu != null && result.totalPower > psu.maxWattage * 0.9f)
        {
            result.allWarnings.Add($"⚠ PSU near limit! Using {result.totalPower}/{psu.maxWattage}W ({Mathf.RoundToInt((result.totalPower/psu.maxWattage)*100)}%).");
        }
        
        // ========== 5. Peak Temperature ==========
        float temp = 35;
        if (cpu != null) temp += cpu.baseTemp;
        if (gpu != null) temp += gpu.baseTemp;
        
        float cooling = 0;
        foreach (var cooler in coolers) cooling += cooler.coolingCapacity;
        foreach (var fan in fans) cooling += 10;
        
        result.peakTemperature = Mathf.Max(40, temp - cooling);
        
        // ========== WARNING 12: High Temperature ==========
        if (result.peakTemperature > 85)
        {
            result.allWarnings.Add($"⚠ Temperature too high ({Mathf.RoundToInt(result.peakTemperature)}°C)! Risk of thermal throttling.");
        }
        else if (result.peakTemperature > 75)
        {
            result.allWarnings.Add($"⚠ High temperature ({Mathf.RoundToInt(result.peakTemperature)}°C)! Consider better cooling.");
        }
        
        // ========== 6. Boot Time ==========
        float bootTime = 25;
        if (storage != null) bootTime -= storage.bootTimeReduction;
        if (cpu != null) bootTime -= cpu.singleCoreScore / 1000;
        result.bootTime = Mathf.Clamp(bootTime, 3, 30);
        
        // ========== WARNING 13: Slow Boot ==========
        if (result.bootTime > 20)
        {
            result.allWarnings.Add($"⚠ Slow boot time ({result.bootTime:F1}s)! Consider upgrading to SSD.");
        }
        
        // ========== 7. Noise Level ==========
        float noise = 30;
        foreach (var cooler in coolers) noise += cooler.noiseLevel;
        foreach (var fan in fans) noise += 20;
        result.noiseLevel = Mathf.Clamp(noise, 25, 85);
        
        // ========== WARNING 14: High Noise ==========
        if (result.noiseLevel > 70)
        {
            result.allWarnings.Add($"⚠ Very noisy system ({Mathf.RoundToInt(result.noiseLevel)}dB)! Consider quieter fans.");
        }
        else if (result.noiseLevel > 55)
        {
            result.allWarnings.Add($"⚠ Noticeable noise ({Mathf.RoundToInt(result.noiseLevel)}dB).");
        }
        
        // ========== Build Final Warning Message ==========
        result.hasWarning = result.allWarnings.Count > 0;
        if (result.hasWarning)
        {
            StringBuilder warningBuilder = new StringBuilder();
            foreach (string warning in result.allWarnings)
            {
                warningBuilder.AppendLine(warning);
            }
            result.warningMessage = warningBuilder.ToString().TrimEnd();
        }
        
        return result;
    }
}