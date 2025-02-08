using UnityEngine;

public class CrouchController : MonoBehaviour
{
    [Header("References")]
    public BoxCollider2D headCollider; // Collider kepala pemain
    public PlayerMovements playerMovement; // Referensi ke PlayerMovement
    public PlayerAnimationController animationController; // Referensi ke PlayerAnimationController

    [Header("Settings")]
    public float crouchSpeedMultiplier = 0.5f; // Pengali kecepatan saat crouch

    public bool isCrouching = false;
    private bool isBlockedByStructure = false;

    private void Update()
    {
        HandleCrouchInput();
    }

    private void HandleCrouchInput()
    {
        if (playerMovement.IsGrounded())
        {
            if (Input.GetKeyDown(KeyCode.C) && !isCrouching)
            {
                StartCrouch();
            }
            else if (Input.GetKeyUp(KeyCode.C) || isBlockedByStructure)
            {
                StopCrouch();
            }
        }
    }

    private void StartCrouch()
    {
        if (!isBlockedByStructure)
        {
            isCrouching = true;
            playerMovement.currentSpeed *= crouchSpeedMultiplier; // Menggunakan properti currentSpeed
            headCollider.enabled = false; // Nonaktifkan collider kepala
            animationController.UpdateCrouchAnimation(true); // Perbarui animasi crouch
        }
    }

    private void StopCrouch()
    {
        if (!isBlockedByStructure)
        {
            isCrouching = false;
            playerMovement.currentSpeed /= crouchSpeedMultiplier; // Menggunakan properti currentSpeed
            headCollider.enabled = true; // Aktifkan kembali collider kepala
            animationController.UpdateCrouchAnimation(false); // Perbarui animasi crouch
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Structure"))
        {
            isBlockedByStructure = true;
            StartCrouch(); // Paksa crouch jika ada struktur di atas
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Structure"))
        {
            isBlockedByStructure = false;
            if (!Input.GetKey(KeyCode.C))
            {
                StopCrouch();
            }
        }
    }

    // Metode untuk mendapatkan status crouch
    public bool IsCrouching()
    {
        return isCrouching;
    }
}