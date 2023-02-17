using RPGCharacterAnims.Actions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonCamera : MonoBehaviour
{
    //Input
    private ThirdPersonInput playerActionAsset;
    private InputAction look;

    public Transform player;
    public float distance = 3;
    public float smoothTime = 0.25f;
    Vector3 currentVelocity;

    private float pitch;
    private float yaw;
    private float sensitivity = 10f;

    private void Awake()
    {
        playerActionAsset = new ThirdPersonInput();
    }

    private void OnEnable()
    {
        look = playerActionAsset.PlayerActions.Look;
   
    }


    void LateUpdate()
    {
        /*Vector3 target = player.position + (transform.position - player.position).normalized * distance;
        transform.position = Vector3.SmoothDamp(transform.position, target, ref currentVelocity, smoothTime);
        transform.LookAt(player);

        //Adjust if Camera is too low
        if (target.y < player.position.y)
        {
            target.y = player.position.y;
        }*/
    }
}
    