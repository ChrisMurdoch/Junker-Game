using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Authored by Joshua Hilliard
public class HookProjectile : MonoBehaviour
{
    private Rigidbody rb;

    [HideInInspector] public Vector3 latchPosition;
    [HideInInspector] public HookLauncher hooklauncher;
    private GameObject Player;


    //private float distanceTravelled = 0;
    private Vector3 lastPosition;
    private Vector3 initialPosition;

    private float launchSpeed;
    private float returnSpeed;
    private float launchLength;

    private bool isReturning = false;
    //// Start is called before the first frame update
    void Start()
    {
        lastPosition = transform.position;
        initialPosition = transform.position;
        rb = GetComponent<Rigidbody>();
        rb.velocity = transform.forward * launchSpeed;
        Physics.IgnoreCollision(Player.GetComponent<Collider>(), GetComponent<Collider>());

    }

    // Update is called once per frame
    void Update()
    {

        //float distance = Vector3.Distance(lastPosition, transform.position);
        //distanceTravelled += distance;
        //lastPosition = transform.position;

        float distance = Vector3.Distance(initialPosition, transform.position);

        if (distance >= launchLength)
        {
            isReturning = true;
        }

        if (isReturning)
        {
            Debug.Log("HOOK RETURNING");
            
            rb.velocity = Vector3.zero;
            transform.position = Vector3.MoveTowards(transform.position, Player.transform.position, returnSpeed * Time.deltaTime);
            GetComponent<Collider>().enabled = false;
            if (transform.position == Player.transform.position)
            {
                Destroy(gameObject);
            }
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Environment"))
        {
            GetComponent<Collider>().enabled = false;
            isReturning = false;
            latchPosition = collision.GetContact(0).point;
            transform.rotation = Quaternion.FromToRotation(Vector3.up, collision.GetContact(0).normal);
            transform.position = collision.GetContact(0).point;
            hooklauncher.hookHitPosition = latchPosition;
            hooklauncher.DidHitWall = true;
            rb.constraints = RigidbodyConstraints.FreezeAll;
            rb.velocity = Vector3.zero;
        }
        else
        {
            isReturning = true;
        }

    }

    public void SetHookParameters(float launchSpeed, float returnSpeed, float launchLength, HookLauncher hooklauncher, GameObject Player)
    {
        this.launchSpeed = launchSpeed;
        this.returnSpeed = returnSpeed;
        this.launchLength = launchLength;
        this.hooklauncher = hooklauncher;
        this.Player = Player;
    }
}
