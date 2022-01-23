using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public GameObject player;
    public float FollowSpeed = 2f;
    public float negativeZAxis = 15f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Vector3 newPosition = target.position;
        newPosition.z = negativeZAxis;
        newPosition.y = target.position.y + 2.5f;

        if (Input.GetKey(KeyCode.S))
        {
            newPosition.y = target.position.y - 3;
        }

        if (Input.GetKey(KeyCode.W))
        {
            newPosition.y = target.position.y + 5;
        }



        transform.position = Vector3.Lerp(transform.position, newPosition, FollowSpeed * Time.deltaTime);
    }
}
