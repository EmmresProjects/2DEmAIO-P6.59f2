using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class gpUIManager : MonoBehaviour
{
    public static gpUIManager Instance;
    public Text moneyText, rentText, expensesText, monthText;
    public GameObject pauseMenu, resultPanel, notEnoughMoneyPanel;
    public Text resultText, notEnoughMoneyText;
    public Button nextMonthButton, mainMenuButton, pauseButton;
    public float notEnoughMoneyDuration = 2f; // Duration to show not enough money panel

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        nextMonthButton.onClick.AddListener(gpGameManager.Instance.NextMonth);
        mainMenuButton.onClick.AddListener(() => SceneManager.LoadScene("StartMenu"));
        pauseButton.onClick.AddListener(TogglePause);
        pauseMenu.SetActive(false);
        resultPanel.SetActive(false);
        notEnoughMoneyPanel.SetActive(false);
    }

    void Update()
    {
        moneyText.text = $"Money: ${gpGameManager.Instance.currentMoney:F0}";
        rentText.text = $"Rent: ${gpGameManager.Instance.monthlyRent:F0}";
        expensesText.text = $"Expenses: ${gpGameManager.Instance.monthlyExpenses:F0}";
        monthText.text = $"Month: {gpGameManager.Instance.currentMonth}";
    }

    public void ShowResult(bool isWin)
    {
        if (isWin)
        {
            resultText.text = $"Congratulations! You reached the goal of ${gpGameManager.Instance.moneyGoal:F0}!";
        }
        else
        {
            resultText.text = "You went into too much debt. Better luck next time!";
        }
        resultPanel.SetActive(true);
        Time.timeScale = 0; // Pause game when showing result
    }

    public void TogglePause()
    {
        pauseMenu.SetActive(!pauseMenu.activeSelf);
        Time.timeScale = pauseMenu.activeSelf ? 0 : 1;
    }

    public void ShowNotEnoughMoney()
    {
        notEnoughMoneyText.text = "Not Enough Money!";
        notEnoughMoneyPanel.SetActive(true);
        Invoke("HideNotEnoughMoney", notEnoughMoneyDuration);
    }

    void HideNotEnoughMoney()
    {
        notEnoughMoneyPanel.SetActive(false);
    }
}