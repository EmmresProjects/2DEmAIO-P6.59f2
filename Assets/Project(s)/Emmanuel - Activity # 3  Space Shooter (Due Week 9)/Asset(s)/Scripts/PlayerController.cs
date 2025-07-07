using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    public float health = 100f;
    public float maxHealth = 100f; // Maximum health for regeneration
    public GameObject bulletPrefab;
    public Transform firePoint;
    public Text healthText;
    public GameObject explosionPrefab;
    public AudioSource shootSound;
    public AudioSource explosionSound;

    // HP Regeneration
    public bool regenerateHP = false; // Toggle for regeneration
    public float regenerationAmount = 5f; // HP regenerated per tick
    public float regenerationRate = 1f; // Time between regeneration ticks (seconds)

    // Auto Fire
    public bool autoFire = false; // Toggle for auto fire
    public float fireSpeed = 5f; // Shots per second
    private float nextFireTime = 0f;

    private Rigidbody2D rb;
    private float minX, maxX, minY, maxY;
    private float nextRegenTime;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        CalculateScreenBounds();
        UpdateHealthText();
    }

    void Update()
    {
        MovePlayer();
        if (autoFire)
        {
            if (Input.GetKey(KeyCode.Space) && Time.time >= nextFireTime)
            {
                Shoot();
                nextFireTime = Time.time + 1f / fireSpeed;
            }
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            Shoot();
        }
        if (regenerateHP && Time.time >= nextRegenTime)
        {
            RegenerateHP();
            nextRegenTime = Time.time + regenerationRate;
        }
        if (health <= 0)
        {
            PlayerPrefs.SetInt("Score", ScoreManager.instance.score);
            UnityEngine.SceneManagement.SceneManager.LoadScene("GameOver");
        }
    }

    void MovePlayer()
    {
        float moveX = Input.GetAxisRaw("Horizontal") * speed * Time.deltaTime;
        float moveY = Input.GetAxisRaw("Vertical") * speed * Time.deltaTime;
        Vector2 newPosition = rb.position + new Vector2(moveX, moveY);
        newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
        newPosition.y = Mathf.Clamp(newPosition.y, minY, maxY);
        rb.MovePosition(newPosition);
    }

    void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        bullet.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(0, 10f);
        if (shootSound != null)
        {
            shootSound.Play();
        }
    }

    void RegenerateHP()
    {
        if (health < maxHealth)
        {
            health += regenerationAmount;
            if (health > maxHealth) health = maxHealth;
            UpdateHealthText();
        }
    }

    void CalculateScreenBounds()
    {
        Camera cam = Camera.main;
        Vector2 screenBottomLeft = cam.ScreenToWorldPoint(new Vector3(0, 0, cam.nearClipPlane));
        Vector2 screenTopRight = cam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, cam.nearClipPlane));
        minX = screenBottomLeft.x + 0.5f;
        maxX = screenTopRight.x - 0.5f;
        minY = screenBottomLeft.y + 0.5f;
        maxY = screenTopRight.y - 0.5f;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("EnemyBullet"))
        {
            health -= 10f;
            UpdateHealthText();
            if (explosionPrefab != null)
            {
                Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            }
            if (explosionSound != null)
            {
                explosionSound.Play();
            }
            Destroy(other.gameObject);
        }
    }

    void UpdateHealthText()
    {
        if (healthText != null)
        {
            healthText.text = "HP: " + health.ToString();
        }
    }
}