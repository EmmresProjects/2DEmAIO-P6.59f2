using UnityEngine;
using UnityEngine.UI;

public class Building : MonoBehaviour
{
    public Text costText;      // Assign legacy Text component in Inspector
    public Text incomeText;    // Assign legacy Text component in Inspector
    private int cost;
    private int incomePerMonth;

    void Start()
    {
        UpdateUI();
        // Register this building with GameManager
        GameManager gameManager = FindObjectOfType<GameManager>();
        if (gameManager != null)
        {
            gameManager.RegisterBuilding(this);
        }
        else
        {
            Debug.LogError("GameManager not found!");
        }
    }

    public void SetValues(int newCost, int newIncome)
    {
        cost = newCost;
        incomePerMonth = newIncome;
        UpdateUI();
    }

    public int GetIncomePerMonth()
    {
        return incomePerMonth;
    }

    void UpdateUI()
    {
        if (costText != null)
            costText.text = "Cost: " + cost;
        else
            Debug.LogError($"CostText not assigned in Building on {gameObject.name}!");

        if (incomeText != null)
            incomeText.text = "Income: " + incomePerMonth + "/month";
        else
            Debug.LogError($"IncomeText not assigned in Building on {gameObject.name}!");
    }
}