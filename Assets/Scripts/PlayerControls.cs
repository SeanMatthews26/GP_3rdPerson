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
    public ThirdPersonInput playerActionAsset;
    public InputAction move;
    private InputAction look;

    //movement
    private Rigidbody rb;
    private Vector3 forceDirection = Vector3.zero;

    [SerializeField] public float movementForce = 1f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float normalMaxSpeed;
    [SerializeField] private float strafeMaxSpeed;
    [SerializeField] public float maxSpeed;
    [SerializeField] private float extraGravity = 1.5f;
    [SerializeField] private float maxFallSpeed;
    [HideInInspector] public int extraJumps = 0;
    public int jumpsLeft;

    //Camera
    [SerializeField] public Camera playerCam;
    public bool camEnabled = true;
    private float pitch;
    private float yaw;
    [SerializeField] private float camSensitivity = 1f;
    [SerializeField] private float dstToCam = 10f;
    [SerializeField] private Vector2 pitchLimits = new Vector2(-40, 85);
    [SerializeField] Vector3 camTargetAbovePlayer;

    //Animation
    public bool attackPressed = false;
    public bool attacking = false;
    public bool jumping = false;
    public bool doubleJumping = false;
    public bool interactPressed = false;
    public bool interacting = false;

    //LockOn
    [SerializeField] float camSwitchSpeed;
    [SerializeField] float lockOnSphereOffset;
    [SerializeField] float lockOnSphereRad;
    [SerializeField] Image targetImage;
    private Vector3 offset2D;
    private float offsetSqur;
    private Vector3 offsetNorm;
    public bool lockedOn = false;
    private Vector3 lockOnCamPos;
    private Vector3 freeCamPos;
    public GameObject currentTarget;

    //Particles
    [SerializeField] public ParticleSystem speedBoostParticles;
    [SerializeField] public ParticleSystem extraJumpParticles;

    //Interact
    [SerializeField] public float interactSphereOffset;
    [SerializeField] public float interactSphereRad;

    //Testing
    [SerializeField] float targetDist;

    private void Awake()
    {
        rb = this.GetComponent<Rigidbody>();
        playerActionAsset = new ThirdPersonInput();
        speedBoostParticles.Stop();
        extraJumpParticles.Stop();
    }

    private void Start()
    {
        
    }

    private void OnEnable()
    {
        playerActionAsset.PlayerActions.Jump.started += DoJump;
        playerActionAsset.PlayerActions.Attack.started += DoAttack;
        playerActionAsset.PlayerActions.LockOn.started += DoLockOn;
        playerActionAsset.PlayerActions.Interact.started += DoInteract;
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
        else
        {
            if(jumpsLeft > 0)
            {
                doubleJumping = true;
                forceDirection += Vector3.up * jumpForce;
                jumpsLeft--;
            }
        }
    }

    private void DoInteract(InputAction.CallbackContext obj)
    {
        if (!interacting)
        {
            bool switchInArea = false;
            Collider[] hits = Physics.OverlapSphere(transform.position + transform.forward * interactSphereOffset, interactSphereRad);

            foreach (Collider hit in hits)
            {
                if (hit.tag == "Switch")
                {
                    if(hit.GetComponent<Renderer>().isVisible)
                    {
                        switchInArea = true;
                        hit.GetComponent<DoorSwitch>().Switch();
                    }
                }
            }

            if(switchInArea)
            {
                interactPressed = true;
                interacting = true;
            }
        }
    }

    private bool IsGrounded()
    {
        Ray ray = new Ray(this.transform.position + Vector3.up * 0.25f, Vector3.down);
        Debug.DrawRay(this.transform.position + Vector3.up * 0.25f, Vector3.down, Color.green);
        if (Physics.Raycast(ray, out RaycastHit hit, 0.45f))
        {
            jumping = false;
            doubleJumping = false;
            jumpsLeft = extraJumps;
            return true;
        }
        else
        {
            jumping = true;
            return false;
        }
    }

    private void LookAt()
    {
        Vector3 direction = rb.velocity;
        direction.y = 0f;

        if(lockedOn)
        {
            Vector3 lockOnDirection = (transform.position - currentTarget.transform.position).normalized;
            lockOnDirection.y = 0f;
            this.rb.rotation = Quaternion.LookRotation(-lockOnDirection, Vector3.up);
        }
        else
        {
            if (move.ReadValue<Vector2>().sqrMagnitude > 0.1f && direction.sqrMagnitude > 0.1f)
            {
                this.rb.rotation = Quaternion.LookRotation(direction, Vector3.up);
            }
            else
            {
                rb.angularVelocity = Vector3.zero;
            }
        }
    }

    private void FixedUpdate()
    {
        //Movement
        IsGrounded();
        forceDirection += move.ReadValue<Vector2>().x * GetCameraRight(playerCam) * movementForce;
        forceDirection += move.ReadValue<Vector2>().y * GetCameraForward(playerCam) * movementForce;
        rb.AddForce(forceDirection, ForceMode.Impulse);
        forceDirection = Vector3.zero;

        //Extra Gravity
        if(rb.velocity.y < 0)
        {
            rb.velocity -= Vector3.down * Physics.gravity.y * Time.fixedDeltaTime * extraGravity;
        }

        Vector3 horizontalVelo = rb.velocity;
        horizontalVelo.y = 0;

        //MaxSpeed
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
        if(!attacking)
        {
            attackPressed = true;
            attacking = true;
        }
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
        RaycastHit[] hits = Physics.RaycastAll(playerCam.transform.position, playerCam.transform.forward, dstToCam);

        if (hits[0].collider == this.gameObject.GetComponent<Collider>())
        {
            Debug.Log("player");
        }
        else
        {
            
        }
    }

    private void LateUpdate()
    {
        if(camEnabled)
        {
            //Camera Stuff
            if (lockedOn)
            {
                maxSpeed = strafeMaxSpeed;
                offset2D = transform.position - currentTarget.transform.position;
                offsetNorm = offset2D.normalized;

                targetImage.enabled = true;
                lockOnCamPos = new Vector3(transform.position.x + (offsetNorm.x * dstToCam), playerCam.transform.position.y, transform.position.z + (offsetNorm.z * dstToCam));
                playerCam.transform.position = Vector3.MoveTowards(playerCam.transform.position, lockOnCamPos, camSwitchSpeed * Time.deltaTime);

                var x = Quaternion.LookRotation(currentTarget.transform.position - playerCam.transform.position);
                playerCam.transform.rotation = Quaternion.Slerp(playerCam.transform.rotation, x, 10 * Time.deltaTime);

                LockOnTarget();
            }
            else
            {
                maxSpeed = normalMaxSpeed;
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
    }

    private GameObject FindTarget()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position + transform.forward * lockOnSphereOffset, lockOnSphereRad);
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

        //LockOn Sphere
        //Gizmos.DrawWireSphere(transform.position + transform.forward * lockOnSphereOffset, lockOnSphereRad);

        //Interact Sphere
        //Gizmos.DrawWireSphere(transform.position + transform.forward * interactSphereOffset, interactSphereRad);

        //IsGrounded

    }

    private void LockOnTarget()
    {
        targetImage.transform.position = playerCam.WorldToScreenPoint(currentTarget.transform.position);
    }
}
