using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Authored by Joshua Hilliard
public class ProjectileBase : MonoBehaviour
{

    Rigidbody rb;

    [Header("Projectile Parameters")]
    public float projectileSpeed = 30;
    public float projectileDamage = 10;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = transform.forward * projectileSpeed;
        Physics.IgnoreCollision(GameObject.FindWithTag("Player").GetComponent<CharacterController>(), GetComponent<Collider>());
        Destroy(gameObject, 10);
    }
    void Update()
    {

    }

    public virtual void SetParameters(float projectileSpeed, float projectileDamage)
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
