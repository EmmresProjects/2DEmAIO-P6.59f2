using System.Collections;
using UnityEngine;

public class ChaseSpawner : MonoBehaviour
{
    [Header("Prefab & Spawn Area")]
    public GameObject npcPrefab;
    public float spawnXMin = -11f;
    public float spawnXMax = -9.8f;
    public float spawnY = -2.73f;

    [Header("Spawn Timing")]
    public bool randomInterval = true;
    public float minInterval = 1f;
    public float maxInterval = 20f;
    public float spawnInterval = 5f;  // Used when randomInterval is false

    void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            float waitTime = randomInterval
                ? Random.Range(minInterval, maxInterval)
                : spawnInterval;

            yield return new WaitForSeconds(waitTime);

            Vector3 spawnPosition = new Vector3(
                Random.Range(spawnXMin, spawnXMax),
                spawnY,
                0f
            );

            Instantiate(npcPrefab, spawnPosition, Quaternion.identity);
        }
    }
}
