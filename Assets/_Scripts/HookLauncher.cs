using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookLauncher : MonoBehaviour
{
    private Vector3 mousePos;
    private Vector3 crossHairPos;

    public GameObject hook;
    public GameObject hookSpawn;
    public Transform rotationCenter;

    [HideInInspector] public GameObject FiredHook;
    [HideInInspector] public HookProjectile hookProjectileScript;
    [HideInInspector] public Vector3 HookHitPosition;

    public float hookSpeed;
    public float returnSpeed;
    public float maxLength;

    float testvariable;

    public float hookPullSpeed;

    public GameObject Crosshair;

    private LineRenderer lr;
    public Transform[] points;

    // Start is called before the first frame update
    void Start()
    {
        lr = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.DrawLine(transform.position, mousePos, Color.red);

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

        if (FiredHook != null)
        {
            lr.enabled = true;
            for (int i = 0; i < points.Length; i++)
            {
                lr.SetPosition(i, points[i].position);
            }
        }
        else
        {
            lr.enabled = false;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            DestroyActiveHook();
        }

    }

    public void DestroyActiveHook()
    {
        Destroy(FiredHook);
    }

    public void FireHook()
    {
        if (FiredHook == null)
        {
            FiredHook = Instantiate(hook, hookSpawn.transform.position, hookSpawn.transform.rotation);
            hookProjectileScript = FiredHook.GetComponent<HookProjectile>();
            hookProjectileScript.launcher = GetComponent<HookLauncher>();
            hookProjectileScript.SetHookParameters(hookSpeed, returnSpeed, maxLength);
            points[1] = FiredHook.transform;

        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
    }
}
