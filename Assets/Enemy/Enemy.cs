using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Enemy : MonoBehaviour
{
    public Animator animator;
    public MeleEnemy mele;
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
    private Camera mainCamera;
    public float shakeIntensity = 0.1f;
    public float shakeDuration = 0.2f;
    public float shakeFrequency = 10f;

    void Start()
    {
        mainCamera = Camera.main;
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
        //Healthbar.SetHealth(currentHealth, maxHealth);
        Debug.Log("Enemy Hit!");

        animator.SetTrigger("Hit");

        mele.cooldownTimer = 0;
        StartCoroutine(FreezeTimeEffect());
        StartCoroutine(CameraShake());

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

    private IEnumerator FreezeTimeEffect()
    {
        float originalTimeScale = Time.timeScale;

        Time.timeScale = 0f;

        yield return new WaitForSecondsRealtime(0.2f);

        Time.timeScale = originalTimeScale;

        yield return new WaitForSecondsRealtime(0.3f);
    }

    private IEnumerator CameraShake()
    {
        float originalRotation = mainCamera.transform.eulerAngles.z;

        float elapsedTime = 0f;

        while (elapsedTime < shakeDuration)
        {
            float shakeRotation = Mathf.PingPong(elapsedTime * shakeFrequency, shakeIntensity * 2) - shakeIntensity;

            mainCamera.transform.eulerAngles = new Vector3(0, 0, originalRotation + shakeRotation);

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        mainCamera.transform.eulerAngles = new Vector3(0, 0, originalRotation);
    }

}
