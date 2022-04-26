using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventoryInteraction : MonoBehaviour
{
    public InventoryManager inventory;
    public HotBarController hotBar;
    public GameObject[] weaponList;
    public WeaponBase activeWeapon;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Reload();

        }
        weaponList[0].GetComponent<PrinterPistol>().PrinterRegenerate();
        if (weaponList[0] == activeWeapon.gameObject)
            UpdateAmmoCount();
        if (activeWeapon.ExposedInputCall())
            UpdateAmmoCount();

    }

    private void UpdateAmmoCount()
    {
        hotBar.AmmoCounter.text =  activeWeapon.currentAmmo.ToString();
    }

    private void Reload()
    {
        if (activeWeapon.name == "Printer Pistol")
            return;
        inventory.ReloadActiveWeapon(activeWeapon);
    }

    public void ActivateWeapon(string itemName)
    {
        foreach(GameObject g in weaponList)
        {
            Debug.Log(g.name + " " + itemName);
            if (g.name == itemName)
            {
                
                activeWeapon.gameObject.SetActive(false);
                g.SetActive(true);
                activeWeapon = g.GetComponent<WeaponBase>();
                UpdateAmmoCount();
                return;
            }
        }
    }

}
