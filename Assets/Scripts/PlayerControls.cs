using RPGCharacterAnims.Actions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControls : MonoBehaviour
{
    //Input
    private ThirdPersonInput playerActionAsset;
    private InputAction move;
    private InputAction look;

    //movement
    private Rigidbody rb;
    private Vector3 forceDirection = Vector3.zero;

    [SerializeField] private float movementForce = 1f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float maxSpeed = 5f;
    [SerializeField] private float extraGravity = 1.5f;

    //Camera
    [SerializeField] private Camera playerCam;
    private float pitch;
    private float yaw;
    [SerializeField] private float camSensitivity = 1f;
    private float dstToCam = 10f;

    private Animator animator;

    private void Awake()
    {
        animator = this.GetComponent<Animator>();
        rb = this.GetComponent<Rigidbody>();
        playerActionAsset = new ThirdPersonInput();
    }

    private void OnEnable()
    {
        playerActionAsset.PlayerActions.Jump.started += DoJump;
        playerActionAsset.PlayerActions.Attack.started += DoAttack;
        move = playerActionAsset.PlayerActions.Move;
        look = playerActionAsset.PlayerActions.Look;
        playerActionAsset.PlayerActions.Enable();
    }

  

    private void OnDisable()
    {
        playerActionAsset.PlayerActions.Jump.started -= DoJump;
    }

    private void DoJump(InputAction.CallbackContext obj)
    {
        if(IsGrounded())
        {
            forceDirection += Vector3.up * jumpForce;
            Debug.Log("Jump");
        }
    }

    private bool IsGrounded()
    {
        Ray ray = new Ray(this.transform.position + Vector3.up * 0.25f, Vector3.down);
        if(Physics.Raycast(ray,out RaycastHit hit, 0.3f))
        {
            animator.SetBool("grounded", true);
            return true;
        }
        else
        {
            animator.SetBool("grounded", false);
            return false;
        }
    }

    private void LookAt()
    {
        Vector3 direction = rb.velocity;
        direction.y = 0f;

        if(move.ReadValue<Vector2>().sqrMagnitude > 0.1f && direction.sqrMagnitude > 0.1f)
        {
            this.rb.rotation = Quaternion.LookRotation(direction, Vector3.up);
        }
        else
        {
            rb.angularVelocity = Vector3.zero;
        }
    }

    private void FixedUpdate()
    {


        IsGrounded();
        forceDirection += move.ReadValue<Vector2>().x * GetCameraRight(playerCam) * movementForce;
        forceDirection += move.ReadValue<Vector2>().y * GetCameraForward(playerCam) * movementForce;

        rb.AddForce(forceDirection, ForceMode.Impulse);
        forceDirection = Vector3.zero;

        if(rb.velocity.y < 0)
        {
            rb.velocity -= Vector3.down * Physics.gravity.y * Time.fixedDeltaTime * extraGravity;
        }

        Vector3 horizontalVelo = rb.velocity;
        horizontalVelo.y = 0;
        if(horizontalVelo.sqrMagnitude > maxSpeed * maxSpeed)
        {
            rb.velocity = horizontalVelo.normalized * maxSpeed + Vector3.up * rb.velocity.y;
        }

        LookAt();
    }

    private Vector3 GetCameraForward(Camera playerCam)
    {
        Vector3 forward = playerCam.transform.forward;
        forward.y = 0;
        return forward.normalized;
    }

    private Vector3 GetCameraRight(Camera playerCam)
    {
        Vector3 right = playerCam.transform.right;
        right.y = 0;
        return right.normalized;
    }

    private void DoAttack(InputAction.CallbackContext obj)
    {
        animator.SetTrigger("attack");
    }

    private void LateUpdate()
    {
        Debug.Log(look.ReadValue<Vector2>().x);

        //Camera Stuff
        yaw += look.ReadValue<Vector2>().x * camSensitivity;
        pitch -= look.ReadValue<Vector2>().y * camSensitivity;

        Vector3 targetRotation = new Vector3(pitch, yaw);
        playerCam.transform.eulerAngles = targetRotation;

        playerCam.transform.position = transform.position - playerCam.transform.forward * dstToCam;
    }

}
