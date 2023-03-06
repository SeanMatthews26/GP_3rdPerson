using RPGCharacterAnims.Actions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator animator;
    private Rigidbody rb;
    private float maxSpeed = 5f;
    private string currentState;
    PlayerControls playerControls;
    private float playerSpeed;

    //Attack
    private float attackTime;

    //Animation Strings
    const string jump = "Jump";
    const string idle = "Idle";
    const string attack1 = "Attack1";
    const string attack2 = "Attack2";
    const string runAttack1 = "RunAttack1";

    private string lastAttack = "none";

    // Start is called before the first frame update
    void Start()
    {
        animator = this.GetComponent<Animator>();
        rb = this.GetComponent<Rigidbody>();
        playerControls = FindObjectOfType<PlayerControls>();
    }

    // Update is called once per frame
    void Update()
    {
        Attack();
        Jump();
        Debug.Log(playerControls.attacking);

        //Speed
        playerSpeed = rb.velocity.magnitude / maxSpeed;
        animator.SetFloat("speed", playerSpeed);
    }

    private void Jump()
    {
        if (playerControls.jumping)
        {
            ChangeAnimation(jump);
        }
        else
        {
            ChangeAnimation(idle);
        }
    }

    private void Attack()
    {
        if (!playerControls.attackPressed)
        {
            return;
        }

        //Current State
        if(currentState == idle)
        {
            if (playerSpeed < 0.2)
            {
                playerControls.attackPressed = false;
                ChangeAnimation(attack1);
                Invoke("ResetAttack",1);
                return;
            }
            else
            {
                playerControls.attackPressed = false;
                ChangeAnimation(runAttack1);
                Invoke("ResetAttack", 1);
                return;
            }
        }
    }

    void ChangeAnimation(string newState)
    {
        if(currentState == newState)
        {
            return;
        }

        animator.Play(newState);
        currentState = newState;
    }

    private void ResetAttack()
    {
        playerControls.attacking = false;
    }

}
