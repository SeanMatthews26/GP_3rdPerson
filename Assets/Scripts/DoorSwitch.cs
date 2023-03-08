using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class DoorSwitch : MonoBehaviour
{
    PlayerControls playerControls;
    GameObject player;
    Vector3 playerSwitchPos;
    [SerializeField] float offset;
    Collider col;
    

    // Start is called before the first frame update
    void Start()
    {
        playerControls = FindObjectOfType<PlayerControls>();
        player = GameObject.FindGameObjectWithTag("Player");
        playerSwitchPos = transform.position + (transform.right * offset);
        col = GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        /*if (playerControls.currentTarget == this.gameObject && playerControls.lockedOn)
        {
            Vector3 newPos = new Vector3(playerSwitchPos.x, player.transform.position.y, playerSwitchPos.z);
            player.transform.position = Vector3.MoveTowards(player.transform.position, newPos, playerControls.maxSpeed);
        }*/

        if(playerControls.interacting)
        {
            Collider[] hits = Physics.OverlapSphere(player.transform.position + player.transform.forward * playerControls.interactSphereOffset, playerControls.interactSphereRad);
            
            foreach(Collider hit in hits)
            {
                if(hit == col)
                {
                    Vector3 newPos = new Vector3(playerSwitchPos.x, player.transform.position.y, playerSwitchPos.z);
                    player.transform.position = Vector3.MoveTowards(player.transform.position, newPos, playerControls.maxSpeed);
                    player.transform.forward = -transform.right;
                }
            }
        }
    }
}
