using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedBoostCollectable : Collectable
{
    [SerializeField] float powerDuration;
    [SerializeField] float boostedSpeed;
    //float normalSpeed;
    PlayerControls playerControls;

    protected override void Start()
    {
        base.Start();
        pd = powerDuration;
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        playerControls = GameObject.FindObjectOfType<PlayerControls>();
        //normalSpeed = playerControls.movementSpeed;
    }

    protected override void Update()
    {
        Rotate();
        Power();
    }

    protected void Power()
    {
        if (activated)
        {
            playerControls.speedBoosted = true;
            playerControls.speedBoostParticles.Play();
        }
        else
        {
            playerControls.speedBoosted = false;
            playerControls.speedBoostParticles.Stop();
        }
    }
}
