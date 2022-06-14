using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Authored by Joshua Hilliard

public class WeaponBase : MonoBehaviour
{
    [Header("Gun Info")]
    public int weaponID;
    public string weaponName;

    //Projectile instantiation data
    [Header("GameObject Parameters")]
    public GameObject projectile;
    public Transform projectileSpawn;
    public GameObject muzzleFlashParticle;

    [Header("Sound Effects")]
    public AudioClip shootSFX;

    [Header("Gun Parameters")]
    public float fireRate;
    public float maxAmmo;
    public float damagePerShot;
    public float projectileSpeed;
    public float spread;
    public bool isSemiAuto;
    public bool infiniteAmmo;
    public string ammoType;

    [Header("Animation Parameters")]
    public GameObject mainHand;
    public GameObject offHand;
    public Animator anim;

    [Header("Current State Info")]
    public float currentAmmo;
    [SerializeField] protected float shootTimer;
    [SerializeField] protected bool canShoot;

    [Header("Audio Intialization Parameters")]
    public AudioClip gunShotSound;
    public AudioClip reloadSound;

    private void Start()
    {
        currentAmmo = maxAmmo;
    }

    private void Update()
    {
        FireRateTimer();
    }

    public bool ExposedInputCall()
    {
        return InputHandler();
    }

    protected virtual void Shoot()
    {
        AudioManager.instance.PlaySound(shootSFX, projectileSpawn.position);
        float x = Random.Range(-spread, spread);
        Quaternion rot = Quaternion.Euler(projectileSpawn.eulerAngles.x + x, projectileSpawn.eulerAngles.y, projectileSpawn.eulerAngles.z);
        GameObject FiredProjectile = Instantiate(projectile, projectileSpawn.transform.position, rot);
        ProjectileBase proj = FiredProjectile.GetComponent<ProjectileBase>();
        proj.SetParameters(projectileSpeed, damagePerShot);
        if(anim != null)
        {
            anim.SetTrigger("fire"); //animate gunshot
        }
        Physics.IgnoreCollision(GameObject.FindWithTag("Player").GetComponent<CharacterController>(), proj.GetComponent<Collider>());
        if (!infiniteAmmo)
        {
            currentAmmo--;
        }
        canShoot = false;
        shootTimer = fireRate;


        GameObject muzzleflash = Instantiate(muzzleFlashParticle, projectileSpawn.transform.position, projectileSpawn.transform.rotation);
    }

    protected virtual void Reload()
    {
        if (currentAmmo < maxAmmo)
        {
            currentAmmo = maxAmmo;
        }
        AudioSource.PlayClipAtPoint(reloadSound, transform.position);
    }

    protected virtual bool InputHandler()
    {
        if (Time.timeScale != 0)
        {
            if (currentAmmo > 0 || infiniteAmmo)
            {
                if (isSemiAuto)
                {
                    if (Input.GetMouseButtonDown(0) && canShoot)
                    {
                        Shoot();
                        return true;
                    }

                }
                else
                {
                    if (Input.GetMouseButton(0) && canShoot)
                    {
                        Shoot();
                        return true;
                    }
                }
            }
        }
        return false;
    }

    protected virtual void FireRateTimer()
    {
        shootTimer -= Time.deltaTime;
        if (shootTimer <= 0)
        {
            shootTimer = 0;
            canShoot = true;
        }
    }

}
