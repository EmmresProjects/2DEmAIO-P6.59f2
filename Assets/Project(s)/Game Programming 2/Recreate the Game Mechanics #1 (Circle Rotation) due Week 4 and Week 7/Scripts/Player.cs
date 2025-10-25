using UnityEngine;
using TMPro;

public class Player : MonoBehaviour
{
    [Header("Game Over UI (Assign from MainCanvas)")]
    public GameObject gameOverPanel;
    public TextMeshProUGUI gameOverText;

    private bool isGameOver = false;

    private void Start()
    {
        // Hide panel at start
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Collectible hit
        if (other.CompareTag("Collectible"))
        {
            ScoreManager.Instance.AddScore(10);
            Destroy(other.gameObject);
            Debug.Log("Collected! Score up.");
        }

        // Obstacle hit
        if (other.CompareTag("Obstacle"))
        {
            if (!isGameOver)
            {
                GameOver();
            }
        }
    }

    private void GameOver()
    {
        isGameOver = true;

        // Update text
        if (gameOverText != null)
        {
            gameOverText.text = "Game Over!\nFinal Score: " + ScoreManager.Instance.GetScore();
        }

        // Show panel (from MainCanvas)
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        // Pause game
        Time.timeScale = 0f;

        Debug.Log("Game Over. Final Score: " + ScoreManager.Instance.GetScore());
    }
}