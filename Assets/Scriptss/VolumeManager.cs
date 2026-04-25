using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeManager : MonoBehaviour
{
    [SerializeField] private AudioMixer myMixer;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    // Minimum volume value to avoid log10 issues
    private const float MinVolume = 0.0001f;

    void Start()
    {
        // Load saved values (0.8f = 80% volume by default)
        float savedMusic = PlayerPrefs.GetFloat("MusicVolume", 0.8f);
        float savedSFX = PlayerPrefs.GetFloat("SFXVolume", 0.8f);

        musicSlider.value = savedMusic;
        sfxSlider.value = savedSFX;

        // Apply the volumes
        SetMusicVolume();
        SetSFXVolume();
    }

    public void SetMusicVolume()
    {
        float volume = Mathf.Max(musicSlider.value, MinVolume);

        if (musicSlider.value <= 0.001f) // Effectively muted
        {
            // Use a very low dB value for mute (but not -infinity)
            myMixer.SetFloat("MusicVol", -80f);
        }
        else
        {
            // Convert linear 0-1 to decibels (-80dB to 0dB)
            myMixer.SetFloat("MusicVol", Mathf.Log10(volume) * 20);
        }

        PlayerPrefs.SetFloat("MusicVolume", musicSlider.value);
    }

    public void SetSFXVolume()
    {
        float volume = Mathf.Max(sfxSlider.value, MinVolume);

        if (sfxSlider.value <= 0.001f) // Effectively muted
        {
            // Use a very low dB value for mute
            myMixer.SetFloat("SFXVol", -80f);
        }
        else
        {
            // Convert linear 0-1 to decibels (-80dB to 0dB)
            myMixer.SetFloat("SFXVol", Mathf.Log10(volume) * 20);
        }

        PlayerPrefs.SetFloat("SFXVolume", sfxSlider.value);
    }

    // Optional: Add a mute toggle button functionality
    public void ToggleMusicMute()
    {
        if (musicSlider.value > 0.001f)
        {
            // Store current volume and mute
            PlayerPrefs.SetFloat("MusicVolume_BeforeMute", musicSlider.value);
            musicSlider.value = 0;
        }
        else
        {
            // Unmute to previous volume or default
            float previousVolume = PlayerPrefs.GetFloat("MusicVolume_BeforeMute", 0.8f);
            musicSlider.value = previousVolume;
        }

        SetMusicVolume();
    }

    public void ToggleSFXMute()
    {
        if (sfxSlider.value > 0.001f)
        {
            PlayerPrefs.SetFloat("SFXVolume_BeforeMute", sfxSlider.value);
            sfxSlider.value = 0;
        }
        else
        {
            float previousVolume = PlayerPrefs.GetFloat("SFXVolume_BeforeMute", 0.8f);
            sfxSlider.value = previousVolume;
        }

        SetSFXVolume();
    }
}