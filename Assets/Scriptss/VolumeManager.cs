using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeManager : MonoBehaviour
{
    [SerializeField] private AudioMixer myMixer;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    void Start()
    {
        // The '0.8f' at the end is the DEFAULT if no save is found.
        // This ensures your first play isn't silent!
        float savedMusic = PlayerPrefs.GetFloat("MusicVolume", 0.8f);
        float savedSFX = PlayerPrefs.GetFloat("SFXVolume", 0.8f);

        musicSlider.value = savedMusic;
        sfxSlider.value = savedSFX;

        // Force the mixer to match these values immediately
        SetMusicVolume();
        SetSFXVolume();
    }
    public void SetMusicVolume()
    {
        float volume = musicSlider.value;
        // This math converts 0.0001-1 into -80dB to 0dB
        myMixer.SetFloat("MusicVol", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("MusicVolume", volume);
    }

    public void SetSFXVolume()
    {
        float volume = sfxSlider.value;
        myMixer.SetFloat("SFXVol", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("SFXVolume", volume);
    }
}