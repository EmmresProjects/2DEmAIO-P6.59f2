using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PatrolNPC : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 1f;
    public float despawnX = 12f;
    public Vector3 moveDirection = Vector3.right;

    [Header("Timings (seconds)")]
    public float minWalkTime = 1f;
    public float maxWalkTime = 3f;
    public float minIdleTime = 0.5f;
    public float maxIdleTime = 2f;

    [Header("Animator")]
    [SerializeField] private string walkBoolParameter = "isWalking";

    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        StartCoroutine(PatrolRoutine());
    }

    IEnumerator PatrolRoutine()
    {
        while (true)
        {
            // WALK PHASE
            float walkDuration = Random.Range(minWalkTime, maxWalkTime);
            animator.SetBool(walkBoolParameter, true);

            float timer = 0f;
            while (timer < walkDuration)
            {
                transform.Translate(moveDirection * speed * Time.deltaTime);
                timer += Time.deltaTime;

                if (transform.position.x >= despawnX)
                {
                    Destroy(gameObject);
                    yield break;
                }

                yield return null;
            }

            // IDLE PHASE
            float idleDuration = Random.Range(minIdleTime, maxIdleTime);
            animator.SetBool(walkBoolParameter, false);
            yield return new WaitForSeconds(idleDuration);
        }
    }
}
