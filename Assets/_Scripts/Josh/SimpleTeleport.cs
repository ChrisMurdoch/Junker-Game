using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleTeleport : MonoBehaviour
{
    public Transform ExitPos;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            GameObject p = other.gameObject;

            p.GetComponent<CharacterController>().enabled = false;
            p.transform.position = ExitPos.position;
            p.GetComponent<CharacterController>().enabled = true;
     
        }
    }
}
