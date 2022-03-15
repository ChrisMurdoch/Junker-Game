using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoshWeaponBase : MonoBehaviour
{
    [Header("Gun Info")]
    public int weaponID;
    public string weaponName;

    //Projectile instantiation data
    [Header("GameObject Parameters")]
    public GameObject projectile;
    public Transform projectileSpawn;

    [Header("Gun Parameters")]
    public float fireRate;
    public float maxAmmo;
    public float damagePerShot;
    public float projectileSpeed;
    public float spread;
    public bool isSemiAuto;
    public bool infiniteAmmo;

    [Header("Current State Info")]
    public float currentAmmo;
    [SerializeField] private float shootTimer;
    [SerializeField] private bool canShoot;




    private void Start()
    {
        currentAmmo = maxAmmo;
    }

    private void Update()
    {
        FireRateTimer();
        InputHandler();
    }

    protected virtual void Shoot()
    {
        float x = Random.Range(-spread, spread);
        Quaternion rot = Quaternion.Euler(projectileSpawn.eulerAngles.x + x, projectileSpawn.eulerAngles.y, projectileSpawn.eulerAngles.z);
        GameObject FiredProjectile = Instantiate(projectile, projectileSpawn.transform.position, rot);
        PlayerProjectile proj = FiredProjectile.GetComponent<PlayerProjectile>();
        proj.SetParameters(projectileSpeed, damagePerShot);
        if (!infiniteAmmo)
        {
            currentAmmo--;
        }
        canShoot = false;
        shootTimer = fireRate;

    }

    protected virtual void Reload()
    {
        if(currentAmmo < maxAmmo)
        {
            currentAmmo = maxAmmo;
        }
    }

    protected virtual void InputHandler()
    {
        if (currentAmmo > 0 || infiniteAmmo)
        {
            if (isSemiAuto)
            {
                if (Input.GetMouseButtonDown(0) && canShoot)
                {
                    Shoot();

                }

            }
            else
            {
                if (Input.GetMouseButton(0) && canShoot)
                {
                    Shoot();
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            Reload();
        }
    }

    protected virtual void FireRateTimer()
    {
        shootTimer -= Time.deltaTime;
        if(shootTimer <= 0)
        {
            canShoot = true;
        }
    }

}