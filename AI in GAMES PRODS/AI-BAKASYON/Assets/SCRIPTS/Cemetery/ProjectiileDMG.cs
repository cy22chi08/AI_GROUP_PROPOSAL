using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class ProjectileDamage_WithProximitySound : MonoBehaviour
{
    [Header("Damage Settings")]
    public int damageAmount = 1;
    public float lifetime = 3f;

    [Header("Audio Settings")]
    public AudioClip projectileSound;       // sound when projectile is flying or active
    public float audibleRange = 10f;        // how far player can hear it
    public float fadeSpeed = 3f;
    public float baseVolume = 0.8f;
    public float minDistance = 2f;

    private AudioSource audioSource;
    private Transform player;
    private bool inAudibleRange = false;

    void Start()
    {
        // Find player (tag must be set to "Player")
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;

        // Audio setup
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = projectileSound;
        audioSource.playOnAwake = false;
        audioSource.loop = true;
        audioSource.volume = 0f;

        // Start lifetime timer
        Destroy(gameObject, lifetime);

        // Start checking range
        StartCoroutine(ProximityCheck());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damageAmount);
            }
            DestroyProjectile();
        }
        else if (collision.CompareTag("Wall"))
        {
            DestroyProjectile();
        }
    }

    void DestroyProjectile()
    {
        StopAllCoroutines();
        if (audioSource.isPlaying)
            audioSource.Stop();
        Destroy(gameObject);
    }

    IEnumerator ProximityCheck()
    {
        // Wait one frame to make sure player exists
        yield return null;

        if (audioSource.clip != null)
            audioSource.Play();

        while (true)
        {
            if (player == null) yield break;

            float distance = Vector2.Distance(transform.position, player.position);

            if (distance <= audibleRange)
            {
                // Inside hearing range
                if (!inAudibleRange)
                    inAudibleRange = true;

                float t = Mathf.InverseLerp(audibleRange, minDistance, distance);
                float targetVolume = Mathf.Lerp(0f, baseVolume, 1 - t);
                audioSource.volume = Mathf.MoveTowards(audioSource.volume, targetVolume, Time.deltaTime * fadeSpeed);
            }
            else
            {
                // Out of hearing range, fade out
                if (inAudibleRange)
                {
                    inAudibleRange = false;
                    StartCoroutine(FadeOutAndStop());
                }
            }

            yield return null;
        }
    }

    IEnumerator FadeOutAndStop()
    {
        while (audioSource.volume > 0.01f)
        {
            audioSource.volume -= Time.deltaTime * fadeSpeed;
            yield return null;
        }
        audioSource.volume = 0f;
        audioSource.Stop();
    }
}
