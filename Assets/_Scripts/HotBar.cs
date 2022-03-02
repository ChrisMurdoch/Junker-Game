using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Authored by John Murphy
public class HotBar : MonoBehaviour
{

    //The hot bar will be configurable from the inventory screen allowing the player to set up their load ouot
    //It will have UI references to update the hud 
    //there is just too much that requires the inventory system to function for now I'm making UI

    GameObject[] itemHotbar;
    string item1Path = "Assets/Prefabs/Gun.prefab";



    // Start is called before the first frame update
    void Start()
    {
        itemHotbar = new GameObject[5];
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
