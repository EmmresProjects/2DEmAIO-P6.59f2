using UnityEngine;

// Attach this to the Scanner GameObject. Add a BoxCollider2D (isTrigger = true) sized to the sensor.
// The package prefab must have a Collider2D (not trigger) and a Rigidbody2D (Body Type = Kinematic).
[RequireComponent(typeof(Collider2D))]
public class ScannerTrigger : MonoBehaviour
{
    public ScannerOrganizer scannerOrganizer; // assign the ScannerOrganizer component (same GameObject)
    public GameManager gameManager;           // assign GameManager
    public string packageTag = "Package";     // tag your package prefabs "Package"

    void Reset()
    {
        var col = GetComponent<Collider2D>();
        if (col != null) col.isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other == null) return;
        if (!other.gameObject.CompareTag(packageTag)) return;

        var pkg = other.GetComponent<Package>();
        if (pkg == null) return;

        // Mark package as at scanner and let ScannerOrganizer set wait timer
        pkg.state = Package.State.AtScanner;
        if (scannerOrganizer != null) scannerOrganizer.OnPackageAtScanner(pkg);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other == null) return;
        if (!other.gameObject.CompareTag(packageTag)) return;

        var pkg = other.GetComponent<Package>();
        if (pkg == null) return;

        // If the package is still AtScanner when it exits the sensor, treat as missed
        if (pkg.IsAtScanner())
        {
            if (gameManager != null) gameManager.PackageMissedAtScanner(pkg);
        }
    }
}
