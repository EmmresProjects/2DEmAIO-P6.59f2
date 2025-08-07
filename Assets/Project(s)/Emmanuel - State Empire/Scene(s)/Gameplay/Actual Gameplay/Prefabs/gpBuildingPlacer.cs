using UnityEngine;

public class gpBuildingPlacer : MonoBehaviour
{
    public static gpBuildingPlacer Instance;
    public GameObject ghostPrefab;
    public Transform pathTransform;
    public AudioSource notEnoughMoneySound; // Assign in Inspector
    public float buildingOffset = 0.5f; // Adjust based on sprite height
    public float buildingWidth = 1f; // Adjust based on building size
    public float minX = -26f; // Customizable left boundary
    public float maxX = 26f;  // Customizable right boundary
    private GameObject ghostInstance;
    private bool isPlacing;
    private gpBuildingType currentType;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (isPlacing)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            float pathY = pathTransform.position.y + buildingOffset;
            ghostInstance.transform.position = new Vector3(Mathf.Clamp(mousePos.x, minX, maxX), pathY, 0);

            if (Input.GetMouseButtonDown(0)) // Left-click to place
            {
                if (IsPlacementValid(ghostInstance.transform.position))
                {
                    PlaceBuilding();
                }
                else
                {
                    Debug.Log("Cannot place: overlap detected");
                }
            }
        }
    }

    public void StartPlacing(gpBuildingType type)
    {
        if (isPlacing) return;
        isPlacing = true;
        currentType = type;
        Vector3 initialPosition = new Vector3(0, pathTransform.position.y + buildingOffset, 0);
        ghostInstance = Instantiate(ghostPrefab, initialPosition, Quaternion.identity);
    }

    public void StopPlacing()
    {
        if (isPlacing)
        {
            Destroy(ghostInstance);
            isPlacing = false;
        }
    }

    bool IsPlacementValid(Vector3 position)
    {
        Collider2D[] colliders = Physics2D.OverlapBoxAll(position, new Vector2(buildingWidth, 0.1f), 0);
        foreach (var collider in colliders)
        {
            if (collider.gameObject != ghostInstance && collider.GetComponent<gpBuilding>() != null)
            {
                return false;
            }
        }
        return true;
    }

    void PlaceBuilding()
    {
        gpBuildingData data = gpGameManager.Instance.GetBuildingData((int)currentType);
        if (gpGameManager.Instance.currentMoney >= data.cost)
        {
            Vector3 placementPosition = new Vector3(
                ghostInstance.transform.position.x,
                pathTransform.position.y + buildingOffset,
                0
            );
            GameObject building = Instantiate(
                gpGameManager.Instance.buildingPrefabs[(int)currentType],
                placementPosition,
                Quaternion.identity
            );
            gpGameManager.Instance.SpendMoney(data.cost);
            gpGameManager.Instance.AddBuilding(building, currentType);
            Destroy(ghostInstance);
            isPlacing = false;
            gpBuildingSelector.Instance.Deselect();
        }
        else
        {
            if (notEnoughMoneySound != null)
            {
                notEnoughMoneySound.Play();
            }
            gpUIManager.Instance.ShowNotEnoughMoney();
        }
    }
}