using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//Authored by John Murphy
public class HudController : MonoBehaviour
{

    //This will be the hud controller script that edits and changes
    //different facets of the players hud, including but not limited to:
    //Player Health
    //Player weapon
    //Anything else we come up with
    //The point being that any and all Hud updates are pushed through this script first
    //that way we have full control over the state of the screen within one single script


    //Player Health functionality
    //Josh pls serialize these variables this script is gonna be rather large and I'd like to section off the aspects
    //of the component into dropdowns to compress things
    int playerHealth;
    int playerMaxHealth;
    float healthHeight = 50f;
    float healthMaxLng = 500f;
    float healthCurrentLength = 500f;
    float healthTargetLength;
    Slider healthBarSlider;
    Image healthBarImage;
    RectTransform healthBarTrans;
    CanvasRenderer healthBarRender;
    GameObject healthHud;
    GameObject healthHudElement;

    private void HealthInitialRender()//Initial Render for player health element
    {
        //Creates a new game object and adds the necessary ui components
        healthHud = new GameObject();
        healthBarTrans = healthHud.AddComponent<RectTransform>();
        healthBarRender = healthHud.AddComponent<CanvasRenderer>();
        healthBarImage = healthHud.AddComponent<Image>();

        //Edits parts of the componets for set up
        healthBarTrans.anchorMax = new Vector2(0.1f, 0.9f);
        healthBarTrans.anchorMin = new Vector2(0.1f, 0.9f);
        healthBarTrans.pivot = new Vector2(0f, 1f);
        healthBarTrans.sizeDelta = new Vector2(500f, 50f);
        healthBarImage.color = new Color(1f, 0f, 0f);
        //Instantiates object
        healthHud = Instantiate(healthHud, transform);
        healthHud.name = "Health Hud";
        //Instantiates the second part of the hud and edits the components of the new object
        healthHudElement = Instantiate(healthHud, healthHud.transform);
        healthHudElement.name = "Health Hud Element";
        healthHudElement.GetComponent<Image>().color = new Color(0f, 1f, 0f);
        healthHudElement.GetComponent<RectTransform>().anchorMax = new Vector2(0f, 1f);
        healthHudElement.GetComponent<RectTransform>().anchorMin = new Vector2(0f, 1f);
        healthHudElement.GetComponent<RectTransform>().sizeDelta = new Vector2(500f, 50f);
    }

    //when target width is not met this will update it so that taking damage has the health bar move
    private void UpdateHealth()
    {
        
        healthHudElement.GetComponent<RectTransform>().sizeDelta = new Vector2(healthCurrentLength,healthHeight);

    }

    private void Start()
    {
        HealthInitialRender();
    }

    private void Update()
    {
        if (healthCurrentLength != healthTargetLength)
            UpdateHealth();
    }
}
