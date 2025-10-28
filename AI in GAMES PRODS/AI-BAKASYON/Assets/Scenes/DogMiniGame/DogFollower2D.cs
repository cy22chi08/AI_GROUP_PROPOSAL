using UnityEngine;
using UnityEngine.SceneManagement; // ✅ For reloading the scene

[RequireComponent(typeof(Rigidbody2D))]
public class DogFollower2D : MonoBehaviour
{
    [Header("Follow Settings")]
    public Transform player;
    public float moveSpeed = 4f;
    public float followDistance = 1.5f;

    [Header("Jump Settings")]
    public float jumpForce = 10f;
    public float obstacleCheckDistance = 1f;
    public Transform groundCheck;
    public LayerMask groundLayer;
    public LayerMask obstacleLayer;
    public float checkRadius = 0.2f;

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool facingRight = true; // ✅ Added missing variable

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (player == null) return;

        // ✅ Ground check
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);

        float distanceX = player.position.x - transform.position.x;
        float dir = Mathf.Sign(distanceX);

        // ✅ Always move slightly toward player to ensure collision
        if (Mathf.Abs(distanceX) > 0.3f) // only stop when very close
        {
            rb.linearVelocity = new Vector2(dir * moveSpeed, rb.linearVelocity.y);
        }
        else
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }

        // ✅ Auto Flip toward player
        if ((dir > 0 && !facingRight) || (dir < 0 && facingRight))
        {
            Flip();
        }

        // ✅ Jump over obstacles
        RaycastHit2D hit = Physics2D.Raycast(
            transform.position + new Vector3(dir * 0.5f, 0, 0),
            Vector2.right * dir,
            obstacleCheckDistance,
            obstacleLayer
        );

        if (hit.collider != null && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        // ✅ Ground correction
        if (isGrounded && rb.linearVelocity.y < -0.1f)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);

        // ✅ Distance-based catch (backup in case collision misses)
        if (Vector2.Distance(transform.position, player.position) < 0.4f)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    // ✅ Collision-based catch
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    // ✅ Flip method
    void Flip()
    {
        facingRight = !facingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }

    // ✅ Debug helper
    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
            Gizmos.DrawWireSphere(groundCheck.position, checkRadius);
    }
}
