using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraJump : Collectable
{
    [SerializeField] float powerDuration;
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
            Debug.Log("Power");
        }
    }
}
