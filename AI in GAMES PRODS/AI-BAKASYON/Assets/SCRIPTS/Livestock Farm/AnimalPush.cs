using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class AnimalPush : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private bool isMoving;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isMoving && collision.gameObject.CompareTag("Player"))
        {
            Vector2 direction = (transform.position - collision.transform.position).normalized;

            // Snap to 4 directions
            Vector2 moveDir = Vector2.zero;
            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
                moveDir = new Vector2(Mathf.Sign(direction.x), 0);
            else
                moveDir = new Vector2(0, Mathf.Sign(direction.y));

            Vector2 targetPos = rb.position + moveDir;

            StartCoroutine(MoveToPosition(targetPos));
        }
    }

    System.Collections.IEnumerator MoveToPosition(Vector2 target)
    {
        isMoving = true;
        while ((target - rb.position).sqrMagnitude > 0.01f)
        {
            rb.MovePosition(Vector2.MoveTowards(rb.position, target, moveSpeed * Time.deltaTime));
            yield return null;
        }
        rb.MovePosition(target);
        isMoving = false;
    }
}
