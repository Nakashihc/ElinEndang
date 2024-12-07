using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;
using UnityEngine.Audio;

public class MeleEnemy : MonoBehaviour
{
    // Attack Parameters
    [Header("Attack Parameters")]
    [SerializeField] private float attackCooldown;
    [SerializeField] private float range;
    [SerializeField] private int damage;

    // Collider Parameters
    [Header("Collider Parameters")]
    [SerializeField] private float colliderDistance;
    [SerializeField] private BoxCollider2D boxCollider;

    // Player Layer
    [Header("Player Layer")]
    [SerializeField] private LayerMask playerLayer;
    private float cooldownTimer = Mathf.Infinity;

    // References
    private Animator anim;
    public Fighting playerscript;
    private EnemyPatrol enemyPatrol;

    [Header("Ikut Player")]
    private GameObject player;
    public float speed;
    private float distance;
    public float jarakPlayer;
    public float stopDistance;

    [Header("Sound Effect")]
    public AudioClip AttackSound;
    public AudioSource audioosorc;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        enemyPatrol = GetComponentInParent<EnemyPatrol>();
        player = GameObject.FindGameObjectWithTag("Player"); // Find the player using the tag "Player"
    }

    private void Update()
    {
        cooldownTimer += Time.deltaTime;

        // Attack only when player in sight
        if (PlayerInSight())
        {
            if (cooldownTimer >= attackCooldown)
            {
                cooldownTimer = 0;
                anim.SetTrigger("Attack");
            }
        }

        if (enemyPatrol != null)
            enemyPatrol.enabled = !PlayerInSight();

        float distance = Vector2.Distance(transform.position, player.transform.position);
        Vector2 direction = player.transform.position - transform.position;
        direction.y = 0; // Set y-component to 0 to move only in x-direction
        direction.Normalize();
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        if (distance < stopDistance)
        {
            anim.SetBool("Walk", false);
            transform.position = Vector2.MoveTowards(this.transform.position, player.transform.position, 0);
        }
        else if (distance < jarakPlayer)
        {
            enemyPatrol.enabled = false;
            transform.position = Vector2.MoveTowards(this.transform.position, player.transform.position, speed * Time.deltaTime);
            anim.SetBool("Walk", true);

            // Change direction towards player using rotation
            if (player.transform.position.x > transform.position.x)
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);
            }
            else
            {
                transform.rotation = Quaternion.Euler(0, 180, 0);
            }
        }
        else
        {
            enemyPatrol.enabled = true;
            anim.SetBool("Walk", false);
        }
    }

    private bool PlayerInSight()
    {
        RaycastHit2D hit =
            Physics2D.BoxCast(boxCollider.bounds.center + transform.right * range * transform.localScale.x * colliderDistance,
            new Vector3(boxCollider.bounds.size.x * range, boxCollider.bounds.size.y, boxCollider.bounds.size.z),
            0, Vector2.left, 0, playerLayer);

        if (hit.collider != null)
            playerscript = hit.transform.GetComponent<Fighting>();

        return hit.collider != null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(boxCollider.bounds.center + transform.right * range * transform.localScale.x * colliderDistance,
            new Vector3(boxCollider.bounds.size.x * range, boxCollider.bounds.size.y, boxCollider.bounds.size.z));
    }

    private void DamagePlayer()
    {
        if (PlayerInSight())
        {
            audioosorc.PlayOneShot(AttackSound, 0.5f);

            playerscript.TakeDamage(damage);
        }
    }
}
