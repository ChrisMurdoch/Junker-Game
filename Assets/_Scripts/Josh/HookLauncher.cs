using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookLauncher : MonoBehaviour
{
    private PlayerController playercontroller;

    [HideInInspector] public GameObject currentHook;
    [HideInInspector] public HookProjectile currentHookScript;

    [HideInInspector] public Vector3 hookHitPosition;

    [HideInInspector] public bool DidHitWall = false;

    [Header("GameObject Parameters")]
    public GameObject hookObject;
    public GameObject hookSpawn;

    [Header("Hook Launcher Parameters")]
    public float launchSpeed;
    public float returnSpeed;
    public float launchLength;

    // Start is called before the first frame update
    void Start()
    {
        playercontroller = GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
           LaunchHook();
        }

        if (Input.GetKeyDown(KeyCode.M) && currentHook != null)
        {
           Destroy(currentHook);
           DidHitWall = false;
        }

        if (DidHitWall)
        {
            playercontroller.currentState = PlayerController.State.Hooking;
            DidHitWall = false;
        }
    }

    public void DestroyActiveHook()
    {
        if (currentHook != null)
        {
            Destroy(currentHook);
            DidHitWall = false;
        }
    }

    public void LaunchHook()
    {
        if(currentHook == null)
        {
            currentHook = Instantiate(hookObject, hookSpawn.transform.position, hookSpawn.transform.rotation);
            currentHookScript = currentHook.GetComponent<HookProjectile>();
            currentHookScript.SetHookParameters(launchSpeed, returnSpeed, launchLength, GetComponent<HookLauncher>(), gameObject);
        }
    }
}
