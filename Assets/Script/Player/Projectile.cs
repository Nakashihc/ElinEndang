using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Transform target;
    private float moveSpeed;

    public GameObject Partikel; // Objek visual yang akan dihilangkan
    public ParticleSystem Efek; // Particle System untuk efek tabrakan

    [SerializeField] private float distanceToTargetToDestroyProjectile = 1f;

    private bool hasHit = false; // Untuk mencegah multiple hit

    private void Update()
    {
        if (!hasHit && target != null)
        {
            Vector3 moveDirNormalized = (target.position - transform.position).normalized;
            transform.position += moveDirNormalized * moveSpeed * Time.deltaTime;

            if (Vector3.Distance(transform.position, target.position) < distanceToTargetToDestroyProjectile)
            {
                HandleHit();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Menangani tabrakan dengan target
        if (!hasHit && other.transform == target)
        {
            HandleHit();
        }
    }

    private void HandleHit()
    {
        hasHit = true; // Tandai bahwa proyektil sudah terkena
        moveSpeed = 0; // Hentikan pergerakan proyektil

        if (Partikel != null)
        {
            Partikel.SetActive(false); // Sembunyikan objek visual proyektil
        }

        if (Efek != null)
        {
            Efek.Play(); // Mainkan efek Particle System
            Destroy(gameObject, Efek.main.duration); // Hancurkan proyektil setelah efek selesai
        }
        else
        {
            Destroy(gameObject); // Jika tidak ada efek, langsung hancurkan
        }
    }

    public void InitializeProjectile(Transform target, float moveSpeed)
    {
        this.target = target;
        this.moveSpeed = moveSpeed;
    }
}
