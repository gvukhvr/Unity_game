using TMPro;
using UnityEngine;

public class HighScoreUI : MonoBehaviour
{
    public TMP_Text text;

    void Start()
    {
        int high = PlayerPrefs.GetInt("HighScore", 0);
        text.text = "High Score: " + high;
    }
}
