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

        // Initialize UI to match current game state
        if (fullscreenToggle != null)
            fullscreenToggle.isOn = Screen.fullScreen;

        if (qualityDropdown != null)
            qualityDropdown.value = QualitySettings.GetQualityLevel();

        // Default volume usually 1.0 (100%)
        if (volumeSlider != null)
            volumeSlider.value = AudioListener.volume;
    }

    // --- Panel Logic ---
    public void OpenSettings() => settingsPanel.SetActive(true);
    public void CloseSettings() => settingsPanel.SetActive(false);

    // 1. QUALITY SETTINGS
    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
        Debug.Log("Quality set to: " + QualitySettings.names[qualityIndex]);
    }

    // 2. VOLUME SETTINGS
    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
    }

    // 3. FULLSCREEN SETTINGS
    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }
}