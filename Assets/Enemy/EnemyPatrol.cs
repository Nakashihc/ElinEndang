using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyPatrol : MonoBehaviour
{
    [Header("Patrol Points")]
    [SerializeField] private Transform leftEdge;
    [SerializeField] private Transform rightEdge;

    [Header("Enemy")]
    [SerializeField] private Transform enemy;

    [Header("Movement parameters")]
    [SerializeField] private float speed;
    private bool movingLeft;

    [Header("Idle Behaviour")]
    [SerializeField] private float idleDuration;
    private float idleTimer;

    [Header("Enemy Animator")]
    [SerializeField] private Animator anim;

    private void OnDisable()
    {
        anim.SetBool("Walk", false);
    }

    private void Update()
    {
        if (movingLeft)
        {
            if (enemy.position.x >= leftEdge.position.x)
                MoveInDirection(-1);
            else
                DirectionChange();
        }
        else
        {
            if (enemy.position.x <= rightEdge.position.x)
                MoveInDirection(1);
            else
                DirectionChange();
        }
    }

    private void DirectionChange()
    {
        idleTimer += Time.deltaTime;
        anim.SetBool("Walk", false);
        if (idleTimer > idleDuration)
        {
            movingLeft = !movingLeft;
        }
    }

    private void MoveInDirection(int _direction)
    {
        idleTimer = 0;
        anim.SetBool("Walk", true);

        // Make enemy face direction using rotation
        if (_direction == 1)
        {
            enemy.rotation = Quaternion.Euler(0, 0, 0); // Face right
        }
        else if (_direction == -1)
        {
            enemy.rotation = Quaternion.Euler(0, 180, 0); // Face left
        }

        // Move in that direction
        enemy.position = new Vector3(enemy.position.x + Time.deltaTime * _direction * speed,
            enemy.position.y, enemy.position.z);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(leftEdge.transform.position, 0.5f);
        Gizmos.DrawWireSphere(rightEdge.transform.position, 0.5f);
        Gizmos.DrawLine(rightEdge.transform.position, leftEdge.transform.position);
    }
}
