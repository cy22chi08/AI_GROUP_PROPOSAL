// EnemyChaser2D.cs
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyChaser2D : MonoBehaviour
{
    public Transform player;                 // drag your player here or assign at runtime
    public float recalcInterval = 0.18f;     // how often to recalc path (tweak for perf)
    public bool clampZToZero = true;         // keep enemy in 2D plane

    NavMeshAgent agent;
    float timer = 0f;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        // 2D adjustments:
        agent.updateRotation = false; // don't let agent rotate in 3D
        agent.updateUpAxis = false;  // ignore navmesh up-axis alignment
    }

    void Start()
    {
        // ensure Rigidbody2D is kinematic (to avoid physics vs navmesh race)
        var rb2d = GetComponent<Rigidbody2D>();
        if (rb2d != null) rb2d.bodyType = RigidbodyType2D.Kinematic;
    }

    void Update()
    {
        if (player == null) return;
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            // set destination (agent will compute path)
            agent.SetDestination(player.position);
            timer = recalcInterval;
        }

        // optional: rotate sprite to face movement direction
        Vector3 vel = agent.velocity;
        if (vel.sqrMagnitude > 0.001f)
        {
            float angle = Mathf.Atan2(vel.y, vel.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle - 90f); // adjust -90 if sprite forward differs
        }

        // keep Z locked to zero (fix offsets introduced by rotated NavMesh)
        if (clampZToZero)
        {
            Vector3 p = transform.position;
            p.z = 0f;
            transform.position = p;
        }
    }
}
