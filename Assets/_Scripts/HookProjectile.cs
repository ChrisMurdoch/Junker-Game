using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookProjectile : MonoBehaviour
{
    private Rigidbody rb;
    private GameObject Player;

    [HideInInspector] public HookLauncher launcher;
    [HideInInspector] public Vector3 latchPosition;

    float distanceTravelled = 0;
    float testvariable;
    Vector3 lastPosition;

    private float hookSpeed;
    private float returnSpeed;
    private float maxLength;

    // Start is called before the first frame update
    void Start()
    {
        lastPosition = transform.position;

        rb = GetComponent<Rigidbody>();
        rb.velocity = transform.forward * hookSpeed;

        Player = FindObjectOfType<PlayerController>().gameObject;

        Physics.IgnoreCollision(Player.GetComponent<Collider>(), GetComponent<Collider>());
    }

    // Update is called once per frame
    void Update()
    {
        float distance = Vector3.Distance(lastPosition, transform.position);
        distanceTravelled += distance;
        lastPosition = transform.position;

        //Debug.Log(distanceTravelled);

        if (distanceTravelled >= maxLength)
        {
            //rb.velocity = -transform.forward * 50;
            rb.velocity = Vector3.zero;
            transform.position = Vector3.MoveTowards(transform.position, Player.transform.position, returnSpeed * Time.deltaTime);
            Physics.IgnoreCollision(Player.GetComponent<Collider>(), GetComponent<Collider>(), false);
            GetComponent<Collider>().enabled = false;
            if(transform.position == Player.transform.position)
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
        }

        if (collision.gameObject.CompareTag("Environment"))
        {
            latchPosition = collision.GetContact(0).point;
            launcher.HookHitPosition = latchPosition;
            Player.GetComponent<PlayerController>().ChangeState(1);
           
        }

        rb.constraints = RigidbodyConstraints.FreezeAll;
        rb.velocity = Vector2.zero;
    }

    public void SetHookParameters(float HookSpeed, float ReturnSpeed, float MaxLength)
    {
        hookSpeed = HookSpeed;
        returnSpeed = ReturnSpeed;
        maxLength = MaxLength;
    }

}
