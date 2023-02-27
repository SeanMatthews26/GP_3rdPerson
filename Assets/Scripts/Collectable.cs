using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    [HideInInspector] public Rigidbody rb;
    [HideInInspector] public BoxCollider boxCollider;
    [HideInInspector] public MeshRenderer mesh;
    [HideInInspector] public bool activated = false;
    [HideInInspector] public float pd;

    [SerializeField] Vector3 rotation;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        boxCollider.isTrigger = true;
        mesh = GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    public void Update()
    {
        rb.transform.Rotate(rotation * Time.deltaTime);
    }

     private void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject);

    }
}
