using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("Collectibles (Use + to add multiple prefabs)")]
    public GameObject[] collectiblePrefabs;
    [Header("Obstacles (Use + to add multiple prefabs)")]
    public GameObject[] obstaclePrefabs;

    public float spawnRate = 2f; // Seconds between spawns
    public float collectibleChance = 0.7f; // Probability of spawning collectible vs obstacle

    private float nextSpawnTime = 0f;
    private float screenEdge = 7f; // Spawn at screen edges
    private float yRange = 4f; // Random Y within -4 to 4

    void Update()
    {
        if (Time.time >= nextSpawnTime)
        {
            SpawnObject();
            nextSpawnTime = Time.time + spawnRate;
        }
    }

    void SpawnObject()
    {
        // Randomly choose spawn side
        bool spawnLeft = Random.Range(0f, 1f) < 0.5f;
        float x = spawnLeft ? -screenEdge : screenEdge;
        float y = Random.Range(-yRange, yRange);
        Vector3 spawnPos = new Vector3(x, y, 0);

        // Choose type
        bool isCollectible = Random.Range(0f, 1f) < collectibleChance;

        GameObject prefabToSpawn = null;
        if (isCollectible)
        {
            if (collectiblePrefabs.Length > 0)
            {
                prefabToSpawn = collectiblePrefabs[Random.Range(0, collectiblePrefabs.Length)];
            }
        }
        else
        {
            if (obstaclePrefabs.Length > 0)
            {
                prefabToSpawn = obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)];
            }
        }

        if (prefabToSpawn != null)
        {
            GameObject spawned = Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);
            // Randomize speed
            MovingObject mover = spawned.GetComponent<MovingObject>();
            if (mover != null)
            {
                mover.speed = Random.Range(2f, 5f);
            }
        }
    }
}