using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

public class FlatPlayerRoute : Route
{
    GameObject[] spline;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        for (int i = 0; i < transform.childCount; i++)
        {
            spline[i] = transform.GetChild(i).gameObject;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(spline.Length);
    }


    private void OnDrawGizmos()
    {
        //Find closest line
        Gizmos.DrawLine(spline[0].GetComponent<Spline>().currentClosestPosition, player.transform.position);
    }
}

