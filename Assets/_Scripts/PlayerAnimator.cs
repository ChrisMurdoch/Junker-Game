using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimator : MonoBehaviour
{

    public GameObject crosshair;
    public Animator anim;

    private int facingDir; //-1 = left, 1 = right

    // Start is called before the first frame update
    void Start()
    {
        anim = this.GetComponent<Animator>();
        facingDir = 1; //start facing right
    }

    void Update()
    {
        CheckMovement();
        CheckForTurn();
    }

    void CheckMovement()
    {
        int moveDir = (int)Input.GetAxisRaw("Horizontal");
        int aimDir = CheckAimDirection();

        if(moveDir != 0) //moving
        {
            anim.SetBool("running", true);

            //running in dir you aren't facing
            if (moveDir != aimDir)
            {
                anim.SetBool("backwards", true);
            }
            else
                anim.SetBool("backwards", false);
        }

        else //idling
        {
            anim.SetBool("running", false);
        }
    }

    void CheckForTurn()
    {
        int newAimDir = CheckAimDirection();
        
        if (newAimDir != facingDir)
        {
            anim.SetTrigger("needsTurn"); //trigger the turn animation
            facingDir = newAimDir; //update facingDir after turn
        }
    }

    int CheckAimDirection()
    {
        float aimX = crosshair.transform.position.x;
        float posX = this.transform.position.x;

        //crosshair to the left of player object
        if(aimX < posX)
            return -1; //aiming left
        else
            return 1; //aiming right (default)
    }

}
