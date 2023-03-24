using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlatPlayerRoute : Route
{
    //2.5D Test
    private GameObject player;
    float currentClosestDistance = Mathf.Infinity;
    float tDistance = 0;
    Vector3 currentClosestPosition = Vector3.zero;
    [SerializeField] bool findClosest;

    private Vector3 gizmosPosition;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

   private void FindClosest(int routeNum)
    {
        Vector3 p0 = routes[routeNum].GetChild(0).position;
        Vector3 p1 = routes[routeNum].GetChild(1).position;
        Vector3 p2 = routes[routeNum].GetChild(2).position;
        Vector3 p3 = routes[routeNum].GetChild(3).position;

        //2.5D Test
        if (findClosest)
        {
            for (float t = 0; t <= 1; t += 0.04f)
            {
                gizmosPosition = Mathf.Pow(1 - t, 3) * p0 + 3 * Mathf.Pow(1 - t, 2) * t * p1 + 3 * (1 - t) * Mathf.Pow(t, 2) * p2 + Mathf.Pow(t, 3) * p3;

                //Gizmos.DrawSphere(gizmosPosition, 0.25f);

                //currentClosest = (gizmosPosition - player.transform.position).sqrMagnitude;

                tDistance = (player.transform.position - gizmosPosition).sqrMagnitude;

                //Check if this is closest
                if (gizmosPosition == currentClosestPosition)
                {
                    currentClosestDistance = tDistance;
                }

                if (currentClosestDistance > tDistance)
                {
                    currentClosestDistance = tDistance;
                    currentClosestPosition = gizmosPosition;
                    Debug.Log(t);
                }


            }
            Gizmos.DrawLine(currentClosestPosition, player.transform.position);
        }

        Gizmos.DrawLine(new Vector3(p0.x, p0.y, p0.z), new Vector3(p1.x, p1.y, p1.z));
        Gizmos.DrawLine(new Vector3(p2.x, p2.y, p2.z), new Vector3(p3.x, p3.y, p3.z));
       
    }

    private void OnDrawGizmos()
    {
        FindClosest(routeToGo);
    }
}
