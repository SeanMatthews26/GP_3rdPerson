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
    [SerializeField] private float dstToCam = 10f;
    [SerializeField] private Vector2 pitchLimits = new Vector2(-40, 85);
    [SerializeField] float targetAbovePlayer;

    //Animation
    private Animator animator;
    private bool attacking = false;
 

    //LockOn
    [SerializeField] GameObject cylinder;
    [SerializeField] float camSwitchSpeed;
    private Vector3 cylinderPos;
    private Vector3 offset;
    private float offsetSqur;
    private Vector3 offsetNorm;
    private bool lockedOn = false;
    private Vector3 lockOnCamPos;
    private Vector3 freeCamPos;


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
        playerActionAsset.PlayerActions.LockOn.started += DoLockOn;
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

    private void DoLockOn(InputAction.CallbackContext obj)
    {
        lockedOn = !lockedOn;
        Debug.Log(lockedOn);
    }

    private void Update()
    {
        //Camera
        cylinderPos = cylinder.transform.position;
        offset = transform.position - cylinderPos;
        offsetSqur = offset.sqrMagnitude;
        offsetNorm = offset.normalized;

    }

    private void LateUpdate()
    {
        //Camera Stuff
        if(lockedOn) 
        {
            lockOnCamPos = new Vector3(transform.position.x + (offsetNorm.x * dstToCam), playerCam.transform.position.y, transform.position.z + (offsetNorm.z * dstToCam));
            playerCam.transform.position = Vector3.MoveTowards(playerCam.transform.position, lockOnCamPos, camSwitchSpeed * Time.deltaTime);
            playerCam.transform.LookAt(cylinderPos);
        }
        else 
        {
            yaw += look.ReadValue<Vector2>().x * camSensitivity;
            pitch -= look.ReadValue<Vector2>().y * camSensitivity;
            pitch = Mathf.Clamp(pitch, pitchLimits.x, pitchLimits.y);

            Vector3 targetRotation = new Vector3(pitch, yaw);
            playerCam.transform.eulerAngles = targetRotation;

            freeCamPos = transform.position - playerCam.transform.forward * dstToCam + new Vector3(0, targetAbovePlayer, 0);
            playerCam.transform.position = Vector3.MoveTowards(playerCam.transform.position, freeCamPos, camSwitchSpeed * Time.deltaTime);

        }
    }

}
