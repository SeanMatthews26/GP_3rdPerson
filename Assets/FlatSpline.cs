using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FlatSpline : Spline
{
        //2.5D Test
    [SerializeField] private GameObject player;
    float currentClosestDistance = Mathf.Infinity;
    float tDistance = 0;
    public Vector3 currentClosestPosition = Vector3.zero;
    Vector3 gizmosPosition;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FindClosestSplinePoint()
    {
        //2.5D Test
        for (float t = 0; t <= 1; t += 0.04f)
        {
            gizmosPosition = Mathf.Pow(1 - t, 3) * splinePoints[0].position + 3 * Mathf.Pow(1 - t, 2) * t * splinePoints[1].position + 3 * (1 - t) * Mathf.Pow(t, 2) * splinePoints[2].position + Mathf.Pow(t, 3) * splinePoints[3].position;
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
            }
        }

        //Find closest line
        Gizmos.DrawLine(currentClosestPosition, player.transform.position);
    }

    public override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        FindClosestSplinePoint();
    }
}
