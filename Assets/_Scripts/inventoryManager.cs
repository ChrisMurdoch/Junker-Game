using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class inventoryManager : MonoBehaviour
{

    private List<Item> items;
    private float moneyCount;
    private float scrapCount;



    public GameObject gridArea; //background of grid (the whole area it takes up)
    public GameObject gridSquarePrefab; //prefab of 1 grid square
    public Vector2 totalGridSpace; //(columns, rows)

    public Text scrapCountText;
    public Text moneyCountText;


    public static bool invScreenActive = false; //whether inventory screen is active
    public GameObject invScreenUI; //holds inventory screen panel

    private GameObject[,] gridSquares; //holds each grid square's rect transform after instantiated

    void Start() {
        gridSquares = new GameObject[(int)totalGridSpace.x, (int)totalGridSpace.y]; //declare gridSquares array to correct size 
        InstantiateGrid();
    }


    void Update() {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (invScreenActive)
            {
                CloseInvScreen();
            }

            else 
            {
                OpenInvScreen();
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
        //float gridWidthTotal = gridRectTransform.sizeDelta.x;
        //Debug.Log("gridWidthTotal = " + gridWidthTotal);

        //float gridHeightTotal = gridRectTransform.sizeDelta.y;

        //calculate width / height of each grid square based on how many squares you want
        //float gridSquareWidth = gridWidthTotal / totalGridSpace.x;
        //float gridSquareHeight = gridHeightTotal / totalGridSpace.y;

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


}
