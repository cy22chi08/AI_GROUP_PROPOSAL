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
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    void Update()
    {
        if (player == null) return;

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            agent.SetDestination(player.position);
            timer = recalcInterval;
        }

        Vector3 velocity = agent.velocity;
        Vector2 moveDir = new Vector2(velocity.x, velocity.y);

        // 🔸 Add a small threshold so tiny movements don't break idle
        if (moveDir.magnitude > 0.05f)
        {
            float absX = Mathf.Abs(moveDir.x);
            float absY = Mathf.Abs(moveDir.y);

            if (absX > absY)
            {
                if (moveDir.x > 0)
                    animator.Play("madghost_right");
                else
                    animator.Play("madghost_left");
            }
            else
            {
                if (moveDir.y > 0)
                    animator.Play("madghost_up");
                else
                    animator.Play("madghost_down");
            }
        }
        else
        {
            animator.Play("madghost_down");
        }

        // clamp z
        if (clampZToZero)
        {
            Vector3 p = transform.position;
            p.z = 0f;
            transform.position = p;
        }
    }
}
