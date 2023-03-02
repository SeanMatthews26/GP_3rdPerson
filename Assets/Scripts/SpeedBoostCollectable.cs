using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedBoostCollectable : Collectable
{
    [SerializeField] float powerDuration;
    [SerializeField] float boostedSpeed;
    float normalSpeed;
    PlayerControls playerControls;

    public void Awake()
    {
        pd = powerDuration;
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        playerControls = player.GetComponent<PlayerControls>();
        normalSpeed = playerControls.movementForce;
    }

    private void Update()
    {
        Rotate();
        Power();
    }

    private void Power()
    {
        if (activated)
        {
            playerControls.movementForce = boostedSpeed;
        }
        else
        {
            playerControls.movementForce = normalSpeed;
        }
    }
}
