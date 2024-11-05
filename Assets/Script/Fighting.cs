using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fighting : MonoBehaviour
{
    public Movement mov;
    public Animator anim;
    public int combo;
    public bool attack;
    public AudioSource audioo;
    public AudioClip[] sounds;
    public float moveDistance = 1f;
    public float moveSpeed = 5f; // Speed for Lerp movement

    private Vector3 startPos;
    private Vector3 targetPos;
    private bool isMoving = false;
    private float moveProgress = 0f;

    void Start()
    {
        anim = GetComponent<Animator>();
        audioo = GetComponent<AudioSource>();
    }

    void Update()
    {
        Combos();

        // Process Lerp movement if moving
        if (isMoving)
        {
            moveProgress += Time.deltaTime * moveSpeed;
            transform.position = Vector3.Lerp(startPos, targetPos, moveProgress);

            // Stop moving if target reached
            if (moveProgress >= 1f)
            {
                isMoving = false;
            }
        }
    }

    public void Combos()
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
        }
    }

    public void Start_Combo()
    {
        attack = false;
        mov.enabled = false;
        if (combo < 3)
        {
            combo++;
        }
    }

    public void Finish_Ani()
    {
        mov.enabled = true;
        attack = false;
        combo = 0;
    }
}
