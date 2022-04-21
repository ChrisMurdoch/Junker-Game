using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAimNew : MonoBehaviour
{
    [HideInInspector] public Vector3 mousePos;

    public GameObject Crosshair; //crosshair image

    public static PlayerAimNew instance; 

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        Plane plane = new Plane(new Vector3(0, 0, 1), 0); //plane centered on origin with normals facing toward background
 
        float distance;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); //make ray from camera going through the mouse's current position

        if (plane.Raycast(ray, out distance)) //get distance along created ray where the plane is hit
        {
            mousePos = ray.GetPoint(distance); //set mousePos V3 at the point where the ray intersects the plane
        }

        mousePos.z = 0;

        //rotationCenter.transform.LookAt(mousePos, Vector3.right);

        Crosshair.transform.position = Input.mousePosition;
    }
}
