using UnityEngine;

public class Chasing : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 5f;
    public Vector3 moveDirection = Vector3.right;  // Set direction (right by default)
    public float despawnX = 12f;                   // Despawn when X exceeds this value

    void Update()
    {
        // Move the object continuously
        transform.Translate(moveDirection * speed * Time.deltaTime);

        // Check for despawn condition
        if (transform.position.x >= despawnX)
        {
            Destroy(gameObject);
        }
    }
}
