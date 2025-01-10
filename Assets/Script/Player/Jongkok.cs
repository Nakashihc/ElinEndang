using UnityEngine;

public class Jongkok : MonoBehaviour
{
    public Sliding sliding;
    public Movement mov;
    public bool isCrouching;
    public bool isRunning;
    public BoxCollider2D headCollider;
    public BoxCollider2D atasCollider;

    private bool isStructureAbove;

    private Rigidbody2D rb;
    private Movement movement;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        movement = GetComponent<Movement>();
    }

    private void Update()
    {
        if (mov.isGrounded)
        {
            // Sliding Logic
            if (sliding.canSlide)
            {
                if (Input.GetKeyDown(KeyCode.C) && isRunning)
                {
                    sliding.StartSlide();
                }
            }

            if (isCrouching && !isRunning)
            {
                mov.anim.SetBool("isCrouching", true);
                UpdateColliderState(false);
            }
            else if (!isCrouching && !isRunning)
            {
                UpdateColliderState(true);
                mov.anim.SetBool("isCrouching", false);
            }

            // Crouch Logic
            if (Input.GetKeyDown(KeyCode.C) && !isRunning && !isCrouching)
            {
                isCrouching = true;
                movement.currentSpeed = movement.jalan * movement.crouchspeed;
            }
            else if (Input.GetKeyUp(KeyCode.C) || isRunning)
            {
                isCrouching = false;
                movement.currentSpeed = movement.jalan;
            }

            // Update speed for running
            if (sliding.canSlide && !isCrouching)
            {
                isRunning = true;
                movement.currentSpeed = movement.jalan * movement.runningSpeed;
            }
            else if (!sliding.canSlide)
            {
                isRunning = false;
                if (!isCrouching)
                    movement.currentSpeed = movement.jalan;
            }

            if (isStructureAbove)
            {
                isCrouching = true;
                UpdateColliderState(false);
            }
        }
    }

    public void UpdateColliderState(bool isStanding)
    {
        atasCollider.enabled = isStanding;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Structure"))
        {
            isStructureAbove = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Structure"))
        {
            isStructureAbove = false;
            if (!Input.GetKey(KeyCode.C))
            {
                isCrouching = false;
                UpdateColliderState(true);
            }
        }
    }
}
