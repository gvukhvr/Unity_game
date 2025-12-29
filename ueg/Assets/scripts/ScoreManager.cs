using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    public int score = 0;
    public TMP_Text scoreText;
    public TMP_Text highScoreText; 

    private int highScore = 0;

    private void Awake()
    {
        Instance = this;
        LoadHighScore();
        UpdateText();
    }
    private int scoreMultiplier = 1;

    public void SetMultiplier(int value)
    {
        scoreMultiplier = value;
    }

 

    //public void AddScore(int amount)
    //{
    //    score += amount;

    //    // Проверяем, побит ли рекорд
    //    if (score > highScore)
    //    {
    //        highScore = score;
    //        SaveHighScore();
    //    }

    //    UpdateText();

    //    // Обновляем уровень если есть LevelManager
    //    if (LevelManager.Instance != null)
    //    {
    //        LevelManager.Instance.UpdateLevel(score);
    //    }
    //}

    public void AddScore(int amount)
    {
       
        score += amount * scoreMultiplier;

       
        if (score > highScore)
        {
            highScore = score;
            SaveHighScore();
        }

        UpdateText();

       
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.UpdateLevel(score);
        }
    }


    private void UpdateText()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + score.ToString();

        if (highScoreText != null)
            highScoreText.text = "Best: " + highScore.ToString();
    }

    public void ResetScore()
    {
        score = 0;
        UpdateText();
    }

    public int GetHighScore()
    {
        return highScore;
    }

    private void SaveHighScore()
    {
        PlayerPrefs.SetInt("HighScore", highScore);
        PlayerPrefs.Save();
        Debug.Log("High Score saved: " + highScore);
    }

    private void LoadHighScore()
    {
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        Debug.Log("High Score loaded: " + highScore);
    }
}