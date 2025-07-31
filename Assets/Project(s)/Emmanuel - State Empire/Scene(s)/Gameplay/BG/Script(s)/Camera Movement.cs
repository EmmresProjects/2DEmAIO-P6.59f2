using UnityEngine;

public class Camera2DHorizontalControl : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;

    [Header("Horizontal Bounds")]
    public float minX = -10f;
    public float maxX = 10f;

    void Update()
    {
        float inputX = Input.GetAxis("Horizontal"); // Left/Right Arrow or A/D keys

        if (inputX != 0f)
        {
            Vector3 newPos = Camera.main.transform.position;
            newPos.x += inputX * moveSpeed * Time.deltaTime;
            newPos.x = Mathf.Clamp(newPos.x, minX, maxX);
            Camera.main.transform.position = newPos;
        }
    }
}
