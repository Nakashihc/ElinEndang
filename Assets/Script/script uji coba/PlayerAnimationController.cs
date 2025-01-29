using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    private Animator anim;
    private PlayerMovements playerMovement;
    private SlidingController slidingController;
    private WallJumpController wallJumpController;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovements>();
        slidingController = GetComponent<SlidingController>();
        wallJumpController = GetComponent<WallJumpController>();
    }

    private void Update()
    {
        UpdateMovementAnimations();
        UpdateSlidingAnimation();
        UpdateWallJumpAnimations();
    }

    private void UpdateMovementAnimations()
    {
        float horizontal = playerMovement.GetHorizontal();
        bool isGrounded = playerMovement.IsGrounded();

        anim.SetBool("isWalking", horizontal != 0 && !Input.GetKey(KeyCode.LeftShift) && isGrounded);
        anim.SetBool("isRunning", horizontal != 0 && Input.GetKey(KeyCode.LeftShift) && isGrounded);
        anim.SetBool("isIdling", horizontal == 0 && isGrounded);
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
}
