using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [HideInInspector] public NavMeshAgent agent;
    [HideInInspector] public Transform playerTrans;
    public LayerMask groundLayer, playerLayer;
    private Rigidbody rb;

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

    //Attacking
    [SerializeField] float attackSpeed;
    public float timeBetweenAttacks;
    private bool attacked;
    public bool playerInAttackRange;
    public float attackRange;
    [SerializeField] private float jumpForce;
    

    //States
    enum State 
    {
        WANDER,
        CHASE,
        ATTACK
    };

    State currentState;


    //Combat
    [SerializeField] private float health;

    // Start is called before the first frame update
    void Start()
    {
        currentState = State.WANDER;
        startPos= transform.position;

        playerTrans = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        agent.acceleration = 40;
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(currentState.ToString());

        agent.destination = playerTrans.position;

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

        if (playerInAttackRange)
        {
            currentState = State.ATTACK;
        }
    }

    void Attack()
    {
        agent.speed = attackSpeed;
        rb.AddForce(jumpForce * Vector3.up, ForceMode.Impulse);
        currentState = State.WANDER;
    }


    void ChangeStatesToWander()
    {
        currentState = State.WANDER;
    }

    private void OnDrawGizmos()
    {
        //Sight Range
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, sightRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }


    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Hit");
    }
}
