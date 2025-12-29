using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using TMPro;

public class GameOverUI : MonoBehaviour
{
    [Header("Panels")]
    public GameObject gameOverPanel;
    public GameObject settingsPanel;

    [Header("Game UI (для скрытия)")]
    public GameObject gameUI; 
    public TMP_Text gameScoreText; 
    public TMP_Text gameHighScoreText; 
    public TMP_Text gameLevelText; 
    public TMP_Text gameMultiplierText; 

    [Header("Game Over UI")]
    public TMP_Text finalScoreText;
    public TMP_Text highScoreText;
    public TMP_Text newRecordText;

    private void Start()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(true);

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        if (newRecordText != null)
            newRecordText.gameObject.SetActive(false);

        
        ShowGameUI(true);

        Time.timeScale = 1f;
    }

    public void ShowGameOver()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        ShowGameUI(false);

    
        DisplayScores();

        EventSystem.current.SetSelectedGameObject(null);
        Time.timeScale = 0f;
    }

    private void ShowGameUI(bool show)
    {
        
        if (gameUI != null)
        {
            gameUI.SetActive(show);
        }

       
        if (gameScoreText != null)
            gameScoreText.gameObject.SetActive(show);

        if (gameHighScoreText != null)
            gameHighScoreText.gameObject.SetActive(show);

        if (gameLevelText != null)
            gameLevelText.gameObject.SetActive(show);

        if (gameMultiplierText != null)
            gameMultiplierText.gameObject.SetActive(show);
    }

    private void DisplayScores()
    {
        if (ScoreManager.Instance == null) return;

        int finalScore = ScoreManager.Instance.score;
        int highScore = ScoreManager.Instance.GetHighScore();

       
        if (finalScoreText != null)
            finalScoreText.text = "Score: " + finalScore.ToString();

 
        if (highScoreText != null)
            highScoreText.text = "Best: " + highScore.ToString();

 
        if (newRecordText != null && finalScore >= highScore && finalScore > 0)
        {
            newRecordText.gameObject.SetActive(true);
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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
        Debug.Log("Quit Game");
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}