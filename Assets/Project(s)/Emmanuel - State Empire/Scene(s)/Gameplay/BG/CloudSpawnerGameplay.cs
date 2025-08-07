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

    private Transform cloudParent;

    void Start()
    {
        if (cloudPrefab == null)
        {
            Debug.LogError("CloudMaker: cloudPrefab not assigned.", this);
            enabled = false;
            return;
        }

        // Find or create Prefabs/Clouds parent
        GameObject prefabsRoot = GameObject.Find("Prefabs");
        if (prefabsRoot == null)
            prefabsRoot = new GameObject("Prefabs");

        Transform clouds = prefabsRoot.transform.Find("Clouds");
        if (clouds == null)
        {
            GameObject cloudsObj = new GameObject("Clouds");
            cloudsObj.transform.SetParent(prefabsRoot.transform);
            clouds = cloudsObj.transform;
        }

        cloudParent = clouds;

        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            float interval = Random.Range(minSpawnInterval, maxSpawnInterval);
            yield return new WaitForSeconds(interval);

            float y = Random.Range(spawnYMin, spawnYMax);
            float z = cloudPrefab.transform.position.z;
            Vector3 spawnPos = new Vector3(spawnX, y, z);

            GameObject cloud = Instantiate(cloudPrefab, spawnPos, Quaternion.identity, cloudParent);
            cloud.transform.localScale = size;

            var move = cloud.GetComponent<CloudMovement>();
            if (move != null)
                move.speed = speed;
            else
                Debug.LogWarning("CloudMaker: CloudMovement missing on prefab.", cloud);
        }
    }
}
