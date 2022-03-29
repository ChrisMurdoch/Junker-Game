using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Authored by Joshua Hilliard
//Edited by Larry keen

//Fix: jump anim cancelling when falling; crouch walk anim goes back through crouch before returning to idle; when jump is pressed, pressing opposite direction takes you the wrong way

public class LarryPlayerController : MonoBehaviour 
{
	
    [HideInInspector] public bool isAirborne;

<<<<<<< HEAD:Assets/_Scripts/LarryPlayerController.cs
=======
    public InventoryManager inventory; //reference to inventory script / canvas
    public KeyCode interactKey; // ex. "e", "Tab", "Mouse 0"
    private bool inPickupRange;
    private GameObject currPickup; //holds the pickup we were last in range of

    public GameObject equippedWeapon;

>>>>>>> inventory:Assets/_Scripts/PlayerController.cs
    private bool canDoubleJump = true;

    private CharacterController characterController;

    private Vector3 velocity;
    private float verticalVelocity;

    private float baseStepOffSet;

    private bool OnSlope;

    private LarryHookLauncher launcher;

    private bool CanHook;

    private GameObject PlayerBody;

    [HideInInspector] public bool isCrouching;

    private bool underObject;

    private State state; //A simple state machine to keep track of player states

    private Animator animator;

    //consts for animator bool parameters
    const string IS_RUN = "isRun";
    const string IS_RUN_TURNING = "isRunTURN";
    const string IS_CROUCH = "isCrouch";
    const string IS_CROUCH_WALK = "isCrouchWalk";
    const string IS_JUMP = "isJump";

    float rotationPerFrame = 15f;



    private enum State
    {
        Normal, Hooking, Clinging

        //Normal is normal movement
        //Hooking is when the hook collides with a wall and beings pulling the player
        //Clinging is when the player is latching to the wall
    }


    [Header("Movement Parameters")] 
    [SerializeField] private float moveSpeed = 3.0f; //Grounded speed
    [SerializeField] private float airSpeed = 4.0f; //Air speed, this should generally be higher that moveSpeed
    [SerializeField] private float crouchSpeed = 4.0f;


    [Header("Jumping Parameters")] //For a snappier jump, turn these way up to a ratio of 1:2 ish or double the amount of gravity in the script
    [SerializeField] private float jumpForce = 8.0f;
    [SerializeField] private float doubleJumpForce = 6.0f;
    [SerializeField] private float gravity = 30.0f;

    [Header("Slope Parameters")]
    [SerializeField] private float slopeSlideSpeed = 8.0f;
    [SerializeField] private bool CanSlideOnSlopes = true;

    //[Header("Other Parameters")]
    //[SerializeField] private LayerMask WhatIsGround; 
    //[SerializeField] public Transform GroundCheckPosition; 

    private Vector3 hitPointNormal;

    private bool IsSliding //A bool that returns the angle of a slope if it exceeds the CharacterControllers slope limit
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
        PlayerBody = transform.Find("Player Body").gameObject;
        characterController = GetComponent<CharacterController>();
        baseStepOffSet = characterController.stepOffset;
        launcher = GetComponent<LarryHookLauncher>();

        animator = GetComponentInChildren<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;

    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            default:
            case State.Normal:
                SlopeCheck();
                InputHandler();
                Crouch();
                GravityHandler();
                characterController.Move(velocity * Time.deltaTime);
                JumpHandler();
                UseHook();
                break;
            case State.Hooking:
                HandleHookPullMovement();
                break;
            case State.Clinging:
                HookClinging();
                break;

        }

    }

    private void UseHook()
    {
        if (Input.GetMouseButton(1) && CanHook) //Right mouse buttons throws the hook
        {
            launcher.FireHook();
            CanHook = false;
        }

        if (characterController.isGrounded && !IsSliding)
        {
            CanHook = true;
        }
    } 

    public void ChangeState(int n) //A public method to allow other scripts to change the players state
    {
        state = (State)n;
    }

    private void SlopeCheck() //A check to see if the player is standing on a slope
    {
        if (characterController.isGrounded && Physics.Raycast(transform.position, Vector3.down, out RaycastHit slopeHit, 2.5f))
        {
            if (slopeHit.normal != Vector3.up)
            {
                OnSlope = true;
            }
            else
            {
                OnSlope = false;
            }
        }
    }

    private void Reload()
    {
        inventory.ReloadActiveWeapon(equippedWeapon.GetComponent<Weapon>());
    }

    private void Crouch()
    {
        if (Input.GetKey(KeyCode.C))
        {
            animator.SetBool(IS_CROUCH, true);

            isCrouching = true;
            PlayerBody.transform.localScale = new Vector3(1, .5f, 1);
            characterController.height = 1;
            characterController.center = new Vector3(0, -.5f, 0);
            PlayerBody.transform.localPosition = characterController.center;
            if (Physics.Raycast(transform.position, Vector3.up, out RaycastHit hit, 1))
            {
                underObject = true;
            }
            else
            {
                underObject = false;
            }
        }
        else if(underObject == true)
        {
            isCrouching = true;
            PlayerBody.transform.localScale = new Vector3(1, .5f, 1);
            characterController.height = 1;
            characterController.center = new Vector3(0, -.5f, 0);
            PlayerBody.transform.localPosition = characterController.center;

            if (Physics.Raycast(transform.position, Vector3.up, out RaycastHit hit, 1))
            {
                underObject = true;
            }
            else
            {
                underObject = false;
            }
        }
        else
        {
            animator.SetBool(IS_CROUCH, false);
            isCrouching = false;
            PlayerBody.transform.localScale = new Vector3(1, 1, 1);
            characterController.height = 2;
            characterController.center = new Vector3(0, 0, 0);
            PlayerBody.transform.localPosition = characterController.center;
        }
    }

    private void InputHandler() //Gets the input of the "Horizontal Axis" or keys A & D.
    {
        float x = Input.GetAxisRaw("Horizontal"); //is equal to -1 if pressing A, 1 if pressing D, or 0 if pressing neither
        float xAir = Input.GetAxis("Horizontal"); //Same as above but ramps to -1 or 1.

        Quaternion curRotation = transform.rotation;
        Vector3 posToLookAt;
        posToLookAt.x = 0;
        posToLookAt.y = 0;
        posToLookAt.z = x;

        if (characterController.isGrounded)
        {
            if (!isCrouching)
            {
                animator.SetBool(IS_CROUCH, false);
                animator.SetBool(IS_CROUCH_WALK, false);

                if (x == 0)
                {
                    animator.SetBool(IS_RUN, false);
                }
                else if(x == -1)
                {
                    animator.SetBool(IS_RUN, true);
                    Quaternion targetRotation = Quaternion.LookRotation(posToLookAt);
                    transform.rotation = Quaternion.Slerp(curRotation, targetRotation, 1);
                    

                    //this makes sure that you move in the negative direction
                    x = -x;
                    
                }
                else if(x == 1)
                {
                    animator.SetBool(IS_RUN, true);
                    Quaternion targetRotation = Quaternion.LookRotation(posToLookAt);
                    transform.rotation = Quaternion.Slerp(curRotation, targetRotation, 1);
                }

                velocity = (transform.right * x) * moveSpeed + Vector3.up * verticalVelocity;
                // A Vector3 used with charactercontroller.move()
                //Transform.right is shorthand for the X axis of the gameobject * x which is either -1, 0, or 1, then multiplied by the movespeed
                //Vector3.up * verticalVelocity is gravity and jump height, with Vector3.up being short for Vector3(0,1,0)
            }
            else if (isCrouching)
            {
                animator.SetBool(IS_RUN, false);
                animator.SetBool(IS_CROUCH, true);

                if (x == 0)
                {
                    animator.SetBool(IS_CROUCH_WALK, false);
                }
                else if (x == -1)
                {
                    animator.SetBool(IS_CROUCH_WALK, true);
                    Quaternion targetRotation = Quaternion.LookRotation(posToLookAt);
                    transform.rotation = Quaternion.Slerp(curRotation, targetRotation, 1);

                    //this makes sure that you move in the negative direction
                    x = -x;

                }
                else if (x == 1)
                {
                    animator.SetBool(IS_CROUCH_WALK, true);
                    Quaternion targetRotation = Quaternion.LookRotation(posToLookAt);
                    transform.rotation = Quaternion.Slerp(curRotation, targetRotation, 1);
                }

                velocity = (transform.right * x) * crouchSpeed + Vector3.up * verticalVelocity;

            }
        }

        if (!characterController.isGrounded)
        {
            animator.SetBool(IS_JUMP, true);
            if (xAir < 0)
            {
                velocity = (transform.right * -xAir) * airSpeed + Vector3.up * verticalVelocity;
            }
            else
            {
                velocity = (transform.right * xAir) * airSpeed + Vector3.up * verticalVelocity;
            }




        }
        else
        {
            animator.SetBool(IS_JUMP, false);
        }

    }

    
    private void JumpHandler()
    {

        if (Input.GetKeyDown(KeyCode.Space) && characterController.isGrounded && !IsSliding) //If player is grounded and not sliding, verticalVelocity becomes the jumpforce
        {
            
            verticalVelocity = 0;
            verticalVelocity = jumpForce;
        }
        

        if (Input.GetKeyDown(KeyCode.Space) && !characterController.isGrounded && canDoubleJump == true) //If player is not grounded, then they can double jump
        {
            
            verticalVelocity = 0;
            verticalVelocity = doubleJumpForce;
            canDoubleJump = false;
            state = State.Normal; //Allows jumping out of the Hooking state

        }
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
            OnSlope = false; //Can't be on a slope if they're not grounded
            verticalVelocity -= gravity * Time.deltaTime;

            if (characterController.collisionFlags == CollisionFlags.Above) //If player hits a ceiling, stop adding upward velocity
            {
                verticalVelocity = 0;                //Stop adding any more velocity.
                characterController.stepOffset = 0;                 //Set stepOffset to zero to prevent player moving and sticking to the ceiling.
            }

        }
        else if (characterController.isGrounded && verticalVelocity < 0 && !IsSliding)
        {
            canDoubleJump = true; //Resets double jump when grounded
            if (characterController.stepOffset == 0) //Resets stepOffset back to the base.
            {
                characterController.stepOffset = baseStepOffSet;
            }

            if (OnSlope)
            {

                verticalVelocity = -20; //Helps player stick to slopes

            }
            else
            {
                verticalVelocity = -1; //Keep this at -1. Basically resets gravity when grounded.
            }

        }

        if (CanSlideOnSlopes && IsSliding && characterController.isGrounded)
        {
            velocity += new Vector3(hitPointNormal.x, -hitPointNormal.y, hitPointNormal.z) * slopeSlideSpeed;
        }


    }

    public void SetVerticalVelocity(float n) //Public method to allow changing player's vertical velocity from other scripts
    {
        verticalVelocity = n;
    }

    //NOTE: All hook stuff is a mess that communicates with 2 other scripts. It works but needs refactoring.

    private void HandleHookPullMovement()
    {
        canDoubleJump = true;

        CanHook = false;
        
        Vector3 hookshotDir = (launcher.HookHitPosition - transform.position).normalized;

        characterController.Move(hookshotDir * launcher.hookPullSpeed * Time.deltaTime);

        float reachedHookHitPosition = 2f;

        if(Vector3.Distance(transform.position, launcher.HookHitPosition) < reachedHookHitPosition)
        {
            StartCoroutine(ClingDelay());
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            verticalVelocity = 0;
            verticalVelocity = doubleJumpForce;
            canDoubleJump = false;
            state = State.Normal;
            launcher.DestroyActiveHook();
        }


    }

    private void HookClinging()
    {
        animator.SetBool(IS_JUMP, false);
        animator.SetBool(IS_RUN, false);
        if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            launcher.DestroyActiveHook();
            verticalVelocity = 0;
            state = State.Normal;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            verticalVelocity = 0;
            verticalVelocity = doubleJumpForce;
            canDoubleJump = true;
            state = State.Normal;
            launcher.DestroyActiveHook();
        }

    }

    IEnumerator ClingDelay()
    {
        
        yield return new WaitForSeconds(.1f);
        state = State.Clinging;
        yield return null;
    }

}
