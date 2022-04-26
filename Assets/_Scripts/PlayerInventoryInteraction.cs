using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventoryInteraction : MonoBehaviour
{
    public InventoryManager inventory;
    public GameObject[] weaponList;
    public WeaponBase activeWeapon;

    private HookLauncher hl;

    // Start is called before the first frame update
    void Start()
    {
        hl = GetComponent<HookLauncher>();
        hl.ChangeLaunchSource(activeWeapon.gameObject.transform.Find("bullet-spawn").transform); //temporary, sets hook's source to bullet spawn point of active weapon
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
            Reload();
        weaponList[0].GetComponent<PrinterPistol>().PrinterRegenerate();
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
                return;
            }
        }

        hl.ChangeLaunchSource(activeWeapon.gameObject.transform.Find("bullet-spawn").transform);
    }

}
