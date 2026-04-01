using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject mainMenuPanel; // The main vertical button group

    [Header("Settings Components")]
    [SerializeField] private TMP_Dropdown qualityDropdown;
    [SerializeField] private Toggle fullscreenToggle;

    void Start()
    {
        // 1. Load Graphics Quality
        int savedQuality = PlayerPrefs.GetInt("QualityLevel", 1);
        qualityDropdown.value = savedQuality;
        SetQuality(savedQuality);

        // 2. Load Fullscreen
        bool savedFullscreen = PlayerPrefs.GetInt("Fullscreen", 1) == 1;
        fullscreenToggle.isOn = savedFullscreen;
        SetFullscreen(savedFullscreen);

        CloseSettings();
    }

    // --- SETTINGS LOGIC ---
    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
        PlayerPrefs.SetInt("QualityLevel", qualityIndex);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt("Fullscreen", isFullscreen ? 1 : 0);
    }

    // --- NAVIGATION LOGIC (The "Back" Button) ---
    public void OpenSettings()
    {
        settingsPanel.SetActive(true);
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
    }
}