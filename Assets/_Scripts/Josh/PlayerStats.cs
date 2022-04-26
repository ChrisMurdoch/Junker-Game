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
    public float startingHealth = 50;

    [Header("Current Player Info")]
    [SerializeField] private float currentHealth;


    private void Awake()
    {
        currentHealth = startingHealth;
    }

    // Start is called before the first frame update
    void Start()
    {
        PlayerHud.UpdateHealth(currentHealth, maxHealth);
        state = State.Alive;
    }

    // Update is called once per frame
    void Update()
    {

        PlayerHud.UpdateHealth(currentHealth, maxHealth);

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
        GameManager.Instance.GameOver();
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
