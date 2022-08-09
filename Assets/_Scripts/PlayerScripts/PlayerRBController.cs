using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerRBController : MonoBehaviour
{
    /// <summary>
    /// player's rigidbody component
    /// </summary>
    private Rigidbody rb;

    /// <summary>
    /// player's Animator component
    /// </summary>
    private Animator anim;

    /// <summary>
    /// input for horizontal movement
    /// </summary>
    private float horizontal;
    /// <summary>
    /// rotation of rigidbody when backward
    /// </summary>
    private Vector3 backRot = new Vector3 (0, 270f, 0);
    /// <summary>
    /// rotation of rigidbody when forward
    /// </summary>
    private Vector3 forwardRot = new Vector3 (0, 90f, 0);
    /// <summary>
    /// whether player is facing forward (to the right)
    /// </summary>
    private bool facingForward = true;
    /// <summary>
    /// whether player is touching the ground
    /// </summary>
    private bool grounded = true; 
    /// <summary>
    /// keeps track of the last ground the player was standing on
    /// </summary>
    private GameObject ground;
    /// <summary>
    /// Ratio of horizontal speed to the standard walking speed, facing forward.   
    /// 1.0 is walking forward, -1.0 is walking backward, 0.0 is stopped.
    /// </summary>
    private float animatorMoveSpeed;



    /// <summary>
    /// speed value applied to rb movement
    /// </summary>
    public float speed;

    /// <summary>
    /// Amount to adjust speed by each frame
    /// </summary>
    public float acceleration;

    /// <summary>
    /// Amound to adjust speed by when slowing down on the ground only
    /// </summary>
    public float deceleration;

    /// <summary>
    /// upward velocity to set when player jumps
    /// </summary>
    public float jumpVelocity;

    /// <summary>
    /// The maximum number of times the player can jump in midair
    /// </summary>
    public float maxAirJumps = 1;

    /// <summary>
    /// The number of midair jumps the player currently has remaining
    /// </summary>
    private float airJumps;

    /// <summary>
    /// The time in seconds since the last jump the player did. Exists to avoid double jumping with one input
    /// </summary>
    private float jumpCooldownTimer = 0f;

    /// <summary>
    /// The time that must elapse after a jump before the player may jump again.
    /// </summary>
    private float jumpCooldownLength = 0.5f;

    /// <summary>
    /// The normal of the wall that the player is attached to during their wallcling
    /// </summary>
    private Vector3 wallNormal;


    /// <summary>
    /// Timer for walljumpDuration
    /// </summary>
    private float walljumpTimer = 0f;

    /// <summary>
    /// The amount of time in seconds the player cannot readjust their speed in the air after a walljump
    /// </summary>
    public float walljumpDuration = 0.5f;

    //want momentum to build (only in air)
    //decelerate to ground speed when grounded
    //upper limit to momentum?

    //ground slide mechanic increases acceleration on ground?
    //grappling hook swing adds momentum, pull does not
    //don't save momentum when grapple pulled to wall
    //want wall jump to work even when coming off grapple

    //extra movement mechanics in separate scripts?
    //  double jump (replace with rocket jump?)
    //  wall slide/jump
    //  grappling hook
    //  ground slide

    //jumping --affected by momentum / direction?
    //clinging to wall halts & saves momentum
    //momentum applied when jumping off wall
    //

    /// <summary>
    /// What the player is currently doing and how input should affect 
    /// </summary>
    public enum ActionState
    {
        /// <summary>
        /// Player input does nothing. Use for cutscenes.
        /// </summary>
        Locked,
        /// <summary>
        /// Player is on the ground, and can walk, crouch, and jump
        /// </summary>
        Grounded,
        /// <summary>
        /// Player is in the air, and can influence their momentum and airjump
        /// </summary>
        Midair,
        /// <summary>
        /// Player is clinging to a wall, and can let go or walljump
        /// </summary>
        Wallcling
    }

    [HideInInspector]
    public ActionState actionState = ActionState.Grounded;

    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        anim = gameObject.GetComponent<Animator>();
        airJumps = maxAirJumps;
        jumpCooldownTimer = 0f;
    }

    private void Update()
    {
        walljumpTimer -= Time.deltaTime;
        switch(actionState)
        {
            case ActionState.Locked: break;
            case ActionState.Grounded: Grounded(); break;
            case ActionState.Midair: Midair(); break;
            case ActionState.Wallcling: Wallcling(); break;
            default: Debug.LogError("Uncaught Player Action State!"); break;
        }
    }

    /// <summary>
    /// Set of code to be executed whenever player returns to the ground
    /// </summary>
    private void EnterGrounded()
    {
        actionState = ActionState.Grounded;
        anim.SetBool("falling", false);
        anim.SetBool("onWall", false);
        airJumps = maxAirJumps;
        jumpCooldownTimer = 0f;
    }

    /// <summary>
    /// Set of code to run on Update when the player is on the ground
    /// </summary>
    private void Grounded()
    {
        if (Input.GetAxis("Jump") > 0) //check for jump input
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpVelocity, rb.velocity.z); //set velocity to speed of jump
            jumpCooldownTimer = jumpCooldownLength;
            anim.SetBool("falling", true);
            actionState = ActionState.Midair;
        }

        MoveHorizontal();

        Turn();
    }

    private void Midair()
    {
        
        if (Input.GetAxis("Jump") > 0 && airJumps > 0) //check for jump input and remaining airjumps
        {
            if (jumpCooldownTimer < 0f && walljumpTimer <= 0f)
            {
                rb.velocity = new Vector3(rb.velocity.x, jumpVelocity, rb.velocity.z);
                anim.Play("Falling Idle");
                jumpCooldownTimer = jumpCooldownLength;
                airJumps--;
            }
        }
        jumpCooldownTimer -= Time.deltaTime;
        if (walljumpTimer <= 0f)
        {
            MoveHorizontal();
        }

        Turn();
    }

    private void EnterWallcling(Vector3 newWallNormal)
    {
        this.wallNormal = newWallNormal;
        Debug.Log("Entering Wallcling");
        actionState = ActionState.Wallcling;
        //airJumps = maxAirJumps;
        anim.SetBool("onWall", true);
        //anim.SetBool("falling", false);
        rb.velocity = Vector3.zero;
    }

    private void Wallcling()
    {
        //rb.velocity = new Vector3(rb.velocity.x, 0.667f * rb.velocity.y, rb.velocity.z);
        rb.velocity = Vector3.zero;

        //on a jump, move away from the wall at max walking speed as well as up with the jump speed
        if (Input.GetAxis("Jump") > 0)
        {
            Vector3 newVelocity = (Vector3.up * jumpVelocity + (wallNormal * speed));
            rb.velocity = newVelocity;
            Debug.Log("off wall velocity: " + newVelocity);
            walljumpTimer = walljumpDuration;
            anim.SetBool("onWall", false);
            anim.SetBool("falling", true);
            actionState = ActionState.Midair;

        }

        MoveHorizontal();

    }

    /// <summary>
    /// turn player to face opposite direction
    /// </summary>
    private void Turn()
    {
        float aimX = GetComponent<PlayerAim>().mousePos.x; //check mouse's position on screen

        if(aimX < transform.position.x) //check if we are aiming left of player
        {
            if (transform.rotation.eulerAngles == forwardRot) //facing right and aiming left
            {
                rb.rotation = Quaternion.Euler(backRot); //rotate to match aim
                facingForward = false;
                //I would plug in the turnaround animation here but it doesn't work with root offset (the thing that causes the animation to move the gameobject) disabled.
                //Can't have root offset enabled with rigidbody physics so probs just turn around frame 1 >:(
            }
        }
        else //aiming right of player
        {
            if (transform.rotation.eulerAngles == backRot) //facing left and aiming right
            {
                rb.rotation = Quaternion.Euler(forwardRot); //rotate to match aim
                facingForward=true;
            }
        }
    }

    private void MoveHorizontal()
    {
        horizontal = Input.GetAxis("Horizontal"); //get horizontal input

        Vector3 targetVelocity;

        if (facingForward) //imput stays normal while front facing
        {
            targetVelocity = (transform.forward * horizontal) * speed;
        }
        else //inputs flipped when facing backward
        {
            targetVelocity = (-transform.forward * horizontal) * speed;
        }

        float currentVelocity = rb.velocity.x;

        if (targetVelocity.x > 0f && currentVelocity < targetVelocity.x)
            rb.AddForce(Vector3.right * acceleration);
        else if (targetVelocity.x < 0f && currentVelocity > targetVelocity.x)
            rb.AddForce(Vector3.left * acceleration);

        //Only decellerate on ground
        if (actionState == ActionState.Grounded)
        {
            if (targetVelocity.x == 0f && currentVelocity < -0.2f)
                rb.AddForce(Vector3.right * deceleration);
            else if (targetVelocity.x == 0f && currentVelocity > 0.2f)
                rb.AddForce(Vector3.left * deceleration);
        }


        //targetVelocity.y = rb.velocity.y; //precents gravity from being overridden by horizontal input
        //rb.velocity = targetVelocity; //apply velocity to rigidbody

        //apply current motion to animator
        animatorMoveSpeed = currentVelocity / speed;
        if (currentVelocity == 0.0f)
            animatorMoveSpeed = 0.0f;
        if (!facingForward)
            animatorMoveSpeed *= -1.0f;
        anim.SetFloat("animatorMoveSpeed", animatorMoveSpeed);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Vector3 normal = collision.GetContact(0).normal; //get the normal from one of the contact points
        normal.z = 0;
        float comparison = Vector3.Angle(normal, Vector3.up); //get an angle to see if the object can be ground

        float acceptableGroundAngle = 75f;
        if(comparison <= acceptableGroundAngle) //make sure collision isn't with anything pointing slightly down
        {
            if (actionState == ActionState.Midair || actionState == ActionState.Wallcling)
                EnterGrounded();
            ground = collision.gameObject; //keep track of the last ground you collided with

        }
        else if(comparison > 85f && comparison < 95f && actionState == ActionState.Midair) //Check if a wall for a wallcling
        {
            EnterWallcling(normal);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (actionState == ActionState.Grounded && collision.gameObject == ground)
        {
            Debug.Log("exit collider");
            actionState = ActionState.Midair; //no longer grounded when you leave the ground you were on
        }
        else if (actionState == ActionState.Wallcling)
        {
            actionState = ActionState.Midair;
            anim.SetBool("onWall", false);
        }
    }
}
