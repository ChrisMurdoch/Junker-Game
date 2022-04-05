using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Authored by John Murphy

public class HotBarController : MonoBehaviour
{
    InventoryManager im;
    InventoryObject[] hotbar = new InventoryObject[4]; // item 0 is printer pistol
    int currentItem = 0;

    public GameObject player;
    public Image currentItemImage;

    private void Start()
    {
        //hotbar[0] = new InventoryObject()
        //currentItemImage = hotbar[0].image;
    }

    private void Update()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0) // forward
        {
            ChangeActiveSlot(1);
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0) // backward
        {
            ChangeActiveSlot(0);
        }
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
        if (hotbar[currentItem] == null)
            return;
        ChangeEquippedItem();
    }

    private void ChangeEquippedItem()
    {
        ItemHandler.PerformItemAction(hotbar[currentItem].itemName, player);
        currentItemImage.sprite = hotbar[currentItem].image;
    }
}