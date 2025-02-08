using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    [Header("References")]
    private Animator anim;
    private PlayerMovements playerMovement;
    private SlidingController slidingController;
    private WallJumpController wallJumpController;
    private CrouchController crouchController;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovements>();
        slidingController = GetComponent<SlidingController>();
        wallJumpController = GetComponent<WallJumpController>();
        crouchController = GetComponent<CrouchController>();
    }

    private void Update()
    {
        UpdateMovementAnimations();
        UpdateSlidingAnimation();
        UpdateWallJumpAnimations();
        UpdateCrouchAnimation(crouchController.IsCrouching());
    }

    private void UpdateMovementAnimations()
    {
        float horizontal = playerMovement.GetHorizontal();
        bool isGrounded = playerMovement.IsGrounded();

        // Walking and Running
        anim.SetBool("isWalking", horizontal != 0 && !Input.GetKey(KeyCode.LeftShift) && isGrounded);
        anim.SetBool("isRunning", horizontal != 0 && Input.GetKey(KeyCode.LeftShift) && isGrounded);
        anim.SetBool("isIdling", horizontal == 0 && isGrounded);

        // Jumping and Falling
        anim.SetBool("isJumping", !isGrounded && playerMovement.rb.linearVelocity.y > 0);
        anim.SetBool("isFalling", !isGrounded && playerMovement.rb.linearVelocity.y < 0);
    }

    private void UpdateSlidingAnimation()
    {
        anim.SetBool("isSliding", slidingController.isSliding);
    }

    private void UpdateWallJumpAnimations()
    {
        anim.SetBool("isWallSliding", wallJumpController.isWallSliding);

        if (wallJumpController.isWallJumping)
        {
            anim.SetTrigger("isWallJumping");
        }
    }

    public void UpdateCrouchAnimation(bool isCrouching)
    {
        anim.SetBool("isCrouching", isCrouching);
    }
}