using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Transform target;
    private float moveSpeed;

    public GameObject Partikel;
    public ParticleSystem Efek;

    private bool hasHit = false;

    public LayerMask targetLayer; // Layer yang menjadi target tabrakan

    private void Update()
    {
        if (!hasHit && target != null)
        {
            Vector3 moveDirNormalized = (target.position - transform.position).normalized;
            transform.position += moveDirNormalized * moveSpeed * Time.deltaTime;
        }
    }

    private void HandleHit()
    {
        hasHit = true;
        moveSpeed = 0;

        if (Partikel != null)
        {
            Partikel.SetActive(false);
        }

        if (Efek != null)
        {
            Efek.Play();
            Destroy(gameObject, Efek.main.duration);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Fungsi ini dipanggil saat proyektil bertabrakan dengan objek lain
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            HandleHit();
        }
        else if (IsObjectInTargetLayer(collision.gameObject))
        {
            HandleHit(); // Jika mengenai objek dengan layer yang ditentukan
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            HandleHit();
        }
        else if (IsObjectInTargetLayer(other.gameObject))
        {
            HandleHit(); // Jika mengenai objek dengan layer yang ditentukan
        }
    }

    // Mengecek apakah objek termasuk dalam layer yang ditentukan
    private bool IsObjectInTargetLayer(GameObject obj)
    {
        return ((targetLayer.value & (1 << obj.layer)) != 0); // Mengecek apakah objek memiliki layer yang sesuai
    }

    public void InitializeProjectile(Transform target, float moveSpeed, LayerMask layerMask)
    {
        this.target = target;
        this.moveSpeed = moveSpeed;
        this.targetLayer = layerMask; // Setel layer yang menjadi target tabrakan
    }
}
