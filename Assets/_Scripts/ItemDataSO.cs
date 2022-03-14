using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Item Data", menuName = "Data/Item Data")]
/// <summary>
/// A SO that can be made for all the different item pickups and store their data since copies share a base,
/// this will save on memory and allows us to edit the data without having to edit every single pickup 
/// </summary>
public class ItemDataSO : ScriptableObject
{


    public Vector2Int itemSize; //format (width, height) measured in grid squares
    public Sprite image; //Image used for rending in the inventory
    public int itemMax; //the maximum number of items in a grid stack

    public string itemName; //name of item
    public string desc; //item description

    public float moneyValue; //how much money you get from selling item
    public float scrapValue; //how much scrap you get from scrapping item




}
