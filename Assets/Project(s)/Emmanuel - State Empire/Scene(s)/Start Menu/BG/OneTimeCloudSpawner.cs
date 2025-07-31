using UnityEngine;

public class OneTimeCloudSpawner : MonoBehaviour
{
    [Header("Cloud Prefab")]
    [Tooltip("Must have a CloudMovement component if you want it to move.")]
    public GameObject cloudPrefab;

    [Header("Number of Clouds")]
    [Tooltip("How many clouds to spawn in one batch.")]
    [Min(1)]
    public int cloudCount = 1;

    [Header("Spawn Area (X & Z range)")]
    public Vector3 minSpawnPosition = new Vector3(1f, 0f, 1f);
    public Vector3 maxSpawnPosition = new Vector3(10f, 0f, 10f);

    [Header("Y Position Range")]
    [Tooltip("Random Y between these values.")]
    public float spawnYMin = 2f;
    public float spawnYMax = 5f;

    [Header("Uniform Size Range")]
    [Tooltip("Clouds will be scaled to (n, n, n) with n between MinSize and MaxSize.")]
    public int minSize = 1;
    public int maxSize = 10;

    /// <summary>
    /// Spawns the configured number of clouds at random positions and scales.
    /// </summary>
    public void SpawnClouds()
    {
        if (cloudPrefab == null)
        {
            Debug.LogError("OneTimeCloudSpawner: cloudPrefab not assigned.", this);
            return;
        }

        for (int i = 0; i < cloudCount; i++)
        {
            // Randomize position
            float x = Random.Range(minSpawnPosition.x, maxSpawnPosition.x);
            float y = Random.Range(spawnYMin, spawnYMax);
            float z = Random.Range(minSpawnPosition.z, maxSpawnPosition.z);
            Vector3 spawnPos = new Vector3(x, y, z);

            // Instantiate
            GameObject cloud = Instantiate(cloudPrefab, spawnPos, Quaternion.identity);

            // Apply uniform random scale
            int uniformScale = Random.Range(minSize, maxSize + 1);
            cloud.transform.localScale = Vector3.one * uniformScale;

            // Optional: check for CloudMovement
            var mover = cloud.GetComponent<CloudMovement>();
            if (mover == null)
                Debug.LogWarning("OneTimeCloudSpawner: CloudMovement missing on prefab.", cloud);
        }
    }

    // Auto-spawn on Start
    void Start()
    {
        SpawnClouds();
    }

    // Context menu option in Inspector
    [ContextMenu("Spawn Clouds")]
    private void ContextSpawn() => SpawnClouds();
}
