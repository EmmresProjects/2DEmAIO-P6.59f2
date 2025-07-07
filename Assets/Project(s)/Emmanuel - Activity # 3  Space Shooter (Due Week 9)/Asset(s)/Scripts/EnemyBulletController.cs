using UnityEngine;

public class EnemyBulletController : MonoBehaviour
{
    public float bulletSpeed = 5f;      // Modifiable bullet speed

    void Start()
    {
        // Set bullet velocity downward using modifiable speed
        GetComponent<Rigidbody2D>().linearVelocity = new Vector2(0, -bulletSpeed);
    }

    void Update()
    {
        // Destroy bullet if it goes off-screen
        if (transform.position.y < -10f) Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Destroy bullet if it hits the player
        if (other.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}