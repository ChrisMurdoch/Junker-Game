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

    [Header("Hook Instantiation Parameters")]
    public GameObject hookObject;
    private Transform hookSpawn;

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
            currentHook = Instantiate(hookObject, hookSpawn.position, hookSpawn.rotation);
            currentHookScript = currentHook.GetComponent<HookProjectile>();
            currentHookScript.SetHookParameters(launchSpeed, returnSpeed, launchLength, GetComponent<HookLauncher>(), gameObject);
        }
    }

    //temporary method to change source of hook launch to the bullet spawn of whatever gun is active
    //called by PlayerInventoryInteraction.ActivateWeapon()
    public void ChangeLaunchSource(Transform sourceTransform)
    {
        hookSpawn = sourceTransform;
    }
}
