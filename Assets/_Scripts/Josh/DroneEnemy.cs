using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Authored by Joshua Hilliard
public class DroneEnemy : EnemyBase
{
    public Transform Player;
    public Transform Gun;

    [SerializeField] private float range = 20;
    [SerializeField] private float stoppingRange = 10;
    [SerializeField] private float rotationSpeed = 3;
    [SerializeField] private float chaseSpeed = 3;

    [SerializeField] private float fireRate = 2;
    private float fireTimer;

    [SerializeField] private float positionChangeTimer = 4;
    private float idleTimer;

    private float AggroTimer;
    [SerializeField] private float AggroLength = 3;

    private float distanceFromPlayer;

    [SerializeField] private GameObject Projectile;
    [SerializeField] private Transform ProjectileSpawn;
    [SerializeField] private GameObject MuzzleFlashParticle;


    public LayerMask RaycastLayerIgnore;

    public AudioClip idleSound;
    public AudioClip attackSound;

    private enum State
    {
        Idle,
        Attacking,
    }

    private State currentState = State.Idle;


    // Start is called before the first frame update
    void Start()
    {
        //Player = GameObject.Find("Player").transform;
        Player = GameObject.FindGameObjectWithTag("Player").transform;
        Physics.IgnoreCollision(Player.GetComponentInParent<CharacterController>(), gameObject.GetComponent<Collider>());
        fireTimer = fireRate;
        idleTimer = positionChangeTimer;

    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case State.Idle:
                IdleState();
                break;
            case State.Attacking:
                AttackingState();
                break;
        }

        if (currentHealth <= 0)
        {
            Die();
        }

        distanceFromPlayer = Vector3.Distance(Player.position, transform.position);

    }

    private void IdleState()
    {
        ReturnToNormal();
        AggroTimer = AggroLength;

        if (distanceFromPlayer <= range)
        {
            Vector3 DirectionToPlayer = Player.position - gameObject.transform.position;
            if (Physics.Raycast(gameObject.transform.position, DirectionToPlayer, out RaycastHit hit, range, ~RaycastLayerIgnore))
            {
                if (hit.transform.CompareTag("Player"))
                {
                    currentState = State.Attacking;
                }
            }
        }
    }

    private void AttackingState()
    {

        LookAtPlayer();
        Aim();
        Shoot();

        Vector3 DirectionToPlayer = Player.position - gameObject.transform.position;

        if (distanceFromPlayer <= range)
        {
            if (distanceFromPlayer >= stoppingRange)
            {
                transform.position = Vector3.MoveTowards(transform.position, Player.position, chaseSpeed * Time.deltaTime);
            }

            if (Physics.Raycast(gameObject.transform.position, DirectionToPlayer, out RaycastHit hit, range, ~RaycastLayerIgnore))
            {

                if (hit.transform.CompareTag("Player"))
                {
                    AggroTimer = AggroLength;
                }


                if (!hit.transform.CompareTag("Player"))
                {
                    AggroTimer -= Time.deltaTime;
                    if (AggroTimer <= 0)
                    {
                        currentState = State.Idle;
                        fireTimer = fireRate;
                    }
                }
            }

        }
        else
        {
            AggroTimer -= Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, Player.position, chaseSpeed * Time.deltaTime);

            if (AggroTimer <= 0)
            {
                currentState = State.Idle;
                fireTimer = fireRate;
            }
        }
    }

    private void Shoot()
    {
        fireTimer -= Time.deltaTime;

        if (fireTimer <= 0)
        {
            GameObject projectile = Instantiate(Projectile, ProjectileSpawn.position, ProjectileSpawn.rotation);
            GameObject muzzleflash = Instantiate(MuzzleFlashParticle, ProjectileSpawn.position, ProjectileSpawn.rotation);
            Physics.IgnoreCollision(projectile.GetComponent<Collider>(), gameObject.GetComponent<Collider>());
            fireTimer = fireRate;
        }
    }

    private void LookAtPlayer() //Makes the whole object look at the player
    {
        Vector3 LookDir = Player.position - transform.position;
        Quaternion look = Quaternion.LookRotation(-LookDir, Vector3.up);
        Vector3 rotation = Quaternion.Lerp(transform.rotation, look, rotationSpeed * Time.deltaTime).eulerAngles;
        transform.rotation = Quaternion.Euler(rotation);
    }

    private void Aim() //Aims the mounted gun at the player
    {
        Vector3 LookDir = Player.position - Gun.position;
        float singleStep = 1f * Time.deltaTime;
        Vector3 newDirection = Vector3.RotateTowards(Gun.forward, LookDir, singleStep, 0f);
        Debug.DrawRay(Gun.position, newDirection, Color.red);
        Quaternion look = Quaternion.LookRotation(newDirection);
        Vector3 test = look.eulerAngles;
        Gun.rotation = Quaternion.Euler(test.x, test.y, 0);

    }

    private void ReturnToNormal()
    {
        Vector3 rotation = Quaternion.Lerp(gameObject.transform.rotation, Quaternion.Euler(0, 0, 0), 2 * Time.deltaTime).eulerAngles;
        gameObject.transform.rotation = Quaternion.Euler(rotation);
    }

}
