using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Authored by Joshua Hilliard
public class PrinterPistol : WeaponBase
{

    [Header("Weapon Specific Parameters")]
    public float printTime = 1.0f;
    public float waitTime = 2.0f;

    [Header("Weapon Specific Current State Info")]
    [SerializeField] private float printTimer;
    [SerializeField] private float waitTimer;



    // Start is called before the first frame update
    void Start()
    {
        currentAmmo = maxAmmo;
        printTimer = printTime;
        waitTimer = waitTime;
    }

    // Update is called once per frame
    void Update()
    {
        base.FireRateTimer();
        InputHandler();
    }

    public void PrinterRegenerate()
    {
        Reload();
    }
    protected override void Reload()
    {
        if(currentAmmo < maxAmmo)
        {
            waitTimer -= Time.deltaTime;
                
            if(waitTimer <= 0)
            {
                printTimer -= Time.deltaTime;

                if (printTimer <= 0)
                {
                    currentAmmo++;
                    printTimer = printTime;
                }
            }

        }
    }

    protected override void InputHandler()
    {
        if (currentAmmo > 0 || infiniteAmmo)
        {
            if (isSemiAuto)
            {
                if (Input.GetMouseButtonDown(0) && canShoot)
                {
                    Shoot();
                    waitTimer = waitTime;
                }

            }
            else
            {
                if (Input.GetMouseButton(0) && canShoot)
                {
                    Shoot();
                    waitTimer = waitTime;

                }
            }
        }
    }
}
