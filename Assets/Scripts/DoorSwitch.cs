using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DoorSwitch : MonoBehaviour
{
    PlayerControls playerControls;

    // Start is called before the first frame update
    void Start()
    {
        playerControls = FindObjectOfType<PlayerControls>();
    }

    // Update is called once per frame
    void Update()
    {
        if(playerControls.currentTarget == this.gameObject)
        {
            Destroy(this.gameObject);
        }
    }
}
