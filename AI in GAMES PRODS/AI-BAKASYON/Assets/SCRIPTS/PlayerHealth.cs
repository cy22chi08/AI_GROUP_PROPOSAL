using UnityEngine;
using UnityEngine.SceneManagement; // For reloading or changing scene

public class PlayerHealth : MonoBehaviour
{
    [Header("Player Health Settings")]
    public int maxHealth = 5;
    public int currentHealth;

    [Header("Damage Cooldown")]
    public float damageCooldown = 0.5f; // half second grace to prevent rapid hits
    private float lastDamageTime = 0f;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        if (Time.time - lastDamageTime < damageCooldown)
            return;

        lastDamageTime = Time.time;
        currentHealth -= amount;

        Debug.Log("Player took damage! Current health: " + currentHealth);

        if (currentHealth <= 0)
        {
            GameOver();
        }
    }

    void GameOver()
    {
        Debug.Log("GAME OVER!");
        // Example: reload scene or show UI
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
