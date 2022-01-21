using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour //Lots of this is ripped from my first person character controller but adapted to 2.5D, hell this ended up better than my FPS controller in some areas.
{
	/*To use, make an empty Gameobject and add this script and a character controller to it. Add a capsule as a child and remove its collider so you have a visual. Add another empty gameobject
	as a child and place it near the bottom of the capsule around -.7f on the y axis (This is the GroundCheckPosition. Finally, make a layer called ground or just keep it set to everything.*/
	
	
    public bool Paused = false; //Relatively unused rn, but they'll come in handy when making player states and such
    public bool CanMove = true;
    public bool CanJump = true;
    public bool CanInput = true;
    public bool AllowEverything = true;

    private bool IsGrounded = false;

    private bool canDoubleJump = true;

    private CharacterController characterController;

    private Vector3 velocity;
    private float verticalVelocity;

    private float baseStepOffSet;

    [Header("Movement Parameters")] 
    [SerializeField] private float moveSpeed = 3.0f; //Grounded speed
    [SerializeField] private float airSpeed = 4.0f; //Air speed, this should generally be higher that moveSpeed

    [Header("Jumping Parameters")] //For a snappier jump, turn these way up to a ratio of 1:2 ish or double the amount of gravity in the script
    [SerializeField] private float jumpForce = 8.0f;
    [SerializeField] private float doubleJumpForce = 6.0f;
    [SerializeField] private float gravity = 30.0f;

    [Header("Slope Parameters")] //Unimplemented slope slide stuff
    [SerializeField] private float slopeSlideSpeed = 8.0f;
    [SerializeField] private float slopeLimit = 45.0f;

    [Header("Other Parameters")]
    [SerializeField] private LayerMask WhatIsGround; //Make a layer called ground or something and apply it to anything you want to jump off of. You can also set this to everything to jump off anything really
    [SerializeField] public Transform GroundCheckPosition; //Make an empty gameobject and put it somewhere near the charactercontroller's bottom.

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        baseStepOffSet = characterController.stepOffset;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        GroundCheck();

        if (CanInput)
        {
            InputHandler();
        }

        GravityHandler();

        velocity = AdjustMovementToSlope(velocity);

        characterController.Move(velocity * Time.deltaTime);

        JumpHandler();

    }

    private void InputHandler() 
    {
        float x = Input.GetAxisRaw("Horizontal"); //A & D //Gets input of either -1 or 1 so you can turn on a dime
        float xAir = Input.GetAxis("Horizontal"); //A & D //Basically "air resistance" adds a ramp up to moving in air

        if (IsGrounded)
        {
            velocity = (transform.right * x) * moveSpeed + Vector3.up * verticalVelocity;
        }

        if (!IsGrounded)
        {
            velocity = (transform.right * xAir) * airSpeed + Vector3.up * verticalVelocity;

        }

    }

    private void JumpHandler()
    {
        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded)
        {
            verticalVelocity = 0;
            verticalVelocity = jumpForce;
        }

        if(Input.GetKeyDown(KeyCode.Space) && !IsGrounded && canDoubleJump == true)
        {
            verticalVelocity = 0;
            verticalVelocity = doubleJumpForce;
            canDoubleJump = false;
        }
    }

    private void GroundCheck() //Has three different ways to check if the player is grounded because charactercontroller.isgrounded gives me conniptions and I'm not using it anymore
    {
        //RaycastHit hit;
        //if (Physics.Raycast(transform.position, Vector3.down, out hit, 1.5f))
        //{
        //    Debug.Log("Grounded");
        //    IsGrounded = true;
        //}
        //else
        //{
        //    Debug.Log("Not Grounded");
        //    IsGrounded = false;
        //}

        if (Physics.CheckSphere(GroundCheckPosition.position, .35f, WhatIsGround)) //CheckSphere probably works the best
        {
            //Debug.Log("Grounded");
            IsGrounded = true;
            canDoubleJump = true;

        }
        else
        {
            //Debug.Log("Not Grounded");
            IsGrounded = false;
        }

        //if (Physics.CheckBox(GroundCheckPosition.position, new Vector3(.7f, .1f, .7f), Quaternion.identity, WhatIsGround)) 
        //{
        //    //Debug.Log("Grounded");
        //    IsGrounded = true;
        //}
        //else
        //{
        //    //Debug.Log("Not Grounded");
        //    IsGrounded = false;
        //}

    }

    void OnDrawGizmos() //Visualizes CheckSphere and CheckBox
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(GroundCheckPosition.position, .35f);
        //Gizmos.DrawCube(GroundCheckPosition.position, new Vector3(.7f, .1f, .7f));
    }

    private void GravityHandler()
    {
        if(!IsGrounded) //Adds artifical gravity if the player isn't grounded
        {
            verticalVelocity -= gravity * Time.deltaTime;

            if (characterController.collisionFlags == CollisionFlags.Above)
            {
                verticalVelocity = -1f;                //Stop adding any more velocity.
                characterController.stepOffset = 0;                 //Set stepOffset to zero to prevent player moving and sticking to the ceiling.
            }
        }
        else if(IsGrounded && verticalVelocity < 0)
        {
            if (characterController.stepOffset == 0) //Resets stepOffset back to the base.
            {
                characterController.stepOffset = baseStepOffSet;
            }
            verticalVelocity = -1f; //Keep this at -1. Basically resets gravity when grounded.
        }
    }

    private Vector3 AdjustMovementToSlope(Vector3 velocity) //Allows sticking to slopes
    {
        var ray = new Ray(transform.position, Vector3.down);

        if (Physics.Raycast(ray, out RaycastHit hitInfo, 1.5f))
        {
            var slopeRotation = Quaternion.FromToRotation(Vector3.up, hitInfo.normal);
            var adjustedVelocity = slopeRotation * velocity;
            //Debug.Log(adjustedVelocity);
            if (adjustedVelocity.y < -1)
            {
                return adjustedVelocity;
            }
        }

        return velocity;
    }

    public void SetVerticalVelocity(float n)
    {
        verticalVelocity = n;
    }

}
