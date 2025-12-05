using UnityEngine;

public class MovingObject : MonoBehaviour
{
    public float speed = 3f;
    private Vector2 direction;

    void Start()
    {
        // Decide direction based on spawn side
        if (transform.position.x < 0)
        {
            direction = Vector2.right; // Move right if spawned left
        }
        else
        {
            direction = Vector2.left; // Move left if spawned right
        }

        // Auto-destroy after 10 seconds as backup
        Destroy(gameObject, 10f);
    }

    void Update()
    {
        // Move horizontally
        transform.Translate(direction * speed * Time.deltaTime, Space.World);

        // Destroy if out of bounds (beyond screen edges)
        if (Mathf.Abs(transform.position.x) > 8f)
        {
            Destroy(gameObject);
        }
    }
}