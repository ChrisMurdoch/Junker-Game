using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Authored by Joshua Hilliard
public class EnemyBase : MonoBehaviour
{
    [SerializeField] protected float currentHealth;
    [SerializeField] protected float maxHealth;

    [SerializeField] private int EnemyID;

    public virtual void TakeDamage(float damage)
    {
        currentHealth -= damage;
    }

    public virtual void Die()
    {
        if(currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }
}
