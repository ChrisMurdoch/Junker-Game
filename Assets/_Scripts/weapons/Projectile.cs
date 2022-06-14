using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    
    public int damage;
    public Rigidbody rb;
    public float forceVal;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        
    }
    
    public void Launch()
    {
        Vector3 forceTotal = transform.forward;
        forceTotal *= forceVal;
        rb.AddForce(forceTotal);
    }


    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Enemy"))
        {

        }
        Destroy(gameObject);
    }
}
