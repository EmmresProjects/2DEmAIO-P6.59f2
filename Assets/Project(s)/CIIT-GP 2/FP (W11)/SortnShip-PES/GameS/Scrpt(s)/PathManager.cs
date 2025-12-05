using UnityEngine;

public class PathManager : MonoBehaviour
{
    [Header("Main path waypoints (0 = spawner ... last = scanner)")]
    public Transform[] mainPath; // WP0_Spawner, WP1_A, WP2_B, WP3_Scanner

    [Header("Package movement defaults")]
    public float packageSpeed = 1.6f;
    public float yOffset = 0f;

    [Header("Organizing (routing)")]
    public float organizeSpeed = 3.0f; // speed used when routing to store or trash

    [Header("Trash")]
    public Transform trashTarget; // assign TrashBin transform
}
