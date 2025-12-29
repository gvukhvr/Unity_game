using UnityEngine;
using TMPro;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    public int level = 1;
    public int scorePerLevel = 30;

    public TMP_Text levelText;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        level = 1;
        UpdateUI();
    }


    public void UpdateLevel(int score)
    {
        int newLevel = score / scorePerLevel + 1;

        if (newLevel != level)
        {
            level = newLevel;
            ApplyDifficulty();
            SaveLevel();
        }

        UpdateUI();
    }

    void ApplyDifficulty()
    {
        if (LevelSpeedManager.Instance != null)
        {
            LevelSpeedManager.Instance.currentStepDelay =
                Mathf.Max(0.1f, 1f - level * 0.07f);
        }
    }
    //void ApplyDifficulty()
    //{
    //    if (LevelSpeedManager.Instance != null)
    //    {
    //        LevelSpeedManager.Instance.currentStepDelay =
    //            Mathf.Max(0.15f, 1f - level * 0.25f);
    //    }
    //}


    void UpdateUI()
    {
        if (levelText != null)
            levelText.text = "Level: " + level;
    }

    void SaveLevel()
    {
        PlayerPrefs.SetInt("LastLevel", level);
    }

    void LoadLevel()
    {
        level = PlayerPrefs.GetInt("LastLevel", 1);
    }
}
