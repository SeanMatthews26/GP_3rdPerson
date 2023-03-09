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
    GameObject switchCam;
    GameObject doorCam;
    [SerializeField] float camSpeed;
    [SerializeField] float camSwitchDelay;


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
        switchCam = transform.GetChild(1).gameObject;
        doorCam = transform.GetChild(2).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Switch()
    {
        //Change Player Pos
        Vector3 newPos = new Vector3(playerSwitchPos.x, player.transform.position.y, playerSwitchPos.z);
        player.transform.position = newPos;
        player.transform.forward = -transform.right;
        
        //Camera Pos
        playerControls.camEnabled = false;
        playerControls.playerCam.transform.position = switchCam.transform.position;
        playerControls.playerCam.transform.rotation = switchCam.transform.rotation;
        Invoke("SwitchToDoorCam", camSwitchDelay);


        if (door.active)
        {
            door.SetActive(false);
        }
        else
        {
            door.SetActive(true);
        }
    }

    void SwitchToDoorCam()
    {
        playerControls.playerCam.transform.position = Vector3.MoveTowards(playerControls.playerCam.transform.position, doorCam.transform.position, camSpeed * Time.deltaTime);
        playerControls.playerCam.transform.rotation = Quaternion.Slerp(playerControls.playerCam.transform.rotation, doorCam.transform.rotation, camSpeed * Time.deltaTime);
    }
}
