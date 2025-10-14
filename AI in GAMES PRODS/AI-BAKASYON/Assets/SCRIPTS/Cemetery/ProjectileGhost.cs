using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class ProjectileGhost : MonoBehaviour
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
    public float firePointYOffset = 0.5f; // 🔸 height offset for bullet spawn

    private float fireCooldown = 0f;
    private bool isChargingShot = false;

    private NavMeshAgent agent;
    private Animator animator;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.speed *= moveSpeedMultiplier;
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance > stopDistance)
        {
            // Chase the player
            if (animator != null)
                animator.Play("projectileghostidle");

            isChargingShot = false;
            agent.isStopped = false;
            agent.SetDestination(player.position);
        }
        else
        {
            // Stop moving and prepare to shoot
            agent.isStopped = true;
            TryShoot();
        }

        // Clamp Z axis
        Vector3 pos = transform.position;
        pos.z = 0;
        transform.position = pos;
    }

    void TryShoot()
    {
        fireCooldown -= Time.deltaTime;

        // Only shoot when cooldown allows and not already charging
        if (fireCooldown <= 0f && !isChargingShot)
        {
            isChargingShot = true;
            fireCooldown = 1f / fireRate;

            // 🔸 Play shoot animation
            if (animator != null)
            {
                animator.Play("projectileghostshoot");
                animator.speed = fireRate; // adjust playback speed to match firing rate
            }

            // 🔸 Delay actual projectile creation to sync with animation frame
            Invoke(nameof(ShootAtPlayer), fireDelay);
        }
    }

    void ShootAtPlayer()
    {
        if (player == null) return;

        // 🔸 Apply height offset to the firePoint position
        Vector3 spawnPos = firePoint.position + new Vector3(0f, firePointYOffset, 0f);

        // Instantiate projectile
        GameObject bullet = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);
        Vector2 dir = (player.position - spawnPos).normalized;

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.linearVelocity = dir * 6f;

        // Allow next shot
        isChargingShot = false;
    }
}
