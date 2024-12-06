using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Enemy : MonoBehaviour
{
    public Animator animator;
    [Header("Kesehatan")]
    public int currentHealth;
    public int maxHealth = 4;

    public HealthbarEnemy Healthbar;

    [Header("Patrol")]
    private EnemyPatrol enemyPatrol;

    [Header("Mati = Hilang")]
    [SerializeField] private Behaviour[] components;
    public UnityEvent MatiKnapa;
    public GameObject Darah;

    [Header("Sound Effect")]
    public AudioClip HitSound;
    public AudioClip DeadSound;

    [Header("AudioSource")]
    public AudioSource audioSource;

    void Start()
    {
        currentHealth = maxHealth;
        Healthbar.SetHealth(currentHealth, maxHealth);
        Healthbar = GetComponentInChildren<HealthbarEnemy>();

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    private void Awake()
    {
        enemyPatrol = GetComponentInParent<EnemyPatrol>();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Healthbar.SetHealth(currentHealth, maxHealth);
        Debug.Log("Enemy Hit!");

        animator.SetTrigger("Hurt");

        // Putar suara "HitSound"
        if (HitSound != null)
        {
            audioSource.volume = 0.5f;
            audioSource.PlayOneShot(HitSound);
        }

        if (currentHealth <= 0)
        {
            animator.SetBool("isDead", true);
            Debug.Log("Enemy Died!");

            GetComponent<Collider2D>().enabled = false;
            GetComponent<Rigidbody2D>().isKinematic = true;
            this.enabled = false;

            StartCoroutine(PlayDeadSoundWithDelay());

            Invoke("DestroyObject", 2f);

            Darah.SetActive(false);

            MatiKnapa?.Invoke();

            foreach (Behaviour component in components)
                component.enabled = false;
        }
    }

    IEnumerator PlayDeadSoundWithDelay()
    {
        yield return new WaitForSeconds(0.5f);
        if (DeadSound != null)
        {
            audioSource.volume = 0.5f;
            audioSource.PlayOneShot(DeadSound);
        }
    }

    void DestroyObject()
    {
        Destroy(gameObject);
    }
}
