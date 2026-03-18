using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsManager : MonoBehaviour
{
    [Header("Panel Control")]
    public GameObject settingsPanel;

    [Header("UI Elements")]
    public Slider volumeSlider;
    public TMP_Dropdown qualityDropdown;
    public Toggle fullscreenToggle;

    void Start()
    {
        // 1. Ensure the panel starts hidden
        if (settingsPanel != null) settingsPanel.SetActive(false);

        // 2. Load current values into the UI
        volumeSlider.value = AudioListener.volume;
        qualityDropdown.value = QualitySettings.GetQualityLevel();
        fullscreenToggle.isOn = Screen.fullScreen;
    }

    // --- Panel Logic ---
    public void OpenSettings() => settingsPanel.SetActive(true);
    public void CloseSettings() => settingsPanel.SetActive(false);

    // --- Settings Logic ---
    public void ChangeVolume(float value)
    {
        AudioListener.volume = value;
    }

    public void ChangeQuality(int index)
    {
        QualitySettings.SetQualityLevel(index);
    }

    public void ChangeFullscreen(bool isFull)
    {
        Screen.fullScreen = isFull;
    }
}