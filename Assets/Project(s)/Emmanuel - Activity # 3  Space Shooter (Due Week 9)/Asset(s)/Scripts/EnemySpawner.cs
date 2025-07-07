using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;      // Enemy prefab to spawn
    public float spawnInterval = 1f;    // Modifiable spawn interval in seconds
    public float minSpawnX = -5f;       // Minimum X position for spawn
    public float maxSpawnX = 5f;        // Maximum X position for spawn

    void Start()
    {
        // Start spawning enemies immediately and repeat at spawnInterval
        InvokeRepeating("SpawnEnemy", 0f, spawnInterval);
    }

    void SpawnEnemy()
    {
        // Generate random X position within defined range
        float spawnX = Random.Range(minSpawnX, maxSpawnX);
        Vector2 spawnPosition = new Vector2(spawnX, 10f); // Spawn from top of screen
        Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
    }
}