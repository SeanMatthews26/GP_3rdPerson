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
    const string runAttack1 = "RunAttack1";

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

        //Speed
        playerSpeed = rb.velocity.magnitude / maxSpeed;
        animator.SetFloat("speed", playerSpeed);

        //Jumping
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
        //Attacking
        if (!playerControls.attackPressed)
        {
            return;
        }

        if (currentState != idle)
        {
            playerControls.attackPressed = false;
            return;
        }

        if (playerSpeed < 0.2)
        {
            playerControls.attackPressed = false;
            ChangeAnimation(attack1);
        }
        else
        {
            playerControls.attackPressed = false;
            ChangeAnimation(runAttack1);
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

}
