using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Authored by Pain and Suffering
public class TurretEnemy : EnemyBase
{
    public Transform Player;
    public Transform turretHead;

    [SerializeField] private float followSpeed = 3;
    [SerializeField] private float range = 20;

    [SerializeField] private float fireRate = 2;
    [SerializeField] private float fireTimer;

    private float distanceFromPlayer;

    private float LockOnLength = 1.5f;
    private float LockOnTimer;

    [SerializeField] private GameObject Projectile;
    [SerializeField] private Transform ProjectileSpawn;


    private void Start()
    {
        LockOnTimer = LockOnLength;
        fireTimer = fireRate;
    }

    private void Update()
    {
        distanceFromPlayer = Vector3.Distance(Player.position, turretHead.position);
        Vector3 turretLookDir = Player.position - turretHead.position;
        RaycastHit hit;

        //Debug.DrawLine(turretHead.position, turretHead.forward, Color.red);
        Debug.DrawRay(turretHead.position, turretHead.forward * range, Color.red);

        if (distanceFromPlayer <= 20)
        {
            if (Physics.Raycast(turretHead.position, turretLookDir, out hit, range))
            {

                if (hit.transform.CompareTag("Player"))
                {
                    Aim();
                    Shoot();
                    LockOnTimer = LockOnLength;
                }


                if (!hit.transform.CompareTag("Player"))
                {
                    LockOnTimer -= Time.deltaTime;
                    if(LockOnTimer <= 0)
                    {
                        ReturnToNormal();
                    }
                    else
                    {
                        Shoot();
                        Aim();
                    }
                }
            }

        }
        else
        {
            fireTimer = fireRate;
            ReturnToNormal();
        }

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        //Gizmos.DrawWireSphere(turretHead.position, 20);
    }

    private void Aim()
    {
        Vector3 turretLookDir = Player.position - turretHead.position;
        Quaternion look = Quaternion.LookRotation(turretLookDir, transform.up);
        Vector3 rotation = Quaternion.Lerp(turretHead.rotation, look, followSpeed * Time.deltaTime).eulerAngles;
        turretHead.rotation = Quaternion.Euler(rotation);
    }

    private void Shoot()
    {
        fireTimer -= Time.deltaTime;

        if(fireTimer <= 0)
        {
            GameObject projectile = Instantiate(Projectile, ProjectileSpawn.position, ProjectileSpawn.rotation);
            fireTimer = fireRate;
        }
    }

    private void ReturnToNormal()
    {
        Vector3 rotation = Quaternion.Lerp(turretHead.localRotation, Quaternion.Euler(0,0,0), 2 * Time.deltaTime).eulerAngles;
        turretHead.localRotation = Quaternion.Euler(rotation);
    }


    public Quaternion XLookRotation(Vector3 right, Vector3 up = default(Vector3))
    {
        if (up == default(Vector3))
            up = Vector3.up;

        Quaternion rightToForward = Quaternion.Euler(0f, 90f, 0f) * transform.rotation;
        Quaternion forwardToTarget = Quaternion.LookRotation(right, up);

        return forwardToTarget * rightToForward;
    }

}

