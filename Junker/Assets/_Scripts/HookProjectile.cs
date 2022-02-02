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

    Vector3 lastPosition;

    float length = 20;

    // Start is called before the first frame update
    void Start()
    {
        lastPosition = transform.position;

        rb = GetComponent<Rigidbody>();
        rb.velocity = transform.forward * 50;

        Player = FindObjectOfType<PlayerController>().gameObject;

        Physics.IgnoreCollision(Player.GetComponent<Collider>(), GetComponent<Collider>());
    }

    // Update is called once per frame
    void Update()
    {
        float distance = Vector3.Distance(lastPosition, transform.position);
        distanceTravelled += distance;
        lastPosition = transform.position;

        Debug.Log(distanceTravelled);

        if (distanceTravelled >= length)
        {
            //rb.velocity = -transform.forward * 50;
            rb.velocity = Vector3.zero;
            transform.position = Vector3.MoveTowards(transform.position, Player.transform.position, 20 * Time.deltaTime);
            Physics.IgnoreCollision(Player.GetComponent<Collider>(), GetComponent<Collider>(), false);
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
            //Debug.Log(collision.gameObject.name);
            //Player.GetComponent<CharacterController>().enabled = false;
            //Player.GetComponent<PlayerController>().SetVerticalVelocity(0);
            //Player.transform.position = collision.GetContact(0).point;
            //Player.GetComponent<CharacterController>().enabled = true;
            latchPosition = collision.GetContact(0).point;
            launcher.hitPoint = latchPosition;
            Player.GetComponent<PlayerController>().ChangeState(1);
            

        }

        rb.constraints = RigidbodyConstraints.FreezeAll;
        rb.velocity = Vector2.zero;
    }

}
