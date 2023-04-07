using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public Transform target; // The target to hop towards
    public float hopInterval = 2f; // The time interval between hops
    public float hopForce = 5f; // The force of the hop
    public float upwardForce = 5f; // The upward force of the hop
    public float forwardForce = 2f; // The forward force of the hop

    private Rigidbody rb;
    private bool isHopping = false;
    private float hopTimer = 0f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (!isHopping)
        {
            hopTimer += Time.deltaTime;
            if (hopTimer >= hopInterval)
            {
                isHopping = true;
                hopTimer = 0f;
                Hop();
            }
        }
    }

    private void Hop()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Vector3 upwardDirection = Vector3.up;
        Vector3 forwardDirection = transform.forward;
        rb.AddForce((upwardDirection * upwardForce) + (forwardDirection * forwardForce) + (direction * hopForce), ForceMode.Impulse);
        isHopping = false;
    }
}

