using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Authored by Joshua Hilliard
public class Shotgun : JoshWeaponBase
{
    [Header("Weapon Specific Parameters")]
    public float NumberOfPellets = 10f;

    // Start is called before the first frame update
    void Start()
    {
        currentAmmo = maxAmmo;
    }

    // Update is called once per frame
    void Update()
    {
        base.FireRateTimer();
        base.InputHandler();
    }

    protected override void Shoot()
    {
        for (int i = 0; i < NumberOfPellets; i++)
        {
            float x = Random.Range(-spread, spread);
            Quaternion rot = Quaternion.Euler(projectileSpawn.eulerAngles.x + x, projectileSpawn.eulerAngles.y, projectileSpawn.eulerAngles.z);
            GameObject FiredProjectile = Instantiate(projectile, projectileSpawn.transform.position, rot);
            ProjectileBase proj = FiredProjectile.GetComponent<ProjectileBase>();
            proj.SetParameters(projectileSpeed, damagePerShot);
            Physics.IgnoreCollision(GameObject.FindWithTag("Player").GetComponent<CharacterController>(), proj.GetComponent<Collider>());

        }
        if (!infiniteAmmo)
        {
            currentAmmo--;
        }
        canShoot = false;
        shootTimer = fireRate;
    }
}
