using UnityEngine;

public class TriggerPanelWithAudio : MonoBehaviour
{
    [Header("UI Settings")]
    public GameObject panel; // The UI panel to show

    [Header("Audio Settings")]
    public AudioSource audioSource; // AudioSource component
    public AudioClip openSound;     // Sound when panel opens
    public AudioClip closeSound;    // Sound when panel closes

    [Header("Player Settings")]
    public string playerTag = "Player"; // Make sure player is tagged as "Player"

    private bool isPaused = false;

    private void Start()
    {
        if (panel != null)
            panel.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag) && !isPaused)
        {
            if (panel != null)
                panel.SetActive(true);

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

    // Called from button or automatically on exit
    public void ResumeGame()
    {
        if (panel != null)
            panel.SetActive(false);

        if (audioSource != null && closeSound != null)
            audioSource.PlayOneShot(closeSound);

        Time.timeScale = 1f; // Resume game
        isPaused = false;
    }
}
