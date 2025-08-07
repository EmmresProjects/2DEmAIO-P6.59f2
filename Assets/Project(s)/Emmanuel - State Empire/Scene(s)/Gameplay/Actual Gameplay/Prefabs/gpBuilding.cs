using UnityEngine;

public class gpBuilding : MonoBehaviour
{
    public gpBuildingType type;

    void Update()
    {
        if (Input.GetMouseButtonDown(1)) // Right-click to sell
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                Debug.Log("Selling building: " + gameObject.name);
                gpGameManager.Instance.SellBuilding(gameObject);
            }
        }
    }
}