using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Text scoreText;

    void Start()
    {
        if (ScoreManager.instance != null)
        {
            ScoreManager.instance.OnScoreChanged += UpdateScoreText;
            UpdateScoreText(ScoreManager.instance.score); // Display initial score
        }
    }

    void UpdateScoreText(int newScore)
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + newScore.ToString();
        }
    }

    void OnDestroy()
    {
        if (ScoreManager.instance != null)
        {
            ScoreManager.instance.OnScoreChanged -= UpdateScoreText;
        }
    }
}