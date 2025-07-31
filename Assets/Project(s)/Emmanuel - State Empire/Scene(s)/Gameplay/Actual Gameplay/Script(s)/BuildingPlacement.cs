using UnityEngine;

public class BuildingPlacement : MonoBehaviour
{
    public GameObject buildingPreview; // Assign preview sprite in Inspector
    public GameManager gameManager;    // Assign GameManager in Inspector
    private bool isPlacing = false;
    private int buildingCost;
    private int buildingIncome;
    private GameObject currentPrefab;   // Current prefab being placed
    private BuildingButton activeButton; // Track the selected button
    private SpriteRenderer previewRenderer; // For updating preview sprite

    void Start()
    {
        if (buildingPreview != null)
        {
            previewRenderer = buildingPreview.GetComponent<SpriteRenderer>();
            if (previewRenderer == null)
            {
                Debug.LogError("BuildingPreview is missing SpriteRenderer!");
            }
            buildingPreview.SetActive(false);
        }
        else
        {
            Debug.LogError("BuildingPreview not assigned in BuildingPlacement!");
        }

        if (gameManager == null)
        {
            Debug.LogError("GameManager not assigned in BuildingPlacement!");
        }
    }

    public void StartPlacingBuilding(int cost, int income, GameObject prefab, BuildingButton button)
    {
        if (gameManager == null || gameManager.GetBalance() < cost)
        {
            Debug.Log($"Not enough balance ({(gameManager != null ? gameManager.GetBalance() : 0)}) to place {prefab?.name ?? "null"}!");
            if (button != null)
                button.ToggleHighlight(false);
            return;
        }

        // Reset previous button's highlight
        if (activeButton != null && activeButton != button)
        {
            activeButton.ToggleHighlight(false);
        }

        isPlacing = true;
        buildingCost = cost;
        buildingIncome = income;
        currentPrefab = prefab;
        activeButton = button;

        // Update preview sprite
        if (previewRenderer != null && currentPrefab != null)
        {
            SpriteRenderer prefabRenderer = currentPrefab.GetComponent<SpriteRenderer>();
            if (prefabRenderer != null)
            {
                previewRenderer.sprite = prefabRenderer.sprite;
                previewRenderer.color = new Color(0.4f, 0.4f, 0.4f, 0.8f);
                buildingPreview.SetActive(true);
            }
            else
            {
                Debug.LogError($"No SpriteRenderer found on prefab {currentPrefab.name}!");
            }
        }
        else
        {
            Debug.LogError($"PreviewRenderer or currentPrefab is null! PreviewRenderer: {previewRenderer}, currentPrefab: {currentPrefab}");
        }
    }

    void Update()
    {
        if (isPlacing)
        {
            if (buildingPreview == null || !buildingPreview.activeSelf)
            {
                Debug.LogError("BuildingPreview is null or inactive during placement!");
                CancelPlacement();
                return;
            }

            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            buildingPreview.transform.position = mousePos;

            if (Input.GetMouseButtonDown(0))
            {
                if (currentPrefab != null)
                {
                    GameObject newBuilding = Instantiate(currentPrefab, mousePos, Quaternion.identity);
                    Building buildingScript = newBuilding.GetComponent<Building>();
                    if (buildingScript != null)
                    {
                        buildingScript.SetValues(buildingCost, buildingIncome);
                    }
                    else
                    {
                        Debug.LogError($"Building script missing on {currentPrefab.name}!");
                    }
                    gameManager.DeductCost(buildingCost);
                }
                CancelPlacement();
            }
            else if (Input.GetMouseButtonDown(1)) // Right-click to cancel
            {
                CancelPlacement();
            }
        }
    }

    private void CancelPlacement()
    {
        isPlacing = false;
        if (buildingPreview != null)
            buildingPreview.SetActive(false);
        if (activeButton != null)
        {
            activeButton.ToggleHighlight(false);
            activeButton = null;
        }
        currentPrefab = null;
    }
}