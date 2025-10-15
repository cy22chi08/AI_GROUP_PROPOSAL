using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class AnimalPush : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private bool isMoving = false;
    private bool canBePushed = true;
    private bool isTouchingPlayer = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.constraints = RigidbodyConstraints2D.FreezeAll; // Freeze by default
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isTouchingPlayer = true;

            // Temporarily unfreeze X and Y so it can move
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;

            if (canBePushed && !isMoving)
            {
                Vector2 direction = (transform.position - collision.transform.position).normalized;

                // Snap to 4 directions
                Vector2 moveDir = Vector2.zero;
                if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
                    moveDir = new Vector2(Mathf.Sign(direction.x), 0);
                else
                    moveDir = new Vector2(0, Mathf.Sign(direction.y));

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

            // Freeze position again when not touching player
            if (!isMoving)
                rb.constraints = RigidbodyConstraints2D.FreezeAll;
        }
    }

    IEnumerator MoveAnimal(Vector2 moveDir)
    {
        isMoving = true;

        float elapsed = 0f;
        while (elapsed < 2f)
        {
            rb.MovePosition(rb.position + moveDir * moveSpeed * Time.deltaTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Stop movement and freeze again
        rb.linearVelocity = Vector2.zero;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        isMoving = false;
    }

    IEnumerator PushCooldown()
    {
        canBePushed = false;
        yield return new WaitForSeconds(1f);
        canBePushed = true;
    }
}
