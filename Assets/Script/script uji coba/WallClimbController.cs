using UnityEngine;

public class WallClimbController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform wallCheck; // Titik deteksi dinding
    [SerializeField] private LayerMask wallLayer; // Layer untuk mendeteksi dinding
    [SerializeField] private Transform groundCheck; // Titik deteksi tanah
    [SerializeField] private LayerMask groundLayer; // Layer untuk mendeteksi tanah
    [SerializeField] private Rigidbody2D rb; // Referensi ke Rigidbody2D
    [SerializeField] private PlayerMovements playerMovement; // Referensi ke PlayerMovement
    [SerializeField] private StaminaController staminaController; // Referensi ke StaminaController

    [Header("Settings")]
    [SerializeField] private float climbSpeed = 2f; // Kecepatan memanjat
    [SerializeField] private float descendSpeed = -2f; // Kecepatan turun

    private void Start()
    {
        // Inisialisasi komponen jika belum diassign di Inspector
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }
        if (playerMovement == null)
        {
            playerMovement = GetComponent<PlayerMovements>();
        }
    }

    private void FixedUpdate()
    {
        HandleWallClimbing();
    }

    private void HandleWallClimbing()
    {
        bool isWalled = IsWalled();
        bool isGrounded = IsGrounded();

        if (isWalled)
        {
            if (Input.GetKey(KeyCode.W) && staminaController.staminaSlider.value > staminaController.staminaSlider.minValue)
            {
                ClimbUp();
            }
            else if (Input.GetKey(KeyCode.S) && staminaController.staminaSlider.value > staminaController.staminaSlider.minValue)
            {
                ClimbDown();
            }
            else if (staminaController.staminaSlider.value > staminaController.staminaSlider.minValue && !isGrounded)
            {
                StopClimbing(); // Karakter diam di tempat jika tidak ada input
            }

            // Nonaktifkan pergerakan horizontal saat memanjat tangga
            // rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        }
        // Recovery stamina saat menyentuh tanah
        if (isGrounded)
        {
            staminaController.GainStamina();
            return;
        }
        // Logika memanjat dinding
        // else
        // {
        //     // Aktifkan kembali pergerakan horizontal jika tidak menyentuh tangga
        //     rb.linearVelocity = new Vector2(playerMovement.GetHorizontal() * playerMovement.currentSpeed, rb.linearVelocity.y);
        // }

    }

    private void ClimbUp()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, climbSpeed);
        staminaController.ToggleStaminaUI(true);
        staminaController.DrainStamina();
    }

    private void ClimbDown()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, descendSpeed);
        staminaController.ToggleStaminaUI(true);
        staminaController.DrainStamina();
    }

    private void StopClimbing()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0.196196f); // Hentikan semua gerakan
        staminaController.ToggleStaminaUI(false);
    }

    private bool IsWalled()
    {
        return Physics2D.OverlapCircle(wallCheck.position, 0.2f, wallLayer);
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }
}