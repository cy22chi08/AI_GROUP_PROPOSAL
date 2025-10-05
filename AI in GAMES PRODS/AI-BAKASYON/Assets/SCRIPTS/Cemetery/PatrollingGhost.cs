using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class PatrolChaseAI : MonoBehaviour
{
    public Transform player;
    public Transform[] patrolPoints;
    public float detectionRange = 6f;    // when to start chasing
    public float chaseStopRange = 9f;    // when to stop chasing (a bit larger to avoid jitter)
    public float recalcInterval = 0.2f;  // how often to update path
    public float patrolWaitTime = 2f;    // wait before moving to next point

    private NavMeshAgent agent;
    private int currentPointIndex = 0;
    private float waitTimer = 0f;
    private float recalcTimer = 0f;
    private bool isChasing = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
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
        }
        else if (isChasing && distance >= chaseStopRange)
        {
            isChasing = false;
            GoToNextPatrolPoint();
        }

        // --- BEHAVIOR ---
        if (isChasing)
            ChasePlayer();
        else
            Patrol();

        // Clamp Z so it stays 2D
        Vector3 pos = transform.position;
        pos.z = 0;
        transform.position = pos;
    }

    void Patrol()
    {
        if (patrolPoints.Length == 0) return;

        // Move to current patrol point
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
