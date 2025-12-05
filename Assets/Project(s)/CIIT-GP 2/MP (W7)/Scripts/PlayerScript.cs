using System.Collections;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public GameObject bulletPrefab;
    public GameObject gameOverUI; // Assign your MainCanvas here in Inspector
    public float shootInterval = 1f;
    public float bulletSpeed = 12f;
    public float detectionRange = 5f; // Increased for testing

    private SpriteRenderer spriteRenderer;
    private SpriteRenderer nozzleRenderer; // New: For nozzle
    private Color[] colors = { Color.red, Color.green, Color.blue };
    private int colorIndex = 0;

    void Start()
    {
        tag = "Player"; // Ensure tag
        spriteRenderer = GetComponent<SpriteRenderer>();
        nozzleRenderer = transform.Find("Nozzle")?.GetComponent<SpriteRenderer>(); // Auto-find child
        SetColor(colors[colorIndex]);
        StartCoroutine(ShootRoutine());
    }

    void Update()
    {
        RotateToNearestEnemy();
    }

    void OnMouseDown()
    {
        colorIndex = (colorIndex + 1) % colors.Length;
        SetColor(colors[colorIndex]);
    }

    void SetColor(Color color)
    {
        spriteRenderer.color = color;
        if (nozzleRenderer != null) nozzleRenderer.color = color; // Tint nozzle too
    }

    IEnumerator ShootRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(shootInterval);
            Shoot();
        }
    }

    void Shoot()
    {
        if (bulletPrefab == null) return;

        // Offset to prevent recoil
        Vector3 offset = transform.up * 0.5f;
        GameObject bullet = Instantiate(bulletPrefab, transform.position + offset, transform.rotation);
        bullet.GetComponent<SpriteRenderer>().color = spriteRenderer.color;
        bullet.GetComponent<Rigidbody2D>().linearVelocity = transform.up * bulletSpeed; // Use velocity for compatibility
        BulletScript bs = bullet.GetComponent<BulletScript>();
        if (bs) bs.bulletColor = spriteRenderer.color;

        // Optional: Flash nozzle brighter on shoot
        if (nozzleRenderer != null)
        {
            StartCoroutine(FlashNozzle());
        }
    }

    // New: Flash coroutine for shoot effect
    IEnumerator FlashNozzle()
    {
        if (nozzleRenderer == null) yield break;
        Color original = nozzleRenderer.color;
        nozzleRenderer.color = Color.white; // Bright flash
        yield return new WaitForSeconds(0.1f);
        nozzleRenderer.color = original;
    }

    void RotateToNearestEnemy()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectionRange);
        GameObject nearest = null;
        float minDist = float.MaxValue;
        int enemyCount = 0;

        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                float dist = Vector2.Distance(transform.position, hit.transform.position);
                if (dist < 0.1f) dist = 0.1f; // Prevent zero-dist overlap glitch
                enemyCount++;
                // Debug.Log("Detected enemy at distance: " + dist); // Optional: Comment out
                if (dist < minDist)
                {
                    minDist = dist;
                    nearest = hit.gameObject;
                }
            }
        }

        if (enemyCount > 0)
        {
            // Debug.Log("Found " + enemyCount + " enemies in range. Rotating to nearest at " + minDist); // Optional
        }

        if (nearest != null)
        {
            Vector2 dir = (nearest.transform.position - transform.position).normalized;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Player hit by enemy! Game Over.");
            if (gameOverUI != null)
            {
                gameOverUI.SetActive(true); // Activate MainCanvas
                // Activate child Panel if it's deactivated
                GameObject panel = gameOverUI.transform.Find("GameOverPanel")?.gameObject;
                if (panel != null)
                {
                    panel.SetActive(true);
                    Debug.Log("GameOverPanel child activated!");
                }
                Debug.Log("GameOver UI activated!");
            }
            else
            {
                Debug.LogError("GameOver UI not assigned in Inspector!");
            }
            Time.timeScale = 0;
        }
    }
}