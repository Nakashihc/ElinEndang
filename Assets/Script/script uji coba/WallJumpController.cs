using UnityEngine;

public class WallJumpController : MonoBehaviour
{
    [Header("Wall Jump Settings")]
    [SerializeField] private Transform wallCheck;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private Vector2 wallJumpPower = new Vector2(8f, 16f);
    [SerializeField] private float wallSlideSpeed = 2f;
    [SerializeField] private float wallJumpDuration = 0.4f;

    private Rigidbody2D rb;
    private PlayerMovements playerMovement;

    public bool isWallSliding;
    public bool isWallJumping;
    private float wallJumpingDirection;
    private float wallJumpingCounter;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerMovement = GetComponent<PlayerMovements>();
    }

    private void Update()
    {
        HandleWallSlide();
        HandleWallJump();
    }

    private void HandleWallSlide()
    {
        if (IsWalled() && !playerMovement.IsGrounded() && playerMovement.GetHorizontal() != 0)
        {
            isWallSliding = true;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Clamp(rb.linearVelocity.y, -wallSlideSpeed, float.MaxValue));
        }
        else
        {
            isWallSliding = false;
        }
    }

    private void HandleWallJump()
    {
        if (isWallSliding && Input.GetButtonDown("Jump"))
        {
            isWallJumping = true;
            wallJumpingDirection = -Mathf.Sign(transform.localScale.x);
            wallJumpingCounter = wallJumpDuration;

            rb.linearVelocity = new Vector2(wallJumpingDirection * wallJumpPower.x, wallJumpPower.y);

            if (transform.localScale.x * wallJumpingDirection < 0)
            {
                Flip();
            }
        }

        if (isWallJumping)
        {
            wallJumpingCounter -= Time.deltaTime;

            if (wallJumpingCounter <= 0f)
            {
                isWallJumping = false;
                Flip();
            }
        }
    }

    private bool IsWalled()
    {
        return Physics2D.OverlapCircle(wallCheck.position, 0.2f, wallLayer);
    }

    private void Flip()
    {
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }
}
