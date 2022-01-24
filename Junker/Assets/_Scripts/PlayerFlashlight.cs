using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFlashlight : MonoBehaviour
{
    [SerializeField] private Transform flashLightLocation;
    [SerializeField] private Light flashLight;

    private Vector3 mousePos;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawLine(flashLightLocation.position, mousePos, Color.red);

        flashLight.transform.LookAt(mousePos);

        Plane plane = new Plane(new Vector3(0, 0, 1), 0);

        float distance;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (plane.Raycast(ray, out distance))
        {
            mousePos = ray.GetPoint(distance);
        }

        mousePos.z = 0;
    }
}
