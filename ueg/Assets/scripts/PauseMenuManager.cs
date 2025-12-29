using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;

public class PauseMenuManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject pausePanel;
    public GameObject settingsPanel;

    [Header("Settings UI - только то что уже есть")]
    public TMP_Dropdown graphicsDropdown;
    public Slider musicVolSlider;

    [Header("Audio")]
    public AudioMixer mainAudioMixer;

    private bool isPaused = false;

    private void Start()
    {
        // Скрываем панели при старте
        if (pausePanel != null)
            pausePanel.SetActive(false);

        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        
        LoadSettings();

        Time.timeScale = 1f;
    }

    private void Update()
    {
        // Нажатие ESC для паузы
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                Resume();
            else
                Pause();
        }
    }

    // ПАУЗА 
    public void Pause()
    {
        isPaused = true;

        // Показываем панель паузы
        if (pausePanel != null)
            pausePanel.SetActive(true);

        // Прячем настройки
        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        Time.timeScale = 0f;
    }

    public void Resume()
    {
        isPaused = false;

        // Прячем обе панели
        if (pausePanel != null)
            pausePanel.SetActive(false);

        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        Time.timeScale = 1f;
    }


    public void OpenSettings()
    {
        PlayerPrefs.SetString("PreviousScene", SceneManager.GetActiveScene().name);
        PlayerPrefs.Save();

        SceneManager.LoadScene("settings");
    }



    public void CloseSettings()
    {
        Debug.Log("Closing Settings Panel");

        // Прячем настройки
        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        // Показываем панель паузы
        if (pausePanel != null)
            pausePanel.SetActive(true);
    }

    // НАСТРОЙКИ - ГРАФИКА 
    public void ChangeGraphicsQuality()
    {
        if (graphicsDropdown == null) return;

        int quality = graphicsDropdown.value;
        QualitySettings.SetQualityLevel(quality);

        // Сохраняем
        PlayerPrefs.SetInt("GraphicsQuality", quality);
        PlayerPrefs.Save();

        Debug.Log("Graphics Quality changed to: " + quality);
    }

    //  НАСТРОЙКИ - МУЗЫКА
    public void ChangeMusicVolume()
    {
        if (musicVolSlider == null || mainAudioMixer == null) return;

        float volume = musicVolSlider.value;
        mainAudioMixer.SetFloat("MusicVol", volume);

        // Сохраняем
        PlayerPrefs.SetFloat("MusicVolume", volume);
        PlayerPrefs.Save();

        Debug.Log("Music Volume changed to: " + volume);
    }

    //  ЗАГРУЗКА НАСТРОЕК
    private void LoadSettings()
    {
        Debug.Log("Loading Settings...");

        // Графика
        if (graphicsDropdown != null)
        {
            int quality = PlayerPrefs.GetInt("GraphicsQuality", QualitySettings.GetQualityLevel());
            graphicsDropdown.value = quality;
            QualitySettings.SetQualityLevel(quality);
            Debug.Log("Loaded Graphics Quality: " + quality);
        }

       
        if (musicVolSlider != null && mainAudioMixer != null)
        {
            float musicVol = PlayerPrefs.GetFloat("MusicVolume", 0f);
            musicVolSlider.value = musicVol;
            mainAudioMixer.SetFloat("MusicVol", musicVol);
            Debug.Log("Loaded Music Volume: " + musicVol);
        }
    }

    //  КНОПКИ МЕНЮ 
    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void MainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("main menu");
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}