using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class EnemyChaser2D : MonoBehaviour
{
    public Transform player;
    public float recalcInterval = 0.18f;
    public bool clampZToZero = true;

    [Header("Slow Effect Settings")]
    public float slowDuration = 3f;
    public float slowMultiplier = 0.4f;
    public Color slowColor = new Color(0.6f, 0.6f, 1f);

    private NavMeshAgent agent;
    private Animator animator;
    private float timer = 0f;
    private SpriteRenderer spriteRenderer;
    private float originalSpeed;
    private Color originalColor;
    private bool isSlowed = false;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        agent.updateRotation = false;
        agent.updateUpAxis = false;

        originalSpeed = agent.speed;
        originalColor = spriteRenderer.color;
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

        // Animation (simple for now)
        float speed = agent.velocity.magnitude;
        animator.Play("madghost2");

        // Clamp Z
        if (clampZToZero)
        {
            Vector3 pos = transform.position;
            pos.z = 0f;
            transform.position = pos;
        }
    }

     public void ApplySlowEffect()
    {
        if (!isSlowed)
        StartCoroutine(SlowEffectRoutine());
    }

        private IEnumerator SlowEffectRoutine()
        {
            isSlowed = true;

            // Slow down and recolor
            agent.speed *= slowMultiplier;
            spriteRenderer.color = slowColor;

             // Disable all EnemyDamage components
            EnemyDamage[] damages = GetComponentsInChildren<EnemyDamage>();
             foreach (var dmg in damages)
             dmg.enabled = false;

            // Optionally disable collider for extra safety
            Collider2D col = GetComponent<Collider2D>();
            if (col != null)
             col.enabled = false;

            yield return new WaitForSeconds(slowDuration);

            // Restore
            agent.speed = originalSpeed;
            spriteRenderer.color = originalColor;

            foreach (var dmg in damages)
            dmg.enabled = true;

            if (col != null)
             col.enabled = true;

            isSlowed = false;
    }

}
