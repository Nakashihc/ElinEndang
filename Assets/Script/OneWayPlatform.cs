using System.Collections;
using UnityEngine;

public class OneWayPlatform : MonoBehaviour
{
    public Collider2D platformCollider; // Drag and drop your platform's BoxCollider here in the inspector
    public float disableDuration = 0.5f; // How long the collider is disabled

    private void Update()
    {
        // Cek jika pemain menekan tombol "S"
        if (Input.GetKeyDown(KeyCode.S))
        {
            // Matikan collider sementara
            StartCoroutine(DisableColliderTemporarily());
        }
    }

    private IEnumerator DisableColliderTemporarily()
    {
        platformCollider.enabled = false; // Matikan collider
        yield return new WaitForSeconds(disableDuration); // Tunggu sebentar
        platformCollider.enabled = true; // Hidupkan kembali collider
    }
}
