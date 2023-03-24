using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlatPlayerRoute : Route
{
    //2.5D Test
    float currentClosestDistance = Mathf.Infinity;
    float tDistance = 0;
    Vector3 currentClosestPosition = Vector3.zero;
    [SerializeField] bool findClosest;
    private Transform[] splinePoints;
    private int splineNum;
    private int pointPerSpline;

    private Vector3 gizmosPosition;

    // Start is called before the first frame update
    void Start()
    {
        pointPerSpline = 4;
        splineNum = 1;

        for (int i = 0; i < pointPerSpline; i++)
        {
            splinePoints[i] = this.gameObject.transform.GetChild(0).gameObject.transform.GetChild(i);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FindClosest()
    {
        


        //2.5D Test
        if (findClosest)
        {
            for (float t = 0; t <= 1; t += 0.04f)
            {
                gizmosPosition = Mathf.Pow(1 - t, 3) * splinePoints[0].position + 3 * Mathf.Pow(1 - t, 2) * t * splinePoints[1].position + 3 * (1 - t) * Mathf.Pow(t, 2) * splinePoints[2].position + Mathf.Pow(t, 3) * splinePoints[3].position;

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
            //Line to Player
            Gizmos.DrawLine(currentClosestPosition, player.transform.position);
        }

        //anchors
        Gizmos.DrawLine(new Vector3(splinePoints[0].position.x, splinePoints[0].position.y, splinePoints[0].position.z), new Vector3(splinePoints[1].position.x, splinePoints[1].position.y, splinePoints[1].position.z));
        Gizmos.DrawLine(new Vector3(splinePoints[2].position.x, splinePoints[2].position.y, splinePoints[2].position.z), new Vector3(splinePoints[3].position.x, splinePoints[3].position.y, splinePoints[3].position.z));

    }

    private void OnDrawGizmos()
    {
        //FindClosest();
    }
}

