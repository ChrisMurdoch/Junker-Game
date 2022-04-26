using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAim : MonoBehaviour
{

    public enum State
    {
        Normal = 0, Paused = 1,
    }

    [HideInInspector] public Vector3 mousePos; //mouse's calculated world position

    public GameObject Crosshair; //crosshair image
    public State currentState;

    void Start() 
    {
        currentState = State.Normal; //start unpaused
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            default:
            case State.Normal:

                Plane plane = new Plane(new Vector3(0, 0, 1), 0); //create z plane facing away from camera

                float distance;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); //get a ray from the mouse position

                if (plane.Raycast(ray, out distance)) //raycast mouse position to plane
                {
                    mousePos = ray.GetPoint(distance); //get point where ray from mouse touches plane
                }

                mousePos.z = 0; //make sure z position stays at 0


                if (Crosshair != null)
                {
                    Crosshair.transform.position = Input.mousePosition; //set crosshair to correct position
                }
                break;

            case State.Paused:
                break;
        }
    }
}
