using UnityEngine;
using UnityEngine.UI;

public class PlayerThrowItem : MonoBehaviour
{
    [Header("Throw Settings")]
    public GameObject throwPrefab; // projectile to throw
    public Transform throwPoint;   // spawn point of projectile
    public float throwForce = 8f;

    [Header("Inventory Settings")]
    public int maxItemCount = 3;
    public int currentItemCount = 0;

    [Header("Garlic UI Icons (assign in order from left to right)")]
    public GameObject[] garlicIcons; // garlic UI icons like hearts

    [Header("Audio Settings")]
    public AudioSource audioSource;
    public AudioClip collectSound;
    public AudioClip throwSound;

    void Start()
    {
        audioSource.loop = false; // ensure it's off
        UpdateGarlicUI();
    }

    void Update()
    {
        // Throw item if available
        if (Input.GetKeyDown(KeyCode.Space) && currentItemCount > 0)
        {
            ThrowItem();
        }
    }

    private void ThrowItem()
    {
        GameObject obj = Instantiate(throwPrefab, throwPoint.position, Quaternion.identity);
        Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            Vector2 direction = Vector2.zero;

            // Determine throw direction
            if (Input.GetAxisRaw("Horizontal") != 0)
                direction = new Vector2(Input.GetAxisRaw("Horizontal"), 0);
            else if (Input.GetAxisRaw("Vertical") != 0)
                direction = new Vector2(0, Input.GetAxisRaw("Vertical"));
            else
                direction = transform.localScale.x < 0 ? Vector2.left : Vector2.right;

            rb.linearVelocity = direction.normalized * throwForce;
        }

        currentItemCount = Mathf.Clamp(currentItemCount - 1, 0, maxItemCount);
        UpdateGarlicUI();

        // Play throw sound
        if (audioSource != null && throwSound != null)
                {
                    audioSource.loop = false;
                    audioSource.PlayOneShot(throwSound);
                }
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Picking up throwable items
        if (other.CompareTag("ThrowableItem"))
        {
            if (currentItemCount < maxItemCount)
            {
                currentItemCount++;
                Destroy(other.gameObject);
                UpdateGarlicUI();

                // Play collect sound
                if (audioSource != null && collectSound != null)
            {
                audioSource.loop = false;
                audioSource.PlayOneShot(collectSound);
            }
            }
        }
    }

    private void UpdateGarlicUI()
    {
        // Show or hide garlic icons based on current count
        for (int i = 0; i < garlicIcons.Length; i++)
        {
            if (garlicIcons[i] != null)
                garlicIcons[i].SetActive(i < currentItemCount);
        }
    }
}
