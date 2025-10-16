using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class ProjectileDamage_WithProximitySound : MonoBehaviour
{
    [Header("Damage Settings")]
    public int damageAmount = 1;
    public float lifetime = 3f;

    [Header("Audio Settings")]
    public AudioClip projectileSound;       // Sound while projectile is active
    public float audibleRange = 10f;        // Max distance player can hear
    public float fullVolumeDistance = 2f;   // Distance for full volume
    public float fadeSpeed = 3f;

    private float baseVolume = 0.8f;
    private AudioSource audioSource;
    private Transform player;
    private bool inAudibleRange = false;
    private bool hasStartedAudio = false;
    private bool isFadingOut = false;

    void Start()
    {
        // Find player
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;

        // Setup AudioSource
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = projectileSound;
        audioSource.playOnAwake = false;
        audioSource.loop = true;
        audioSource.volume = 0f; // start silent

        // Auto-destroy projectile after lifetime
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        if (player == null) return;

        // Keep volume synced with global SFX setting
        if (AudioManager.Instance != null)
            baseVolume = AudioManager.Instance.sfxVolume;

        HandleProximitySound();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
            if (playerHealth != null)
                playerHealth.TakeDamage(damageAmount);

            DestroyProjectile();
        }
        else if (collision.CompareTag("Wall"))
        {
            DestroyProjectile();
        }
    }

    void HandleProximitySound()
    {
        if (projectileSound == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        // Inside audible range
        if (distance <= audibleRange)
        {
            inAudibleRange = true;
            isFadingOut = false;

            if (!hasStartedAudio)
            {
                if (!audioSource.isPlaying)
                    audioSource.Play();
                hasStartedAudio = true;
            }

            float t = Mathf.InverseLerp(audibleRange, fullVolumeDistance, distance);
            float targetVolume = Mathf.Lerp(0f, baseVolume, 1 - t);
            audioSource.volume = Mathf.MoveTowards(audioSource.volume, targetVolume, Time.deltaTime * fadeSpeed);
        }
        else
        {
            // Outside range
            if (inAudibleRange && !isFadingOut)
            {
                inAudibleRange = false;
                StartCoroutine(FadeOutAndStop());
            }
        }
    }

    IEnumerator FadeOutAndStop()
    {
        isFadingOut = true;

        while (audioSource.volume > 0.01f)
        {
            audioSource.volume -= Time.deltaTime * fadeSpeed;
            yield return null;
        }

        audioSource.volume = 0f;
        audioSource.Stop();
        hasStartedAudio = false;
        isFadingOut = false;
    }

    void DestroyProjectile()
    {
        StopAllCoroutines();

        if (audioSource.isPlaying)
            StartCoroutine(FadeOutAndDestroy());
        else
            Destroy(gameObject);
    }

    IEnumerator FadeOutAndDestroy()
    {
        while (audioSource.volume > 0.01f)
        {
            audioSource.volume -= Time.deltaTime * fadeSpeed;
            yield return null;
        }

        audioSource.Stop();
        Destroy(gameObject);
    }
}
