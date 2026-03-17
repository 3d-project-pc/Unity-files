using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio; // Required for volume control

public class SettingsManager : MonoBehaviour
{
    public GameObject settingsPanel;

    // Call this from your "Settings" button on the main menu
    public void OpenSettings()
    {
        settingsPanel.SetActive(true);
    }

    // Call this from your "Back" button
    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        Debug.Log("Fullscreen: " + isFullscreen);
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void SetVolume(float volume)
    {
        // This is a simple version. For real audio, use an AudioMixer.
        AudioListener.volume = volume;
    }
}