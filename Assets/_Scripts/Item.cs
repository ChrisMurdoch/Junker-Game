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



    public Vector2 GetGridShape () {
        return new Vector2 (gridShape.x, gridShape.y);
    }

    public void DestroyPickup() {
        Destroy(this.gameObject);
    }

    public void CreateInvObject() {
        
    }

    private Image CreateInvBackground() {
        
    }

#region Properties

   public bool Stackable {
       get {return stackable; }
       set {stackable = value; }
   }

   public Vector2 GridPosition {
       get {return gridPosition; }
       set {gridPosition = value; }
   }

   public Vector2 GridSquareSize {
       get {return gridSquareSize; }
       set {gridSquareSize = value; }
   }

#endregion
}
