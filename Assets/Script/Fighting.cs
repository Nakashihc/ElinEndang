using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fighting : MonoBehaviour
{
    [Header("Health")]
    public int Darah;

    [Header("Script")]
    public Movement mov;
    public Jongkok jong;
    public Animator anim;

    [Header("Attack")]
    public int combo;
    public bool attack;
    public AudioSource audioo;
    public AudioClip[] sounds;
    public float moveDistance = 1f;
    public float moveSpeed = 5f;
    public float comboCooldown = 1f;

    [Header("Attack")]
    public float Damage;
    public Transform attackPoint;

    public float attackRange = 0.5f;
    public LayerMask enemyLayers;
    public int attackDamage = 2;

    private Vector3 startPos;
    private Vector3 targetPos;
    private bool isMoving = false;
    private float moveProgress = 0f;
    private float comboTimer = 0f;

    [Header("Kamera")]
    public float shakeIntensity = 0.1f;
    public float shakeDuration = 0.2f;
    public float shakeFrequency = 10f;
    private Camera mainCamera;

    void Start()
    {
        anim = GetComponent<Animator>();
        audioo = GetComponent<AudioSource>();
        mainCamera = Camera.main;
    }

    void Update()
    {
        Combos();

        if (isMoving)
        {
            moveProgress += Time.deltaTime * moveSpeed;
            transform.position = Vector3.Lerp(startPos, targetPos, moveProgress);

            if (moveProgress >= 1f)
            {
                isMoving = false;
            }
        }

        if (combo > 0)
        {
            comboTimer += Time.deltaTime;
            if (comboTimer >= comboCooldown)
            {
                ResetCombo();
                mov.enabled = true;
            }
        }
    }

    private void DamageEnemy()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        foreach (Collider2D enemy in hitEnemies)
        {
            enemy.GetComponent<Enemy>().TakeDamage(attackDamage);
        }
    }

    public void Combos()
    {
        if (!jong.isCrouching)
        {
            if (Input.GetMouseButtonDown(0) && !attack && !isMoving)
            {
                attack = true;
                anim.SetTrigger("" + combo);

                if (combo < sounds.Length)
                {
                    audioo.clip = sounds[combo];
                    audioo.Play();
                }

                startPos = transform.position;
                float direction = transform.localScale.x > 0 ? 1 : -1;
                targetPos = startPos + new Vector3(moveDistance * direction, 0, 0);

                isMoving = true;
                moveProgress = 0f;
                comboTimer = 0f;
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                attack = true;
                anim.SetTrigger("crouchAttk");
            }
        }
    }

    public void Start_Combo()
    {
        attack = false;
        mov.enabled = false;
        if (combo < 3)
        {
            combo++;
            comboTimer = 0f;
        }
    }

    public void Finish_Ani()
    {
        mov.enabled = true;
        attack = false;
        ResetCombo();
    }

    private void ResetCombo()
    {
        combo = 0;
        comboTimer = 0f;
    }

    public void TakeDamage(int Damage)
    {
        if (Darah <= 0)
        {
            //pass
        }
        else
        {
            Darah -= Damage;
            anim.SetTrigger("isHurt");
            StartCoroutine(FreezeTimeEffect());
            StartCoroutine(CameraShake());
        }
    }

    

    private IEnumerator FreezeTimeEffect()
    {
        float originalTimeScale = Time.timeScale;

        Time.timeScale = 0f;

        yield return new WaitForSecondsRealtime(0.2f);

        Time.timeScale = originalTimeScale;

        yield return new WaitForSecondsRealtime(0.3f);
        StopAllCoroutines();
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

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
