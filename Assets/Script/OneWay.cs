using System.Collections;
using UnityEngine;

public class OneWay : MonoBehaviour
{
    public Collider2D platformCollider;
    public float disableDuration = 0.5f;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
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