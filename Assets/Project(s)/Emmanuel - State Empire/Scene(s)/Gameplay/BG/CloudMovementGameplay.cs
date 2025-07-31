using UnityEngine;

public class CloudMovementGameplay : MonoBehaviour
{
    [Header("Movement Speed")]
    [Tooltip("Units per second to move right.")]
    public float speed = 2f;

    [Header("Despawn")]
    [Tooltip("World X position at which this cloud is destroyed.")]
    public float despawnX = 12f;

    void Update()
    {
        // Drift right
        transform.position += Vector3.right * speed * Time.deltaTime;

        // Despawn when past boundary
        if (transform.position.x >= despawnX)
            Destroy(gameObject);
    }
}
