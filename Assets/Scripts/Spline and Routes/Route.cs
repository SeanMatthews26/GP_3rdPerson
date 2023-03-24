using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Route : MonoBehaviour
{
    [SerializeField] public Transform[] routes;

    [HideInInspector] public int routeToGo;
    [HideInInspector] public float tParam;
    [HideInInspector] public GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        routeToGo = 0;
        tParam = 0f;
    }
    
}
