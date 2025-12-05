// Spawner.cs
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class StorePrefab
{
    public string storeName;
    public GameObject prefab;
}

public class Spawner : MonoBehaviour
{
    public PathManager pathManager;
    public float spawnInterval = 1.2f;
    public Transform[] spawnPoints; // optional
    public StorePrefab[] storePrefabs; // map store name -> prefab
    public GameObject defaultPackagePrefab;

    bool running = false;
    Coroutine spawnCoroutine = null;

    void Start()
    {
        if (pathManager == null) pathManager = FindObjectOfType<PathManager>();
    }

    /// <summary>
    /// Start the spawn loop if not already running.
    /// </summary>
    public void StartSpawning()
    {
        if (running) return;
        running = true;
        if (spawnCoroutine != null) StopCoroutine(spawnCoroutine);
        spawnCoroutine = StartCoroutine(SpawnLoop());
        Debug.Log("Spawner: StartSpawning called");
    }

    /// <summary>
    /// Stop the spawn loop completely.
    /// </summary>
    public void StopSpawning()
    {
        if (!running) return;
        running = false;
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
        Debug.Log("Spawner: StopSpawning called");
    }

    IEnumerator SpawnLoop()
    {
        // Use scaled time so pausing the game (Time.timeScale = 0) will automatically pause spawning.
        while (running)
        {
            SpawnOne();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnOne()
    {
        if (pathManager == null || pathManager.mainPath == null || pathManager.mainPath.Length == 0)
        {
            Debug.LogWarning("Spawner: pathManager or mainPath not assigned or empty.");
            return;
        }

        Vector3 pos = (spawnPoints != null && spawnPoints.Length > 0) ? spawnPoints[0].position : pathManager.mainPath[0].position;
        if (GameManager.Instance == null)
        {
            Debug.LogWarning("Spawner: GameManager.Instance is null.");
            return;
        }

        string assignedStore = GameManager.Instance.GetNextAssignedStore();

        GameObject prefab = GetPrefabForStore(assignedStore) ?? defaultPackagePrefab;
        if (prefab == null)
        {
            Debug.LogWarning("Spawner: no prefab for store " + assignedStore);
            return;
        }

        GameObject go = Instantiate(prefab, pos, Quaternion.identity);
        Package pkg = go.GetComponent<Package>();
        if (pkg == null)
        {
            Debug.LogWarning("Spawner: prefab missing Package script");
            Destroy(go);
            return;
        }

        List<Transform> path = new List<Transform>(pathManager.mainPath);
        pkg.SetPath(path, pathManager.packageSpeed, pathManager.yOffset, assignedStore, Package.State.MovingToScanner);
        GameManager.Instance.RegisterSpawnedPackage(pkg);
    }

    GameObject GetPrefabForStore(string storeName)
    {
        if (string.IsNullOrEmpty(storeName)) return null;
        for (int i = 0; i < storePrefabs.Length; i++)
            if (storePrefabs[i] != null && storePrefabs[i].storeName == storeName)
                return storePrefabs[i].prefab;
        return null;
    }
}
