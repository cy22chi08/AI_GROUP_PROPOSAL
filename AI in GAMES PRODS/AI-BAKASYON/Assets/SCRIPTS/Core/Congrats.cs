using UnityEngine;

public class TriggerPanelWithAudio : MonoBehaviour
{
    [Header("UI Settings")]
    public GameObject Congratspanel;  // The UI panel to show
    public GameObject ItemUIpanel;    // The item UI panel to hide

    [Header("Audio Settings")]
    public AudioSource audioSource;   // AudioSource component
    public AudioClip openSound;       // Sound when panel opens
    public AudioClip closeSound;      // Sound when panel closes

    [Header("Player Settings")]
    public string playerTag = "Player"; // Make sure player is tagged "Player"

    private bool isPaused = false;

    private void Start()
    {
        if (Congratspanel != null)
            Congratspanel.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag) && !isPaused)
        {
            if (Congratspanel != null)
                Congratspanel.SetActive(true);

            // ✅ Hide the item UI when game completes
            if (ItemUIpanel != null)
                ItemUIpanel.SetActive(false);

            if (audioSource != null && openSound != null)
                audioSource.PlayOneShot(openSound);

            Time.timeScale = 0f; // Pause the game
            isPaused = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(playerTag) && isPaused)
        {
            ResumeGame();
        }
    }

    // ✅ Called from button or automatically on exit
    public void ResumeGame()
    {
        if (Congratspanel != null)
            Congratspanel.SetActive(false);

        // Re-enable Item UI when game resumes
        if (ItemUIpanel != null)
            ItemUIpanel.SetActive(true);

        if (audioSource != null && closeSound != null)
            audioSource.PlayOneShot(closeSound);

        Time.timeScale = 1f; // Resume the game
        isPaused = false;
    }
}
