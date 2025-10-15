using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class PatrolChaseAI : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Transform[] patrolPoints;
    private NavMeshAgent agent;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    [Header("Behavior Settings")]
    public float detectionRange = 6f;    // start chasing
    public float chaseStopRange = 9f;    // stop chasing
    public float recalcInterval = 0.2f;  // path update rate
    public float patrolWaitTime = 2f;    // wait before next patrol

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

        agent.updateRotation = false;
        agent.updateUpAxis = false;

        lastPosition = transform.position;

        if (patrolPoints.Length > 0)
            GoToNextPatrolPoint();
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
            Debug.Log("→ Switching to Chase");
        }
        else if (isChasing && distance >= chaseStopRange)
        {
            isChasing = false;
            animator.SetBool("IsChasing", false);
            Debug.Log("→ Switching back to Patrol");
            GoToNextPatrolPoint();
        }

        // --- BEHAVIOR ---
        if (isChasing)
            ChasePlayer();
        else
            Patrol();

        // --- FLIP SPRITE BASED ON DIRECTION ---
        Vector3 movement = transform.position - lastPosition;
        if (movement.x > 0.01f)
            spriteRenderer.flipX = false; // facing right
        else if (movement.x < -0.01f)
            spriteRenderer.flipX = true; // facing left

        lastPosition = transform.position;

        // Clamp Z axis for 2D
        Vector3 pos = transform.position;
        pos.z = 0;
        transform.position = pos;
    }

    void Patrol()
    {
        if (patrolPoints.Length == 0) return;

        // Check if reached patrol point
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
}
