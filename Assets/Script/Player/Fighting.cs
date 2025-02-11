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
    public MagicFight Magicc;
    public Animator anim;
    //VisualizationShake untuk memberi efek kamera shake
    public VisualizationShake visualizationShake;

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

    private float idletime;

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

        if (!attack)
        {
            idletime = 0;
        }
        else
        {
            idletime += Time.deltaTime;
            if (idletime > 0.7)
            {
                attack = false;
            }
        }

        if (Magicc != null)
        {
            if (Magicc.canMagic)
            {
                if (Input.GetKeyDown(KeyCode.J))
                {
                    Magicc.canMagic = false;
                }
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.J))
                {
                    Magicc.canMagic = true;
                }
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

    public void TakeDamage(int Damage, Vector2 knockbackDirection)
    {
        if (Darah <= 0)
        {
            //pass
        }
        else
        {
            Darah -= Damage;
            anim.SetTrigger("isHurt");
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            Debug.Log(rb + " " + knockbackDirection);
            KnockbackManager.Instance.ApplyKnockback(rb, knockbackDirection);
            StartCoroutine(CameraShake());
            visualizationShake.TriggerShake();

            if(idletime > 0)
            {
                mainCamera.transform.eulerAngles = new Vector3(0, 0, 0);
            }
        }
    }

    private IEnumerator CameraShake()
    {
        float elapsedTime = 0f;

        while (elapsedTime < shakeDuration)
        {
            float shakeRotation = Mathf.PingPong(elapsedTime * shakeFrequency, shakeIntensity * 2) - shakeIntensity;

            mainCamera.transform.eulerAngles = new Vector3(0, 0, 0 + shakeRotation);

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        mainCamera.transform.eulerAngles = new Vector3(0, 0, 0);
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
