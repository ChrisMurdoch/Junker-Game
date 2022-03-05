using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class inventoryManager : MonoBehaviour
{

    private List<GameObject> items;
    private List<int> itemAmts;
    private float moneyCount;
    private float scrapCount;
    private bool holdingItem;
    private int heldItemIndex;



    public GameObject gridArea; //background of grid (the whole area it takes up)
    public GameObject gridSquarePrefab; //prefab of 1 grid square
    public Vector2 totalGridSpace; //(columns, rows)

    public Text scrapCountText;
    public Text moneyCountText;


    public static bool invScreenActive = false; //whether inventory screen is active
    public GameObject invScreenUI; //holds inventory screen panel

    private GameObject[,] gridSquares; //holds each grid square's rect transform after instantiated

    public GameObject Player;

    void Start() {
        gridSquares = new GameObject[(int)totalGridSpace.x, (int)totalGridSpace.y]; //declare gridSquares array to correct size 
        InstantiateGrid();
        items = new List<GameObject>();
        itemAmts = new List<int>();
        holdingItem = false;
    }


    void Update() {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (invScreenActive)
            {
                holdingItem = false; //also need to put item back where it was last
                CloseInvScreen();
            }

            else 
            {
                OpenInvScreen();
            }
        }

        if (invScreenActive) {

            if(Input.GetMouseButtonDown(0)) {
                Vector3 clickedPosition = Input.mousePosition;

                if(!holdingItem) {
                    heldItemIndex = MouseGrabItem(new Vector2(clickedPosition.x, clickedPosition.y));
                }

                if(holdingItem) {

                }

            }
            if(holdingItem) {

                Vector3 newItemPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                newItemPosition.z = items[heldItemIndex].transform.position.z;
                items[heldItemIndex].transform.position = newItemPosition; //change to be item image's position

            }
        }
    }


    void OpenInvScreen() {

        invScreenUI.SetActive(true);
        Time.timeScale = 0f; //pauses game
        invScreenActive = true;
    }

    void CloseInvScreen() {

        invScreenUI.SetActive(false);
        Time.timeScale = 1f; //unpauses game
        invScreenActive = false;
    }

    void InstantiateGrid() { //creates grid & squares for inventory

        //get pixel width & height for grid area
        RectTransform gridRectTransform = gridArea.GetComponent<RectTransform>();
  

        // get width & height of grid squares as % so we can draw using anchor min / max
        float squareWidthPercent = 1/totalGridSpace.x;
        float squareHeightPercent = 1/totalGridSpace.y;

        //create grid square with desired components
        GameObject gridSquareOrig = new GameObject();
        gridSquareOrig.AddComponent<RectTransform>();
        gridSquareOrig.AddComponent<CanvasRenderer>();
        gridSquareOrig.AddComponent<Image>();

        GameObject newGridSquare;
        Vector2 squareAnchorMin = new Vector2(0, 0);
        Vector2 squareAnchorMax = new Vector2(squareWidthPercent, squareHeightPercent);

        //variables for adding grid squares to array
        int currentX = 0;
        int currentY = 0;

        for (int i = 0; i < totalGridSpace.y; i++) {
            for (int j = 0; j < totalGridSpace.x; j++) {

                //instantiate & set common components
                newGridSquare = Instantiate(gridSquareOrig, gridArea.transform); //instantiate with grid area's transform
                newGridSquare.name = "Grid Square";
                newGridSquare.GetComponent<Image>().color = new Color(1f, 1f, 1f, .5f);

                //draw square in correct position using anchor min & max
                newGridSquare. GetComponent<RectTransform>().anchorMin = squareAnchorMin;
                newGridSquare.GetComponent<RectTransform>().anchorMax = squareAnchorMax;
                newGridSquare.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0);

                //update x values for next square in row
                squareAnchorMin.x = squareAnchorMax.x; //next square immediately next to this one (no offset)
                squareAnchorMax.x += squareWidthPercent;

                //add to array & move to next array space in row
                gridSquares[currentX,currentY]= newGridSquare;
                currentX++;

            }

             //reset x values
             squareAnchorMin.x = 0;
             squareAnchorMax.x = squareWidthPercent;
                
            //update y values for next row
            squareAnchorMin.y = squareAnchorMax.y; //next row immediately below prev (no offset)
            squareAnchorMax.y += squareHeightPercent;

            //move to next array row
            currentX = 0;
            currentY++;
        }

    }


    //called by PlayerController's OnTrigger
    //Playercontroller passes item script and whether its a weapon
    public void PickUpItem(GameObject itemObject, bool isWeapon) {
        
        Item item = itemObject.gameObject.GetComponent<Item>();
        Debug.Log(itemObject.gameObject.GetComponent<Item>());

        Vector2 itemSize = item.GetGridShape();
        Debug.Log("item name = " + item.itemName);
        int itemIndex = CheckForItem(item.itemName);
        if(item.Stackable) {
            if(itemIndex >= 0)
                itemAmts[itemIndex]++;
        }

        Vector2 currentPos;
        Vector2 availSpace;
        bool placed = false;
        
        for(int i = 1; i <= totalGridSpace.y; i++) {
            for(int j = 1; j <= totalGridSpace.x; j++) {
                currentPos = new Vector2(j, i);
                availSpace = CheckGridSpace(currentPos);
                if (availSpace.x >= itemSize.x && availSpace.y >= itemSize.y){
                    items.Add(itemObject);
                    itemAmts.Add(1);
                    item.GridPosition = currentPos;

                    Debug.Log("Item added at ( " + currentPos.x + " , " + currentPos.y + " )");

                    //check if weapon & add to weapon wheel
                    placed = true;
                    break;
                }
            }

            if(placed) 
                break;
        }

        if(!placed) {
            Debug.Log("No Room!");
        }
        // else, open inventory menu and show in separate box
        //allow player to manually move / scrap items, or drop pickup
        item.DestroyPickup();
    }

    public int MouseGrabItem(Vector2 clickPosition) {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(clickPosition),Vector2.zero);
        int objectIndex = -1;
        if (hit) {
            if (hit.transform.gameObject.tag == "InvItem")
            {
                holdingItem = true;
                objectIndex = CheckForItem(hit.transform.gameObject.name);
            }
        }

        return objectIndex;
    }

    public void MouseDropItem(int index, Vector2 clickPosition) {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(clickPosition), Vector2.zero);

        if(hit) {
            if(hit.transform.gameObject.name == "Grid Square") {
                //get position of topLeft item square (if it wasn't grabbed)
                //get grid square under top left item square
                //use CheckGridSpace on grid Square
                //if there's enough space, set item's gridPosition to grid square position
                //else, leave item's gridPosition as is
                //draw item's image in correct place
                heldItemIndex = -1;
                holdingItem = false;
            }
        }
    }

    //check if item is already in inventory list
    int CheckForItem(string itemName) {

        if(itemName != null) {
            Debug.Log("passed item name");
        }
        if(items.Count <= 0) {
            Debug.Log("Empty item list");
            return -1;
        }
        for(int i = 0; i < items.Count; i++) {

            if (itemName == items[i].GetComponent<Item>().itemName)
                return i;
        }

        return -1;
    }

    Vector2 CheckGridSpace (Vector2 topLeft) {

        Vector2 openGridSpace = new Vector2(0,0);
        Vector2 spaceToCheck = topLeft;
        
        //check if first position is empty
        if(!CheckPositionEmpty(topLeft))
            return openGridSpace;
        else {
            openGridSpace.x++; //at least 1,1 space if empty
            openGridSpace.y++;
        }

        spaceToCheck.x++; //move a space to the right

        //move right until you find a full space or reach right end of grid
        while(CheckPositionEmpty(spaceToCheck) && spaceToCheck.x <= totalGridSpace.x ) {
            openGridSpace.x++; //if next position is empty, increment x
            spaceToCheck.x++; //move right
        }

        spaceToCheck = topLeft; //reset back to top-left position
        spaceToCheck.y++; //move one space down

        //move down until you find a full space of reach bottom of grid
        while(CheckPositionEmpty(spaceToCheck) && spaceToCheck.y <= totalGridSpace.y) {
            openGridSpace.y++;
            spaceToCheck.y++;
        }

        return openGridSpace; //returns Vector2 (# of columns, # of rows)
    }

    bool CheckPositionEmpty (Vector2 position) {

        for(int i = 0; i < items.Count; i++) {
            if(position == items[i].GetComponent<Item>().GridPosition)
                return false; //grid position isn't empty
        }

        return true; //grid position is empty
    }

}
