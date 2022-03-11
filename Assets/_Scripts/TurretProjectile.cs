using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretProjectile : MonoBehaviour
{

    Rigidbody rb;

    public float speed = 10;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = transform.forward * speed;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject);
    }
}
