using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AbilityUnlock : MonoBehaviour
{

    public Abilities Ability;

    public UnityEvent UnlockEvent;

    public enum Abilities
    {
        DoubleJump = 0, HookLauncher = 1,
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerController>())
        {
            UnlockEvent.Invoke();

            PlayerController pc = other.gameObject.GetComponent<PlayerController>();

            if(Ability == Abilities.DoubleJump)
            {
                pc.doubleJumpUnlocked = true;
            }

            if (Ability == Abilities.HookLauncher)
            {
                pc.hookLauncherUnlocked = true;
            }

            Destroy(gameObject);
        }
    }
}
