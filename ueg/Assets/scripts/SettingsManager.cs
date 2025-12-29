using UnityEngine;
using UnityEngine.Audio;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance;

    [Header("Audio")]
    public AudioSource musicSource;
    public AudioMixer mainAudioMixer;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        Debug.Log("SettingsManager created");
    }

    private void Start()
    {
       
        LoadSettings();
    }

    //  GRAPHICS 
    public void SetGraphics(int quality)
    {
        QualitySettings.SetQualityLevel(quality);
        PlayerPrefs.SetInt("GraphicsQuality", quality);
        PlayerPrefs.Save();
        Debug.Log("Graphics Quality set to: " + quality);
    }

    //MUSIC 
    public void SetMusicVolume(float volume)
    {
        if (mainAudioMixer == null)
        {
            Debug.LogError("AudioMixer is NULL!");
            return;
        }

        bool success = mainAudioMixer.SetFloat("MusicVol", volume);

        if (success)
        {
            Debug.Log("Music Volume set to: " + volume);
        }
        else
        {
            Debug.LogError("Failed to set MusicVol! Check if 'MusicVol' parameter is exposed in AudioMixer");
        }

        PlayerPrefs.SetFloat("MusicVolume", volume);
        PlayerPrefs.Save();
    }

    private void LoadSettings()
    {
        Debug.Log("Loading settings...");

        // GRAPHICS
        int quality = PlayerPrefs.GetInt("GraphicsQuality", QualitySettings.GetQualityLevel());
        QualitySettings.SetQualityLevel(quality);
        Debug.Log("Loaded Graphics Quality: " + quality);

        // MUSIC
        float volume = PlayerPrefs.GetFloat("MusicVolume", 0f);

        if (mainAudioMixer != null)
        {
            bool success = mainAudioMixer.SetFloat("MusicVol", volume);

            if (success)
            {
                Debug.Log("Loaded Music Volume: " + volume);
            }
            else
            {
                Debug.LogError("Failed to load MusicVol! Parameter might not be exposed.");
            }
        }
        else
        {
            Debug.LogError("AudioMixer is NULL! Cannot load music volume.");
        }
    }
}