using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioSource))]
public class PatrolChaseAI_WithProximitySound : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Transform[] patrolPoints;

    private NavMeshAgent agent;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private AudioSource audioSource;

    [Header("Behavior Settings")]
    public float detectionRange = 6f;    // Start chasing
    public float chaseStopRange = 9f;    // Stop chasing
    public float recalcInterval = 0.2f;  // Path update rate
    public float patrolWaitTime = 2f;    // Wait before next patrol

    [Header("Audio Settings")]
    public AudioClip patrolClip;
    public AudioClip chaseClip;
    public float fadeSpeed = 2f;
    public float baseVolume = 0.8f;

    [Header("Proximity Settings")]
    public float audibleRange = 8f;      // Max range where sound can be heard
    public float fullVolumeDistance = 2f; // Distance for full volume

    private int currentPointIndex = 0;
    private float waitTimer = 0f;
    private float recalcTimer = 0f;
    private bool isChasing = false;
    private Vector3 lastPosition;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();

        agent.updateRotation = false;
        agent.updateUpAxis = false;

        audioSource.playOnAwake = false;
        audioSource.loop = true;
        audioSource.volume = 0f;

        lastPosition = transform.position;

        if (patrolPoints.Length > 0)
            GoToNextPatrolPoint();

        PlayClip(patrolClip);
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        // --- STATE SWITCH ---
        if (!isChasing && distance <= detectionRange)
        {
            isChasing = true;
            animator.SetBool("IsChasing", true);
            CrossfadeTo(chaseClip);
        }
        else if (isChasing && distance >= chaseStopRange)
        {
            isChasing = false;
            animator.SetBool("IsChasing", false);
            CrossfadeTo(patrolClip);
            GoToNextPatrolPoint();
        }

        // --- BEHAVIOR ---
        if (isChasing)
            ChasePlayer();
        else
            Patrol();

        // --- PROXIMITY SOUND VOLUME ---
        UpdateProximityVolume(distance);

        // --- SPRITE FLIP ---
        Vector3 movement = transform.position - lastPosition;
        if (movement.x > 0.01f)
            spriteRenderer.flipX = false;
        else if (movement.x < -0.01f)
            spriteRenderer.flipX = true;

        lastPosition = transform.position;

        // Clamp Z axis
        Vector3 pos = transform.position;
        pos.z = 0;
        transform.position = pos;
    }

    void Patrol()
    {
        if (patrolPoints.Length == 0) return;

        if (!agent.pathPending && agent.remainingDistance < 0.2f)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= patrolWaitTime)
            {
                GoToNextPatrolPoint();
                waitTimer = 0f;
            }
        }
    }

    void ChasePlayer()
    {
        recalcTimer -= Time.deltaTime;
        if (recalcTimer <= 0f)
        {
            agent.SetDestination(player.position);
            recalcTimer = recalcInterval;
        }
    }

    void GoToNextPatrolPoint()
    {
        if (patrolPoints.Length == 0) return;

        agent.destination = patrolPoints[currentPointIndex].position;
        currentPointIndex = (currentPointIndex + 1) % patrolPoints.Length;
    }

    // --- SOUND FUNCTIONS ---
    void PlayClip(AudioClip clip)
    {
        if (clip == null) return;
        audioSource.clip = clip;
        audioSource.Play();
    }

    void CrossfadeTo(AudioClip newClip)
    {
        if (newClip == null || audioSource.clip == newClip)
            return;

        StopAllCoroutines();
        StartCoroutine(FadeSound(newClip));
    }

    IEnumerator FadeSound(AudioClip newClip)
    {
        // Fade out current
        while (audioSource.volume > 0.01f)
        {
            audioSource.volume -= Time.deltaTime * fadeSpeed;
            yield return null;
        }

        // Switch to new clip
        audioSource.clip = newClip;
        audioSource.Play();

        // Fade in
        while (audioSource.volume < baseVolume)
        {
            audioSource.volume += Time.deltaTime * fadeSpeed;
            yield return null;
        }

        audioSource.volume = baseVolume;
    }

    // --- PROXIMITY SYSTEM ---
    void UpdateProximityVolume(float distance)
    {
        if (!audioSource.isPlaying) return;

        if (distance <= audibleRange)
        {
            float t = Mathf.InverseLerp(audibleRange, fullVolumeDistance, distance);
            float targetVolume = Mathf.Lerp(0f, baseVolume, 1 - t);
            audioSource.volume = Mathf.MoveTowards(audioSource.volume, targetVolume, Time.deltaTime * fadeSpeed);
        }
        else
        {
            audioSource.volume = Mathf.MoveTowards(audioSource.volume, 0f, Time.deltaTime * fadeSpeed);
        }
    }
}
