using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// This Class handles all item functionalities. It allows for the use of items in menus,
/// equipping items to the player to use in the scene, anyother things we might need.
/// </summary>
public static class ItemHandler
{
    
    /// <summary>
    /// This takes in the string of the item that is being used and 
    /// a game object as the actor; the actor is the game object that get's affected.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="actor"></param>
    public static void PerformItemAction(string item, GameObject actor)
    {
        if(actor.tag == "Player")
        {
            //if(item is a weapon, change the equiped item on the player)
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

    static void EquipWeapon(string itemName, GameObject player)
    {
        string filePath; // = folder for the weapon equips + itemName
        //place the active weapon in the players active weapon slot and enable the game object
        //there has to be a better way to do this, especially if we want upgrading weapons
        //but for now this should do

    }






}
