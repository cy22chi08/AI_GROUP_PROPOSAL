using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class AnimalPush : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 2.5f;
    public float moveDuration = 2f;
    public float pushCooldownTime = 1f;

    [Header("Destruction Settings (Optional)")]
    public Transform destroyTarget;        // assign if needed
    public float destroyRadius = 0.5f;     // how close before it's destroyed

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private bool isMoving = false;
    private bool canBePushed = true;
    private bool isTouchingPlayer = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        rb.gravityScale = 0;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;

        // Start in idle (frame 1 of the animation)
        animator.speed = 0f;
    }

    void Update()
    {
        // Optional destroy logic
        if (destroyTarget != null && Vector2.Distance(transform.position, destroyTarget.position) <= destroyRadius)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isTouchingPlayer = true;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;

            if (canBePushed && !isMoving)
            {
                Vector2 direction = (transform.position - collision.transform.position).normalized;

                // Snap to 4 directions only
                Vector2 moveDir = Vector2.zero;
                if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
                    moveDir = new Vector2(Mathf.Sign(direction.x), 0);
                else
                    moveDir = new Vector2(0, Mathf.Sign(direction.y));

                // Flip sprite only horizontally
                if (moveDir.x < 0)
                    spriteRenderer.flipX = true;
                else if (moveDir.x > 0)
                    spriteRenderer.flipX = false;

                StartCoroutine(MoveAnimal(moveDir));
                StartCoroutine(PushCooldown());
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isTouchingPlayer = false;
            if (!isMoving)
                rb.constraints = RigidbodyConstraints2D.FreezeAll;
        }
    }

    IEnumerator MoveAnimal(Vector2 moveDir)
    {
        isMoving = true;

        // 🔸 Play animation (start animating)
        animator.speed = 1f;

        float elapsed = 0f;
        while (elapsed < moveDuration)
        {
            rb.MovePosition(rb.position + moveDir * moveSpeed * Time.deltaTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // 🔸 Stop movement and return to first frame
        rb.linearVelocity = Vector2.zero;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        animator.speed = 0f;  // pause at first frame again
        isMoving = false;
    }

    IEnumerator PushCooldown()
    {
        canBePushed = false;
        yield return new WaitForSeconds(pushCooldownTime);
        canBePushed = true;
    }
}
