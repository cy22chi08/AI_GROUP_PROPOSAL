using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Player Health Settings")]
    public int maxHealth = 5;
    public int currentHealth;

    [Header("Heart Icons (assign in order from left to right)")]
    public GameObject[] hearts; // 5 heart GameObjects

    [Header("Damage Cooldown")]
    public float damageCooldown = 0.5f;
    private float lastDamageTime = 0f;

    [Header("UI Settings")]
    public GameObject gameOverPanel; // Game Over panel

    [Header("Audio Settings")]
    public AudioSource audioSource;
    public AudioClip damageSound;
    public AudioClip gameOverSound;

    private bool isGameOver = false;

    void Start()
    {
        currentHealth = maxHealth;

        // Hide game over panel at start
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        UpdateHearts();
    }

    public void TakeDamage(int amount)
    {
        if (isGameOver || Time.time - lastDamageTime < damageCooldown)
            return;

        lastDamageTime = Time.time;
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        // Play damage sound
        if (audioSource != null && damageSound != null)
            audioSource.PlayOneShot(damageSound);

        Debug.Log("Player took damage! Current health: " + currentHealth);

        UpdateHearts();

        if (currentHealth <= 0)
            GameOver();
    }

    void UpdateHearts()
    {
        // Show or hide hearts based on current health
        for (int i = 0; i < hearts.Length; i++)
        {
            if (hearts[i] != null)
                hearts[i].SetActive(i < currentHealth);
        }
    }

    void GameOver()
    {
        if (isGameOver) return;
        isGameOver = true;

        Debug.Log("GAME OVER!");
        Time.timeScale = 0f; // Pause game

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        if (audioSource != null && gameOverSound != null)
            audioSource.PlayOneShot(gameOverSound);
    }

    // Called from Restart button
    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Called from Quit button
    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }
}
