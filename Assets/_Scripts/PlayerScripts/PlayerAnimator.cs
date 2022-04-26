using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimator : MonoBehaviour
{

    public GameObject crosshair;
    private Animator anim;
    private PlayerController pc;
    [SerializeField] private float landingDist;
    public float fallThreshold; //how far the player has to be from the ground to trigger falling animation

    //public bool jumpFinished;
    private float storedMoveSpeed;
    private float storedAirSpeed;

    private AnimationClip turnClip;
    [HideInInspector] public bool facingRight; //false = left, true = right
    [HideInInspector] public bool finishedTurn;
    [HideInInspector] public bool finishedLand;

    private WeaponIK weaponIK; //ik reference to disable when turning

    // Start is called before the first frame update
    void Start()
    {
        anim = this.GetComponent<Animator>();
        weaponIK = GetComponent<WeaponIK>();
        pc = GetComponent<PlayerController>();
        facingRight = true; //start facing right
        finishedTurn = true; //since we aren't actively turning on start
        finishedLand = true; //since we aren't actively falling/landing

        storedMoveSpeed = pc.moveSpeed;
        storedAirSpeed = pc.airSpeed;

        AddAnimationEvent(1.09f, "FinishTurn", 1);
        AddAnimationEvent(0.15f, "AddForce", 4);
        AddAnimationEvent(1.01f, "EndLanding", 6);

        Debug.Log("animation order");
        foreach (AnimationClip ac in anim.runtimeAnimatorController.animationClips)
            Debug.Log(ac.name);

        foreach (AnimationEvent evt in anim.runtimeAnimatorController.animationClips[7].events)
            Debug.Log(evt.functionName + " == " + evt.time);


    }

    void Update()
    {
        CheckMovement();
        CheckForTurn();

        if(pc.isCrouching) {
            anim.SetBool("crouching", true);
        }
        else {
            anim.SetBool("crouching", false);
        }


        //check if cc left the ground. Extra check for if you are outside of a certain distance from the ground (aviods isGrounded glitchyness)
        if (!GetComponent<CharacterController>().isGrounded && !Physics.Raycast(transform.position, -Vector3.up, out RaycastHit hit, fallThreshold)) {
            anim.SetBool("falling", true); //play falling idle anim
            finishedLand = false;            
        }

        if(anim.GetCurrentAnimatorStateInfo(0).IsName("Falling Idle"))
            CheckForGround(); //while falling, check for when to play landing anim

        if(anim.GetCurrentAnimatorStateInfo(0).IsName("Falling To Landing")) {
            pc.moveSpeed = 0f;
            pc.airSpeed = 0f;
        }
        else {
            pc.moveSpeed = storedMoveSpeed;
            pc.airSpeed = storedAirSpeed;
        }

        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Standing Idle"))
            anim.ResetTrigger("needsLanding");

        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Quick 180 Turn"))
            anim.ResetTrigger("jumping"); //stop jumps from queueing during turns
    }

    void LateUpdate() {
        if(finishedTurn) {

            weaponIK.enabled = true;

            if(facingRight) {
                transform.rotation = Quaternion.Euler(0, 90f, 0);
            } else {
                transform.rotation = Quaternion.Euler(0, -90f, 0);
            }
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
                anim.SetBool("backwards", true);
            }
            else {
                anim.SetBool("backwards", false);
            }
        }

        else //idling
        {
            anim.SetBool("running", false);
            anim.SetBool("backwards", false);
        }
    }

    void CheckForTurn()
    {
        bool newFacingRight = CheckAimDirection();

        Debug.Log("grounded = " + GetComponent<CharacterController>().isGrounded);
        if (newFacingRight != facingRight && finishedTurn && GetComponent<CharacterController>().isGrounded) //need to face new direction & are not actively turning
        {
            Debug.Log("TURN");
            weaponIK.enabled = false;
            anim.SetTrigger("needsTurn"); //trigger the turn animation
           finishedTurn = false; //denote active turn anim
        }
    }

    //checks whether you are aiming to the right of the player object
    bool CheckAimDirection()
    {
        float aimX = GetComponent<PlayerAim>().mousePos.x; //get mouse's position from player aim script
        float posX = this.transform.position.x; 

        //crosshair to the left of player object
        if(aimX < posX)
            return false; //aiming left
        else
            return true; //aiming right (default)
    }

    //check if we are close enough to the ground to trigger the landing anim
    void CheckForGround()
    {
        if (Physics.Raycast(transform.position, -Vector3.up, out RaycastHit hit, landingDist))
        {
            Debug.DrawRay(transform.position, -Vector3.up * hit.distance, Color.green); 
            Debug.Log("NEEDS LANDING");
            anim.SetTrigger("needsLanding"); //trigger landing animation
        }

    }

    void FinishTurn()
    {
        facingRight = !facingRight; //switch facing direction to opposite
        finishedTurn = true;
    }

    void AddForce()
    {
        pc.AddJumpForce();
    }

    void EndLanding()
    {
        anim.SetBool("falling", false); //landed, so no longer falling
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
