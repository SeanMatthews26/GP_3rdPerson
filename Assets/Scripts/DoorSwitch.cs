using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DoorSwitch : MonoBehaviour
{
    PlayerControls playerControls;
    GameObject player;
    Vector3 playerSwitchPos;
    [SerializeField] float offset;
    
    

    // Start is called before the first frame update
    void Start()
    {
        playerControls = FindObjectOfType<PlayerControls>();
        player = GameObject.FindGameObjectWithTag("Player");
        playerSwitchPos = transform.position + (transform.right * offset);
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 switchDistance = new Vector2(player.transform.position.x - transform.position.x, player.transform.position.y - transform.position.y);

        Debug.Log(switchDistance);

        if (playerControls.currentTarget == this.gameObject)
        {
            player.transform.position = new Vector3(playerSwitchPos.x, player.transform.position.y, playerSwitchPos.z);
        }
    }
}
