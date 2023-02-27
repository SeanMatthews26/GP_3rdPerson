using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedBoostCollectable : Collectable
{
    [SerializeField] float powerDuration;
    [SerializeField] PlayerControls playerControls;

    public void Awake()
    {
        pd = powerDuration;
    }

    public void Update()
    {

    }

  

}
