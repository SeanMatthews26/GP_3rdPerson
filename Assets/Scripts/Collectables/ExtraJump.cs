using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraJump : Collectable
{
    [SerializeField] float powerDuration;
    [SerializeField] int boostedExtraJumps = 1;
    [SerializeField] int normalExtraJumps = 0;
    PlayerControls playerControls;

    public void Awake()
    {
        pd = powerDuration;
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        playerControls = player.GetComponent<PlayerControls>();
    }

    private void Update()
    {
        Rotate();
        Power();
    }

    private void Power()
    {
        if(activated)
        {
            playerControls.extraJumps = boostedExtraJumps;
            playerControls.extraJumpParticles.Play();
        }
        else
        {
            playerControls.extraJumps = normalExtraJumps;
            playerControls.extraJumpParticles.Stop();
        }
    }
}
