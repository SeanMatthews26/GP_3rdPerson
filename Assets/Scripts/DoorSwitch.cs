using System.Collections;
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
    [SerializeField] float camSwitchDelay;
    [SerializeField] GameObject sword;

    private bool opening = false;
    private bool closing = false;
    private bool isOpen = false;
    private float shrinkTime = 1;
    private float currentTime = 0;
    private Vector3 scale;
    private Vector3 originalDoorPos;
    private Vector3 closedDoorPos;


    void Awake()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        playerControls = FindObjectOfType<PlayerControls>();
        player = GameObject.FindGameObjectWithTag("Player");
        playerSwitchPos = transform.position + (transform.right * offset);
        playerAnimation = FindObjectOfType<PlayerAnimation>();
        col = GetComponent<Collider>();
        door = transform.GetChild(0).gameObject;
        switchCam = transform.GetChild(1).gameObject;
        doorCam = transform.GetChild(2).gameObject;
        scale = door.transform.localScale;
        originalDoorPos = door.transform.position;
        closedDoorPos = originalDoorPos + Vector3.up * (100);
    }

    // Update is called once per frame
    void Update()
    {
        if (opening)
        {
            currentTime += Time.deltaTime;
            float shrinkLerp = currentTime / shrinkTime;
            door.transform.localScale = new Vector3(scale.x, scale.y, Mathf.Lerp(scale.z, 0f, shrinkLerp));
            //door.transform.position = new Vector3(originalDoorPos.x, originalDoorPos.y, Mathf.Lerp(originalDoorPos.z, closedDoorPos.z, shrinkLerp));
            door.transform.position = Vector3.MoveTowards(originalDoorPos, closedDoorPos, shrinkLerp * 2.5f);

            if (shrinkLerp >= 1f)
            {
                opening = false;
                isOpen = true;
                currentTime = 0f;
            }
        }

        if(closing )
        {
            door.transform.localScale = new Vector3(scale.x, scale.y, scale.z);
            door.transform.position = originalDoorPos;

            closing = false;
            isOpen = false;
        }

    }

    public void Switch()
    {
        playerControls.playerActionAsset.Disable();
        playerControls.forceDirection = Vector3.zero;

        //Change Player Pos
        Vector3 newPos = new Vector3(playerSwitchPos.x, player.transform.position.y, playerSwitchPos.z);
        player.GetComponent<Rigidbody>().velocity = Vector3.zero;
        player.transform.position = newPos;
        player.transform.forward = -transform.right;
        sword.GetComponent<Collider>().enabled = false;

        //Camera Pos
        playerControls.camEnabled = false;
        playerControls.playerCam.transform.position = switchCam.transform.position;
        playerControls.playerCam.transform.rotation = switchCam.transform.rotation;

        Invoke("SwitchToDoorCam", camSwitchDelay);
        Invoke("OpenOrClose", camSwitchDelay * 2);
        Invoke("ResetPlayer", camSwitchDelay * 3);
    }

    private void OpenOrClose()
    {
        if (!isOpen)
        {
            opening = true;
        }
        else
        {
            closing = true;
        }
    }

    void SwitchToDoorCam()
    {
        playerControls.playerCam.transform.position = doorCam.transform.position;
        playerControls.playerCam.transform.rotation = doorCam.transform.rotation;
    }

    void ResetPlayer()
    {
        playerControls.camEnabled = true;
        playerControls.playerActionAsset.Enable();
        sword.GetComponent<Collider>().enabled = true;
    }
}


