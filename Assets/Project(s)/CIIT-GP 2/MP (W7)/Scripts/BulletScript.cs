using UnityEngine;

public class BulletScript : MonoBehaviour
{
    public Color bulletColor;

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            SpriteRenderer enemySR = collision.gameObject.GetComponent<SpriteRenderer>();
            if (enemySR.color == bulletColor)
            {
                Destroy(collision.gameObject);
                Destroy(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    void Update()
    {
        if (transform.position.magnitude > 15f) Destroy(gameObject);
    }
}