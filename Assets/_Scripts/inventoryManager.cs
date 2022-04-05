using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

//Authored by Christine Murdoch
//Extended by John Murphy

public class InventoryManager : MonoBehaviour
{    

    
    public List<InventoryObject> items; //New list to hold data references to the game objects
    private float moneyCount; //how much money the player has
    private float scrapCount; //how much scrap the player has
    private bool holdingItem; //whether the mouse is holding an item
    private int heldItemIndex; //tracks item mouse is holding (if any)



    public GameObject gridArea; //background of grid (the whole area it takes up)
    public Vector2Int totalGridSpace; //(columns, rows)

    public GameObject hotBarArea;
    public int hotBarSlots = 3;

    public Text scrapCountText;
    public Text moneyCountText;


    public static bool invScreenActive = false; //whether inventory screen is active
    public GameObject invScreenUI; //holds inventory screen panel

    public GameObject[,] gridSquares; //holds each grid square's rect transform after instantiated
    public GameObject[] hotbarSquares; //holds each hotbar square's rect transform after instantiated
    public float squareSizeValue;
    public InventoryObject[] hotBarItems;

    public GameObject player;
    public GameObject hotBar;
    public float canvasScaleFactor;
    float offsetWorldX;
    float offsetWorldY;
    public GameObject imagePrefab;
    [SerializeField] GraphicRaycaster m_Raycaster;
    [SerializeField] EventSystem m_EventSystem;


    void Start() {
        gridSquares = new GameObject[totalGridSpace.x, totalGridSpace.y]; //declare gridSquares array to correct size
        hotbarSquares = new GameObject[hotBarSlots];
        hotBarItems = new InventoryObject[hotBarSlots];
        canvasScaleFactor = gameObject.GetComponent<CanvasScaler>().scaleFactor;
        InstantiateGrid();
        items = new List<InventoryObject>();
        holdingItem = false;
    }


    void Update() {

        //use tab key to open / close inventory
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (invScreenActive)
            {
                if (holdingItem)
                {
                    holdingItem = false; //also need to put item back where it was last
                    RectTransform heldRect = items[heldItemIndex].uiImage.GetComponent<RectTransform>();
                    heldRect.position = GetItemCenter(items[heldItemIndex].GridPosition, items[heldItemIndex].itemSize);
                    heldItemIndex = -1;
                }
                CloseInvScreen();
            }

            else 
            {
                OpenInvScreen();
            }
        }

        if (invScreenActive) {

            // code to allow for clicking to pick up / drop item images on grid
            if(Input.GetMouseButtonDown(0)) {
                Vector3 clickedPosition = Input.mousePosition;
                Debug.Log("Clicked");
                if (!holdingItem) {
                    heldItemIndex = MouseGrabItem(new Vector2(clickedPosition.x, clickedPosition.y));
                }
                else if (holdingItem)
                {
                    MouseDropItem(new Vector2(clickedPosition.x, clickedPosition.y));
                }


            }
            if(holdingItem) {

                Vector3 newItemPosition = Input.mousePosition;
                //items[heldItemIndex].transform.position = newItemPosition; //change to be item image's position
                RectTransform trans = items[heldItemIndex].uiImage.GetComponent<RectTransform>();
                trans.position = newItemPosition;
            }
        }
    }


    //pauses the game and opens the inventory screen
    void OpenInvScreen() {

        invScreenUI.SetActive(true);
        Time.timeScale = 0f; //pauses game
        invScreenActive = true;
    }

    //closes the inventory screen and unpauses the game
    void CloseInvScreen() {

        invScreenUI.SetActive(false);
        Time.timeScale = 1f; //unpauses game
        invScreenActive = false;
    }

    void InstantiateGrid() { //creates grid & squares for inventory

        //get pixel width & height for grid area
        RectTransform gridRectTransform = gridArea.GetComponent<RectTransform>();

        // get width & height of grid squares
        float squareWidth = gridRectTransform.sizeDelta.x/(totalGridSpace.x + 1); //divide width
        float squareHeight = gridRectTransform.sizeDelta.y/(totalGridSpace.y + 1); //divide height
        squareSizeValue = squareWidth;

        // put width & height in world measurement units
        float gridWorldWidth = gridRectTransform.sizeDelta.x * canvasScaleFactor;
        float gridWorldHeight = gridRectTransform.sizeDelta.y * canvasScaleFactor;
        float squareWidthWorld = squareWidth * canvasScaleFactor;
        float squareHeightWorld = squareHeight * canvasScaleFactor;

        //get world pos of grid area's bottomLeft corner
        Vector2 bottomLeft = new Vector2(gridArea.transform.position.x - (gridWorldWidth / 2f), gridArea.transform.localPosition.y - (gridWorldHeight / 2f)); 

        //get x and y offset in world units
        float squareWidthOffset = squareWidth / (totalGridSpace.x + 1); //how much space between squares in x direction
        float squareHeightOffset = squareHeight / (totalGridSpace.y + 1); //how much space between squares in y direction
        offsetWorldX = squareWidthOffset * canvasScaleFactor; //offset in x direction in world space
        offsetWorldY = squareHeightOffset * canvasScaleFactor; //offset in y direction in world space


        //create grid square with desired components
        GameObject gridSquareOrig = new GameObject();
        gridSquareOrig.AddComponent<RectTransform>();
        gridSquareOrig.AddComponent<CanvasRenderer>();
        gridSquareOrig.AddComponent<Image>();


        //set recttransform of first grid square
        gridSquareOrig.GetComponent<RectTransform>().anchorMin = new Vector2(0f, 0f); //anchor min/max stay in the center of grid area
        gridSquareOrig.GetComponent<RectTransform>().anchorMax = gridSquareOrig.GetComponent<RectTransform>().anchorMin;
        gridSquareOrig.GetComponent<RectTransform>().sizeDelta = new Vector2(squareWidth, squareHeight);

        //calculate first grid square's position in world space
        //Vector3 gridSquarePos = new Vector3(gridArea.transform.position.x + offsetWorldX + (squareWidthWorld / 2), gridArea.transform.position.y + (offsetWorldY) + (squareHeightWorld / 2), gridArea.transform.position.z);
        Vector3 gridSquarePos = new Vector3( -(gridRectTransform.sizeDelta.x/2) + offsetWorldX + (squareWidthWorld / 2), -(gridRectTransform.sizeDelta.y/2) + offsetWorldY + (squareHeightWorld / 2));
        Debug.Log("(" + gridSquarePos.x + ", " + gridSquarePos.y + ", " + gridSquarePos.z + ")");

        //holds x position of 1st grid square so it can be reset for new rows
        float resetXPos = -(gridRectTransform.sizeDelta.x / 2) + offsetWorldX + (squareWidthWorld / 2);


        //variables for adding grid squares to array
        int currentX = 0;
        int currentY = 0;

        for (int i = 0; i < totalGridSpace.y; i++) {
            for (int j = 0; j < totalGridSpace.x; j++) {

                GameObject newGridSquare; //create space to hold next created grid square

                //instantiate & set common components
                newGridSquare = Instantiate(gridSquareOrig, gridArea.transform); //instantiate grid square


                newGridSquare.name = "Grid Square";
                newGridSquare.tag = "GridSquare";
                newGridSquare.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.2f);
                newGridSquare.transform.localPosition = gridSquarePos; //move grid square to correct position


                //update x value for next square in row
                gridSquarePos.x += offsetWorldX + squareWidthWorld;

                //add to array & move to next array space in row
                gridSquares[currentX,currentY]= newGridSquare;
                currentX++;

            }

             //reset x value
             gridSquarePos.x = resetXPos;
                
            //update y value for next row
            gridSquarePos.y += squareHeightWorld + offsetWorldY;

            //move to next array row
            currentX = 0;
            currentY++;
        }

        RectTransform hotBarRectTransform = hotBarArea.GetComponent<RectTransform>();

        float hotBarSquareWidth = hotBarRectTransform.sizeDelta.x / (hotBarSlots + 1); //divide width
        float hotBarSquareHeight = hotBarSquareWidth;

        float hotBarWorldWidth = hotBarRectTransform.sizeDelta.x * canvasScaleFactor;
        float hotBarWorldHeight = hotBarRectTransform.sizeDelta.y * canvasScaleFactor;
        float hotBarSquareWidthWorld = hotBarSquareWidth * canvasScaleFactor;
        float hotBarSquareHeightWorld = hotBarSquareHeight * canvasScaleFactor;

        float hotBarSquareWidthOffset = hotBarSquareWidth / hotBarSlots; //how much space between squares in x direction
        float hotBarSquareHeightOffset = hotBarSquareHeight; //how much space between squares in y direction
        offsetWorldX = hotBarSquareWidthOffset * canvasScaleFactor; //offset in x direction in world space
        offsetWorldY = hotBarSquareHeightOffset * canvasScaleFactor; //offset in y direction in world space

        GameObject hotBarSquareOrig = new GameObject();
        hotBarSquareOrig.AddComponent<RectTransform>();
        hotBarSquareOrig.AddComponent<CanvasRenderer>();
        hotBarSquareOrig.AddComponent<Image>();

        hotBarSquareOrig.GetComponent<RectTransform>().anchorMin = new Vector2(1f, 0f); //anchor min/max stay in the center of grid area
        hotBarSquareOrig.GetComponent<RectTransform>().anchorMax = new Vector2(1f, 0f);
        hotBarSquareOrig.GetComponent<RectTransform>().pivot = new Vector2(1f, 0.5f);
        hotBarSquareOrig.GetComponent<RectTransform>().sizeDelta = new Vector2(hotBarSquareWidth, hotBarSquareHeight);

        Vector3 hotBarSquarePos = new Vector3(-(hotBarRectTransform.sizeDelta.x / 2) - offsetWorldX - (hotBarSquareWidthWorld / 2), -(hotBarRectTransform.sizeDelta.y / 2) + offsetWorldY + (hotBarSquareHeightWorld / 2));
        Debug.Log("(" + hotBarSquarePos.x + ", " + hotBarSquarePos.y + ", " + hotBarSquarePos.z + ")");

        for (int i = 0; i < hotBarSlots; i++)
        {
            GameObject newHotBarSquare; //create space to hold next created grid square

            //instantiate & set common components
            newHotBarSquare = Instantiate(hotBarSquareOrig, hotBarArea.transform); //instantiate grid square


            newHotBarSquare.name = "Hot Bar Square";
            newHotBarSquare.tag = "HotBarSquare";
            newHotBarSquare.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.2f);
            newHotBarSquare.transform.localPosition = hotBarSquarePos; //move grid square to correct position

            hotbarSquares[i] = newHotBarSquare;

            hotBarSquarePos.x += offsetWorldX + hotBarSquareWidthWorld;
        }
    }

    public void ReloadActiveWeapon(Weapon activeWeapon)
    {
        int amountUsed;
        foreach(InventoryObject io in items)
            if(io.itemName == activeWeapon.ammoType)
            {
                amountUsed = activeWeapon.magazineCapacity - activeWeapon.ammoCount;
                if(io.amount > amountUsed)
                {
                    io.amount -= amountUsed;
                    activeWeapon.ammoCount += amountUsed;
                    amountUsed = 0;
                }
                else
                {
                    amountUsed -= io.amount;
                    activeWeapon.ammoCount += io.amount;
                    items.Remove(io);
                }
                if (amountUsed == 0)
                    break;
            }
    }
    public void ReloadActiveWeapon(WeaponBase activeWeapon)
    {
        int amountUsed;
        List<InventoryObject> removeList = new List<InventoryObject>();
        foreach(InventoryObject io in items)
            if(io.itemName == activeWeapon.ammoType)
            {
                amountUsed = (int)(activeWeapon.maxAmmo - activeWeapon.currentAmmo);
                if(io.amount > amountUsed)
                {
                    io.amount -= (int)amountUsed;
                    activeWeapon.currentAmmo += amountUsed;
                    amountUsed = 0;
                }
                else
                {
                    amountUsed -= io.amount;
                    activeWeapon.currentAmmo += io.amount;
                    removeList.Add(io);
                }
                if (amountUsed == 0)
                    break;
            }
        foreach(InventoryObject io in removeList)
        {
            GameObject.Destroy(io.uiImage);
            items.Remove(io);
        }
    }

    //called by PlayerController's OnTrigger
    //Playercontroller passes item script and whether its a weapon
    public void PickUpItem(GameObject itemObject, bool isWeapon) {
        
        Item item = itemObject.GetComponent<Item>();
        InventoryObject item2 = new InventoryObject(itemObject, item);

        Vector2Int itemSize = item2.itemSize;
        Debug.Log("item name = " + item2.itemName);
        int itemIndex = CheckForItem(item2.itemName);
        if(item2.Stackable) {
            if(itemIndex >= 0)
                items[itemIndex].amount++;
        }

        Vector2Int currentPos = new Vector2Int(0,0);
        Vector2Int availSpace = new Vector2Int(0,0);
        bool placed = false;
        
        for(int i = 0; i <= totalGridSpace.y; i++) {
            for(int j = 0; j <= totalGridSpace.x; j++) {
                currentPos = new Vector2Int(j, i);
                bool avaiableSpace = CheckGridSpaceAvailable(currentPos, itemSize);

                if(avaiableSpace)
                {
                    items.Add(item2);
                    item2.GridPosition = currentPos;
                    item2.amount = item.ammount;

                    Debug.Log("Item added at ( " + currentPos.x + " , " + currentPos.y + " )");

                    placed = true;
                    break;
                }
            }
            if (placed)
                break;
        }

        if(!placed) {
            Debug.Log("No Room!");
            return;
        }

        // else, open inventory menu and show in separate box
        //allow player to manually move / scrap items, or drop pickup
        item2.uiImage = Instantiate(imagePrefab, invScreenUI.transform);
        Image newImage = item2.uiImage.GetComponent<Image>();
        newImage.sprite = item2.image;
        newImage.rectTransform.sizeDelta = new Vector2(squareSizeValue * item2.itemSize.x, squareSizeValue * item2.itemSize.y);
        //place the image in it's spot on the grid
        newImage.rectTransform.position = GetItemCenter(currentPos, itemSize);
        newImage.tag = "InvItem";
        item.DestroyPickup();
    }

    public Vector3 GetItemCenter(Vector2Int gridPos, Vector2Int itemSize)
    {
        Vector3 trans = new Vector3();
        RectTransform[] transforms = new RectTransform[itemSize.x * itemSize.y];
        int count = 0;
        Debug.Log("gridPos(" + gridPos.x + "," + gridPos.y + ")");
        for(int i = gridPos.y; i < itemSize.y + gridPos.y; i++ )
        {
            for(int j = gridPos.x; j < itemSize.x + gridPos.x; j++)
            {               
                transforms[count] = gridSquares[j, i].GetComponent<RectTransform>();
                count++;
            }
        }
        for(int i = 0; i < count; i++)
        {
            trans += transforms[i].position;
        }
        trans = new Vector3( trans.x / count, trans.y / count, 0f);
        return trans;
    }

    // makes item image & background follow mouse until player clicks again
    public int MouseGrabItem(Vector2 clickPosition) {
        //Set up the new Pointer Event
        PointerEventData m_PointerEventData = new PointerEventData(m_EventSystem);
        //Set the Pointer Event Position to that of the game object
        m_PointerEventData.position = Input.mousePosition;

        //Create a list of Raycast Results
        List<RaycastResult> results = new List<RaycastResult>();

        m_Raycaster.Raycast(m_PointerEventData, results);


        int objectIndex = -1;
        if (results.Count == 0)
            return objectIndex;
        Debug.Log(results.Count);
        foreach(RaycastResult result in results)
        {
            Debug.Log(result.gameObject.name);
            if(result.gameObject.tag == "InvItem")
            {
                holdingItem = true;
                objectIndex = CheckForUIItem(result.gameObject);
                break;
            }
            else if(result.gameObject.tag == "HotBarSquare")
            {
                for (int i = 0; i < hotBarSlots; i++)
                {
                    if (hotbarSquares[i] == result.gameObject)
                    {
                        hotBar.GetComponent<HotBarController>().RemoveItemInSlot(i);
                        break;
                    }
                }
            }
        }
        return objectIndex;
    }

    //check if UI element has an associated item
    //return its index
    int CheckForUIItem(GameObject uiImage)
    {

        if (uiImage != null)
        {
            Debug.Log("passed item name");
        }
        if (items.Count <= 0)
        {
            Debug.Log("Empty item list");
            return -1;
        }
        for (int i = 0; i < items.Count; i++)
        {

            if (uiImage == items[i].uiImage)
                return i;
        }

        return -1;
    }

    //called when player clicks mouse while "holding" an item
    //places item in grid space player clicks on, or back in original position if it doesn't fit
    public void MouseDropItem(Vector2 clickPosition) {

        PointerEventData m_PointerEventData = new PointerEventData(m_EventSystem);
        //Set the Pointer Event Position to that of the game object
        m_PointerEventData.position = new Vector2(
            Input.mousePosition.x,
            Input.mousePosition.y);

        //Create a list of Raycast Results
        List<RaycastResult> results = new List<RaycastResult>();

        m_Raycaster.Raycast(m_PointerEventData, results);


        foreach(RaycastResult result in results)
        {
            Debug.Log(result.gameObject.name);
            if (result.gameObject.tag == "GridSquare")
            {
                for(int y = 0; y < gridSquares.GetLength(0); y++)
                {
                    for (int x = 0; x < gridSquares.GetLength(1); x++)
                    {
                        if (result.gameObject == gridSquares[x,y])
                        {
                            Debug.Log("grid square found");
                            if (CheckGridSpaceAvailable(new Vector2Int(x,y), items[heldItemIndex].itemSize))
                            {
                                items[heldItemIndex].GridPosition = new Vector2Int(x,y);
                                items[heldItemIndex].uiImage.GetComponent<RectTransform>().position =
                                    GetItemCenter(new Vector2Int(x, y), items[heldItemIndex].itemSize);
                                heldItemIndex = -1;
                                holdingItem = false;
                                
                            }
                        }
                    }
                }

            }
            else if(result.gameObject.tag == "HotBarSquare")
            {
                for(int i = 0; i < hotbarSquares.Length; i++)
                {
                    if(result.gameObject == hotbarSquares[i])
                    {
                        hotBar.GetComponent<HotBarController>().PlcaeItemInSlot(items[heldItemIndex], i+1);

                        GameObject newImage = Instantiate(imagePrefab, hotbarSquares[i].transform);
                        newImage.GetComponent<Image>().sprite = items[heldItemIndex].image;
                        newImage.GetComponent<RectTransform>().sizeDelta = hotbarSquares[i].GetComponent<RectTransform>().sizeDelta;
                        hotBarItems[i] = items[heldItemIndex];

                        holdingItem = false;
                        RectTransform heldRect = items[heldItemIndex].uiImage.GetComponent<RectTransform>();
                        heldRect.position = GetItemCenter(items[heldItemIndex].GridPosition, items[heldItemIndex].itemSize);
                        heldItemIndex = -1;
                    }
                }
            }
        }

    }

    //check if item is already in inventory list
    //return its index
    int CheckForItem(string itemName) {

        if(itemName != null) {
            Debug.Log("passed item name");
        }
        if(items.Count <= 0) {
            Debug.Log("Empty item list");
            return -1;
        }
        for(int i = 0; i < items.Count; i++) {

            if (itemName == items[i].itemName)
                return i;
        }

        return -1;
    }


    //Check if grid space is avaialable based on the size of the item passed and the position given
    bool CheckGridSpaceAvailable (Vector2Int bottomLeft, Vector2Int itemSize) {


        for(int y = bottomLeft.y; y < bottomLeft.y + itemSize.y; y++)
        {
            if (y > totalGridSpace.y-1)
                return false;
            for(int x = bottomLeft.x; x < bottomLeft.x + itemSize.x; x++)
            {
                if (x > totalGridSpace.x-1)
                    return false;
                if (CheckPositionEmpty(new Vector2Int(x, y)) == false)
                    return false;
            }
        }

        return true;
    }

    // check if given grid space is empty
    bool CheckPositionEmpty (Vector2Int position) {

        for(int i = 0; i < items.Count; i++) {

            if (holdingItem && i == heldItemIndex)
                continue;
            for(int y = items[i].GridPosition.y; y < items[i].GridPosition.y + items[i].itemSize.y; y++)
            {
                for(int x = items[i].GridPosition.x; x < items[i].GridPosition.x + items[i].itemSize.x; x++)
                {
                    if (position == new Vector2Int(x, y))
                    {
                        return false;
                    }
                }
            }
        }

        return true; //grid position is empty
    }

}



/// <summary>
/// A class unique to the inventory manager meant to simplify the holding of listed objects
/// </summary>
public class InventoryObject
{

    public string itemName; // the name of the item
    public GameObject prefab;//The Pickup Prefab it references for use when dropping the item
    public Sprite image;//The image UI element associated with it in the inventory manager
    public int amount = 0;//Total number of the item
    public int amountMax = 1; //Maximum total of item stack

    public GameObject uiImage; // the ui element associated with this item
    public Vector2Int itemSize; //size of the object in grid squares
    private Vector2Int gridPosition; //pos of bottom-left corner in inv grid

    public void RemoveObject()
    {
        GameObject.Destroy(uiImage);
    }

    public InventoryObject(GameObject pickup, Item item)
    {
        //prefab = new GameObject(pickup.name, pickup.GetComponents<Type>());
        //copy all of the components of the pickup onto this new gameobject;
        image = item.data.image;
        itemSize = item.data.itemSize;
        amountMax = item.data.itemMax;
        itemName = item.data.itemName;

    }

    public InventoryObject(Sprite image, Vector2Int itemSize, int amountMax, int amount, string itemName)
    {
        //prefab = new GameObject(pickup.name, pickup.GetComponents<Type>());
        //copy all of the components of the pickup onto this new gameobject;
        this.image = image;
        this.itemSize = itemSize;
        this.amountMax = amountMax;
        this.amount = amount;
        this.itemName = itemName;

    }
    #region Properties

    //whether or not this item can stack on top of other items of the same type (instead of taking up another grid space)
    public bool Stackable {
       get {if(amountMax > 1) return true; return false; }
   }

    //which grid square the item's top-left corner is on
   public Vector2Int GridPosition {
       get {return gridPosition; }
       set {gridPosition = value; }
   }
#endregion
}
