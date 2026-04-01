using UnityEngine;

public class PersistentAudio : MonoBehaviour
{
    private static PersistentAudio instance;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    // A simple function we can call from any button
    public void PlayButtonSound()
    {
        AudioSource source = GetComponent<AudioSource>();
        if (source != null)
        {
            source.PlayOneShot(source.clip);
        }
    }
}