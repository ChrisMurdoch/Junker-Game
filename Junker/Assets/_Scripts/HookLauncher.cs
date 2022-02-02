using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookLauncher : MonoBehaviour
{
    private Vector3 mousePos;

    public GameObject hook;
    public GameObject hookSpawn;
    public Transform gunCenter;

    [HideInInspector] public GameObject FiredHook;
    [HideInInspector] public HookProjectile HookScript;
    [HideInInspector] public Vector3 hitPoint;


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

        gunCenter.transform.LookAt(mousePos, Vector3.right);


        if (Input.GetMouseButtonDown(0) && FiredHook == null)
        {
            //GameObject projectile = Instantiate(hook, hookSpawn.transform.position, hookSpawn.transform.rotation);
            FiredHook = Instantiate(hook, hookSpawn.transform.position, hookSpawn.transform.rotation);
            HookScript = FiredHook.GetComponent<HookProjectile>();
            HookScript.launcher = GetComponent<HookLauncher>();
        }

    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
    }
}
