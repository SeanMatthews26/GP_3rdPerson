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

    //Animation Strings
    const string jump = "Jump";
    const string idle = "Idle";

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
        animator.SetFloat("speed", rb.velocity.magnitude / maxSpeed);

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
}
