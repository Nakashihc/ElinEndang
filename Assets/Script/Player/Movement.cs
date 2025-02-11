using System.Runtime.CompilerServices;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public Jongkok jongkok;
    public Sliding sliding;
    public bool canmove;    

    [Header("Walk Run Jump")]
    public float currentSpeed;
    public float jalan = 3f;
    public float runningSpeed = 6f;
    public float crouchspeed = 1f;
    [SerializeField] private float jumpingPower = 10f;
    [SerializeField] private float doubleJumpPower = 7f;
    private float horizontal;
    private bool isFacingRight = true;
    public bool canJump;
    public Animator anim;

    [Header("Wall Jump")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private BoxCollider2D atas;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private LayerMask wallLayer;

    private bool isWallSliding;
    [SerializeField] private float wallSlidingSpeed = 2f;

    private bool isWallJumping;
    private float wallJumpingDirection;
    private float wallJumpingTime = 0.2f;
    private float wallJumpingCounter;
    private float wallJumpingDuration = 0.4f;
    [SerializeField] private Vector2 wallJumpingPower = new Vector2(8f, 16f);

    [Header("Sliding")]
    private bool isSliding = false;

    public bool isGrounded = false;
    private int jumpCount = 0;
    private int maxJumps = 2;


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentSpeed = jalan;
    }

    private void Update()
    {
        if (canmove)
        {
            // Cek input horizontal
            horizontal = Input.GetAxisRaw("Horizontal");
            if (jongkok.isCrouching)
            {
                currentSpeed = crouchspeed;
                sliding.canSlide = false;
                anim.SetBool("isCrouching", true);
                anim.SetBool("isRunning", false);
            }
            else
            {
                // Lari
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    currentSpeed = runningSpeed;
                    sliding.canSlide = true; // Sliding diizinkan
                    anim.SetBool("isCrouching", false);
                }
                else
                {
                    // Jalan
                    currentSpeed = jalan;
                    sliding.canSlide = false; // Tidak bisa sliding
                    anim.SetBool("isCrouching", false);
                }
            }

            anim.SetBool("isWalking", horizontal != 0 && currentSpeed == jalan);
            anim.SetBool("isRunning", horizontal != 0 && currentSpeed == runningSpeed);
            anim.SetBool("isIdling", horizontal == 0 && !jongkok.isCrouching && !Input.GetKey(KeyCode.LeftShift));

            isGrounded = IsGrounded();
            anim.SetBool("isFalling", !isGrounded);

            if (canJump)
            {
                if (Input.GetButtonDown("Jump"))
                {
                    if (isGrounded)
                    {
                        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpingPower);
                        jumpCount = 1;
                        anim.SetTrigger("isJumping");
                    }
                    else if (jumpCount < maxJumps)
                    {
                        rb.linearVelocity = new Vector2(rb.linearVelocity.x, doubleJumpPower);
                        jumpCount++;
                    }
                }

                if (Input.GetButtonUp("Jump") && rb.linearVelocity.y > 0f)
                {
                    rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
                }
            }
            
            WallSlide();
            WallJump();

            if (!isWallJumping)
            {
                Flip();
            }
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
            horizontal = 0f;
            anim.SetBool("isWalking", false);
            anim.SetBool("isRunning", false);
            anim.SetBool("isCrouching", false);
            anim.SetBool("isFalling", false);
            anim.SetBool("isIdling", true);
        }
        if (!isGrounded)
        {
            sliding.canSlide = false;
        }
    }

    private void FixedUpdate()
    {
        if (!isWallJumping && !isSliding)
        {
            rb.linearVelocity = new Vector2(horizontal * currentSpeed, rb.linearVelocity.y);
        }
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    private bool IsWalled()
    {
        return Physics2D.OverlapCircle(wallCheck.position, 0.2f, wallLayer);
    }

    private void WallSlide()
    {
        if (IsWalled() && !IsGrounded() && horizontal != 0f)
        {
            anim.SetBool("isHang", true);
            isWallSliding = true;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Clamp(rb.linearVelocity.y, -wallSlidingSpeed, float.MaxValue));
        }
        else
        {
            isWallSliding = false;
            anim.SetBool("isHang", false);
        }
    }

    private void WallJump()
    {
        if (isWallSliding)
        {
            isWallJumping = false;
            wallJumpingDirection = -transform.localScale.x;
            wallJumpingCounter = wallJumpingTime;

            CancelInvoke(nameof(StopWallJumping));
        }
        else
        {
            wallJumpingCounter -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump") && wallJumpingCounter > 0f)
        {
            isWallJumping = true;
            rb.linearVelocity = new Vector2(wallJumpingDirection * wallJumpingPower.x, wallJumpingPower.y);
            wallJumpingCounter = 0f;

            if (transform.localScale.x != wallJumpingDirection)
            {
                isFacingRight = !isFacingRight;
                Vector3 localScale = transform.localScale;
                localScale.x *= -1f;
                transform.localScale = localScale;
            }

            Invoke(nameof(StopWallJumping), wallJumpingDuration);
        }
    }

    private void StopWallJumping()
    {
        isWallJumping = false;
    }

    private void Flip()
    {
        if (isFacingRight && horizontal < 0f || !isFacingRight && horizontal > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & groundLayer) != 0)
        {
            isGrounded = true;
            jumpCount = 0;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & groundLayer) != 0)
        {
            isGrounded = false;
        }
    }

    public float GetHorizontal()
    {
        return horizontal;
    }

    public void DisableCollider()
    {
        atas.enabled = false;
    }

    public void Idle()
    {
        anim.SetBool("isWalking", false);
        anim.SetBool("isRunning", false);
        anim.SetBool("isCrouching", false);
        anim.SetBool("isFalling", false);
        anim.SetBool("isIdling", true);
    }

    public void CanJumpp()
    {
        canJump = true;
    }

    public void CantJumpp()
    {
        canJump = false;
    }
}
