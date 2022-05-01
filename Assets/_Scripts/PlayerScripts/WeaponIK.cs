using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HumanBone {
    public HumanBodyBones bone; //reference to this bone
    public float weight; //how much this bone is influenced by the ik aim
}

public class WeaponIK : MonoBehaviour
{
    public Transform aimTransform; //place you are aiming from = spawn point of gun
    public int iterations; //how many times to aim per update. Affects visual accuracy

    [Range(0,1)] public float weight; //how much this script affects the model/animation
    public float angleLimit = 90.0f; //stops character from aiming behind itself(or while turning)
    public float distanceLimit = 1.5f; //stops ik from glitching when the crosshair gets too close to the character

    private PlayerAnimator pa; //reference to animation script
    public HumanBone[] humanBones; //holds references to bones this script affects
    private Transform[] boneTransforms; //holds transforms of bones this script affects

    private Vector3 targetPosition;
    private Vector3 storedTargetPosition; //used to keep aim stationary while turning

    // Start is called before the first frame update
    void Start()
    {
        Animator anim = GetComponent<Animator>();

        //get transforms of referenced bones
        boneTransforms = new Transform[humanBones.Length];
        for(int i = 0; i < boneTransforms.Length; i++) {
            boneTransforms[i] = anim.GetBoneTransform(humanBones[i].bone);
        }

        pa = GetComponent<PlayerAnimator>();
    }


    void LateUpdate()
    {
        storedTargetPosition = targetPosition; //save previous position
        targetPosition = GetTargetPosition(); //get position to aim at (crosshair) and adjust for angle/distance limits

        for(int i = 0; i < iterations; i++) { //calculate target aim many times per frame
            for(int b = 0; b < boneTransforms.Length; b++) {
                AimAtTarget(boneTransforms[b], targetPosition, humanBones[b].weight * weight); //aim each of the bones
            }
        }
    }


    private Vector3 GetTargetPosition() 
    {

        //get direction vector from bullet spawn to crosshair
        Vector3 targetDirection = GetComponent<PlayerAim>().mousePos - aimTransform.position;

        Vector3 aimDirection = aimTransform.forward; 

        //make sure aim stays directly forward
        targetDirection.z = 0;
        aimDirection.z = 0;

        float blendOut = 0.0f; //corrects the angle when it exceeds limits

        //get angle between gun's aim and direction to crosshair
        float targetAngle = Vector3.Angle(targetDirection, aimDirection);
        //if angle is > limit, blend back to an aim within bounds
        if(targetAngle > angleLimit)
        {
            blendOut += (targetAngle - angleLimit) / 50.0f; 
        }

        // float xDistance;
        // if(pa.facingRight) //mousePos should be higher x value
        //     xDistance = GetComponent<PlayerAim>().mousePos.x - aimTransform.position.x;
        // else //aimTransform should be higher x value
        //     xDistance = aimTransform.position.x - GetComponent<PlayerAim>().mousePos.x;

        //Debug.Log("xDistance = " + xDistance);
        float targetDistance = targetDirection.magnitude; //get distance from bullet spawn to crosshair

        //if closer than distance limit, blend back to neutral aim
        if(targetDistance < distanceLimit) {
            blendOut += distanceLimit - targetDistance;
        }

        //get direction to aim at by blending between straight forward and direction of crosshair
        Vector3 direction = Vector3.Slerp(targetDirection, aimDirection, blendOut);
        return aimTransform.position + direction; //return direction to aim in
    }

    private void AimAtTarget(Transform bone, Vector3 position, float w)
    {
        Vector3 aimDirection = aimTransform.forward;

        aimDirection.z = 0.0f; //no z direction, aiming in 2d xy plane

        Vector3 targetDirection = position - aimTransform.position;
        targetDirection.z = 0.0f;

        Quaternion aimTowards = Quaternion.FromToRotation(aimDirection, targetDirection); //get angle between 2 directions
        Quaternion blendRotation = Quaternion.Slerp(Quaternion.identity, aimTowards, w);
        bone.rotation = blendRotation * bone.rotation;
    }
}
