using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource backgroundMusic;
    public AudioSource sfxSource;

    [Header("Volume Settings")]
    [Range(0f, 1f)] public float musicVolume = 1f;
    [Range(0f, 1f)] public float sfxVolume = 1f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
            sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);

            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        ApplyVolumes();
        FindAndPlaySceneMusic();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Stop any old music and play the new scene’s music
        FindAndPlaySceneMusic();
    }

    private void FindAndPlaySceneMusic()
    {
        // Stop old music first
        if (backgroundMusic != null)
            backgroundMusic.Stop();

        // Find the new music in the loaded scene
        AudioSource newMusicSource = GameObject.FindWithTag("Music")?.GetComponent<AudioSource>();

        if (newMusicSource != null)
        {
            backgroundMusic = newMusicSource;
            backgroundMusic.volume = musicVolume;
            backgroundMusic.loop = true;
            backgroundMusic.Play();
        }
        else
        {
            backgroundMusic = null; // No music in this scene
        }
    }

    public void SetMusicVolume(float value)
    {
        musicVolume = value;
        if (backgroundMusic != null)
            backgroundMusic.volume = musicVolume;

        PlayerPrefs.SetFloat("MusicVolume", value);
    }

    public void SetSFXVolume(float value)
    {
        sfxVolume = value;
        if (sfxSource != null)
            sfxSource.volume = sfxVolume;

        PlayerPrefs.SetFloat("SFXVolume", value);
    }

    public void PlayClick()
    {
        if (sfxSource != null)
        {
            sfxSource.volume = sfxVolume;
            sfxSource.Play();
        }
    }

    public void ApplyVolumes()
    {
        if (backgroundMusic != null)
            backgroundMusic.volume = musicVolume;
        if (sfxSource != null)
            sfxSource.volume = sfxVolume;
    }

    public void PlaySFXAtPoint(AudioClip clip, Vector3 position)
    {
        if (clip == null) return;
        AudioSource.PlayClipAtPoint(clip, position, sfxVolume);
    }
}
