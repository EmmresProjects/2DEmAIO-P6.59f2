using UnityEngine;

public class ScannerOrganizer : MonoBehaviour
{
    [Header("Scanner sensor (local offset)")]
    public Vector2 sensorPosition = Vector2.zero; // relative to Scanner transform
    public Vector2 sensorSize = new Vector2(1f, 1f);

    [Header("Scanner settings")]
    public float scannerWaitTime = 1.2f; // how long a package waits at scanner before auto-miss
    public float organizingSpeed = 3.0f; // speed used when routing after input

    // Called by GameManager when a package reaches scanner to set its wait timer
    public void OnPackageAtScanner(Package pkg)
    {
        if (pkg == null) return;
        pkg.waitTimer = scannerWaitTime;
    }

    // Visualize sensor in editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 pos = transform.position + new Vector3(sensorPosition.x, sensorPosition.y, 0f);
        Gizmos.DrawWireCube(pos, new Vector3(sensorSize.x, sensorSize.y, 0.1f));
    }

    // Helper: check if a world position is inside the scanner sensor
    public bool IsInsideSensor(Vector3 worldPos)
    {
        Vector3 center = transform.position + new Vector3(sensorPosition.x, sensorPosition.y, 0f);
        Vector2 half = sensorSize * 0.5f;
        if (worldPos.x < center.x - half.x) return false;
        if (worldPos.x > center.x + half.x) return false;
        if (worldPos.y < center.y - half.y) return false;
        if (worldPos.y > center.y + half.y) return false;
        return true;
    }
}
