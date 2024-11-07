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
    public float comboCooldown = 1f; // Cooldown time for combo reset

    private Vector3 startPos;
    private Vector3 targetPos;
    private bool isMoving = false;
    private float moveProgress = 0f;
    private float comboTimer = 0f;

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

        // Update combo timer and reset combo if cooldown exceeded
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
            comboTimer = 0f; // Reset combo timer on each attack
        }
    }

    public void Start_Combo()
    {
        attack = false;
        mov.enabled = false;
        if (combo < 3)
        {
            combo++;
            comboTimer = 0f; // Reset timer when advancing to the next combo
        }
    }

    public void Finish_Ani()
    {
        mov.enabled = true;
        attack = false;
        ResetCombo(); // Reset combo when animation finishes
    }

    private void ResetCombo()
    {
        combo = 0;
        comboTimer = 0f;
    }
}
