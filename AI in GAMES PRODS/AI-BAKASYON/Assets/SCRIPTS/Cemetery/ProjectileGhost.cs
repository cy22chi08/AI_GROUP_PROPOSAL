using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioSource))]
public class ProjectileGhost_WithProximitySound : MonoBehaviour
{
    [Header("AI Settings")]
    public Transform player;
    public float recalcInterval = 0.2f;
    public float stopDistance = 4f; // distance to start shooting
    public float moveSpeedMultiplier = 0.8f;

    [Header("Attack Settings")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float fireRate = 1f;       // shots per second
    public float fireDelay = 0.35f;   // delay to match 4th–5th frame timing
    public float firePointYOffset = 0.5f; // height offset for bullet spawn

    [Header("Audio Settings")]
    public AudioClip ghostSound;       // assign looping ghost sound
    public float audibleRange = 8f;    // how far player can hear
    public float minDistance = 2f;     // distance for full volume
    public float fadeSpeed = 2f;
    public float baseVolume = 0.8f;

    private NavMeshAgent agent;
    private Animator animator;
    private AudioSource audioSource;
    private float fireCooldown = 0f;
    private bool isChargingShot = false;
    private bool inAudibleRange = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        // NavMesh setup
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.speed *= moveSpeedMultiplier;

        // Audio setup
        audioSource.clip = ghostSound;
        audioSource.playOnAwake = false;
        audioSource.loop = true;
        audioSource.volume = AudioManager.Instance.sfxVolume;

        if (ghostSound != null)
            audioSource.Play();
    }

    void Update()
    {
        if (player == null) return;

        HandleProximitySound(); // 🔊 manage audio fading
        HandleBehavior();       // 🤖 handle chase + attack
    }

    void HandleBehavior()
    {
        float distance = Vector2.Distance(transform.position, player.position);

        if (distance > stopDistance)
        {
            // Chase player
            if (animator != null)
                animator.Play("projectileghostidle");

            isChargingShot = false;
            agent.isStopped = false;
            agent.SetDestination(player.position);
        }
        else
        {
            // Stop and shoot
            agent.isStopped = true;
            TryShoot();
        }

        // Clamp Z axis
        Vector3 pos = transform.position;
        pos.z = 0;
        transform.position = pos;
    }

    void HandleProximitySound()
    {
        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= audibleRange)
        {
            // Inside range
            if (!inAudibleRange) inAudibleRange = true;

            float t = Mathf.InverseLerp(audibleRange, minDistance, distance);
            float targetVolume = Mathf.Lerp(0f, baseVolume, 1 - t);
            audioSource.volume = Mathf.MoveTowards(audioSource.volume, targetVolume, Time.deltaTime * fadeSpeed);
        }
        else
        {
            // Outside range
            if (inAudibleRange)
            {
                inAudibleRange = false;
                StartCoroutine(FadeOutAndStop());
            }
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

    void TryShoot()
    {
        fireCooldown -= Time.deltaTime;

        if (fireCooldown <= 0f && !isChargingShot)
        {
            isChargingShot = true;
            fireCooldown = 1f / fireRate;

            // play animation
            if (animator != null)
            {
                animator.Play("projectileghostshoot");
                animator.speed = fireRate;
            }

            // delay for animation sync
            Invoke(nameof(ShootAtPlayer), fireDelay);
        }
    }

    void ShootAtPlayer()
    {
        if (player == null) return;

        Vector3 spawnPos = firePoint.position + new Vector3(0f, firePointYOffset, 0f);
        GameObject bullet = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);

        Vector2 dir = (player.position - spawnPos).normalized;
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.linearVelocity = dir * 6f;

        isChargingShot = false;
    }
}
