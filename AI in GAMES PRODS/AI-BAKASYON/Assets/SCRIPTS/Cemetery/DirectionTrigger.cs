using UnityEngine;

public class DirectionTrigger : MonoBehaviour
{
    public GameObject directionNPC;   // The NPC prefab to show
    public DirectionType direction;   // Enum for direction
    public float npcOffsetX = 1.5f;   // Offset for where to spawn NPC
    public AudioClip appearSound;     // Sound to loop when NPC appears
    public float soundVolume = 1f;    // Volume of the sound

    private GameObject spawnedNPC;
    private AudioSource audioSource;

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
                // Spawn the NPC
                spawnedNPC = Instantiate(directionNPC, transform.position + Vector3.up * npcOffsetX, Quaternion.identity);
                RotateArrow();

                // Play looping sound
                if (appearSound != null)
                {
                    audioSource = gameObject.AddComponent<AudioSource>();
                    audioSource.clip = appearSound;
                    audioSource.loop = true;
                    audioSource.volume = AudioManager.Instance.sfxVolume;
                    audioSource.spatialBlend = 1f; // 3D sound effect
                    audioSource.minDistance = 2f;  // Start to fade after this distance
                    audioSource.maxDistance = 10f; // Fully fade out after this
                    audioSource.Play();
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (spawnedNPC != null)
            {
                Destroy(spawnedNPC);
            }

            // Stop and remove audio
            if (audioSource != null)
            {
                audioSource.Stop();
                Destroy(audioSource);
            }
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
