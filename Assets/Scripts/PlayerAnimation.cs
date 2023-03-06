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
    private bool attacking = false;

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
        //Attacking
        if(!attacking)
        {
            if (playerControls.attackPressed)
            {
                if(playerSpeed < 0.2)
                {
                    playerControls.attackPressed = false;
                    attacking = true;
                    ChangeAnimation(attack1);
                    Invoke("StopAttack", 1f);
                }
                else
                {
                    playerControls.attackPressed = false;
                    attacking = true;
                    ChangeAnimation(runAttack1);
                    Invoke("StopAttack", 1f);
                }
            }
        }

        //Speed
        playerSpeed = rb.velocity.magnitude / maxSpeed;
        animator.SetFloat("speed", playerSpeed);

        //Jumping
        if(playerControls.jumping)
        {
            ChangeAnimation(jump);
        }
        else
        {
            ChangeAnimation(idle);
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
    void StopAttack()
    {
        attacking = false;
    }
}
