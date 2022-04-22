using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Authored by Joshua Hilliard
public class PlayerStats : MonoBehaviour
{

    private State state; 
    private enum State
    {
        Alive, Dead

    }

    public HudController PlayerHud;

    [Header("Player Parameters")]
    public float maxHealth;

    [Header("Current Player Info")]
    [SerializeField] private float currentHealth;

    
    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        state = State.Alive;
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            default:
            case State.Alive:
                break;
            case State.Dead:
                //Death stuff here
                break;
        }
    }

    public void TakeDamage(float n)
    {
        if(currentHealth > 0)
        {
            currentHealth -= n;
            if(currentHealth <= 0)
            {
                currentHealth = 0;
                PlayerDeath();
            }
        }
        PlayerHud.UpdateHealth(currentHealth, maxHealth);
    }

    public void PlayerDeath()
    {
        state = State.Dead;
    }

    public void RestoreHealth(float n)
    {
        if(currentHealth < maxHealth)
        {
            if((currentHealth + n) > maxHealth)
            {
                currentHealth = maxHealth;
            }
            else
            {
                currentHealth += n;
            }
        }
        PlayerHud.UpdateHealth(currentHealth, maxHealth);
    }
}
