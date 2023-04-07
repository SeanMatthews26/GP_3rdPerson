using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public Transform player;
    public float attackDistance = 2f;
    public float moveSpeed = 3f;
    public float jumpForce = 10f;
    public float jumpUpwardsForce = 5f; // added upwards force for the jump
    private enum State { Idle, Chase }
    private State state = State.Idle;
    private Rigidbody rb;
    private float lastJumpTime = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        switch (state)
        {
            case State.Idle:
                if (distanceToPlayer < attackDistance)
                {
                    state = State.Chase;
                }
                break;

            case State.Chase:
                if (Time.time > lastJumpTime + 2f)
                {
                    Vector3 direction = (player.position - transform.position).normalized;
                    // add upwards force to the jump by modifying the direction vector
                    direction += Vector3.up * jumpUpwardsForce;
                    rb.AddForce(direction.normalized * jumpForce, ForceMode.Impulse);
                    lastJumpTime = Time.time;
                }
                transform.LookAt(player.position + (Vector3.up * 3));
                //transform.position += transform.forward * moveSpeed * Time.deltaTime;
                if (distanceToPlayer > attackDistance)
                {
                    state = State.Idle;
                }
                break;
        }
    }
}
