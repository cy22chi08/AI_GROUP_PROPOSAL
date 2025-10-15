using UnityEngine;

public class FireflyBehavior : MonoBehaviour
{
    public float moveRadius = 2f;
    public float moveSpeed = 0.5f;
    private Vector3 targetPosition;

    void Start()
    {
        SetNewTarget();
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            SetNewTarget();
    }

    void SetNewTarget()
    {
        Vector2 randomOffset = Random.insideUnitCircle * moveRadius;
        targetPosition = transform.parent.position + new Vector3(randomOffset.x, randomOffset.y, 0);
    }
}
