using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Authored by Pain and Suffering
public class TurretEnemy : EnemyBase
{
    public Transform Player;
    public Transform turretBase;
    public Transform turretBarrel;

    private void Start()
    {
        
    }

    private void Update()
    {
        //Vector3 dir = Player.position - transform.position;
        //Quaternion look = Quaternion.LookRotation(dir);
        //Vector3 rotation = Quaternion.Lerp(turretBase.rotation, look, 5 * Time.deltaTime).eulerAngles;
        //turretBase.localRotation = Quaternion.Euler(0f, rotation.y, 0f);

        //Vector3 dir2 = Player.position - turretBarrel.position;
        //Quaternion look2 = Quaternion.LookRotation(dir2);
        //Vector3 rotation2 = Quaternion.Lerp(turretBarrel.rotation, look2, 5 * Time.deltaTime).eulerAngles;
        ////turretBarrel.localRotation = Quaternion.Euler(turretBase.rotation.x, turretBase.rotation.y, rotation2.z);
        //turretBarrel.localRotation = Quaternion.Euler(0, 0, rotation2.z);

        Test();

    }

    void Aim(Vector3 targetPoint)
    {


    }

    private void Test()
    {
        //Vector3 turretLookDir = Player.position - transform.position;
        ////Quaternion look = Quaternion.LookRotation(turretLookDir, transform.up);
        //Quaternion look = XLookRotation(turretLookDir, transform.up);
        //Vector3 rotation = Quaternion.Lerp(turretBase.localRotation, look, 5 * Time.deltaTime).eulerAngles;
        ////Vector3 rotation = Quaternion.RotateTowards(turretBase.localRotation, look, 1).eulerAngles;
        //turretBase.localRotation = Quaternion.Euler(transform.rotation.x, rotation.y, transform.rotation.z);

        Vector3 turretLookDir = Player.position - transform.position;
        //Quaternion look = Quaternion.LookRotation(turretLookDir, transform.up);
        Quaternion look = XLookRotation(turretLookDir, transform.up);
        //Vector3 rotation = Quaternion.Lerp(turretBase.localRotation, look, 5 * Time.deltaTime).eulerAngles;
        Vector3 rotation = Quaternion.RotateTowards(turretBase.localRotation, look, 1).eulerAngles;

        turretBase.localRotation = Quaternion.Euler(transform.rotation.x, rotation.y, transform.rotation.z);


        //Vector3 flattenedVector = Vector3.ProjectOnPlane(-turretBase.right, Vector3.up);

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

