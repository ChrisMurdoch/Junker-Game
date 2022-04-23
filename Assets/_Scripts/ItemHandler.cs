using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// This Class handles all item functionalities. It allows for the use of items in menus,
/// equipping items to the player to use in the scene, anyother things we might need.
/// </summary>
public static class ItemHandler
{

    static private List<string> guns = new List<string> { "Printer Pistol", "AutoRifle", "Shotgun" };
    static private List<string> medicine = new List<string> { "Medkit" };

    /// <summary>
    /// This takes in the string of the item that is being used and 
    /// a game object as the actor; the actor is the game object that get's affected.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="actor"></param>
    public static void PerformItemAction(string item, GameObject actor)
    {

        if (actor.tag == "Player")
        {
            if (guns.Contains(item))
                EquipWeapon(item, actor);
            if (medicine.Contains(item))
                HealPlayer(item, actor);
            //if(item is a healing one, heal the player)
        }
        else if(actor.tag == "Inventory")
        {
            //if(item is a pack expansion, increase the size of the player inventory limit)
        }
        else if(actor.tag == "HotBar")
        {

        }

    }

    static void HealPlayer(string itemName, GameObject player)
    {
        float healamt = 0;
        switch(itemName)
        {
            case "Medkit":
                healamt = 50;
                break;
        }
        player.GetComponent<PlayerStats>().RestoreHealth(healamt);
    }

    static void EquipWeapon(string itemName, GameObject player)
    {
        player.GetComponent<PlayerInventoryInteraction>().ActivateWeapon(itemName);
    }






}
