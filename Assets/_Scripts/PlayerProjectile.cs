using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProjectile : MonoBehaviour
{
    Rigidbody rb;

    public float projectileSpeed = 30;
    public float projectileDamage = 10;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = transform.forward * projectileSpeed;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetParameters(float projectileSpeed, float projectileDamage)
    {
        this.projectileSpeed = projectileSpeed;
        this.projectileDamage = projectileDamage;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            other.gameObject.GetComponent<EnemyBase>().TakeDamage(projectileDamage);
        }

        Destroy(gameObject);
    }
}
