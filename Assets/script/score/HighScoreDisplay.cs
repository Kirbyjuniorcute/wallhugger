using UnityEngine;
using UnityEngine.UI;


public class HighScoreDisplay : MonoBehaviour
{
    public Text highScoreText; // Assign in Inspector

    void Start()
    {
        int highScore = PlayerPrefs.GetInt("HighScore", 0);
        if (highScoreText != null)
        {
            highScoreText.text = "High Score: " + highScore;
        }
    }
}
