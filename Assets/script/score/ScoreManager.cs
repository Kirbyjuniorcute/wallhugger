
using UnityEngine;
using UnityEngine.UI;


public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    [Header("XP Settings")]
    public int xpPerEnemyKill = 10;
    public int xpPerWave = 50;

    [Header("UI")]
    public Text scoreText;

    private int currentScore = 0;

    void Awake()
    {
        // Singleton setup
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        UpdateScoreUI();
    }

    public void AddEnemyKillXP()
    {
        currentScore += xpPerEnemyKill;
        UpdateScoreUI();
    }

    public void AddWaveXP()
    {
        currentScore += xpPerWave;
        UpdateScoreUI();
    }

    void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + currentScore;
        }
    }

    public int GetScore()
    {
        return currentScore;
    }

    public void SaveHighScore()
    {
        int savedHighScore = PlayerPrefs.GetInt("HighScore", 0);
        if (currentScore > savedHighScore)
        {
            PlayerPrefs.SetInt("HighScore", currentScore);
            PlayerPrefs.Save(); // Optional, forces save to disk
            Debug.Log("New High Score Saved: " + currentScore);
        }
    }

}
