using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{

    [SerializeField]
    private Vector2 gridShape; //format (width, height) measured in grid squares

    
    public string itemName; //name of item
    public string desc; //item description


    public float moneyValue; //how much money you get from selling item
    public float scrapValue; //how much scrap you get from scrapping item
    public Image invImage; //image of the item used for inventory

    private bool stackable; //whether item can stack together in inventory grid
    private Vector2 gridPosition; //pos of top-left corner in inv grid
    private Vector2 gridSquareSize; //hold size of each grid square after calculation
    private Vector2 gridoffsetSize; //size of x and y offsets on grid


    //how much of the grid this item will take up when added to inventory
    public Vector2 GetGridShape () {
        return new Vector2 (gridShape.x, gridShape.y); //x = how many columns, y = how many rows
    }

    //called by inventory manager to destroy item in game world when it is picked up
    public void DestroyPickup() {
        Destroy(this.gameObject);
    }

    //called by inventory manager to instantiate item image at the right grid position
    public void CreateInvObject() {
        
    }

    //called by CreateInvObject() to create translucent backing image (goes under item image) with dimensions based on item's grid size
    private Image CreateInvBackground() {
        
    }

#region Properties

    //whether or not this item can stack on top of other items of the same type (instead of taking up another grid space)
   public bool Stackable {
       get {return stackable; }
       set {stackable = value; }
   }

    //which grid square the item's top-left corner is on
   public Vector2 GridPosition {
       get {return gridPosition; }
       set {gridPosition = value; }
   }

    //how large each grid square is (sizeDelta)
   public Vector2 GridSquareSize {
       get {return gridSquareSize; }
       set {gridSquareSize = value; }
   }

#endregion
}
