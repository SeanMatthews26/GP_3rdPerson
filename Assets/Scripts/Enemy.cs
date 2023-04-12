using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
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
    private int health;
    [SerializeField] int startingHealth;
    public float timeBetweenAttacks;
    private bool attacked = false;
    public bool playerInAttackRange;
    public float attackRange;
    [SerializeField] private float jumpForce;
    private bool damageTaken = false;
    [SerializeField] Material normalMat;
    [SerializeField] Material damagedMat;
    public int lives = 3;


    //Projectile
    [SerializeField] GameObject projectile;
    [SerializeField] float targetAbovePlayer;
    [SerializeField] float projectileSpeed;
    

    //States
    enum State 
    {
        WANDER,
        CHASE,
        ATTACK
    };

    State currentState;

    // Start is called before the first frame update
    void Start()
    {
        currentState = State.WANDER;
        startPos= transform.position;

        player = GameObject.FindGameObjectWithTag("Player");
        agent = GetComponent<NavMeshAgent>();
        agent.acceleration = 40;
        rb = GetComponent<Rigidbody>();
        renderer = GetComponent<Renderer>();
        renderer.material= normalMat;
        playerControls = player.GetComponent<PlayerControls>();
        health = startingHealth;
    }

    // Update is called once per frame
    void Update()
    {

        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, playerLayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, playerLayer);

        //States
        if(currentState == State.WANDER)
        {
            Wander();
        }
        if (currentState == State.CHASE)
        {
            Chase();
        }
        if(currentState == State.ATTACK) 
        {
            Attack();
        }

        Health();
    }

    private void Wander()
    {
        agent.speed = 0;

        if (playerInSightRange)
        {
            currentState = State.CHASE;
        }
    }

    void Chase()
    {
        agent.speed = enemySpeed;
        agent.destination = player.transform.position;

        if (playerInAttackRange)
        {
            currentState = State.ATTACK;
        }
    }

    void Attack()
    {
        agent.SetDestination(transform.position);

        transform.LookAt(player.transform.position);

        if(!attacked)
        {

            attacked = true;
            Shoot();
            Invoke(nameof(ResetAttack), 2f);
        }

        if(!playerInAttackRange)
        {
            currentState = State.CHASE;
        }
    }

    void Shoot()
    {
        Vector3 target = player.transform.position + (Vector3.up * targetAbovePlayer);
        GameObject x = Instantiate(projectile, transform.position, Quaternion.identity);
        x.GetComponent<Rigidbody>().velocity = (target - x.transform.position).normalized * projectileSpeed;
        //x.transform.position = Vector3.MoveTowards(x.transform.position, target, 1f);
    }

    private void OnDrawGizmos()
    {
        //Sight Range
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, sightRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }


    private IEnumerator OnTriggerEnter(Collider other)
    {
        if (damageTaken)
        {
            yield return null;
        }

        if (other.gameObject == playerControls.sword && playerControls.attacking)
        {
            renderer.material = damagedMat;
            Invoke(nameof(ResetColour), 0.2f);
            damageTaken = true;
            Debug.Log("Hit");
            health--;

            yield return new WaitForSeconds(0.5f);
            damageTaken = false;
        }
    }

    private void ResetAttack()
    {
        attacked= false;
    }

    private void Health()
    {
        if(health <= 0)
        {
            lives--;
            if(lives <= 0)
            {
                Destroy(gameObject);
                return;
            }

            transform.position += Vector3.right * -2;
            transform.localScale *= 0.7f;
            health = startingHealth;
            Instantiate(this, transform.position + Vector3.right * 2, Quaternion.identity);
        }
    }

    private void ResetColour()
    {
        renderer.material = normalMat;
    }

}
