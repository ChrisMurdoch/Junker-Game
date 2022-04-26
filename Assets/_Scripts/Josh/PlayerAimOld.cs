using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAimOld : MonoBehaviour
{
    public Transform rotationCenter;
    [HideInInspector] public Vector3 mousePos;

    public GameObject Crosshair;

    public enum State
    {
        Normal = 0, Paused = 1,
    }

    public State currentState;

    // Start is called before the first frame update
    void Start()
    {
        currentState = State.Normal;
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            default:
            case State.Normal:

                Plane plane = new Plane(new Vector3(0, 0, 1), 0);

                float distance;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (plane.Raycast(ray, out distance))
                {
                    mousePos = ray.GetPoint(distance);
                }

                mousePos.z = 0;

                rotationCenter.transform.LookAt(mousePos, Vector3.right);

                if (Crosshair != null)
                {
                    Crosshair.transform.position = Input.mousePosition;
                }
                break;
            case State.Paused:
                break;
        }


        //Plane plane = new Plane(new Vector3(0, 0, 1), 0);

        //float distance;
        //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        //if (plane.Raycast(ray, out distance))
        //{
        //    mousePos = ray.GetPoint(distance);
        //}

        //mousePos.z = 0;

        //rotationCenter.transform.LookAt(mousePos, Vector3.right);

        //if(Crosshair != null)
        //{
        //    Crosshair.transform.position = Input.mousePosition;
        //}
    }
}
