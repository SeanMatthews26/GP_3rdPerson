using RPGCharacterAnims.Actions;
using System;
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
    const string doubleJump = "DoubleJump";
    const string idle = "Idle";
    const string strafe = "Strafe";
    const string attack1 = "Attack1";
    const string attack2 = "Attack2";
    const string runAttack1 = "RunAttack1";
    const string interact = "Interact";

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
        //parameters
        animator.SetBool("lockedOn", playerControls.lockedOn);
        animator.SetFloat("VeloX", playerControls.move.ReadValue<Vector2>().x);
        animator.SetFloat("VeloY", playerControls.move.ReadValue<Vector2>().y);

        //Speed
        playerSpeed = playerControls.move.ReadValue<Vector2>().sqrMagnitude;
        animator.SetFloat("speed", playerSpeed);

        //Interact
        if(playerControls.interacting)
        {
            Interact();
            return;
        }

        //Attack
        if(playerControls.attackPressed)
        {
            Attack();
            return;
        }

        //Jump
        if(playerControls.doubleJumping)
        {
            DoubleJump();
            return;
        }

        if(playerControls.jumping)
        {
            Jump();
            return;
        }

        if(!playerControls.attacking)
        {
            if(!playerControls.lockedOn)
            {
                ChangeAnimation(idle);
            }
            else
            {
                ChangeAnimation(strafe);
            }
        }
    }

    private void Interact()
    {
        playerControls.interactPressed = false;
        ChangeAnimation(interact);
        Invoke("ResetInteract", 0.6f);
        return;
    }

    private void Jump()
    {
        ChangeAnimation(jump);
    }

    private void DoubleJump()
    {
        ChangeAnimation(doubleJump);
    }

    private void Attack()
    {
        if (currentState == idle || currentState == strafe)
        {
            if (playerSpeed < 0.2)
            {
                playerControls.attackPressed = false;
                ChangeAnimation(attack1);
                Invoke("ResetAttack", 1);
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
        else
        {
            playerControls.attackPressed = false;
            ResetAttack();
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

    private void ResetInteract()
    {
        playerControls.interacting = false;
    }

}
