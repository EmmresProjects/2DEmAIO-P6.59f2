using UnityEngine;

public class Glide : MonoBehaviour
{
    [Header("Horizontal Movement")]
    [Tooltip("Units per second to drift right")]
    public float speed = 2f;
    [Tooltip("World X at which this object destroys itself")]
    public float despawnX = 12f;

    [Header("Vertical Bobbing")]
    [Tooltip("Amplitude of up/down motion")]
    public float verticalAmplitude = 0.5f;
    [Tooltip("Oscillations per second for bobbing")]
    public float verticalFrequency = 2f;

    [Header("Rotation Sway")]
    [Tooltip("Max tilt angle in degrees")]
    public float rotationAmplitude = 15f;
    [Tooltip("Oscillations per second for swaying")]
    public float rotationFrequency = 2f;

    // Private state
    private float baseY;
    private float baseZ;
    private float timer;

    void Start()
    {
        // Capture initial position for Y and Z
        baseY = transform.position.y;
        baseZ = transform.position.z;
        // Randomize phase for unsynchronized bobbing and swaying
        timer = Random.Range(0f, Mathf.PI * 2f);
    }

    void Update()
    {
        // Advance timer
        timer += Time.deltaTime * Mathf.PI * 2f; // Scale by 2π for proper sine wave frequency

        // 1) Move right
        float newX = transform.position.x + speed * Time.deltaTime;

        // 2) Compute vertical offset
        float newY = baseY + Mathf.Sin(timer * verticalFrequency) * verticalAmplitude;

        // 3) Apply updated position
        transform.position = new Vector3(newX, newY, baseZ);

        // 4) Compute rotation around Z axis
        float angleZ = Mathf.Sin(timer * rotationFrequency) * rotationAmplitude;
        transform.rotation = Quaternion.Euler(0f, 0f, angleZ);

        // 5) Despawn check
        if (newX >= despawnX)
        {
            Destroy(gameObject);
        }
    }
}