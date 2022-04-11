using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Authored by Joshua Hilliard


[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(HookLauncher))]
public class PlayerControllerNew : MonoBehaviour
{
    public InventoryManager inventory; //reference to inventory script / canvas
    public KeyCode interactKey; // ex. "e", "Tab", "Mouse 0"
    private bool inPickupRange;
    private GameObject currPickup; //holds the pickup we were last in range of

    public Animator anim; //holds animator for player (for animations that need to be played from this script)

    //PLEASE MAKE A COPY OF THE SCRIPT WHEN ADDING STUFF IF YOU'RE NOT THE AUTHOR^

    private bool canDoubleJump = true;
    private bool OnSlope;

    private CharacterController characterController;
    private GameObject PlayerBody;

    [HideInInspector] public Vector3 velocity; //made public to allow for animations to be handled in different script
    private float verticalVelocity;

    private float baseStepOffSet;

    [HideInInspector] public bool isCrouching;
    private bool underObject;

    private bool isWallSliding = false;
    private ControllerColliderHit wallHit;

    private float x;
    private float xAir;

    [HideInInspector] public State currentState; //A simple state machine to keep track of player states

    Vector3 impact = Vector3.zero;
    float mass = 3.0F; // defines the character mass for adding impacts

    private HookLauncher hooklauncher;
    private bool CanHook = true;

    [HideInInspector] public enum State
    {
        Normal, Hooking, Clinging

        //Normal is normal movement
        //Hooking is when the hook collides with a wall and beings pulling the player
        //Clinging is when the player is latching to the wall
    }


    [Header("Movement Parameters")]
    [SerializeField] private float moveSpeed = 10.0f; //Grounded speed
    [SerializeField] private float airSpeed = 12.0f; //Air speed, this should generally be higher that moveSpeed
    [SerializeField] private float crouchSpeed = 5.0f;
    [SerializeField] private float wallSlideSpeed = 4.0f;

    [Header("Jumping Parameters")] //For a snappier jump, turn these way up to a ratio of 1:2 ish or double the amount of gravity in the script
    [SerializeField] private float jumpForce = 20.0f;
    [SerializeField] private float doubleJumpForce = 25.0f;
    [SerializeField] private float WallJumpHeight = 20.0f;
    [SerializeField] private float WallJumpPropulsion = 150.0f;
    [SerializeField] private float gravity = 60.0f;

    [Header("Slope Parameters")]
    [SerializeField] private float slopeSlideSpeed = 8.0f;
    [SerializeField] private bool CanSlideOnSlopes = true;

    [Header("Hook Launcher Parameters")]
    [SerializeField] private float PullSpeed = 20f;

    //[Header("Other Parameters")]
    //[SerializeField] private LayerMask WhatIsGround; 
    //[SerializeField] public Transform GroundCheckPosition; 

    private Vector3 hitPointNormal;
    private PlayerAnimator pa;
    
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
        //PlayerBody = transform.Find("Player Body").gameObject;
        hooklauncher = GetComponent<HookLauncher>();
        characterController = GetComponent<CharacterController>();
        baseStepOffSet = characterController.stepOffset;

        pa = GetComponent<PlayerAnimator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        inPickupRange = false;
    }

    // Update is called once per frame
    void Update()
    {

        switch (currentState)
        {
            default:
            case State.Normal:
                SlopeCheck();
                InputHandler();
                //Crouch();
                WallSlide();
                GravityHandler();
                Debug.Log("VELOCITY = " + velocity);

                //walking forward to the left
                if(!pa.facingRight && pa.finishedTurn && !anim.GetBool("backwards")) {
                    Debug.Log("FORWARD LEFT"); 
                    velocity.x *= -1.0f;
                    characterController.Move(velocity * Time.deltaTime);

                //walking backward to the left
                } else if (pa.facingRight && pa.finishedTurn && anim.GetBool("backwards")) {
                    velocity.x *= -1.0f;
                    characterController.Move(velocity * Time.deltaTime);
                }
                
                else {
                    characterController.Move(velocity * Time.deltaTime);
                }
                WallJump();
                JumpHandler();
                UseHook();
                break;
            case State.Hooking:
                HookPull();
                break;
            case State.Clinging:
                HookCling();
                break;

        }

        transform.position = new Vector3(transform.position.x, transform.position.y, 0); // Makes sure player stays at 0 on the Z axis


        if (inPickupRange && Input.GetKeyDown(interactKey))
        {
            bool weaponPickup = false;
            if (currPickup.GetComponent<Weapon>() != null)
                weaponPickup = true;
            inventory.PickUpItem(currPickup, weaponPickup);
            inPickupRange = false;
        }

    }


    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Pickups")
        {
            inPickupRange = true;
            currPickup = other.gameObject;
        }

    }

    void OnTriggerExit(Collider other)
    {
    if (other.gameObject == currPickup)
        inPickupRange = false;
    }

    public void AddImpact(Vector3 dir, float force) //Allows adding "force" with a character controller
    {
        dir.Normalize();
        impact += dir.normalized * force / mass;
    }


    public void ChangeState(int n) //A public method to allow other scripts to change the players state
    {
        currentState = (State)n;
    }

    private void SlopeCheck() //A check to see if the player is standing on a slope
    {
        if (characterController.isGrounded && Physics.Raycast(this.transform.position, Vector3.down, out RaycastHit slopeHit, 2f))
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

    // private void Crouch()
    // {
    //     if (Input.GetKey(KeyCode.C) && characterController.isGrounded)
    //     {
    //         isCrouching = true;
    //         PlayerBody.transform.localScale = new Vector3(1, .5f, 1);
    //         characterController.height = 1;
    //         characterController.center = new Vector3(0, -.5f, 0);
    //         PlayerBody.transform.localPosition = characterController.center;
    //         if (Physics.Raycast(transform.position, Vector3.up, out RaycastHit hit, 1))
    //         {
    //             underObject = true;
    //         }
    //         else
    //         {
    //             underObject = false;
    //         }
    //     }
    //     else if (underObject == true)
    //     {
    //         isCrouching = true;
    //         PlayerBody.transform.localScale = new Vector3(1, .5f, 1);
    //         characterController.height = 1;
    //         characterController.center = new Vector3(0, -.5f, 0);
    //         PlayerBody.transform.localPosition = characterController.center;

    //         if (Physics.Raycast(transform.position, Vector3.up, out RaycastHit hit, 1))
    //         {
    //             underObject = true;
    //         }
    //         else
    //         {
    //             underObject = false;
    //         }
    //     }
    //     else
    //     {
    //         isCrouching = false;
    //         PlayerBody.transform.localScale = new Vector3(1, 1, 1);
    //         characterController.height = 2;
    //         characterController.center = new Vector3(0, 0, 0);
    //         PlayerBody.transform.localPosition = characterController.center;
    //     }
    // }

    private void WallSlide()
    {
        if (characterController.isGrounded || ((characterController.collisionFlags & CollisionFlags.Sides) == 0))
        {
            isWallSliding = false;
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (((characterController.collisionFlags & CollisionFlags.Sides) != 0) && !characterController.isGrounded) //changed to match instructions from documentation
        {
            wallHit = hit;

            if (verticalVelocity <= 0)
            {
                isWallSliding = true;
                verticalVelocity = -wallSlideSpeed;
            }

        }

    }

    private void InputHandler() //Gets the input of the "Horizontal Axis" or keys A & D.
    {

        x = Input.GetAxisRaw("Horizontal"); //is equal to -1 if pressing A, 1 if pressing D, or 0 if pressing neither
        xAir = Input.GetAxis("Horizontal"); //Same as above but ramps to -1 or 1.

        if (characterController.isGrounded)
        {
            if (!isCrouching)
            {
                Debug.Log("X = " + x);
                velocity = (transform.forward * x) * moveSpeed + Vector3.up * verticalVelocity;
                /* A Vector3 used with charactercontroller.move()
                Transform.right is shorthand for the X axis of the gameobject * x which is either -1, 0, or 1, then multiplied by the movespeed
                Vector3.up * verticalVelocity is gravity and jump height, with Vector3.up being short for Vector3(0,1,0) */
            }
            else if (isCrouching)
            {
                velocity = (transform.forward * x) * crouchSpeed + Vector3.up * verticalVelocity;
            }
        }

        if (!characterController.isGrounded)
        {
            Debug.Log("NOT GROUNDED");
            velocity = (transform.forward * xAir) * airSpeed + Vector3.up * verticalVelocity;
        }


    }

    private void WallJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isWallSliding)
        {
            verticalVelocity = 0;
            verticalVelocity = WallJumpHeight;
            AddImpact(wallHit.normal, WallJumpPropulsion);
        }

        if (impact.magnitude > 0.2f)
        {
            characterController.Move(impact * Time.deltaTime);
        }

        impact = Vector3.Lerp(impact, Vector3.zero, 7 * Time.deltaTime);
    }

    private void JumpHandler()
    {

        if (Input.GetKeyDown(KeyCode.Space) && characterController.isGrounded && !IsSliding && !isWallSliding) //If player is grounded and not sliding, verticalVelocity becomes the jumpforce
        {
            //verticalVelocity = 0;
            anim.SetTrigger("jumping");
        }

        if (Input.GetKeyDown(KeyCode.Space) && !characterController.isGrounded && canDoubleJump == true && !isWallSliding) //If player is not grounded, then they can double jump
        {
            //verticalVelocity = 0;
            anim.SetTrigger("jumping");
            canDoubleJump = false;
        }
    }

    public void AddJumpForce()
    {
        Debug.Log("AddJumpForce called");
        verticalVelocity = 0;

        if(!characterController.isGrounded && canDoubleJump == true)
            verticalVelocity = doubleJumpForce;
        else
            verticalVelocity = jumpForce;
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
            characterController.stepOffset = baseStepOffSet;
            

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

    private void UseHook()
    {
        if (Input.GetMouseButton(1) && CanHook) //Right mouse buttons throws the hook
        {
            hooklauncher.LaunchHook();
            CanHook = false;
        }

        if (characterController.isGrounded && !IsSliding)
        {
            CanHook = true;
        }
    }

    private void HookPull()
    {
        canDoubleJump = transform;
        Vector3 hookshotDir = (hooklauncher.hookHitPosition - transform.position).normalized;
        velocity = hookshotDir * PullSpeed * Time.deltaTime;
        characterController.Move(velocity);

        float reachedHookHitPosition = 2f;

        if (Vector3.Distance(transform.position, hooklauncher.hookHitPosition) < reachedHookHitPosition)
        {
            StartCoroutine(ClingDelay());
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            StopAllCoroutines();
            verticalVelocity = 0;
            verticalVelocity = doubleJumpForce;
            canDoubleJump = false;
            currentState = State.Normal;
            hooklauncher.DestroyActiveHook();
        }
    }

    private void HookCling()
    {
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            hooklauncher.DestroyActiveHook();
            verticalVelocity = 0;
            currentState = State.Normal;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            verticalVelocity = 0;
            verticalVelocity = doubleJumpForce;
            canDoubleJump = true;
            currentState = State.Normal;
            hooklauncher.DestroyActiveHook();
        }

    }

    public void SetVerticalVelocity(float n) //Public method to allow changing player's vertical velocity from other scripts
    {
        verticalVelocity = n;
    }

    IEnumerator ClingDelay()
    {
        yield return new WaitForSeconds(.1f);
        currentState = State.Clinging;
        yield return null;
    }

}

