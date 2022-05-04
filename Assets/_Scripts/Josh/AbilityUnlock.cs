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
        if (other.gameObject.GetComponent<PlayerController>() || other.gameObject.GetComponent<PlayerControllerOld>())
        {
            UnlockEvent.Invoke();

            PlayerController pc = other.gameObject.GetComponent<PlayerController>();
            PlayerControllerOld pc2 = other.gameObject.GetComponent<PlayerControllerOld>();


            if (Ability == Abilities.DoubleJump)
            {
                if(pc != null)
                {
                    pc.doubleJumpUnlocked = true;
                }

                if(pc2 != null)
                {
                    pc2.doubleJumpUnlocked = true;

                }
            }

            if (Ability == Abilities.HookLauncher)
            {
                if (pc != null)
                {
                    pc.hookLauncherUnlocked = true;
                }

                if (pc2 != null)
                {
                    pc2.hookLauncherUnlocked = true;

                }
            }

            Destroy(gameObject);
        }
    }
}
