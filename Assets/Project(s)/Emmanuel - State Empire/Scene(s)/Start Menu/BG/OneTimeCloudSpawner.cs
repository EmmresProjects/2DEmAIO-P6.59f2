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

    private Transform cloudParent;

    void Start()
    {
        SetupParent();
        SpawnClouds();
    }

    [ContextMenu("Spawn Clouds")]
    private void ContextSpawn()
    {
        SetupParent();
        SpawnClouds();
    }

    private void SetupParent()
    {
        GameObject prefabsRoot = GameObject.Find("Prefabs");
        if (prefabsRoot == null)
            prefabsRoot = new GameObject("Prefabs");

        Transform oneTimeClouds = prefabsRoot.transform.Find("OneTimeClouds");
        if (oneTimeClouds == null)
        {
            GameObject cloudsObj = new GameObject("OneTimeClouds");
            cloudsObj.transform.SetParent(prefabsRoot.transform);
            oneTimeClouds = cloudsObj.transform;
        }

        cloudParent = oneTimeClouds;
    }

    public void SpawnClouds()
    {
        if (cloudPrefab == null)
        {
            Debug.LogError("OneTimeCloudSpawner: cloudPrefab not assigned.", this);
            return;
        }

        for (int i = 0; i < cloudCount; i++)
        {
            float x = Random.Range(minSpawnPosition.x, maxSpawnPosition.x);
            float y = Random.Range(spawnYMin, spawnYMax);
            float z = Random.Range(minSpawnPosition.z, maxSpawnPosition.z);
            Vector3 spawnPos = new Vector3(x, y, z);

            GameObject cloud = Instantiate(cloudPrefab, spawnPos, Quaternion.identity, cloudParent);

            int uniformScale = Random.Range(minSize, maxSize + 1);
            cloud.transform.localScale = Vector3.one * uniformScale;

            var mover = cloud.GetComponent<CloudMovement>();
            if (mover == null)
                Debug.LogWarning("OneTimeCloudSpawner: CloudMovement missing on prefab.", cloud);
        }
    }
}
