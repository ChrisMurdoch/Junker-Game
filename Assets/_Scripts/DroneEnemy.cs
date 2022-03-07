using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneEnemy : EnemyBase
{
    public Transform Player;
    public Transform Gun;

    [SerializeField] private float range = 20;
    [SerializeField] private float followSpeed = 3;
    [SerializeField] private float chaseSpeed = 3;

    [SerializeField] private float fireRate = 2;
    private float fireTimer;

    [SerializeField] private float positionChangeTimer = 4;
    private float idleTimer;

    private float distanceFromPlayer;

    [SerializeField] private GameObject Projectile;
    [SerializeField] private Transform ProjectileSpawn;


    // Start is called before the first frame update
    void Start()
    {
        fireTimer = fireRate;
        idleTimer = positionChangeTimer;

    }

    // Update is called once per frame
    void Update()
    {
        distanceFromPlayer = Vector3.Distance(Player.position, transform.position);

        if (distanceFromPlayer <= range)
        {
            transform.position = Vector3.MoveTowards(transform.position, Player.position, chaseSpeed * Time.deltaTime);
            Shoot();
            LookAtPlayer();
            Aim();
        }
        else
        {
            //Idle();
        }
    }

    //private void Idle()
    //{
    //    idleTimer -= Time.deltaTime;

    //    if(idleTimer <= 0)
    //    {
    //        float x = transform.position.x + Random.Range(-10, 10);
    //        float y = transform.position.x + Random.Range(-10, 10);
    //        Vector3 pos = new Vector3(x, y, 0);
    //        idleTimer = positionChangeTimer;

    //        StartCoroutine(ChangePosition(pos));

    //    }

    //}

    //IEnumerator ChangePosition(Vector3 pos)
    //{
    //    Vector3 LookDir = pos - transform.position;
    //    Quaternion look = Quaternion.LookRotation(-LookDir, Vector3.up);
    //    Vector3 rotation = Quaternion.Lerp(transform.rotation, look, followSpeed * Time.deltaTime).eulerAngles;
    //    transform.rotation = Quaternion.Euler(rotation);
    //    transform.position = Vector3.MoveTowards(transform.position, pos, chaseSpeed * Time.deltaTime);
    //    yield return null;
    //}

    private void Shoot()
    {
        fireTimer -= Time.deltaTime;

        if (fireTimer <= 0)
        {
            GameObject projectile = Instantiate(Projectile, ProjectileSpawn.position, ProjectileSpawn.rotation);
            fireTimer = fireRate;
        }
    }

    private void LookAtPlayer()
    {
        Vector3 LookDir = Player.position - transform.position;
        Quaternion look = Quaternion.LookRotation(-LookDir, Vector3.up);
        Vector3 rotation = Quaternion.Lerp(transform.rotation, look, followSpeed * Time.deltaTime).eulerAngles;
        transform.rotation = Quaternion.Euler(rotation);
    }

    private void Aim()
    {
        Vector3 LookDir = Player.position - Gun.position;
        float singleStep = 1f * Time.deltaTime;
        Vector3 newDirection = Vector3.RotateTowards(Gun.forward, LookDir, singleStep, 0f);
        Debug.DrawRay(Gun.position, newDirection, Color.red);
        Quaternion look = Quaternion.LookRotation(newDirection);
        Vector3 test = look.eulerAngles;
        Gun.rotation = Quaternion.Euler(test.x, test.y, 0);

    }

}
