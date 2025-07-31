using UnityEngine;
using System.Collections;

public class CloudMakerGameplay : MonoBehaviour
{
    [Header("Cloud Prefab")]
    [Tooltip("Must have CloudMovement component.")]
    public GameObject cloudPrefab;

    [Header("Spawn Timing (seconds)")]
    [Tooltip("Minimum time between spawns.")]
    public float minSpawnInterval = 1f;
    [Tooltip("Maximum time between spawns.")]
    public float maxSpawnInterval = 3f;

    [Header("Spawn Position")]
    [Tooltip("X position at which clouds are spawned.")]
    public float spawnX = -10f;
    [Tooltip("Minimum Y position.")]
    public float spawnYMin = -2f;
    [Tooltip("Maximum Y position.")]
    public float spawnYMax = 2f;

    [Header("Cloud Size & Speed")]
    [Tooltip("Fixed local scale (X, Y, Z) for every spawned cloud.")]
    public Vector3 size = Vector3.one;
    [Tooltip("Fixed movement speed for every spawned cloud.")]
    public float speed = 2f;

    void Start()
    {
        if (cloudPrefab == null)
        {
            Debug.LogError("CloudMaker: cloudPrefab not assigned.", this);
            enabled = false;
            return;
        }
        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            // 1) Wait a random interval
            float interval = Random.Range(minSpawnInterval, maxSpawnInterval);
            yield return new WaitForSeconds(interval);

            // 2) Determine spawn position
            float y = Random.Range(spawnYMin, spawnYMax);
            float z = cloudPrefab.transform.position.z;
            Vector3 spawnPos = new Vector3(spawnX, y, z);

            // 3) Instantiate cloud
            GameObject cloud = Instantiate(cloudPrefab, spawnPos, Quaternion.identity);

            // 4) Apply fixed size
            cloud.transform.localScale = size;

            // 5) Apply fixed speed
            var move = cloud.GetComponent<CloudMovement>();
            if (move != null)
                move.speed = speed;
            else
                Debug.LogWarning("CloudMaker: CloudMovement missing on prefab.", cloud);
        }
    }
}
