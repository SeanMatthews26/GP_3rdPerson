using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spline : MonoBehaviour
{
    [SerializeField] private Transform[] splinePoints;
    private int pointCount;

    //2.5D Test
    [SerializeField] GameObject player;
    float currentClosestDistance = Mathf.Infinity;
    float tDistance = 0;
    Vector3 currentClosestPosition = Vector3.zero;

    private Vector3 gizmosPosition;

    private void Start()
    {
        pointCount = transform.childCount;

        for (int i = 0; i < pointCount; i++)
        {
            splinePoints[i] = transform.GetChild(i).transform;
        }
    }

    private void OnDrawGizmos()
    {
        

        for (float t = 0; t <= 1; t += 0.04f)
        {
            gizmosPosition = Mathf.Pow(1 - t, 3) * splinePoints[0].position + 3 * Mathf.Pow(1 - t, 2) * t * splinePoints[1].position + 3 * (1 - t) * Mathf.Pow(t, 2) * splinePoints[2].position + Mathf.Pow(t, 3) * splinePoints[3].position;

            Gizmos.DrawSphere(gizmosPosition, 0.25f);
        }

        //2.5D Test
        for (float t = 0; t <= 1; t += 0.04f)
        {
            gizmosPosition = Mathf.Pow(1 - t, 3) * splinePoints[0].position + 3 * Mathf.Pow(1 - t, 2) * t * splinePoints[1].position + 3 * (1 - t) * Mathf.Pow(t, 2) * splinePoints[2].position + Mathf.Pow(t, 3) * splinePoints[3].position;

            //Gizmos.DrawSphere(gizmosPosition, 0.25f);

            //currentClosest = (gizmosPosition - player.transform.position).sqrMagnitude;

            tDistance = (player.transform.position - gizmosPosition).sqrMagnitude;

            //Check if this is closest
            if(gizmosPosition == currentClosestPosition)
            {
                currentClosestDistance = tDistance;
            }

            if (currentClosestDistance > tDistance)
            {
                currentClosestDistance = tDistance;
                currentClosestPosition = gizmosPosition;
            }

            
        }
        Gizmos.DrawLine(currentClosestPosition, player.transform.position);

        Gizmos.DrawLine(new Vector3(splinePoints[0].position.x, splinePoints[0].position.y, splinePoints[0].position.z), new Vector3(splinePoints[1].position.x, splinePoints[1].position.y, splinePoints[1].position.z));
        Gizmos.DrawLine(new Vector3(splinePoints[2].position.x, splinePoints[2].position.y, splinePoints[2].position.z), new Vector3(splinePoints[3].position.x, splinePoints[3].position.y, splinePoints[3].position.z));

    }

    private void Update()
    {
        /*if(pointCount> 0)
        {
            for (int i = 0; i < pointCount; i++)
            {
                Debug.DrawLine(splinePoints[i].position, splinePoints[i + 1].position, Color.white);
            }
        }*/
    }
}
