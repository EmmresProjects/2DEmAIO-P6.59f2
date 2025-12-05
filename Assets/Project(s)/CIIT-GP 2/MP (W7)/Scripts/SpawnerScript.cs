using UnityEngine;

public class SpawnerScript : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float interval = 3f;
    public float spawnRadius = 8f;

    void Start()
    {
        InvokeRepeating(nameof(Spawn), 1f, interval);
    }

    void Spawn()
    {
        Vector2 pos = Random.insideUnitCircle.normalized * spawnRadius;
        Instantiate(enemyPrefab, pos, Quaternion.identity);
    }
}