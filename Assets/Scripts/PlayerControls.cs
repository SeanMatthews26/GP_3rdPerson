using RPGCharacterAnims.Actions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using Unity.VisualScripting;
using UnityEngine.InputSystem.HID;

public class PlayerControls : MonoBehaviour
{
    //Input
    public ThirdPersonInput playerActionAsset;
    public InputAction move;
    private InputAction look;

    //movement
    private Rigidbody rb;
    [HideInInspector] public Vector3 forceDirection = Vector3.zero;
    private Vector3 clampedVelo;

    [Header("---Movement---")]
    public float movementSpeed;
    [SerializeField] public float normalMovementSpeed;
    [SerializeField] private float normalMaxSpeed;
    [SerializeField] private float strafeMaxSpeed;
    [SerializeField] public float maxSpeed;
    [SerializeField] private float extraGravity = 1.5f;
    [SerializeField] private float maxFallSpeed;
    [HideInInspector] public bool speedBoosted;
    [SerializeField] float boostedMaxSpeed;

    //Attack
    [Header("---Attack---")]
    [SerializeField] public GameObject sword;


    //Jump
    [Header("---Jump---")]
    [SerializeField] private float jumpForce = 5f;
    [HideInInspector] public int jumpsLeft;
    private float jumpDirection;
    [HideInInspector] public int extraJumps = 0;

    //Camera
    [Header("---Camera---")]
    [SerializeField] public Camera playerCam;
    [HideInInspector] public bool camEnabled = true;
    private float pitch;
    private float yaw;
    [SerializeField] private float camSensitivity = 1f;
    [SerializeField] private float dstToCam2D = 10f;
    [SerializeField] private Vector2 pitchLimits = new Vector2(-40, 85);
    [SerializeField] Vector3 camTargetAbovePlayer;
    private Vector3 playerToCamVector;
    private Vector3 playerToCamDirection;
    [SerializeField] private float headToFootDst;
    [SerializeField] private LayerMask playerLayer;
    Vector3 camVector;
    Vector3 camDir;
    float camDistance;
    ///Occlusion
    Vector3 cameraDirection;
    Vector2 cameraDistanceMinMax;



    //Animation
    [HideInInspector] public bool attackPressed = false;
    [HideInInspector] public bool attacking = false;
    [HideInInspector] public bool jumping = false;
    [HideInInspector] public bool doubleJumping = false;
    [HideInInspector] public bool interactPressed = false;
    [HideInInspector] public bool interacting = false;

    //LockOn
    [Header("---Lock On---")]
    [SerializeField] float camSwitchSpeed;
    [SerializeField] float lockOnSphereOffset;
    [SerializeField] float lockOnSphereRad;
    [SerializeField] Image targetImage;
    private Vector3 offset2D;
    private float offsetSqur;
    private Vector3 offsetNorm;
    [HideInInspector] public bool lockedOn = false;
    private Vector3 lockOnCamPos;
    private Vector3 freeCamPos;
    [HideInInspector] public GameObject currentTarget;

    //Particles
    [Header("---Particles---")]
    [SerializeField] public ParticleSystem speedBoostParticles;
    [SerializeField] public ParticleSystem extraJumpParticles;

    //Interact
    [Header("---Interact---")]
    [SerializeField] public float interactSphereOffset;
    [SerializeField] public float interactSphereRad;

    //Platform
    [HideInInspector] public bool onPlatform = false;
    [HideInInspector] public GameObject currentPlat = null;
    MovingPlatform movingPlatform;
    Vector3 currentPlatVelo;
    private const int normalDrag = 4;
    private const int onPlatDrag = 2;

    private void Awake()
    {
        rb = this.GetComponent<Rigidbody>();
        playerActionAsset = new ThirdPersonInput();
        speedBoostParticles.Stop();
        extraJumpParticles.Stop();
        //extraJumpParticles.enableEmission = false;
    }

    private void Start()
    {
        movingPlatform = FindObjectOfType<MovingPlatform>();

        cameraDirection = transform.localPosition.normalized;
        cameraDistanceMinMax = new Vector2(0.5f, 12);

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
            jumpDirection = jumpForce;
        }
        else
        {
            if(jumpsLeft > 0)
            {
                doubleJumping = true;
                jumpDirection = jumpForce;
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

        //new movement
        Vector2 horizontalVelo;
        horizontalVelo = Vector2.ClampMagnitude(new Vector2(forceDirection.x, forceDirection.z), maxSpeed);
        rb.velocity = new Vector3(horizontalVelo.x, rb.velocity.y, horizontalVelo.y);

        //old movement
        //rb.velocity = Vector3.ClampMagnitude(rb.velocity + forceDirection, maxSpeed); 

        rb.velocity += jumpDirection * Vector3.up;

        //Update current platform velocity
        if (currentPlat != null)
        {
            currentPlatVelo = currentPlat.GetComponent<Rigidbody>().velocity;
        }

        if (!onPlatform)
        {
            rb.drag = normalDrag;
        }
        else
        {
            rb.velocity += currentPlatVelo;
            rb.drag = onPlatDrag;
        }

        forceDirection = Vector3.zero;
        jumpDirection = 0;

        //Extra Gravity
        if(rb.velocity.y < -0.1)
        {
            //rb.velocity -= Vector3.down * Physics.gravity.y * Time.fixedDeltaTime * extraGravity;
            rb.velocity += Vector3.down * extraGravity;
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
        if(attacking)
        {
            return;
        }
        attackPressed = true;
        attacking = true;
    }

    private void Attacking()
    {

    }

    private void DoLockOn(InputAction.CallbackContext obj)
    {
        //Lock On
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
        //Lock Off
        else if(lockedOn)
        {
            pitch = playerCam.transform.eulerAngles.x;
            yaw = playerCam.transform.eulerAngles.y;

            lockedOn = !lockedOn;
        }
    }

    private void Update()
    {
        //Move Input
        forceDirection += move.ReadValue<Vector2>().x * GetCameraRight(playerCam) * movementSpeed;
        forceDirection += move.ReadValue<Vector2>().y * GetCameraForward(playerCam) * movementSpeed;

        playerToCamVector = (transform.position + Vector3.up * headToFootDst - playerCam.transform.position);
        playerToCamDirection = playerToCamVector.normalized;
        RaycastHit[] hits = Physics.RaycastAll(playerCam.transform.position, playerToCamDirection);

        //Attack
        if(attacking)
        {
            Attacking();
        }

        LoseTarget();
        SetSpeed();

        float x = (transform.position - playerCam.transform.position).magnitude;
        Debug.Log(x);
    }

    private void LateUpdate()
    {
        if(camEnabled)
        {
            //Camera Stuff
            if (lockedOn)
            {
                offset2D = transform.position - currentTarget.transform.position;
                offsetNorm = offset2D.normalized;

                targetImage.enabled = true;
                lockOnCamPos = new Vector3(transform.position.x + (offsetNorm.x * dstToCam2D), playerCam.transform.position.y, transform.position.z + (offsetNorm.z * dstToCam2D));
                playerCam.transform.position = Vector3.MoveTowards(playerCam.transform.position, lockOnCamPos, camSwitchSpeed * Time.deltaTime);

                var x = Quaternion.LookRotation(currentTarget.transform.position - playerCam.transform.position);
                playerCam.transform.rotation = Quaternion.Slerp(playerCam.transform.rotation, x, 10 * Time.deltaTime);

                LockOnTarget();
            }
            else
            {
                targetImage.enabled = false;
                yaw += look.ReadValue<Vector2>().x * camSensitivity;
                pitch -= look.ReadValue<Vector2>().y * camSensitivity;
                pitch = Mathf.Clamp(pitch, pitchLimits.x, pitchLimits.y);

                Vector3 targetRotation = new Vector3(pitch, yaw);
                playerCam.transform.eulerAngles = targetRotation;

                freeCamPos = transform.position - (playerCam.transform.forward * dstToCam2D) + camTargetAbovePlayer;

                camVector = freeCamPos - transform.position;
                camDir = camVector.normalized;
                camDistance = camVector.magnitude;

                //playerCam.transform.position = Vector3.MoveTowards(playerCam.transform.position, freeCamPos, camSwitchSpeed * Time.deltaTime);
                playerCam.transform.position = Vector3.MoveTowards(playerCam.transform.position, transform.position + (camDir * camDistance), camSwitchSpeed * Time.deltaTime);
                CamOcclusion();
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

    private void CamOcclusion()
    {
        RaycastHit[] hit = Physics.RaycastAll(transform.position + Vector3.up * headToFootDst, -playerToCamDirection, 12);
        if (hit[0].collider.gameObject.tag != "MainCamera")
        {
            if(hit[0].collider.gameObject.tag == "Player")
            {
                return;
            }
            else
            {
                Debug.Log("Blocked");
                camDistance = Mathf.Clamp(hit[0].distance * 0.8f, cameraDistanceMinMax.x, cameraDistanceMinMax.y);
                playerCam.transform.position = transform.position + (camDir * camDistance);
                playerCam.transform.forward = playerToCamDirection;
            }
        }



        /*if (hits[0].collider.gameObject.tag != "MainCamera")
        {
            camDistance = Mathf.Clamp(hits[0].distance * 0.8f, cameraDistanceMinMax.x, cameraDistanceMinMax.y);
        }
        else
        {
            camDistance = cameraDistanceMinMax.y;
        }*/
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        //LockOn Sphere
        //Gizmos.DrawWireSphere(transform.position + transform.forward * lockOnSphereOffset, lockOnSphereRad);

        //Interact Sphere
        //Gizmos.DrawWireSphere(transform.position + transform.forward * interactSphereOffset, interactSphereRad);

        //IsGrounded

        Gizmos.DrawLine(playerCam.transform.position, playerCam.transform.position + (playerCam.transform.forward * 12));

    }

    private void LockOnTarget()
    {
        targetImage.transform.position = playerCam.WorldToScreenPoint(currentTarget.transform.position);
    }

    private void SetSpeed()
    {
        //Speed Boosted
        if(speedBoosted)
        {
            maxSpeed = boostedMaxSpeed;
            return;
        }

        //Strafing
        if(lockedOn)
        {
            maxSpeed = strafeMaxSpeed;
            return;
        }

        //Regular Speed
        maxSpeed = normalMaxSpeed;
    }

    private void LoseTarget()
    {
        if(currentTarget == null)
        {
            currentTarget = FindTarget();
        }

        if(currentTarget == null)
        {
            pitch = playerCam.transform.eulerAngles.x;
            yaw = playerCam.transform.eulerAngles.y;

            lockedOn = false;
        }
    }
}
