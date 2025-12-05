using UnityEngine;
using System.Collections.Generic;

public class Package : MonoBehaviour
{
    public enum State { MovingToScanner, AtScanner, MovingToStore, MovingToTrash }

    public List<Transform> path = new List<Transform>();
    public float speed = 1.6f;
    public float yOffset = 0f;
    public string assignedStore = ""; // "Lazada" or "Shopee"
    public State state = State.MovingToScanner;
    public bool wasCorrect = false;

    // scanner wait timer (set when package reaches scanner)
    public float waitTimer = 0f;

    int index = 0;
    bool isMoving = true;

    void Start()
    {
        if (path.Count > 0)
        {
            Vector3 p0 = path[0].position;
            transform.position = new Vector3(p0.x, p0.y + yOffset, p0.z);
        }
    }

    void Update()
    {
        if (!isMoving || path.Count == 0) return;

        // safety clamp
        if (index < 0) index = 0;
        if (index >= path.Count) index = path.Count - 1;

        Transform target = path[index];
        Vector3 targetPos = new Vector3(target.position.x, target.position.y + yOffset, target.position.z);
        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPos) < 0.01f)
        {
            index++;
            if (index >= path.Count)
            {
                isMoving = false;
                if (state == State.MovingToScanner)
                {
                    state = State.AtScanner;
                    GameManager.Instance.PackageReachedScanner(this);
                }
                else if (state == State.MovingToStore)
                {
                    GameManager.Instance.PackageArrivedAtStore(this);
                }
                else if (state == State.MovingToTrash)
                {
                    GameManager.Instance.PackageArrivedAtTrash(this);
                }
            }
        }

        // If waiting at scanner, decrement timer
        if (state == State.AtScanner)
        {
            if (waitTimer > 0f)
            {
                waitTimer -= Time.deltaTime;
                if (waitTimer <= 0f)
                {
                    // timed out -> miss
                    GameManager.Instance.PackageMissedAtScanner(this);
                }
            }
        }
    }

    // newState indicates whether this path is toward scanner or toward store/trash
    public void SetPath(List<Transform> newPath, float newSpeed, float newYOffset, string store, State newState)
    {
        path = new List<Transform>(newPath);
        speed = newSpeed;
        yOffset = newYOffset;
        assignedStore = store;
        state = newState;
        index = 0;
        isMoving = true;

        // If the first waypoint equals current position (common when routing from scanner),
        // advance index so the package moves to the next waypoint immediately.
        if (path.Count > 0)
        {
            Vector3 firstPos = new Vector3(path[0].position.x, path[0].position.y + newYOffset, path[0].position.z);
            if (Vector3.Distance(transform.position, firstPos) < 0.01f)
            {
                index = 1; // skip the first waypoint because we're already there
            }
        }
    }

    public bool IsAtScanner() => state == State.AtScanner;
}
