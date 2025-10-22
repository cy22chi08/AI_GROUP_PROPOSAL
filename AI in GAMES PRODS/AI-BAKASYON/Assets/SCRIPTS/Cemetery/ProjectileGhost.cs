using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioSource))]
public class ProjectileGhost : MonoBehaviour
{
    [Header("AI Settings")]
    public Transform player;
    public float recalcInterval = 0.2f;
    public float stopDistance = 4f;
    public float moveSpeedMultiplier = 0.8f;

    [Header("Attack Settings")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float fireRate = 1f;
    public float fireDelay = 0.35f;
    public float firePointYOffset = 0.5f;

    [Header("Audio Settings")]
    public AudioClip ghostSound;
    public float audibleRange = 8f;
    public float fullVolumeDistance = 2f;
    public float fadeSpeed = 2f;

    private float baseVolume = 0.8f;

    // --- Added slow/freeze vars ---
    [Header("Hit Reaction Settings")]
    public float slowMultiplier = 0.4f;       // how much slower it moves
    public float slowDuration = 3f;           // how long it stays slowed
    public Color slowColor = new Color(0.5f, 0.5f, 1f, 1f);

    private bool isSlowed = false;
    private float originalSpeed;
    private Color originalColor;
    private bool canShoot = true;

    // Components
    private NavMeshAgent agent;
    private Animator animator;
    private AudioSource audioSource;
    private SpriteRenderer spriteRenderer;

    private float fireCooldown = 0f;
    private bool isChargingShot = false;
    private bool inAudibleRange = false;
    private bool hasStartedAudio = false;
    private float recalcTimer = 0f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        // NavMesh setup
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.speed *= moveSpeedMultiplier;

        // Save defaults
        originalSpeed = agent.speed;
        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;

        // Audio setup
        audioSource.clip = ghostSound;
        audioSource.playOnAwake = false;
        audioSource.loop = true;
        audioSource.volume = 0f;
    }

    void Update()
    {
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
            else return;
        }

        if (AudioManager.Instance != null)
            baseVolume = AudioManager.Instance.sfxVolume;

        HandleBehavior();
        HandleProximitySound();
    }

    // -------------------------------
    // AI BEHAVIOR
    // -------------------------------
    void HandleBehavior()
    {
        if (isSlowed) return; // ✅ stop updating if stunned/slowed heavily

        float distance = Vector2.Distance(transform.position, player.position);

        recalcTimer -= Time.deltaTime;
        if (recalcTimer <= 0f)
        {
            recalcTimer = recalcInterval;

            if (distance > stopDistance)
            {
                if (animator != null)
                    animator.Play("projectileghostidle");

                isChargingShot = false;
                agent.isStopped = false;
                agent.SetDestination(player.position);
            }
            else
            {
                agent.isStopped = true;
                if (canShoot)
                    TryShoot();
            }

            Vector3 pos = transform.position;
            pos.z = 0;
            transform.position = pos;
        }
    }

    // -------------------------------
    // SOUND HANDLING
    // -------------------------------
    void HandleProximitySound()
    {
        if (ghostSound == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= audibleRange)
        {
            inAudibleRange = true;

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
            if (inAudibleRange)
            {
                inAudibleRange = false;
                hasStartedAudio = false;
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

    // -------------------------------
    // SHOOTING
    // -------------------------------
    void TryShoot()
    {
        if (!canShoot) return; // ✅ skip when slowed/hit
        fireCooldown -= Time.deltaTime;

        if (fireCooldown <= 0f && !isChargingShot)
        {
            isChargingShot = true;
            fireCooldown = 1f / fireRate;

            if (animator != null)
            {
                animator.Play("projectileghostshoot");
                animator.speed = fireRate;
            }

            Invoke(nameof(ShootAtPlayer), fireDelay);
        }
    }

    void ShootAtPlayer()
    {
        if (!canShoot) return; // ✅ safety check

        if (player == null) return;
        if (projectilePrefab == null || firePoint == null) return;

        Vector3 spawnPos = firePoint.position + new Vector3(0f, firePointYOffset, 0f);
        GameObject bullet = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);

        Vector2 dir = (player.position - spawnPos).normalized;
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.linearVelocity = dir * 6f;

        isChargingShot = false;
    }

    // -------------------------------
    // SLOW / HIT REACTION
    // -------------------------------
    public void ApplySlowEffect()
    {
        if (!isSlowed)
            StartCoroutine(SlowEffectRoutine());
    }

    private IEnumerator SlowEffectRoutine()
    {
        isSlowed = true;
        canShoot = false;

        // Slow and recolor
        agent.speed *= slowMultiplier;
        if (spriteRenderer != null)
            spriteRenderer.color = slowColor;

        yield return new WaitForSeconds(slowDuration);

        // Restore
        agent.speed = originalSpeed;
        if (spriteRenderer != null)
            spriteRenderer.color = originalColor;

        canShoot = true;
        isSlowed = false;
    }
}
