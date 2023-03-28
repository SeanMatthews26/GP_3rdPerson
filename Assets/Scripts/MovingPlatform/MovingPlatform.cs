using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    Rigidbody rb;
    public Vector3 platVelo;
    Vector3 lastPosition;

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
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("Platform");
            //other.transform.parent = transform;
            other.GetComponent<PlayerControls>().onPlatform = true;
            other.GetComponent<PlayerControls>().currentPlat = this.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            //other.transform.parent = null;
            other.GetComponent<PlayerControls>().onPlatform = false;
        }
    }
}
