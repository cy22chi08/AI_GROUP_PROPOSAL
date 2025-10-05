using UnityEngine;

public class DirectionTrigger : MonoBehaviour
{
    public GameObject directionNPC;  // The NPC prefab to show
    public DirectionType direction;  // Enum for direction
    public float npcOffsetX = 1.5f;  // Optional offset above player or area

    private GameObject spawnedNPC;

    public enum DirectionType
    {
        Up,
        Down,
        Left,
        Right
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && directionNPC != null)
        {
            if (spawnedNPC == null)
            {
                spawnedNPC = Instantiate(directionNPC, transform.position + Vector3.up * npcOffsetX, Quaternion.identity);
                RotateArrow();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && spawnedNPC != null)
        {
            Destroy(spawnedNPC);
        }
    }

    void RotateArrow()
    {
        if (spawnedNPC == null) return;

        float rotationZ = 0f;
        switch (direction)
        {
            case DirectionType.Up: rotationZ = 0f; break;
            case DirectionType.Right: rotationZ = -90f; break;
            case DirectionType.Down: rotationZ = 180f; break;
            case DirectionType.Left: rotationZ = 90f; break;
        }

        spawnedNPC.transform.rotation = Quaternion.Euler(0, 0, rotationZ);
    }
}
