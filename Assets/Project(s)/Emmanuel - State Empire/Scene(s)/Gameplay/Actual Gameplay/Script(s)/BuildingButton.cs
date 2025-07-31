using UnityEngine;
using UnityEngine.UI;

public class BuildingButton : MonoBehaviour
{
    public BuildingPlacement buildingPlacement; // Assign in Inspector
    public GameObject buildingPrefab;           // Assign specific building prefab
    public int cost = 100;                     // Set in Inspector
    public int income = 10;                    // Set in Inspector
    public GameObject highlightImage;          // Assign highlight image

    private Button button;

    void Start()
    {
        button = GetComponent<Button>();
        if (highlightImage != null)
            highlightImage.SetActive(false);
        else
            Debug.LogError($"HighlightImage not assigned in BuildingButton on {gameObject.name}!");

        if (buildingPrefab == null)
            Debug.LogError($"BuildingPrefab not assigned in BuildingButton on {gameObject.name}!");

        if (buildingPlacement == null)
            Debug.LogError($"BuildingPlacement not assigned in BuildingButton on {gameObject.name}!");
    }

    public void OnButtonClick()
    {
        if (buildingPlacement != null && buildingPrefab != null)
        {
            buildingPlacement.StartPlacingBuilding(cost, income, buildingPrefab, this);
            ToggleHighlight(true);
        }
        else
        {
            Debug.LogError($"BuildingPlacement or BuildingPrefab not assigned in BuildingButton on {gameObject.name}!");
        }
    }

    public void ToggleHighlight(bool isSelected)
    {
        if (highlightImage != null)
            highlightImage.SetActive(isSelected);
        else
            Debug.LogError($"HighlightImage not assigned when toggling highlight on {gameObject.name}!");
    }
}