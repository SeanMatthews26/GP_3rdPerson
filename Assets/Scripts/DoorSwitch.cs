using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class DoorSwitch : MonoBehaviour
{
    PlayerControls playerControls;
    GameObject player;
    PlayerAnimation playerAnimation;
    Vector3 playerSwitchPos;
    [SerializeField] float offset;
    Collider col;
    GameObject door;


    void Awake()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        playerControls = FindObjectOfType<PlayerControls>();
        player = GameObject.FindGameObjectWithTag("Player");
        playerSwitchPos = transform.position + (transform.right * offset);
        playerAnimation= FindObjectOfType<PlayerAnimation>();
        col = GetComponent<Collider>();
        door = transform.GetChild(0).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Switch()
    {
        Vector3 newPos = new Vector3(playerSwitchPos.x, player.transform.position.y, playerSwitchPos.z);
        player.transform.position = Vector3.MoveTowards(player.transform.position, newPos, 5000 * Time.deltaTime);
        player.transform.forward = -transform.right;
        Debug.Log("Door");
        if(door.active)
        {
            door.SetActive(false);
        }
        else
        {
            door.SetActive(true);
        }
    }
}
