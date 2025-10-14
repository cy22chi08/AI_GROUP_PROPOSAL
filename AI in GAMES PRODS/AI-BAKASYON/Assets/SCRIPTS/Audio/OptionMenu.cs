using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    public Slider musicSlider;
    public Slider sfxSlider;
    public GameObject optionsPanel;

    void Start()
    {
        musicSlider.onValueChanged.AddListener(AudioManager.Instance.SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(AudioManager.Instance.SetSFXVolume);
    }

    public void CloseOptions()
    {
        AudioManager.Instance.PlayClick();
        optionsPanel.SetActive(false);
    }
}
