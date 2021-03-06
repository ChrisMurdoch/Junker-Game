using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Authored by Joshua Hilliard

/*Edits by Christine Murdoch:
    animation code
    regions
    required components
*/

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(HookLauncher))]
public class PlayerController : MonoBehaviour
{
    public InventoryManager inventory; //reference to inventory script / canvas
    public KeyCode interactKey; // ex. "e", "Tab", "Mouse 0"
    private bool inPickupRange;
    private GameObject currPickup; //holds the pickup we were last in range of

    private Animator anim; //holds animator for player (for animations that need to be played from this script)

    //PLEASE MAKE A COPY OF THE SCRIPT WHEN ADDING STUFF IF YOU'RE NOT THE AUTHOR^

    private bool canDoubleJump = true;
    private bool OnSlope;

    public bool doubleJumpUnlocked = true;
    public bool hookLauncherUnlocked = true;

    private CharacterController characterController;

    [HideInInspector] public Vector3 velocity; //made public to allow for animations to be handled in different script
    private float verticalVelocity;

    private float baseStepOffSet;

    [HideInInspector] public bool isCrouching;
    private bool underObject;

    private bool isWallSliding = false;
    private bool touchingWall = false;
    private ControllerColliderHit wallHit;
    private Vector3 wallNormal;
    private Vector3 wallHitDirection;

    private float x;
    private float xAir;

    [HideInInspector] public State currentState; //A simple state machine to keep track of player states

    Vector3 impact = Vector3.zero;
    float mass = 3.0F; // defines the character mass for adding impacts

    private HookLauncher hooklauncher;
    private bool CanHook = true;

    [HideInInspector] public enum State
    {
        Normal = 0, Hooking = 1, Clinging = 2, Paused = 3

        //Normal is normal movement
        //Hooking is when the hook collides with a wall and beings pulling the player
        //Clinging is when the player is latching to the wall
    }


    [Header("Movement Parameters")]
    public float moveSpeed = 10.0f; //Grounded speed, changed to public so movement can freeze during certain animations
    public float airSpeed = 12.0f; //Air speed, this should generally be higher that moveSpeed
    [SerializeField] private float crouchSpeedPct = 0.5f; //multiply movespeed by this when crouching
    [SerializeField] private float crouchHeightPct = 0.5f; // multiply this to character controller's height while crouched
    [SerializeField] private float wallSlideSpeed = 4.0f;
    [SerializeField] private float backwardSpeedPct = 0.5f; //multiply this * moveSpeed when moving backward
    public float zPosition = 0.0f; //zPosition the characer should stay at

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

    [Header("Sound Effects")]
    [SerializeField] private AudioClip jumpSFX;
    [SerializeField] private AudioClip hookLauncherSFX;

    //[Header("Other Parameters")]
    //[SerializeField] private LayerMask WhatIsGround; 
    //[SerializeField] public Transform GroundCheckPosition; 

    private Vector3 hitPointNormal;

    private PlayerAnimator pa;
    private float ccTopY; //origin is at feet for animation, this keeps track of where the top of the character controller is
    
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
        hooklauncher = GetComponent<HookLauncher>();
        characterController = GetComponent<CharacterController>();
        baseStepOffSet = characterController.stepOffset;
        anim = GetComponent<Animator>();
        pa = GetComponent<PlayerAnimator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        inPickupRange = false;

        //use transform.position & cc's height to get original top position
        ccTopY = transform.position.y + characterController.height;
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            default:
            case State.Normal:
                anim.SetBool("hooked", false);
                SlopeCheck();
                InputHandler();
                //Crouch();
                WallSlide();
                GravityHandler();

                //walking forward to the left
                if(!pa.facingRight && pa.finishedTurn && !anim.GetBool("backwards")) {
                    velocity.x *= -1.0f;
                    Debug.Log("VELOCITY = " + velocity);
                    characterController.Move(velocity * Time.deltaTime);

                //walking backward to the left
                } else if (!pa.facingRight && pa.finishedTurn && anim.GetBool("backwards")) {
                    velocity.x *= -1.0f;
                    Debug.Log("VELOCITY = " + velocity);
                    characterController.Move(velocity * Time.deltaTime);
                }
                
                else {
                    characterController.Move(velocity * Time.deltaTime);
                }

                WallJump();
                JumpHandler();


                if(hookLauncherUnlocked)
                    UseHook();

                break;
                
            case State.Hooking:
                anim.SetBool("hooked", true);
                HookPull();
                break;
            case State.Clinging:
                anim.SetBool("hooked", true);
                HookCling();
                break;
            case State.Paused:
                Debug.Log("PAUSED STATE");
                break;
        }

        //if player gets off z = 0, smoothly move them back
        if(transform.position.z != zPosition)
        {
            Vector3 moveOffset  = new Vector3 (0.0f, 0.0f, (zPosition - transform.position.z) * 0.05f);
            characterController.Move (moveOffset);
        }


        if (inPickupRange && Input.GetKeyDown(interactKey))
        {
            bool weaponPickup = false;
            if (currPickup.GetComponent<Weapon>() != null)
                weaponPickup = true;
            inventory.PickUpItem(currPickup, weaponPickup);
            inPickupRange = false;
        }

    }

    public void ChangeState(int n) //A public method to allow other scripts to change the players state
    {
        currentState = (State)n;
    }

    public void SetVerticalVelocity(float n) //Public method to allow changing player's vertical velocity from other scripts
    {
        Debug.Log("Called SetVerticalVelocity");
        verticalVelocity = n;
    }

    public void AddJumpForce()
    {
        verticalVelocity = 0;
        AudioManager.instance.PlaySound(jumpSFX, transform.position);
        if(doubleJumpUnlocked && !characterController.isGrounded && canDoubleJump) {
            anim.ResetTrigger("needsLanding"); //clear any landing trigger for prev jump
            verticalVelocity = doubleJumpForce;
        } else {
            verticalVelocity = jumpForce;
        }
    }

    private void InputHandler() //Gets the input of the "Horizontal Axis" or keys A & D.
    {

        x = Input.GetAxisRaw("Horizontal"); //is equal to -1 if pressing A, 1 if pressing D, or 0 if pressing neither
        xAir = Input.GetAxis("Horizontal"); //Same as above but ramps to -1 or 1.

        if (characterController.isGrounded)
        {
            velocity = (transform.forward * x) * moveSpeed + Vector3.up * verticalVelocity;
            /* A Vector3 used with charactercontroller.move()
            Transform.right is shorthand for the X axis of the gameobject * x which is either -1, 0, or 1, then multiplied by the movespeed
            Vector3.up * verticalVelocity is gravity and jump height, with Vector3.up being short for Vector3(0,1,0) */

            if (isCrouching)
            {
                velocity.x *= crouchSpeedPct;
            }
        }

        if (!characterController.isGrounded)
        {
            velocity = (transform.forward * xAir) * airSpeed + Vector3.up * verticalVelocity;
        }

        if (anim.GetBool("backwards"))
            velocity.x *= backwardSpeedPct; //only a percent of movement speed when backwards
    }


#region TriggersForPickups
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
#endregion

#region NormalStateMovement

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

    private void Crouch()
    {
        if ((Input.GetKey(KeyCode.S) || underObject) && characterController.isGrounded) //changed to "down" button for crouching
        {

            if (!isCrouching) //if you weren't already crouched
            {
                characterController.height *= crouchHeightPct; //get amt of current height for crouch

                //multiply by same percent to get correct center height, keep x and z the same
                characterController.center = new Vector3(characterController.center.x, 
                                                    characterController.center.y * crouchHeightPct,
                                                    characterController.center.z);
                
                ccTopY = transform.position.y + characterController.height;
            }

            isCrouching = true;


            //turn position at top of character to vector3 for raycasting
            Vector3 rayCastPos = new Vector3(transform.position.x, ccTopY, transform.position.z);
            
            //raycast from top of charactercontroller to find any block directly above
            if (Physics.Raycast(rayCastPos, Vector3.up, out RaycastHit hit, 1))
            {
                Debug.DrawRay(rayCastPos, Vector3.up * hit.distance, Color.green); 
                underObject = true;
            }
            else
            {
                underObject = false;
            }
        }
       
        else //became ungrounded or not under object & not holding s key
        {
            UnCrouch();
        }
    }

    

    private void UnCrouch()
    {        
        if(isCrouching) {

            characterController.height /= crouchHeightPct; //reverse operation to get back to original height

            //reverse operation on y center, keep x and z the same
            characterController.center = new Vector3(characterController.center.x, 
                                                characterController.center.y / crouchHeightPct,
                                                characterController.center.z);

            ccTopY = transform.position.y + characterController.height; //recalculate top of character controller
        }

        isCrouching = false;
    }

    private void WallSlide()
    {
        // if(touchingWall) {            

        //     // get touched wall's normal (will use if wall jump occurs)
        //     wallNormal = GetWallNormals();
            
        //     //no normal = not touching a wall
        //     if (wallNormal == Vector3.zero) {
        //         Debug.Log("MOVED OFF WALL");
        //         wallHitDirection = Vector3.zero; //reset direction when not touching wall
        //         touchingWall = false;
        //     }
        // }

        if (isWallSliding) {
            //Debug.Log("WALL SLIDING");
            anim.applyRootMotion = false;
        }

        if (characterController.isGrounded || (characterController.collisionFlags != CollisionFlags.Sides))
        {
            //Debug.Log("STOP WALL SLIDE");
            isWallSliding = false;
            anim.applyRootMotion = true;
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        //Debug.Log("isGrounded = " + characterController.isGrounded);
        if ((characterController.collisionFlags == CollisionFlags.Sides) && !characterController.isGrounded) 
        {
            //verticalVelocity = 0f;
            Debug.DrawRay(hit.point, hit.normal, Color.red, 1.25f);
            wallHit = hit;
            // Debug.Log("HIT OBJECT = " + wallHit.gameObject.name);
            // Debug.Log("wall x = " + wallHit.transform.position.x);
            // Debug.Log("player x  = " + wallHit.transform.position.x);
            // if(wallHit.transform.position.x < transform.position.x) {
            //     Debug.Log("LEFT WALL");
            //     wallHitDirection = -Vector3.right;
            // } else  {
            //     Debug.Log("RIGHT WALL");
            //     wallHitDirection = Vector3.right;
            // }

            // touchingWall = true;

            if (verticalVelocity <= 0)
            {
                isWallSliding = true;
                verticalVelocity = -wallSlideSpeed;
            }

        }

    }

    // private Vector3 GetWallNormals()
    // {
    //     //raycast starts at player's center
    //     Vector3 startPoint = transform.position + characterController.center;
    //     Debug.DrawRay(startPoint, wallHitDirection, Color.blue, 0.1f);

    //     //raycast short distance from point on player to direction of wall
    //     if(Physics.Raycast(startPoint, wallHitDirection, out RaycastHit rayHit, 0.05f)) {
    //         return rayHit.normal;
    //     }
    //     else {
    //         Debug.Log("NOT ON WALL");
    //         return Vector3.zero; //return 0 if you aren't touching a wall
    //     }
    // }

    public void AddImpact(Vector3 dir, float force) //Allows adding "force" with a character controller
    {
        // dir.Normalize();
        impact = dir.normalized * force / mass;
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

        impact = Vector3.Lerp(impact, Vector3.zero, Time.deltaTime);
    }

    private void JumpHandler()
    {

        if (Input.GetKeyDown(KeyCode.Space) && characterController.isGrounded && !IsSliding && !isWallSliding) //If player is grounded and not sliding, verticalVelocity becomes the jumpforce
        {
            //verticalVelocity = 0;

            // if (isCrouching)
            //     UnCrouch(); //jump automatically un-does crouch

            anim.SetTrigger("jumping");
            AddJumpForce();
        }

        if(doubleJumpUnlocked) {
            if (Input.GetKeyDown(KeyCode.Space) && !characterController.isGrounded && canDoubleJump == true && !isWallSliding) //If player is not grounded, then they can double jump
            {
                //verticalVelocity = 0;
                anim.SetTrigger("jumping");
                AddJumpForce();
                canDoubleJump = false;
            }
        }
    }

    private void GravityHandler()
    {
        if (!characterController.isGrounded) //Adds artifical gravity if the player isn't grounded
        {

            OnSlope = false; //Can't be on a slope if they're not grounded

            //only increase downward velocity if not wall sliding
            if(!isWallSliding) {
                verticalVelocity -= gravity * Time.deltaTime;
            }
            if (characterController.collisionFlags == CollisionFlags.Above) //If player hits a ceiling, stop adding upward velocity
            {
                Debug.Log("HIT CEILING");
                verticalVelocity = 0;                //Stop adding any more velocity.
                characterController.stepOffset = 0;                 //Set stepOffset to zero to prevent player moving and sticking to the ceiling.
            }

        }
        else if (characterController.isGrounded && verticalVelocity < 0 && !IsSliding)
        {
            impact = Vector3.zero; //reset impact value when you hit the ground
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
            Debug.Log("Slope Sliding");
            velocity += new Vector3(hitPointNormal.x, -hitPointNormal.y, hitPointNormal.z) * slopeSlideSpeed;
        }

    }

    private void UseHook()
    {
        if (Input.GetMouseButton(1) && CanHook) //Right mouse buttons throws the hook
        {
            hooklauncher.LaunchHook();
            AudioManager.instance.PlaySound(hookLauncherSFX, transform.position);
            CanHook = false;
        }

        if (characterController.isGrounded && !IsSliding && hooklauncher.currentHook == null)
        {
            CanHook = true;
        }
    }

    #endregion

#region HookAndClingStateFunctions

    private void HookPull()
    {
        canDoubleJump = transform;
        Vector3 hookshotDir = (hooklauncher.hookHitPosition - transform.position).normalized; //angle between player's body and hook
        velocity = hookshotDir * PullSpeed * Time.deltaTime;
        characterController.Move(velocity);

        float reachedHookHitPosition = 2f; //how close the player should be to the hook to finish moving

        ccTopY = transform.position.y + characterController.height; //get position of top of character controller

        //vector3 representing position at top of character controller (accounts for player's height when reaching hook point)
        Vector3 playerTopPosition = new Vector3(transform.position.x, ccTopY, transform.position.z);

        //move to clinging state if you reach the hook
        if (Vector3.Distance(playerTopPosition, hooklauncher.hookHitPosition) < reachedHookHitPosition)
        {
            StartCoroutine(ClingDelay());
        }

        //jump from hooked position
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StopAllCoroutines(); //stops cling delay
            verticalVelocity = 0;
            verticalVelocity = doubleJumpForce; //jumping from hooked state counts as double jump
            canDoubleJump = false;

            hooklauncher.DestroyActiveHook(); //destroy hook when jumping
            hooklauncher.DidHitWall = false; //hook is no longer stuck to wall
            currentState = State.Normal; //go back to normal state
        }
    }

    private void HookCling()
    {

        anim.applyRootMotion = false; //turn off root motion (prevents model from sinking back down when clinging)

        //if walking, fall from hooked position
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            anim.applyRootMotion = true; //re-activate root motion for walking / turning
            verticalVelocity = 0;
            hooklauncher.DestroyActiveHook();
            currentState = State.Normal;
        }

        //jump from hooked position
        if (Input.GetKeyDown(KeyCode.Space))
        {
            anim.applyRootMotion = true; //re-activate root motion for jump
            anim.SetTrigger("jumping"); //start jump animation, anim event adds force

            canDoubleJump = true;
            currentState = State.Normal;
            hooklauncher.DestroyActiveHook();
        }

    }

    IEnumerator ClingDelay()
    {
        yield return new WaitForSeconds(.1f);
        currentState = State.Clinging;
        yield return null;
    }
    
#endregion

}

