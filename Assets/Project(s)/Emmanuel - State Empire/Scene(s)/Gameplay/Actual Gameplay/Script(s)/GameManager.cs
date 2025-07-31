using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public Text balanceText;       // Assign legacy Text component in Inspector
    public Text monthText;         // Assign legacy Text component in Inspector
    public int initialBalance = 1000;
    public int monthlyTax = 50;
    public float monthDuration = 30f; // 30 seconds per month

    private int balance;
    private int month = 0;
    private float timer = 0f;
    private List<Building> buildings = new List<Building>(); // Track buildings

    void Start()
    {
        balance = initialBalance;
        UpdateUI();
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= monthDuration)
        {
            timer = 0f;
            month++;
            balance -= monthlyTax;
            // Add income from all buildings
            foreach (Building building in buildings)
            {
                balance += building.GetIncomePerMonth();
            }
            UpdateUI();
        }
    }

    void UpdateUI()
    {
        if (balanceText != null)
            balanceText.text = "Balance: " + balance;
        else
            Debug.LogError("BalanceText not assigned in GameManager!");

        if (monthText != null)
            monthText.text = "Month: " + month;
        else
            Debug.LogError("MonthText not assigned in GameManager!");
    }

    public void DeductCost(int cost)
    {
        balance -= cost;
        UpdateUI();
    }

    public void AddIncome(int income)
    {
        balance += income;
        UpdateUI();
    }

    public int GetBalance()
    {
        return balance;
    }

    public void RegisterBuilding(Building building)
    {
        if (!buildings.Contains(building))
        {
            buildings.Add(building);
        }
    }
}