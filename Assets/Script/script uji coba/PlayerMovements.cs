using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovements : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 3f;
    public float runSpeed = 6f;
    public float crouchSpeed = 1f;

    [Header("Jump Settings")]
    [SerializeField] private float jumpPower = 10f;
    [SerializeField] private float doubleJumpPower = 7f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;

    public Rigidbody2D rb;
    private SlidingController slidingController;
    private WallJumpController wallJumpController;
    private CrouchController crouchController;

    private float horizontal;
    private bool isFacingRight = true;
    private bool isGrounded;
    private int jumpCount;
    private int maxJumps = 2;

    // Properti untuk kecepatan saat ini
    private float _currentSpeed;
    public float currentSpeed
    {
        get => _currentSpeed;
        set => _currentSpeed = value;
    }
    public float speedAtTheMoment;

    public bool CanMove { get; set; } = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        slidingController = GetComponent<SlidingController>();
        wallJumpController = GetComponent<WallJumpController>();
        crouchController = GetComponent<CrouchController>();

        if (wallJumpController == null)
        {
            Debug.LogError("WallJumpController component is missing on the GameObject.");
        }

        // Inisialisasi kecepatan awal
        _currentSpeed = walkSpeed;
    }

    private void Update()
    {
        if (CanMove && !slidingController.isSliding)
        {
            HandleMovementInput();
            HandleJumpInput();
            
        }
        speedAtTheMoment = _currentSpeed;
        UpdateCurrentSpeed();
    }

    private void FixedUpdate()
    {
        if (!slidingController.isSliding && !wallJumpController.isWallJumping)
        {
            rb.linearVelocity = new Vector2(horizontal * currentSpeed, rb.linearVelocity.y);
        }
    }

    private void HandleMovementInput()
    {
        horizontal = Input.GetAxisRaw("Horizontal");

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            slidingController.StartSlide();
        }

        Flip(horizontal);
    }

    private void HandleJumpInput()
    {
        isGrounded = IsGrounded();

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
    }

    private void UpdateCurrentSpeed()
    {
        if (crouchController.IsCrouching())
        {
            _currentSpeed = crouchSpeed;
        }
        else if (Input.GetKey(KeyCode.LeftShift))
        {
            _currentSpeed = runSpeed;
        }
        else
        {
            _currentSpeed = walkSpeed;
        }
    }
    public bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    private void Flip(float horizontalInput)
    {
        if ((horizontalInput > 0 && !isFacingRight) || (horizontalInput < 0 && isFacingRight))
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    public float GetHorizontal()
    {
        return horizontal;
    }
}