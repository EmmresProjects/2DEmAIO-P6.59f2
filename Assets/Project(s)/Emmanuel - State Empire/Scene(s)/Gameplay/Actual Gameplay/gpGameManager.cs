using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum gpBuildingType { SmallHouse, MediumHouse, BigHouse, Apartment, Shop, Mall, Factory }

public class gpBuildingData
{
    public float cost, rent, maintenance, sellPrice;
    public GameObject prefab; // Added for prefab reference
    public gpBuildingData(float c, float r, float m, float s) { cost = c; rent = r; maintenance = m; sellPrice = s; }
}

public class gpGameManager : MonoBehaviour
{
    public static gpGameManager Instance;
    public GameObject[] buildingPrefabs; // Assign in Inspector
    public float startingMoney, moneyGoal, costMultiplier;
    public float currentMoney, monthlyRent, monthlyExpenses;
    public int currentMonth;
    private List<GameObject> buildings = new List<GameObject>();
    private gpBuildingData[] buildingData;

    void Awake()
    {
        Instance = this;
        buildingData = new gpBuildingData[]
        {
            new gpBuildingData(100f, 20f, 2f, 20f),  // Small House
            new gpBuildingData(200f, 40f, 3f, 40f),  // Medium House
            new gpBuildingData(300f, 60f, 4f, 60f),  // Big House
            new gpBuildingData(500f, 100f, 5f, 100f), // Apartment
            new gpBuildingData(150f, 30f, 2f, 30f),  // Shop
            new gpBuildingData(400f, 80f, 4f, 80f),  // Mall
            new gpBuildingData(250f, 50f, 3f, 50f)   // Factory
        };
        for (int i = 0; i < buildingPrefabs.Length; i++)
        {
            buildingData[i].prefab = buildingPrefabs[i];
        }
        StartCoroutine(MonthlyCycle());
    }

    void Update()
    {
        if (currentMoney >= moneyGoal)
        {
            gpUIManager.Instance.ShowResult(true);
        }
        else if (currentMoney < 0)
        {
            gpUIManager.Instance.ShowResult(false);
        }
    }

    public void SetDifficulty(float startMoney, float goal, float multiplier)
    {
        startingMoney = startMoney;
        moneyGoal = goal;
        costMultiplier = multiplier;
        currentMoney = startingMoney;
        currentMonth = 1;
        foreach (var data in buildingData)
        {
            data.cost *= costMultiplier;
            data.sellPrice = data.cost * 0.2f;
        }
    }

    public gpBuildingData GetBuildingData(int index)
    {
        return buildingData[index];
    }

    public void SpendMoney(float amount)
    {
        currentMoney -= amount;
    }

    public void AddBuilding(GameObject building, gpBuildingType type)
    {
        buildings.Add(building);
        gpBuildingData data = buildingData[(int)type];
        monthlyRent += data.rent;
        monthlyExpenses += data.maintenance;
    }

    public void SellBuilding(GameObject building)
    {
        int index = buildings.IndexOf(building);
        if (index != -1)
        {
            gpBuildingData data = buildingData[(int)building.GetComponent<gpBuilding>().type];
            currentMoney += data.sellPrice;
            monthlyRent -= data.rent;
            monthlyExpenses -= data.maintenance;
            buildings.RemoveAt(index);
            Destroy(building);
            Debug.Log("Building sold. Current money: " + currentMoney);
        }
    }

    IEnumerator MonthlyCycle()
    {
        while (true)
        {
            yield return new WaitForSeconds(30f);
            NextMonth();
        }
    }

    public void NextMonth()
    {
        currentMoney += monthlyRent - monthlyExpenses;
        currentMonth++;
    }
}