using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bergerak : MonoBehaviour
{
    public float jalan;
    public float lari;
    public float slideSpeed = 10f;
    public float slideDuration = 0.5f;
    public float jumpForce = 10f;
    public LayerMask groundLayer;

    private float kecepatan;
    private bool isSliding = false;
    private float slideTimer = 0f;
    private Vector3 slideStartPos;
    private Vector3 slideEndPos;
    public bool isGrounded = false;
    private int jumpCount = 0;
    private int maxJumps = 2;

    public BoxCollider2D KarakterCollider1;
    public BoxCollider2D KarakterCollider2;
    public BoxCollider2D ColliderKaki;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (ColliderKaki != null)
            ColliderKaki.isTrigger = true;
    }

    void Update()
    {
        float moveInputX = Input.GetAxis("Horizontal");

        if (Input.GetKeyDown(KeyCode.Space) && (isGrounded || jumpCount < maxJumps))
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpCount++;
            isGrounded = false;
        }

        // Slide
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.C) && !isSliding && moveInputX != 0)
        {
            isSliding = true;
            slideTimer = 0f;

            slideStartPos = transform.position;
            slideEndPos = slideStartPos + transform.right * moveInputX * slideSpeed;

            if (KarakterCollider2 != null)
            {
                KarakterCollider2.enabled = false;
            }
        }

        // Proses slide
        if (isSliding)
        {
            slideTimer += Time.deltaTime;

            transform.position = new Vector3(
                Mathf.Lerp(slideStartPos.x, slideEndPos.x, slideTimer / slideDuration),
                transform.position.y,
                transform.position.z
            );

            if (slideTimer >= slideDuration)
            {
                isSliding = false;

                if (KarakterCollider2 != null)
                {
                    KarakterCollider2.enabled = true;
                }
            }
        }
        else
        {
            kecepatan = Input.GetKey(KeyCode.LeftShift) ? lari : jalan;

            Vector3 movement = new Vector3(moveInputX, 0f, 0f) * kecepatan * Time.deltaTime;
            transform.position += movement;
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
}
