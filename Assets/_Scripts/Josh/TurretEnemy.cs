using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Authored by Joshua Hilliard
public class TurretEnemy : EnemyBase
{
    [HideInInspector] public Transform Player;
    public Transform turretHead;

    [SerializeField] private float followSpeed = 3;
    [SerializeField] private float range = 20;

    [SerializeField] private float fireRate = 2;
    private float fireTimer;

    private float distanceFromPlayer;
    private Vector3 colliderCenter;

    private float LockOnLength = 1.5f;
    private float LockOnTimer;

    [SerializeField] private GameObject Projectile;
    [SerializeField] private Transform ProjectileSpawn;
    [SerializeField] private GameObject MuzzleFlashParticle;

    [SerializeField] private AudioClip shootSFX;

    public LayerMask RaycastLayerIgnore;

    private enum State
    {
        Idle,
        Attacking,
    }

    private State currentState = State.Idle;
    

    private void Start()
    {
        //Player = GameObject.Find("Player").transform;
        Player = GameObject.FindGameObjectWithTag("Player").transform;
        colliderCenter = Player.gameObject.GetComponent<CharacterController>().center;
        LockOnTimer = LockOnLength;
        fireTimer = fireRate;
    }

    private void Update()
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

        distanceFromPlayer = Vector3.Distance(Player.position, turretHead.position);

    }

    private void IdleState()
    {
        ReturnToNormal();
        if (distanceFromPlayer <= range)
        {
            Vector3 turretLookDir = (Player.position + colliderCenter) - turretHead.position;
            if (Physics.Raycast(turretHead.position, turretLookDir, out RaycastHit hit, range, ~RaycastLayerIgnore))
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
        Aim();
        Shoot();

        Vector3 turretLookDir = (Player.position + colliderCenter) - turretHead.position;
        if (distanceFromPlayer <= range)
        {
            if (Physics.Raycast(turretHead.position, turretLookDir, out RaycastHit hit, range, ~RaycastLayerIgnore))
            {

                if (hit.transform.CompareTag("Player"))
                {
                    LockOnTimer = LockOnLength;
                }


                if (!hit.transform.CompareTag("Player"))
                {
                    LockOnTimer -= Time.deltaTime;
                    if (LockOnTimer <= 0)
                    {
                        currentState = State.Idle;
                        fireTimer = fireRate;
                    }
                }
            }

        }
        else
        {
            fireTimer = fireRate;
            currentState = State.Idle;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        //Gizmos.DrawWireSphere(turretHead.position, range);
    }

    private void Aim()
    {
        Vector3 turretLookDir = (Player.position + colliderCenter) - turretHead.position;
        Quaternion look = Quaternion.LookRotation(turretLookDir, transform.up);
        Vector3 rotation = Quaternion.Lerp(turretHead.rotation, look, followSpeed * Time.deltaTime).eulerAngles;
        turretHead.rotation = Quaternion.Euler(rotation);
    }

    private void Shoot()
    {
        fireTimer -= Time.deltaTime;

        if(fireTimer <= 0)
        {
            AudioManager.instance.PlaySound(shootSFX, transform.position);
            GameObject projectile = Instantiate(Projectile, ProjectileSpawn.position, ProjectileSpawn.rotation);
            GameObject muzzleflash = Instantiate(MuzzleFlashParticle, ProjectileSpawn.position, ProjectileSpawn.rotation);
            Physics.IgnoreCollision(projectile.GetComponent<Collider>(), gameObject.GetComponent<Collider>());
            fireTimer = fireRate;
        }
    }

    private void ReturnToNormal()
    {
        Vector3 rotation = Quaternion.Lerp(turretHead.localRotation, Quaternion.Euler(0,0,0), 2 * Time.deltaTime).eulerAngles;
        turretHead.localRotation = Quaternion.Euler(rotation);
    }

}

