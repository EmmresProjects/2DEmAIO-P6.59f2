using UnityEngine;

public class Rotator : MonoBehaviour
{
    public float speed = 50f; // Degrees per second
    private bool isClockwise = true; // Starts clockwise

    void Update()
    {
        if (speed <= 0f)
        {
            Debug.LogWarning("Speed is zero or negative; no rotation.");
            return;
        }

        // Detect mouse click (or touch on mobile) anywhere on screen
        if (Input.GetMouseButtonDown(0))
        {
            SwitchDirection();
        }

        // Positive rotation is counterclockwise in Unity; negative for clockwise
        float direction = isClockwise ? -1f : 1f;
        transform.Rotate(0f, 0f, speed * direction * Time.deltaTime);
    }

    public void SwitchDirection()
    {
        isClockwise = !isClockwise;
    }
}