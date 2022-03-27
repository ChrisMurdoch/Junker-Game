using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAim : MonoBehaviour
{
    public Transform rotationCenter;
    [HideInInspector] public Vector3 mousePos;

    public GameObject Crosshair;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Plane plane = new Plane(new Vector3(0, 0, 1), 0);

        float distance;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (plane.Raycast(ray, out distance))
        {
            mousePos = ray.GetPoint(distance);
        }

        mousePos.z = 0;

        rotationCenter.transform.LookAt(mousePos, Vector3.right);

        Crosshair.transform.position = Input.mousePosition;
    }
}
