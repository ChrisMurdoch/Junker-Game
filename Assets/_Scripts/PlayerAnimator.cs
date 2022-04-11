using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimator : MonoBehaviour
{

    public GameObject crosshair;
    public Animator anim;
    public PlayerControllerNew pc;

    //public bool jumpFinished;

    private AnimationClip turnClip;
    [HideInInspector] public bool facingRight; //false = left, true = right
    [HideInInspector] public bool finishedTurn;

    // Start is called before the first frame update
    void Start()
    {
        anim = this.GetComponent<Animator>();
        facingRight = true; //start facing right
        finishedTurn = true; //since we aren't actively turning on start

        AddAnimationEvent(1.09f, "FinishTurn", 1);
        AddAnimationEvent(0.60f, "AddForce", 4);


    }

    void Update()
    {
        CheckMovement();
        CheckForTurn();
    }

    void LateUpdate() {
        if(finishedTurn) {
            if(facingRight)
                transform.rotation = Quaternion.Euler(0, 90f, 0);
            else
                transform.rotation = Quaternion.Euler(0, -90f, 0);
        }
    }

    void CheckMovement()
    {
        int moveDir = (int)Input.GetAxisRaw("Horizontal");
        bool movingRight;


        if(moveDir != 0) //moving
        {

            anim.SetBool("running", true);

            if(moveDir < 0)
                movingRight = false;
            else  
                movingRight = true;

            //running in dir you aren't facing
            if (movingRight != facingRight)
            {
                Debug.Log("BACKWARDS");
                anim.SetBool("backwards", true);
            }
            else {
                Debug.Log("FORWARD");
                anim.SetBool("backwards", false);
            }
        }

        else //idling
        {
            anim.SetBool("running", false);
        }
    }

    void CheckForTurn()
    {
        bool newFacingRight = CheckAimDirection();
        
        if (newFacingRight != facingRight && finishedTurn) //need to face new direction & are not actively turning
        {
            anim.SetTrigger("needsTurn"); //trigger the turn animation
           finishedTurn = false; //denote active turn anim
           Debug.Log("TURN");
        }
    }

    //checks whether you are aiming to the right of the player object
    bool CheckAimDirection()
    {
        float aimX = PlayerAimNew.instance.mousePos.x; //get mouse's position from player aim script
        float posX = this.transform.position.x; 

        //crosshair to the left of player object
        if(aimX < posX)
            return false; //aiming left
        else
            return true; //aiming right (default)
    }

    void FinishTurn()
    {
        facingRight = !facingRight; //switch facing direction to opposite
        finishedTurn = true;
        Debug.Log("finished turn");
    }

    void AddForce()
    {
        pc.AddJumpForce();
    }

    void AddAnimationEvent(float animTime, string fName, int clipIndex)
    {
         //create animation event
        AnimationEvent evt = new AnimationEvent();
        evt.time = animTime; //event happens at animTime seconds in animation
        evt.functionName = fName; //event will call this function
        
        AnimationClip clip = anim.runtimeAnimatorController.animationClips[clipIndex]; //get clip from animator's array
        clip.AddEvent(evt); //add event to clip
    }

}
