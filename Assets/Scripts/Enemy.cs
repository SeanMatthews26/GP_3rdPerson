using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using UnityEngine.UIElements.Experimental;
using static UnityEngine.GraphicsBuffer;

public class Enemy : MonoBehaviour
{
    [HideInInspector] public NavMeshAgent agent;
    [HideInInspector] public GameObject player;
    public LayerMask groundLayer, playerLayer;
    private Rigidbody rb;
    private PlayerControls playerControls;
    private Renderer renderer;
    private EnemyManager enemyManager;
    private Camera cam;

    //Wander
    public Vector3 walkPoint;
    private bool walkPointSet;
    public float walkPointRange;
    private Vector3 startPos;
    [SerializeField] float wanderRange;
    private Vector3 wanderDestination;

    //Chase
    [SerializeField] float enemySpeed;
    public bool playerInSightRange;
    public float sightRange;

    //Attacking/Combat
    [SerializeField] float attackSpeed;
    [SerializeField] private float health;
    [SerializeField] float startingHealth;
    public float timeBetweenAttacks;
    private bool attacked = false;
    public bool playerInAttackRange;
    public float attackRange;
    [SerializeField] private float jumpForce;
    private bool damageTaken = false;
    [SerializeField] Material normalMat;
    [SerializeField] Material damagedMat;
    [SerializeField] Material attackMat;
    public int lives = 3;
    public bool attackingOne = true;
    public bool canAttack = true;
    [SerializeField] Image healthBar;
    [SerializeField] private Canvas canvas;

    //Projectile
    [SerializeField] GameObject projectile;
    [SerializeField] float targetAbovePlayer;
    [SerializeField] float projectileSpeed;
    

    //States
    public enum State 
    {
        WANDER,
        CHASE,
        ATTACK,
        RETREAT
    };

    public State currentState;

    // Start is called before the first frame update
    void Start()
    {
        currentState = State.WANDER;
        startPos= transform.position;

        player = GameObject.FindGameObjectWithTag("Player");
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        renderer = GetComponent<Renderer>();
        renderer.material= normalMat;
        playerControls = player.GetComponent<PlayerControls>();
        health = startingHealth;
        enemyManager= FindObjectOfType<EnemyManager>();
        cam = Camera.main;
    }

    private IEnumerator Awake()
    {
        Invincibility();
        yield return null;
    }

    // Update is called once per frame
    void Update()
    {
        agent.acceleration = enemySpeed;

        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, playerLayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, playerLayer);

        //States
        if(currentState == State.WANDER)
        {
            Wander();
        }
        if (currentState == State.CHASE)
        {
            Invoke(nameof(Chase), 0.5f);
        }
        if(currentState == State.ATTACK) 
        {
            Attack();
        }
        if(currentState == State.RETREAT)
        {
            Retreat();
        }

        Health();
        HealthbarRotation();
    }

    private void Wander()
    {
        agent.speed = 0;

        if (playerInSightRange && canAttack)
        {
            currentState = State.CHASE;
        }
    }

    void Chase()
    {
        agent.speed = enemySpeed;
        agent.destination = player.transform.position;
        transform.LookAt(player.transform.position);

        if (playerInAttackRange)
        {
            currentState = State.ATTACK;
        }
    }

    void Attack()
    {
        if(attackingOne && canAttack)
        {
            agent.destination = transform.position;

            Invoke(nameof(AttackLeap), 1f);
            Invoke(nameof(ResetWander), 3f);
            Invoke(nameof(SetCanAttackFalse), 3f);
            Invoke(nameof(SetCanAttackTrue), 4.5f);
        }

        if(!attackingOne)
        {
            agent.SetDestination(transform.position);
        }

        if(!playerInAttackRange && !canAttack)
        {
            currentState = State.CHASE;
        }
    }

    private void AttackLeap()
    {
        agent.transform.position = transform.position + (transform.forward * 5 * Time.deltaTime);
    }

    private void Retreat()
    {
        agent.transform.position = transform.position - (transform.forward * 4 * Time.deltaTime);

        Invoke(nameof(ResetWander), 2f);
    }

    public void SetAttacking(bool newAttacking)
    {
        attackingOne = newAttacking;
    }

    private void OnDrawGizmos()
    {
        //Sight Range
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, sightRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }


    void OnTriggerEnter(Collider other)
    {
        if (damageTaken)
        {
            return;
        }

        if (other.gameObject == playerControls.sword && playerControls.attacking)
        {
            TakeDamage();
            return;
        }
        
        if(other.gameObject == player)
        {
            currentState = State.RETREAT;

            if (playerControls.invincible)
            {
                return;
            }

            playerControls.TakeDamage();
        }
    }

    private void ResetAttack()
    {
        attacked= false;
    }

    private void ResetWander()
    {
        currentState= State.WANDER;
    }

    private void ResetRetreat()
    {
        currentState = State.RETREAT;
    }

    private void Health()
    {
        if(health <= 0)
        {
            lives--;
            if(lives <= 0)
            {
                LockOff();
                Destroy(gameObject);
                return;
            }

            transform.position += Vector3.right * -2;
            transform.localScale *= 0.7f;
            startingHealth--;
            health = startingHealth;
            UpdateHealthbar();

            Instantiate(this, transform.position + Vector3.right * 2, Quaternion.identity);
        }
    }

    private void ResetColour()
    {
        renderer.material = normalMat;
    }

    private IEnumerator Invincibility()
    {
        damageTaken = true;
        yield return new WaitForSeconds(0.5f);
        damageTaken = false;
    }

    private void SetCanAttackTrue()
    {
        canAttack= true;
    }

    private void SetCanAttackFalse()
    {
        canAttack = false;
    }

    private void TakeDamage()
    {
        renderer.material = damagedMat;
        Invoke(nameof(ResetColour), 0.2f);
        health--;
        Invincibility();
        UpdateHealthbar();
    }

    private void UpdateHealthbar()
    {
        healthBar.fillAmount = health / startingHealth;
    }

    private void HealthbarRotation() 
    {
        canvas.transform.rotation = cam.transform.rotation;
    }

    private void LockOff()
    {
        if(playerControls.currentTarget == this.gameObject)
        {
            playerControls.lockedOn= false;
        }
    }
}
