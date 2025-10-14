using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class EnemyChaser2D : MonoBehaviour
{
    public Transform player;
    public float recalcInterval = 0.18f;
    public bool clampZToZero = true;

    private NavMeshAgent agent;
    private Animator animator;
    private float timer = 0f;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        // 2D settings for NavMeshAgent
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    void Update()
    {
        if (player == null) return;

        // Recalculate path at intervals
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            agent.SetDestination(player.position);
            timer = recalcInterval;
        }

        // Movement detection
        Vector3 velocity = agent.velocity;
        float speed = velocity.magnitude;

        // If moving, play the ghost animation. Otherwise, idle.
        if (speed > 0.05f)
        {
            animator.Play("madghost2");
        }
        else
        {
            animator.Play("madghost2"); // same animation for idle
        }

        // Clamp Z position to keep it 2D
        if (clampZToZero)
        {
            Vector3 pos = transform.position;
            pos.z = 0f;
            transform.position = pos;
        }
    }
}
