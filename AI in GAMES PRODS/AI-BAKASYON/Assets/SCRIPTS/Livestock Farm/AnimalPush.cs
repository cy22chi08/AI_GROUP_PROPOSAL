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

    [Header("Destruction Settings")]
    public Transform destroyTarget;
    public float destroyRadius = 0.5f;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private bool isMoving;
    private bool canBePushed = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        rb.gravityScale = 0;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        animator.speed = 0f; // idle pose
    }

    void Update()
    {
        if (destroyTarget && Vector2.Distance(transform.position, destroyTarget.position) <= destroyRadius)
            Destroy(gameObject);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player") || !canBePushed || isMoving)
            return;

        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        Vector2 direction = (transform.position - collision.transform.position).normalized;
        Vector2 moveDir = Mathf.Abs(direction.x) > Mathf.Abs(direction.y)
            ? new Vector2(Mathf.Sign(direction.x), 0)
            : new Vector2(0, Mathf.Sign(direction.y));

        spriteRenderer.flipX = moveDir.x < 0;
        StartCoroutine(MoveAnimal(moveDir));
        StartCoroutine(PushCooldown());
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !isMoving)
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
    }

    IEnumerator MoveAnimal(Vector2 moveDir)
    {
        isMoving = true;
        animator.speed = 1f;

        float elapsed = 0f;
        while (elapsed < moveDuration)
        {
            rb.MovePosition(rb.position + moveDir * moveSpeed * Time.deltaTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        rb.linearVelocity = Vector2.zero;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        animator.speed = 0f;
        isMoving = false;
    }

    IEnumerator PushCooldown()
    {
        canBePushed = false;
        yield return new WaitForSeconds(pushCooldownTime);
        canBePushed = true;
    }
}
