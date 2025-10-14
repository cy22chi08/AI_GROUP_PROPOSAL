using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject optionsPanel;
    public void StartGame()
    {
        AudioManager.Instance.PlayClick();
        SceneManager.LoadScene("Town_one"); // change to your game scene name
    }

    public void OpenOptions()
    {
        AudioManager.Instance.PlayClick();
        // Activate your options panel here
        optionsPanel.SetActive(true);
    }

    public void QuitGame()
    {
        AudioManager.Instance.PlayClick();
        Application.Quit();
        Debug.Log("Quit Game");
    }
}
