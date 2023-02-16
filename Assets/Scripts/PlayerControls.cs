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

    //movement
    private Rigidbody rb;
    private Vector3 forceDirection = Vector3.zero;

    [SerializeField] private float movementForce = 1f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float maxSpeed = 5f;
    [SerializeField] private float extraGravity = 1.5f;

    //Camera
    [SerializeField] private Camera playerCam;

    private void Awake()
    {
        rb = this.GetComponent<Rigidbody>();
        playerActionAsset = new ThirdPersonInput();
    }

    private void OnEnable()
    {
        playerActionAsset.PlayerActions.Jump.started += DoJump;
        move = playerActionAsset.PlayerActions.Move;
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
            Debug.Log("HI");
            return true;
        }
        else
        {
            Debug.Log("NO");
            return true;
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
}
