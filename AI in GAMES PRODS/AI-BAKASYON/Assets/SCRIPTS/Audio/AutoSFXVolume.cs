using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AutoSFXVolume : MonoBehaviour
{
    private AudioSource audioSource;
    private float lastVolume;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        UpdateVolume();
    }

    void Update()
    {
        // Keep updating while playing (if the slider is moved mid-game)
        if (AudioManager.Instance != null && AudioManager.Instance.sfxVolume != lastVolume)
            UpdateVolume();
    }

    void UpdateVolume()
    {
        if (AudioManager.Instance != null && audioSource != null)
        {
            audioSource.volume = AudioManager.Instance.sfxVolume;
            lastVolume = AudioManager.Instance.sfxVolume;
        }
    }
}
