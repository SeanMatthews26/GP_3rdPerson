using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spline : MonoBehaviour
{
    [SerializeField] private Transform[] splinePoints;
    private int pointCount;

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
        for (float t = 0; t <= 1; t += 0.05f)
        {
            gizmosPosition = Mathf.Pow(1 - t, 3) * splinePoints[0].position + 3 * Mathf.Pow(1 - t, 2) * t * splinePoints[1].position + 3 * (1 - t) * Mathf.Pow(t, 2) * splinePoints[2].position + Mathf.Pow(t, 3) * splinePoints[3].position;

            Gizmos.DrawSphere(gizmosPosition, 0.25f);
        }

        Gizmos.DrawLine(new Vector3(splinePoints[0].position.x, splinePoints[0].position.y, splinePoints[0].position.z), new Vector3(splinePoints[1].position.x, splinePoints[1].position.y, splinePoints[1].position.z));
        Gizmos.DrawLine(new Vector3(splinePoints[2].position.x, splinePoints[2].position.y, splinePoints[2].position.z), new Vector3(splinePoints[3].position.x, splinePoints[3].position.y, splinePoints[3].position.z));

    }
}
