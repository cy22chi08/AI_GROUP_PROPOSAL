using UnityEngine;
using UnityEngine.AI;

public class ProjectileEnemy : MonoBehaviour
{
    [Header("AI Settings")]
    public Transform player;
    public float recalcInterval = 0.2f;
    public float stopDistance = 4f; // distance to start shooting
    public float moveSpeedMultiplier = 0.8f; // slower than chaser

    [Header("Attack Settings")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float fireRate = 1f;
    private float fireCooldown = 0f;

    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.speed *= moveSpeedMultiplier;
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        // Movement
        if (distance > stopDistance)
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);
        }
        else
        {
            agent.isStopped = true;
            ShootAtPlayer();
        }

        // Clamp Z
        Vector3 pos = transform.position;
        pos.z = 0;
        transform.position = pos;
    }

    void ShootAtPlayer()
    {
        fireCooldown -= Time.deltaTime;

        if (fireCooldown <= 0f)
        {
            fireCooldown = 1f / fireRate;

            // Instantiate projectile
            GameObject bullet = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
            Vector2 dir = (player.position - firePoint.position).normalized;

            bullet.GetComponent<Rigidbody2D>().linearVelocity = dir * 6f; // bullet speed
        }
    }
}
