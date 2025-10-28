using UnityEngine;

public class CameraFollowing2D : MonoBehaviour
{
    [Header("Target to Follow")]
    public Transform target; // Assign the player here

    [Header("Follow Settings")]
    public float smoothSpeed = 5f;
    public Vector3 offset; // e.g., (0, 1, -10)

    [Header("Camera Limits (optional)")]
    public bool useLimits = false;
    public Vector2 minPosition;
    public Vector2 maxPosition;

    void LateUpdate()
    {
        if (target == null) return;

        // Target position with offset
        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        // Apply limits if enabled
        if (useLimits)
        {
            smoothedPosition.x = Mathf.Clamp(smoothedPosition.x, minPosition.x, maxPosition.x);
            smoothedPosition.y = Mathf.Clamp(smoothedPosition.y, minPosition.y, maxPosition.y);
        }

        transform.position = smoothedPosition;
    }
}
