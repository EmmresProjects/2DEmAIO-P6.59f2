using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public float speed = 2f;
    private Transform player;
    private SpriteRenderer sr;
    private Color[] colors = { Color.red, Color.green, Color.blue };

    void Start()
    {
        tag = "Enemy"; // Set tag immediately
        player = GameObject.FindWithTag("Player")?.transform;
        if (player == null)
        {
            Debug.LogError("Enemy can't find Player! Check Player tag.");
            return;
        }
        sr = GetComponent<SpriteRenderer>();
        if (sr == null)
        {
            Debug.LogError("Enemy missing SpriteRenderer!");
            return;
        }
        sr.color = colors[Random.Range(0, 3)];
        // Debug.Log("Enemy spawned with tag 'Enemy' and color: " + sr.color); // Optional: Comment out for clean Console
    }

    void Update()
    {
        if (player)
        {
            float dist = Vector2.Distance(transform.position, player.position);
            if (dist > 0.6f) // Stop just before overlap
            {
                Vector2 dir = (player.position - transform.position).normalized;
                transform.position = Vector3.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
            }
        }
    }
}