using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookLauncher : MonoBehaviour
{
    private Vector3 mousePos;

    public GameObject hook;
    public GameObject hookSpawn;

    public GameObject WorldPos;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawLine(transform.position, mousePos, Color.red);

        Plane plane = new Plane(new Vector3(0, 0, 1), 0);

        float distance;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (plane.Raycast(ray, out distance))
        {
            mousePos = ray.GetPoint(distance);
        }

        mousePos.z = 0;

        Debug.Log(mousePos);

        //if(Physics.Raycast(transform.position, mousePos, out RaycastHit hit))
        //{
        //    Debug.Log(hit.point);
        //}

        //Debug.Log(ray.GetPoint(distance));

        WorldPos.transform.position = mousePos;

        hookSpawn.transform.LookAt(mousePos, Vector3.right);

        //hookSpawn.transform.rotation = Quaternion.LookRotation(mousePos);
        //WorldPos.transform.rotation = Quaternion.LookRotation(mousePos);

        //Debug.Log(Quaternion.LookRotation(mousePos));

        if (Input.GetMouseButtonDown(0))
        {
            GameObject projectile = Instantiate(hook, hookSpawn.transform.position, hookSpawn.transform.rotation);
            //Quaternion.LookRotation(mousePos)
            //projectile.transform.rotation = Quaternion.LookRotation(ray.direction);
            //projectile.GetComponent<Rigidbody>().velocity = transform.forward * 10;
            Destroy(projectile, 10);
        }

        ////hookSpawn.transform.LookAt(mousePos);

    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
    }
}
