using Unity.Android.Types;
using UnityEngine;


//efek knockback bisa terjadi jika faktor kontrol gerakan dimatikan
//itu skrip movement kenapa gabisa dinon-aktifin bjir 💀
public class KnockbackManager : MonoBehaviour
{
    public Movement movement;
    public static KnockbackManager Instance;

    public float knockbackForce = 10f; // Besar gaya knockback
    public float knockbackDuration = 0.5f; // Durasi knockback

    private void Awake()
    {
        
        // Pastikan hanya ada satu instance dari KnockbackManager
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ApplyKnockback(Rigidbody2D rb, Vector2 direction)
    {
        //nonaktifkan kontrol pergerakan
        movement.enabled = false;
        // Terapkan gaya knockback sebagai impulse
        rb.AddForce(direction.normalized * knockbackForce, ForceMode2D.Impulse);
        StartCoroutine(ResetKnockback(rb));
    }

    //IEnumerator untuk memberikan efek knockback yang lebih instant
    private System.Collections.IEnumerator ResetKnockback(Rigidbody2D rb)
    {
        // Nonaktifkan kontrol gerakan selama durasi knockback
        // Anda bisa menambahkan logika untuk menonaktifkan kontrol gerakan di sini

        float elapsedTime = 0f;
        float duration = knockbackDuration;

        while (elapsedTime < duration)
        {
            // Calculate the easing factor (ease in)
            float t = elapsedTime / duration;
            float easeInFactor = t * t; // Quadratic ease in

            // Reduce the velocity based on the easing factor
            rb.linearVelocity *= 1 - easeInFactor;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Set velocity to zero after the knockback duration
        rb.linearVelocity = Vector2.zero;

        yield return new WaitForSeconds(knockbackDuration);

        // Kembalikan kontrol gerakan di sini
        movement.enabled = true;
    }
}