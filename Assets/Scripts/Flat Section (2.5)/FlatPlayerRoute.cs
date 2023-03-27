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
    private GameObject[] spline;

    private Vector3 gizmosPosition;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        pointPerSpline = 4;
        splineNum = 1;

        //Init Splines
        for (int i = 0; i < this.gameObject.transform.childCount; i++)
        {
            spline[i] = this.gameObject.transform.GetChild(i).gameObject;
        }

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
       for(int i = 0; i < spline.Length; i++)
        {
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
                    Debug.Log(t);
                }
            }
        }
    }

   
}

