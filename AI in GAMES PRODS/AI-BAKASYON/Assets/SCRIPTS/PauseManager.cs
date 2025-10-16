using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject pauseMenuPanel;     // The main pause/options panel
    public GameObject settingsPanel;      // Optional: separate if you have a sub-panel for sound

    [Header("Optional Settings")]
    public string mainMenuSceneName = "MainMenu"; // name of your main menu scene

    private bool isPaused = false;

    void Start()
    {
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);

        if (settingsPanel != null)
            settingsPanel.SetActive(false);
    }

    // 📦 Called by your Settings button
    public void TogglePause()
    {
        if (isPaused)
            ResumeGame();
        else
            PauseGame();
    }

    // 🎮 Pause logic
    public void PauseGame()
    {
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(true);

        Time.timeScale = 0f; // freeze everything
        isPaused = true;

        // Optional: play click sound
        AudioManager.Instance?.PlayClick();
    }

    // ▶️ Resume logic
    public void ResumeGame()
    {
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);

        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        Time.timeScale = 1f; // resume game
        isPaused = false;

        AudioManager.Instance?.PlayClick();
    }

    // ⚙️ Open sound options (uses your OptionsMenu.cs)
    public void OpenSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
            AudioManager.Instance?.PlayClick();
        }
    }

    // ⬅️ Return to Main Menu
    public void GoToMainMenu()
    {
        AudioManager.Instance?.PlayClick();
        Time.timeScale = 1f; // unpause before switching
        SceneManager.LoadScene(mainMenuSceneName);
    }

    // 🧭 Optional: Esc key toggles pause
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }
}
