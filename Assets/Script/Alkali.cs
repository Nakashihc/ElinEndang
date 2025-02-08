using UnityEngine;

public class Alkali : MonoBehaviour
{
    public Transform player;
    public float moveSpeed = 1f;
    public float runSpeed = 2f;
    public float stopDistance = 1f;
    public float rundistance = 3f;

    private Animator animator;
    private Rigidbody2D rb;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        FollowPlayer();
    }

    void FollowPlayer()
    {
        float distance = Vector2.Distance(transform.position, player.position);
        Vector2 direction = (player.position - transform.position).normalized;

        if (distance >= rundistance)
        {
            rb.linearVelocity = new Vector2(direction.x * runSpeed, rb.linearVelocity.y);
            animator.SetBool("Jalan", true);
            animator.SetBool("Idle", false);
        }
        else if (distance > stopDistance)
        {
            rb.linearVelocity = new Vector2(direction.x * moveSpeed, rb.linearVelocity.y);
            animator.SetBool("Jalan", true);
            animator.SetBool("Idle", false);
        }
        else
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            animator.SetBool("Jalan", false);
            animator.SetBool("Idle", true);
        }

        if (player.position.x > transform.position.x)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (player.position.x < transform.position.x)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }
}
