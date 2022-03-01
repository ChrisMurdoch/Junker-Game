using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneEnemy : EnemyBase
{
    public Transform Player;
    public Transform Gun;

    [SerializeField] private float range = 20;
    [SerializeField] private float followSpeed = 3;

    private float distanceFromPlayer;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        distanceFromPlayer = Vector3.Distance(Player.position, transform.position);

        if (distanceFromPlayer <= range)
        {
            LookAtPlayer();
            Aim();
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
