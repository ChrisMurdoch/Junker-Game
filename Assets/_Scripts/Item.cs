using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;




/// <summary>
/// This will be a class attatched to the item pickups it will take certain data and 
/// create inventory objects when pickedup
/// </summary>
public class Item : MonoBehaviour
{
    #region Refactor
    public ItemDataSO data; //The data containing its pick up information
    public int ammount; //the number of items in this pick up
    #endregion


    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlayerController>().inventory.PickUpItem(gameObject, false);
        }
    }
    //called by inventory manager to destroy item in game world when it is picked up
    public void DestroyPickup() {
        Destroy(this.gameObject);
    }
}
