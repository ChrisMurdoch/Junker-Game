using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{

    private Vector2 gridShape; //format (width, height) measured in grid squares

    
    private string itemName; //name of item
    private string desc; //item description


    public float moneyValue; //how much money you get from selling item
    public float scrapValue; //how much scrap you get from scrapping item

    private bool stackable; //whether item can stack together in inventory grid

    private Vector2 gridPosition; //pos of top-left corner in inv grid



    public Vector2 GetGridShape () {
        return new Vector2 (gridShape.x, gridShape.y);
    }

#region Properties

   public string ItemName {
       get {return itemName; }
       set {itemName = value; }
   }

   public string Desc {
       get {return desc; }
       set {desc = value; }
   }

   public bool Stackable {
       get {return stackable; }
       set {stackable = value; }
   }



#endregion
}
