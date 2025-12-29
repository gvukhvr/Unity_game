using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class SettingsMenuManager : MonoBehaviour
{
    public TMP_Dropdown graphicsDropdown;
    public Slider musicSlider;

    private void Start()
    {
        if (SettingsManager.Instance == null) return;

        graphicsDropdown.value =
            PlayerPrefs.GetInt("GraphicsQuality", QualitySettings.GetQualityLevel());

        musicSlider.value =
            PlayerPrefs.GetFloat("MusicVolume", 0f);
    }

    public void ChangeGraphics()
    {
        SettingsManager.Instance.SetGraphics(graphicsDropdown.value);
    }

    public void ChangeMusic()
    {
        SettingsManager.Instance.SetMusicVolume(musicSlider.value);
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("main menu");
    }

    public void CloseSettings()
    {
        string previousScene = PlayerPrefs.GetString("PreviousScene", "main menu");
        SceneManager.LoadScene(previousScene);
    }
}
