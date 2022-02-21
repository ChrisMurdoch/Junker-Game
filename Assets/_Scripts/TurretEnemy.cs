using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        Vector3 dir = Player.position - transform.position;
        Quaternion look = Quaternion.LookRotation(dir);
        Vector3 rotation = Quaternion.Lerp(turretBase.rotation, look, 5 * Time.deltaTime).eulerAngles;
        turretBase.localRotation = Quaternion.Euler(0f, rotation.y, 0f);

        Vector3 dir2 = Player.position - turretBarrel.position;
        Quaternion look2 = Quaternion.LookRotation(dir2);
        Vector3 rotation2 = Quaternion.Lerp(turretBarrel.rotation, look2, 5 * Time.deltaTime).eulerAngles;
        //turretBarrel.localRotation = Quaternion.Euler(turretBase.rotation.x, turretBase.rotation.y, rotation2.z);
        turretBarrel.localRotation = Quaternion.Euler(0, 0, rotation2.z);


    }

    void Aim(Vector3 targetPoint)
    {


    }

}

