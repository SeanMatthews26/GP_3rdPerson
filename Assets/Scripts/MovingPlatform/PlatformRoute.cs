using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformRoute : Route
{
    private GameObject obj;
    [SerializeField] private float speedModifier;

    private Rigidbody rb;
    private Vector3 objectPosition;
    private bool coroutineAllowed;
    public Vector3 movement;
    private MovingPlatform movingPlatform;
    private PlayerControls playerControls;

    // Start is called before the first frame update
    void Start()
    {
        coroutineAllowed = true;

        obj = transform.GetChild(0).gameObject;
        rb = obj.GetComponent<Rigidbody>();
        movingPlatform = obj.GetComponent<MovingPlatform>();
        playerControls = player.GetComponent<PlayerControls>();
    }

    void FixedUpdate()
    {
        if (coroutineAllowed)
        {
            StartCoroutine(GoByTheRoute(routeToGo));
        }
    }

    private IEnumerator GoByTheRoute(int routeNum)
    {
        coroutineAllowed = false;

        Vector3 p0 = routes[routeNum].GetChild(0).position;
        Vector3 p1 = routes[routeNum].GetChild(1).position;
        Vector3 p2 = routes[routeNum].GetChild(2).position;
        Vector3 p3 = routes[routeNum].GetChild(3).position;

        while (tParam < 1)
        {
            tParam += Time.deltaTime * speedModifier;

            objectPosition = Mathf.Pow(1 - tParam, 3) * p0 + 3 * Mathf.Pow(1 - tParam, 2) * tParam * p1 + 3 * (1 - tParam) * Mathf.Pow(tParam, 2) * p2 + Mathf.Pow(tParam, 3) * p3;

            movement = (objectPosition - obj.transform.position) * Time.deltaTime;

            if (movingPlatform.playerOnPlat)
            {
                //playerControls.AddPlatformMovement(movement);
            }
            else
            {
               
            }

            //obj.transform.position = objectPosition;
            obj.GetComponent<Rigidbody>().Move(objectPosition, transform.rotation);
            yield return new WaitForEndOfFrame();
        }

        tParam = 0;
        routeToGo += 1;

        if (routeToGo > routes.Length - 1)
        {
            routeToGo = 0;
        }

        coroutineAllowed = true;

    }

    public Vector3 GetMovement()
    {
        return movement;
    }
}
