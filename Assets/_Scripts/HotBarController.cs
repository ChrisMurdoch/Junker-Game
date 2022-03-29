using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Authored by John Murphy

public class HotBarController : MonoBehaviour
{
    InventoryManager im;
    InventoryObject[] hotbar = new InventoryObject[4];
    int currentItem = 0;

    public GameObject player;
    public Image currentItemImage;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
            ChangeActiveSlot(0);
        else if (Input.GetKeyDown(KeyCode.Q))
            ChangeActiveSlot(1);
    }
    
    public void PlcaeItemInSlot(InventoryObject io, int slot)
    {
        hotbar[slot] = io;
        if(slot == currentItem)
        {
            ChangeEquippedItem();
        }
    }
    
    public void RemoveItemInSlot(int slot)
    {
        hotbar[slot] = null;
        if (slot == currentItem)
        {
            ChangeEquippedItem();
        }
    }

    public void ChangeActiveSlot(int dir)
    {
        if(dir == 0)//left
        {
            currentItem--;
            if (currentItem < 0)
                currentItem = hotbar.Length-1;
        }
        else if (dir == 1)//ight
        {
            currentItem++;
            if (currentItem > 3)
                currentItem = 0;
        }
        ChangeEquippedItem();
    }

    private void ChangeEquippedItem()
    {
        ItemHandler.PerformItemAction(hotbar[currentItem].itemName, player);
        currentItemImage.sprite = hotbar[currentItem].image;
    }
}