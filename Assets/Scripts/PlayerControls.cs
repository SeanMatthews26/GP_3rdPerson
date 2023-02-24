using RPGCharacterAnims.Actions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
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
    [SerializeField] Vector3 camTargetAbovePlayer;

    //Animation
    private Animator animator;
    private bool attacking = false;


    //LockOn
    [SerializeField] float camSwitchSpeed;
    [SerializeField] float sphereOffset;
    [SerializeField] Image targetImage;
    private Vector3 offset2D;
    private float offsetSqur;
    private Vector3 offsetNorm;
    private bool lockedOn = false;
    private Vector3 lockOnCamPos;
    private Vector3 freeCamPos;
    private GameObject currentTarget;



    //Testing
    [SerializeField] float targetDist;
    [SerializeField] float sphereRad;

    private void Awake()
    {
        animator = this.GetComponent<Animator>();
        rb = this.GetComponent<Rigidbody>();
        playerActionAsset = new ThirdPersonInput();
    }

    private void Start()
    {
        
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
        if(!lockedOn)
        {
            currentTarget = FindTarget();

            if(currentTarget == null) 
            {
                lockedOn = false;
            }
            else
            {
                lockedOn = !lockedOn;
            }

        }
        else if(lockedOn)
        {
            pitch = playerCam.transform.eulerAngles.x;
            yaw = playerCam.transform.eulerAngles.y;

            lockedOn = !lockedOn;
        }
    }

    private void Update()
    {
        Debug.Log(Vector3.Distance(transform.position, playerCam.transform.position));
        //Camera
        offset2D = transform.position - currentTarget.transform.position;
        offsetSqur = offset2D.sqrMagnitude;
        offsetNorm = offset2D.normalized;
    }

    private void LateUpdate()
    {
        //Camera Stuff
        if(lockedOn) 
        {
            targetImage.enabled = true;
            lockOnCamPos = new Vector3(transform.position.x + (offsetNorm.x * dstToCam), playerCam.transform.position.y, transform.position.z + (offsetNorm.z * dstToCam));
            playerCam.transform.position = Vector3.MoveTowards(playerCam.transform.position, lockOnCamPos, camSwitchSpeed * Time.deltaTime);

            //playerCam.transform.LookAt(currentTarget.transform.position);
            var x = Quaternion.LookRotation(currentTarget.transform.position - playerCam.transform.position);
            playerCam.transform.rotation = Quaternion.Slerp(playerCam.transform.rotation, x, 10 * Time.deltaTime);
        }
        else 
        {
            targetImage.enabled = false;
            yaw += look.ReadValue<Vector2>().x * camSensitivity;
            pitch -= look.ReadValue<Vector2>().y * camSensitivity;
            pitch = Mathf.Clamp(pitch, pitchLimits.x, pitchLimits.y);

            Vector3 targetRotation = new Vector3(pitch, yaw);
            playerCam.transform.eulerAngles = targetRotation;

            freeCamPos = transform.position - (playerCam.transform.forward * dstToCam) + camTargetAbovePlayer;

            playerCam.transform.position = Vector3.MoveTowards(playerCam.transform.position, freeCamPos, camSwitchSpeed * Time.deltaTime);
        }
    }

    private GameObject FindTarget()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position + transform.forward * sphereOffset, sphereRad);
        List<GameObject> possibleTarget = new List<GameObject>();
        float minDist = Mathf.Infinity;
        GameObject target= null;

        foreach (Collider hit in hits)
        {
            if (hit.tag == "Target" && hit.gameObject.GetComponent<Renderer>().isVisible)
            {
                possibleTarget.Add(hit.gameObject);
            }
        }

        foreach (GameObject t in possibleTarget)
        {
            float distance = Vector3.Distance(transform.position, t.transform.position);

            if(distance < minDist)
            {
                target = t;
                minDist = distance;
            }
        }

        if(minDist == Mathf.Infinity)
        {
            return null;
        }
        else
        {
            return target;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + transform.forward * sphereOffset, sphereRad);
    }
}
