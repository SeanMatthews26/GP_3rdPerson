using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    Rigidbody rb;
    public Vector3 platVelo;
    public bool playerOnPlat;
    [SerializeField] GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player)
        {
            playerOnPlat= true;
            player.GetComponent<PlayerControls>().onPlatform = true;
            player.GetComponent<PlayerControls>().currentPlat = this.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == player)
        {
            playerOnPlat= false;
            player.GetComponent<PlayerControls>().onPlatform = false;
            player.GetComponent<PlayerControls>().currentPlat = null;
        }
    }
}
