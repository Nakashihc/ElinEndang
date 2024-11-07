using System.Runtime.CompilerServices;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [Header("Walk Run Jump")]
    [SerializeField] private float baseSpeed = 3f;
    [SerializeField] private float runningSpeed = 6f;
    [SerializeField] private float jumpingPower = 10f;
    [SerializeField] private float doubleJumpPower = 7f;
    private float horizontal;
    private float speed;
    private bool isFacingRight = true;

    public Animator anim;

    [Header("Wall Jump")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private BoxCollider2D atas;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private LayerMask wallLayer;

    private bool isWallSliding;
    private float wallSlidingSpeed = 2f;

    private bool isWallJumping;
    private float wallJumpingDirection;
    private float wallJumpingTime = 0.2f;
    private float wallJumpingCounter;
    private float wallJumpingDuration = 0.4f;
    private Vector2 wallJumpingPower = new Vector2(8f, 16f);

    [Header("Sliding")]
    public float slideSpeed = 10f;
    public float slideDuration = 0.5f;
    private bool isSliding = false;
    private float slideTimer = 0f;
    private Vector3 slideStartPos;
    private Vector3 slideEndPos;
    [SerializeField] private LayerMask wallSlideLayer; // Layer for wall collision during slide

    // Energy variables
    [SerializeField] private float maxEnergy = 100f; // Maximum energy
    [SerializeField] private float energyCost = 20f; // Energy cost per slide
    private float currentEnergy;
    [SerializeField] private float energyRegenRate = 5f; // Rate of energy regeneration per second

    // Variables for jump logic
    private bool isGrounded = false;
    private int jumpCount = 0;
    private int maxJumps = 2; // Allowing double jump


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentEnergy = maxEnergy; // Set energy to max at the start
    }

    private void Update()
    {
        // Set speed based on Shift key input
        speed = Input.GetKey(KeyCode.LeftShift) ? runningSpeed : baseSpeed;

        horizontal = Input.GetAxisRaw("Horizontal");

        // Update animation states based on movement and speed
        anim.SetBool("isWalking", horizontal != 0 && speed == baseSpeed);
        anim.SetBool("isRunning", horizontal != 0 && speed == runningSpeed);
        anim.SetBool("isIdling", horizontal == 0);

        // Check if grounded and set "Air" animation
        isGrounded = IsGrounded();
        anim.SetBool("isFalling", !isGrounded);

        // Jump logic
        if (Input.GetButtonDown("Jump"))
        {
            if (isGrounded)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
                jumpCount = 1;
                anim.SetTrigger("isJumping");
                anim.SetBool("isIdling", false);
            }
            else if (jumpCount < maxJumps)
            {
                rb.velocity = new Vector2(rb.velocity.x, doubleJumpPower);
                jumpCount++;
                anim.SetBool("isJumping", false);
            }
        }

        // Reduce jump height if jump button is released
        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
        }

        // Slide logic with energy check
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.C) && !isSliding && horizontal != 0 && currentEnergy >= energyCost)
        {
            StartSlide();
        }

        // Process slide
        ProcessSlide();

        // Regenerate energy over time if it's below maxEnergy
        if (currentEnergy < maxEnergy && !isSliding)
        {
            currentEnergy += energyRegenRate * Time.deltaTime;
            currentEnergy = Mathf.Min(currentEnergy, maxEnergy); // Ensure energy doesn't exceed max
        }

        WallSlide();
        WallJump();

        if (!isWallJumping)
        {
            Flip();
        }

    }

    private void FixedUpdate()
    {
        // Move character only when not wall jumping or sliding
        if (!isWallJumping && !isSliding)
        {
            rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
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
            isWallSliding = true;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlidingSpeed, float.MaxValue));
        }
        else
        {
            isWallSliding = false;
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
            rb.velocity = new Vector2(wallJumpingDirection * wallJumpingPower.x, wallJumpingPower.y);
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

    private void StartSlide()
    {
        isSliding = true;
        slideTimer = 0f;
        atas.enabled = false;

        // Deduct energy for sliding
        currentEnergy -= energyCost;

        slideStartPos = transform.position;
        slideEndPos = slideStartPos + transform.right * horizontal * slideSpeed;
    }

    private void ProcessSlide()
    {
        if (isSliding)
        {
            slideTimer += Time.deltaTime;

            // Use BoxCast to detect collision with wall layer during slide
            BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
            if (boxCollider != null)
            {
                RaycastHit2D hit = Physics2D.BoxCast(
                    boxCollider.bounds.center,
                    boxCollider.bounds.size,
                    0f,
                    Vector2.right * Mathf.Sign(slideEndPos.x - slideStartPos.x),
                    0.2f,
                    wallSlideLayer
                );

                if (hit.collider != null)
                {
                    isSliding = false; // Stop sliding if a wall is detected
                    return;
                }
            }

            // Interpolate position for slide
            transform.position = new Vector3(
                Mathf.Lerp(slideStartPos.x, slideEndPos.x, slideTimer / slideDuration),
                transform.position.y,
                transform.position.z
            );

            // Reset sliding if slide duration has elapsed
            if (slideTimer >= slideDuration)
            {
                isSliding = false;
                atas.enabled = true;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & groundLayer) != 0)
        {
            isGrounded = true;
            jumpCount = 0; // Reset jump count when grounded
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & groundLayer) != 0)
        {
            isGrounded = false;
        }
    }

    // Method to get current energy for UI display
    public float GetCurrentEnergy()
    {
        return currentEnergy;
    }
}
