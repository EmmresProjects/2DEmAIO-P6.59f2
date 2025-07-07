using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float enemySpeed = 3f;
    public GameObject enemyBulletPrefab;
    public float fireRate = 2f;
    public float health = 100f; // Enemy health
    public float bulletDamage = 50f; // Damage taken from player bullets
    public float bulletSpeed = 5f; // Modifiable bullet speed

    private float nextFireTime;

    void Start()
    {
        nextFireTime = Time.time + fireRate;
    }

    void Update()
    {
        transform.Translate(Vector2.down * enemySpeed * Time.deltaTime);
        if (Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
        if (transform.position.y < -10f) Destroy(gameObject);
    }

    void Shoot()
    {
        GameObject bullet = Instantiate(enemyBulletPrefab, transform.position, Quaternion.identity);
        EnemyBulletController bulletController = bullet.GetComponent<EnemyBulletController>();
        if (bulletController != null)
        {
            bulletController.bulletSpeed = bulletSpeed;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerBullet"))
        {
            health -= bulletDamage;
            Destroy(other.gameObject);
            if (health <= 0)
            {
                ScoreManager.instance.AddScore(100);
                Destroy(gameObject);
            }
        }
    }
}