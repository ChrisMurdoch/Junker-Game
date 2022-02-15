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

    [HideInInspector] public bool isAirborne;

    private bool canDoubleJump = true;
    
    private CharacterController characterController;

    private Vector3 velocity;
    private float verticalVelocity;

    private float baseStepOffSet;

    private bool OnSlope;

    [Header("Movement Parameters")] 
    [SerializeField] private float moveSpeed = 3.0f; //Grounded speed
    [SerializeField] private float airSpeed = 4.0f; //Air speed, this should generally be higher that moveSpeed

    [Header("Jumping Parameters")] //For a snappier jump, turn these way up to a ratio of 1:2 ish or double the amount of gravity in the script
    [SerializeField] private float jumpForce = 8.0f;
    [SerializeField] private float doubleJumpForce = 6.0f;
    [SerializeField] private float gravity = 30.0f;

    [Header("Slope Parameters")] //Unimplemented slope slide stuff
    [SerializeField] private float slopeSlideSpeed = 8.0f;
    [SerializeField] private bool CanSlideOnSlopes = true;

    [Header("Other Parameters")]
    [SerializeField] private LayerMask WhatIsGround; //Make a layer called ground or something and apply it to anything you want to jump off of. You can also set this to everything to jump off anything really
    [SerializeField] public Transform GroundCheckPosition; //Make an empty gameobject and put it somewhere near the charactercontroller's bottom.

    private Vector3 hitPointNormal;
    private bool IsSliding
    {
        get
        {
            if (characterController.isGrounded && Physics.Raycast(transform.position, Vector3.down, out RaycastHit slopeHit, 2.5f))
            {
                hitPointNormal = slopeHit.normal;
                return Vector3.Angle(hitPointNormal, Vector3.up) > characterController.slopeLimit;
            }
            else
            {
                return false;
            }
        }
    }

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        baseStepOffSet = characterController.stepOffset;
    }

    #region weapon function
    //region by John Murphy
    //This section will contain all that is necessary to make use of the weapon system,
    //it will keep track of a player component that itself keeps track of the weapons through
    //the inventory system. This section is meant keep track of that with inverse kinematics
    //for now it will just do so with simple game objects I will come back to this later when necessary
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (characterController.isGrounded && Physics.Raycast(transform.position, Vector3.down, out RaycastHit slopeHit, 2.5f))
        {
            if(slopeHit.normal != Vector3.up)
            {
                OnSlope = true;
            }
            else
            {
                OnSlope = false;
            }
        }

        //Debug.Log(OnSlope);

        GroundCheck();

        if (CanInput)
        {
            InputHandler();
        }

        GravityHandler();

        //velocity = AdjustMovementToSlope(velocity);

        characterController.Move(velocity * Time.deltaTime);

        JumpHandler();

        

    }

    private void InputHandler() 
    {
        float x = Input.GetAxisRaw("Horizontal"); //A & D //Gets input of either -1 or 1 so you can turn on a dime
        float xAir = Input.GetAxis("Horizontal"); //A & D //Basically "air resistance" adds a ramp up to moving in air

        if (characterController.isGrounded)
        {
            velocity = (transform.right * x) * moveSpeed + Vector3.up * verticalVelocity;
        }

        if (!characterController.isGrounded)
        {
            velocity = (transform.right * xAir) * airSpeed + Vector3.up * verticalVelocity;

        }

    }

    private void JumpHandler()
    {

        if (Input.GetKeyDown(KeyCode.Space) && characterController.isGrounded && !IsSliding)
        {
            verticalVelocity = 0;
            verticalVelocity = jumpForce;
        }

        if (Input.GetKeyDown(KeyCode.Space) && !characterController.isGrounded && canDoubleJump == true)
        {
            verticalVelocity = 0;
            verticalVelocity = doubleJumpForce;
            canDoubleJump = false;
        }
    }

    //I have finally wrangled charactercontroller.isgrounded and it functions pretty well now. Keeping this method just in case
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

        //if (Physics.CheckSphere(GroundCheckPosition.position, .45f, WhatIsGround)) //CheckSphere probably works the best
        //{
        //    //Debug.Log("Grounded");
        //    IsGrounded = true;
        //    canDoubleJump = true;

        //}
        //else
        //{
        //    //Debug.Log("Not Grounded");
        //    IsGrounded = false;
        //}

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
        //Gizmos.DrawSphere(GroundCheckPosition.position, .45f);
        //Gizmos.DrawCube(GroundCheckPosition.position, new Vector3(.7f, .1f, .7f));
    }

    private void GravityHandler()
    {
        if (!characterController.isGrounded) //Adds artifical gravity if the player isn't grounded
        {
            isAirborne = true;

            OnSlope = false;

            verticalVelocity -= gravity * Time.deltaTime;

            if (characterController.collisionFlags == CollisionFlags.Above)
            {
                verticalVelocity = 0;                //Stop adding any more velocity.
                characterController.stepOffset = 0;                 //Set stepOffset to zero to prevent player moving and sticking to the ceiling.
            }

        }
        else if (characterController.isGrounded && verticalVelocity < 0 && !IsSliding)
        {
            canDoubleJump = true;
            if (characterController.stepOffset == 0) //Resets stepOffset back to the base.
            {
                characterController.stepOffset = baseStepOffSet;
            }

            if (OnSlope)
            {
                verticalVelocity = -12;
            }
            else
            {
                verticalVelocity = -1; //Keep this at -1. Basically resets gravity when grounded.
            }

            //verticalVelocity = -1; //Keep this at -1. Basically resets gravity when grounded.
        }

        if (CanSlideOnSlopes && IsSliding && characterController.isGrounded)
        {
            velocity += new Vector3(hitPointNormal.x, -hitPointNormal.y, hitPointNormal.z) * slopeSlideSpeed;
        }

        if (characterController.isGrounded)
        {
            isAirborne = false;
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

            if (adjustedVelocity.y < -1 && characterController.isGrounded)
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