using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    public Animator animator;
    public float horizontal;
    public float vertical;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        animator.SetBool("Forward", vertical > 0);
        animator.SetBool("Backward", vertical < 0);
        animator.SetBool("Right", horizontal > 0);
        animator.SetBool("Left", horizontal < 0);

        if (Input.GetButtonDown("Jump"))
        {
            animator.SetTrigger("Jump");
        }
    }
}