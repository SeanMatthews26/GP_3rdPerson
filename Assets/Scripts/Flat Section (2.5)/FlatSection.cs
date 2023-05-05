using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlatSection : MonoBehaviour
{
    [SerializeField] GameObject camSpline;
    [SerializeField] GameObject cam;
    [SerializeField] GameObject player;
    private PlayerControls playerControls;
    private FlatSpline flatSpline;
    [SerializeField] float camSpeed;

    // Start is called before the first frame update
    void Start()
    {
        playerControls = player.GetComponent<PlayerControls>();
        flatSpline = camSpline.GetComponent<FlatSpline>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            playerControls.camEnabled = false;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            cam.transform.position = Vector3.MoveTowards(cam.transform.position, flatSpline.currentClosestPosition, camSpeed * Time.deltaTime);
            cam.transform.LookAt(player.transform.position);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            playerControls.camEnabled = true;
        }
    }
}
