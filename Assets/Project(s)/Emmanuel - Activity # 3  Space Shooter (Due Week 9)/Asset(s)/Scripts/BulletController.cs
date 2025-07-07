using UnityEngine;

public class BulletController : MonoBehaviour
{
    public GameObject explosionPrefab;
    public AudioSource explosionSound;

    void Update()
    {
        if (transform.position.y > 10f) Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            if (explosionSound != null) explosionSound.Play();
            ScoreManager.instance.AddScore(100);
            Destroy(other.gameObject);
            Destroy(gameObject);
        }
    }
}