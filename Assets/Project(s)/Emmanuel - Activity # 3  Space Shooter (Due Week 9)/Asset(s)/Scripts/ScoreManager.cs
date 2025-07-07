using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;
    public int score = 0;
    public event System.Action<int> OnScoreChanged;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddScore(int points)
    {
        score += points;
        OnScoreChanged?.Invoke(score);
    }

    public void ResetScore()
    {
        score = 0;
        OnScoreChanged?.Invoke(score);
    }
}