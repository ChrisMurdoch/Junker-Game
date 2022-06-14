using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Authored by John Murphy
public class Weapon : MonoBehaviour
{
    #region Script Design
    //This scripts purpose is to provide a base
    //for the weapons to be created from. 
    //It will likely use inheritence from Items
    //in order to make the inventory system


    //Universal spects of the weapons will include:
    //-Animation references
    //--particle effects attatched to the weapon
    //-Instantiation of projectiles
    //--particle effects attatched to the projectiles
    //-magazine capacity
    //-ammo count
    //-rate of fire
    //-accuracy
    //-recoil
    //--recoil decay
    //--effective acuracy (accuracy + recoil)
    //-Hud references for ui update
    //-A reload function
    //--It will need to communicate with the inventory manager
    #endregion 


    string id;
    string nameWeapon;
    public KeyCode shootkey;
    int damage;
    //Projectile instantiation data
    public GameObject ammoBullet;
    public Transform spawnPoint;



    //accuracy handling
    public bool isSemiAuto;
    float maxAngle = 120f;
    float accuracyBase = 0.9f;
    public float accuracyEft;
    float recoil = 0.05f;
    public float unrecoil = 0.1f;
    bool hasRecoil = false;
    void RecoilCalculator()//adds recoil to effecctive accuracy
    {
        accuracyEft -= recoil;
        if (accuracyEft < 0)
            accuracyEft = 0;
        hasRecoil = true;
    }
    void RecoilDeltaBase()//moves effective accuracy towards base accuracy
    {
        if (accuracyBase > accuracyEft)
            accuracyEft += unrecoil * Time.deltaTime;
        else
        {
            accuracyEft = accuracyBase;
            hasRecoil = false;
        }
    }
    float DeviationCalculator()//calculates the deviation value of the accuracy
    {
        float devAngle = 0f;
        float angleRange = maxAngle * (1 - accuracyEft) * 0.5f;
        devAngle = Random.Range(-angleRange, angleRange);
        return devAngle;
    }

    //ammo handling
    public int ammoCount = 10;
    public int magazineCapacity;
    public string ammoType;
    //Reload Function will provide a call to the inventory manager in order to get ammo reserve data

    //Rate of fire handling
    public float fireRate;
    float timer;
    bool hasShot = false;
    void ROFTimer()//Set's up the timer to handle fireRate
    {
        timer = 60.0f / fireRate;
        hasShot = true;
    }
    void UpdateTimer()//Updates the timer bring it towards zero
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            timer = 0;
            hasShot = false;
        }
    }


    //Hud and UI handling
    //ammo counter
    //other things I think of

    //Animation and fx handling for the weapon itself these will 
    //have thier own methods that get called in other functions

    private void Start()
    {
        accuracyEft = accuracyBase;
    }

    private void Update()
    {
        if (!hasShot)
            if (Input.anyKey)
                InputHandler();
            else
                UpdateTimer();
        else
            UpdateTimer();
        if (hasRecoil)
            RecoilDeltaBase();
    }

    private void InputHandler()
    {
        if (ammoCount > 0)
        {
            if (isSemiAuto)
            {
                if (Input.GetKeyDown(shootkey))
                    Shoot();
            }
            else
                if (Input.GetKey(shootkey))
                Shoot();
        }
    }

    /// <summary>
    /// This Method will differ depending on then weapon because while most will 
    /// use the original form, some weapons will have to have a unique shoot method
    /// </summary>
    protected virtual void Shoot()
    {
        //instnatiate projectile
        Vector3 pos = spawnPoint.position;
        Quaternion rot = Quaternion.Euler(spawnPoint.eulerAngles.x + DeviationCalculator(), spawnPoint.eulerAngles.y, spawnPoint.eulerAngles.z);
        GameObject bullet = Instantiate(ammoBullet, pos, rot);
        Projectile round = bullet.GetComponent<Projectile>();
        round.damage = damage;
        round.forceVal = 1000.0f;
        round.Launch();
        //edit components of the projectile with properties from the weapon
        //this way multiple weapons can reuse projectiles models
        RecoilCalculator();
        ammoCount--;
        ROFTimer();
    }


}
