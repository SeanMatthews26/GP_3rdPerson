using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Route : MonoBehaviour
{
    [SerializeField] public Transform[] routes;

    [HideInInspector] public int routeToGo;
    [HideInInspector] public float tParam;

    // Start is called before the first frame update
    void Start()
    {
        routeToGo = 0;
        tParam = 0f;
    }
    
}
