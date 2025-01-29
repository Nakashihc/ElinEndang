using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovements : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 3f;
    public float runSpeed = 6f;
    public float crouchSpeed = 1f;
    [SerializeField] private float jumpPower = 10f;
    [SerializeField] private float doubleJumpPower = 7f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;

    public Rigidbody2D rb;
    private Animator anim;
    private SlidingController slidingController;
    private WallJumpController wallJumpController;

    private float horizontal;
    private bool isFacingRight = true;
    private bool isGrounded;
    private int jumpCount;
    private int maxJumps = 2;

    public bool CanMove { get; set; } = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        slidingController = GetComponent<SlidingController>();
        wallJumpController = GetComponent<WallJumpController>();

        if (wallJumpController == null)
        {
            Debug.LogError("WallJumpController component is missing on the GameObject.");
        }
    }

    private void Update()
    {
        if (CanMove && !slidingController.isSliding)
        {
            HandleMovementInput();
            HandleJumpInput();
        }
        else
        {
            ResetAnimatorStates();
        }
    }

    private void FixedUpdate()
    {
        if (!slidingController.isSliding && !wallJumpController.isWallJumping)
        {
            rb.linearVelocity = new Vector2(horizontal * GetCurrentSpeed(), rb.linearVelocity.y);
        }
    }

    private void HandleMovementInput()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        anim.SetBool("isWalking", horizontal != 0 && GetCurrentSpeed() == walkSpeed);
        anim.SetBool("isRunning", horizontal != 0 && GetCurrentSpeed() == runSpeed);
        anim.SetBool("isIdling", horizontal == 0);

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            slidingController.StartSlide();
        }

        Flip(horizontal);
    }

    private void HandleJumpInput()
    {
        isGrounded = IsGrounded();
        anim.SetBool("isGrounded", isGrounded);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isGrounded)
            {
                Jump(jumpPower);
                jumpCount = 1;
            }
            else if (jumpCount < maxJumps)
            {
                Jump(doubleJumpPower);
                jumpCount++;
            }
        }

        if (Input.GetKeyDown(KeyCode.Space) && rb.linearVelocity.y > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
        }
    }

    private void Jump(float power)
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, power);
        anim.SetTrigger("isJumping");
    }

    private float GetCurrentSpeed()
    {
        if (Input.GetKey(KeyCode.LeftShift)) return runSpeed;
        if (Input.GetKey(KeyCode.LeftControl)) return crouchSpeed;
        return walkSpeed;
    }

    public bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    private void Flip(float horizontalInput)
    {
        if (horizontalInput > 0 && !isFacingRight || horizontalInput < 0 && isFacingRight)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    private void ResetAnimatorStates()
    {
        anim.SetBool("isWalking", false);
        anim.SetBool("isRunning", false);
        anim.SetBool("isIdling", true);
        anim.SetBool("isGrounded", false);
    }

    public float GetHorizontal() => horizontal;
}
