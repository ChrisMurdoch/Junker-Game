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
    /// speed for player to change direction
    /// </summary>
    public float turnSpeed;
    /// <summary>
    /// upward force added when player jumps
    /// </summary>
    public float jumpForce;

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

    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        anim = gameObject.GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        horizontal = Input.GetAxis("Horizontal"); //get horizontal input

        if((Input.GetAxis("Jump") > 0) && grounded) //check for jump input & grounded condition
        {
            rb.AddForce(transform.up * jumpForce); //add force to initiate jump
            anim.SetTrigger("jumping");
        }

        Vector3 velocity;

        if (facingForward) //imput stays normal while front facing
        {
            velocity = (transform.forward * horizontal) * speed * Time.deltaTime;
        } else //inputs flipped when facing backward
        {
            velocity = (-transform.forward * horizontal) * speed * Time.deltaTime;
        }

        velocity.y = rb.velocity.y; //precents gravity from being overridden by horizontal input
        rb.velocity = velocity; //apply velocity to rigidbody

        //apply current motion to animator
        animatorMoveSpeed = velocity.x / speed / Time.deltaTime;
        if (velocity.x == 0.0f)
            animatorMoveSpeed = 0.0f;
        if (!facingForward)
            animatorMoveSpeed *= -1.0f;
        anim.SetFloat("animatorMoveSpeed", animatorMoveSpeed);

        Turn(); //check if you need to turn
    }

    private void Update()
    {

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

    private void OnCollisionEnter(Collision collision)
    {
        Vector3 normal = collision.contacts[0].normal; //get the normal from one of the contact points
        float comparison = Vector3.Angle(normal, Vector3.up); //get an angle to see if the object can be ground

        Debug.Log("collision angle = " + comparison);
        if(comparison <= 90f || comparison >= -90f) //make sure collision isn't with anything pointing slightly down
        {
            Debug.Log("COLLISION WITH GROUND");
            grounded = true; 
            ground = collision.gameObject; //keep track of the last ground you collided with
            anim.SetBool("falling", false);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (grounded && collision.gameObject == ground)
        {
            Debug.Log("exit collider");
            grounded = false; //no longer grounded when you leave the ground you were on
            anim.SetBool("falling", true);
        }
    }
}
