using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverController : MonoBehaviour
{
    public Text scoreText;

    void Start()
    {
        int score = PlayerPrefs.GetInt("Score", 0);
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score.ToString();
        }
    }

    public void Restart()
    {
        if (ScoreManager.instance != null)
        {
            ScoreManager.instance.ResetScore();
        }
        SceneManager.LoadScene("StartMenu");
    }
}