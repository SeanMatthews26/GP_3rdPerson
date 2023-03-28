using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [HideInInspector] public NavMeshAgent agent;
    [HideInInspector] public Transform playerTrans;
    public LayerMask groundLayer, playerLayer;

    //Wander
    public Vector3 walkPoint;
    private bool walkPointSet;
    public float walkPointRange;
    private Vector3 startPos;
    [SerializeField] float wanderRange;
    private Vector3 wanderDestination;

    //Attacking
    public float timeBetweenAttacks;
    private bool attacked;

    //States
    enum State { wander, chase, attack };
    State currentState;
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    //Combat
    [SerializeField] private float health;

    // Start is called before the first frame update
    void Start()
    {
        currentState = State.wander;
        startPos= transform.position;

        playerTrans = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, playerLayer);

        //States
        if(currentState == State.wander)
        {
            Wander();
        }
        if (currentState == State.chase)
        {
            Chase();
        }
    }

    void Chase()
    {
        agent.destination = playerTrans.position;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }

    private void Wander()
    {
        if (playerInSightRange)
        {
            Debug.Log("PlayerSpotted");
            currentState = State.chase;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Hit");
    }
}
