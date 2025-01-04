using System.Data.Common;
using UnityEngine;

public class WallClimb : MonoBehaviour
{
    [SerializeField] private Transform wallCheck;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private Rigidbody2D rb;
    public Movement movement;
    public Stamina stamina;
    private float vertical;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        movement = GetComponent<Movement>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // vertical = Input.GetAxisRaw("Vertical");
        if (IsWalled() && Input.GetKey(KeyCode.W) && stamina.slide.value > stamina.slide.minValue)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 2f);
            stamina.TurnOnOffObject(true);
            stamina.StaminaDrain();

            // anim.SetBool("isHang", true);
            // isWallSliding = true;
            // rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Clamp(rb.linearVelocity.y, -wallSlidingSpeed, float.MaxValue));
        }
        else if (IsWalled() && Input.GetKey(KeyCode.S) && stamina.slide.value > stamina.slide.minValue)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, -2f);
            stamina.StaminaDrain();
        }
        else if (!IsWalled())
        {
            stamina.StaminaGain();
        }
    }
    private bool IsWalled()
    {
        return Physics2D.OverlapCircle(wallCheck.position, 0.2f, wallLayer);
    }
}
