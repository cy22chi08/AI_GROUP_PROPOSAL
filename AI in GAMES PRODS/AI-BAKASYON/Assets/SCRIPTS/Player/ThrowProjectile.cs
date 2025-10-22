using UnityEngine;

public class ThrowProjectile : MonoBehaviour
{
    public float lifetime = 3f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // If it hits something tagged "Enemy"
        if (collision.CompareTag("Enemy"))
        {
            // Works for EnemyChaser2D
            EnemyChaser2D chaser = collision.GetComponent<EnemyChaser2D>();
            if (chaser != null)
            {
                chaser.ApplySlowEffect();
            }

            // Works for PatrolChaseAI
            PatrollingGhost patrol = collision.GetComponent<PatrollingGhost>();
            if (patrol != null)
            {
                patrol.FreezeEnemy(); 
            }

            ProjectileGhost ghost = collision.GetComponent<ProjectileGhost>();
            if (ghost != null)
            {
                ghost.ApplySlowEffect();
            }

            Destroy(gameObject); // Destroy projectile on hit
        }
    }
}
