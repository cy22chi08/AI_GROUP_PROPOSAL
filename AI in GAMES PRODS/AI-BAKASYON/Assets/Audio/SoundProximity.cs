using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class NPCProximitySound : MonoBehaviour
{
    [Header("References")]
    public Transform player;          // Assign your player object here

    [Header("Sound Settings")]
    public float maxDistance = 6f;    // Max distance where the sound is audible
    public float minDistance = 1.5f;  // Distance for full volume
    public float fadeSpeed = 2f;      // How fast the sound fades in/out

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = true; // Loop sound continuously
        audioSource.volume = AudioManager.Instance.sfxVolume;
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= maxDistance)
        {
            // Start sound if not playing
            if (!audioSource.isPlaying)
                audioSource.Play();

            // Calculate volume based on proximity (closer = louder)
            float t = Mathf.InverseLerp(maxDistance, minDistance, distance);
            float targetVolume = Mathf.Lerp(0f, 1f, 1 - t);

            // Smoothly adjust volume over time
            audioSource.volume = Mathf.MoveTowards(audioSource.volume, targetVolume, Time.deltaTime * fadeSpeed);
        }
        else
        {
            // Fade out and stop when player leaves range
            if (audioSource.isPlaying)
            {
                audioSource.volume = Mathf.MoveTowards(audioSource.volume, 0f, Time.deltaTime * fadeSpeed);
                if (audioSource.volume <= 0.01f)
                    audioSource.Stop();
            }
        }
    }
}
