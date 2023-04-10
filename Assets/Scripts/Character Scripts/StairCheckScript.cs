using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class StairCheckScript : MonoBehaviour
{
    [SerializeField] Rigidbody rb;
    [SerializeField] GeneralCharacter character;
    #region Stair
    Ray ray;
    float castDistance = 0.35f;
    float stairMaxHeight = 0.6f;
    float castCurrentHeight;
    float iterateIncrease = 0.05f;
    [SerializeField]float stairJumpForce;
    LayerMask lMask = ~128;

    RaycastHit hitInfo;
    bool isHit;

    Ray ray_2;
    Ray ray_3;
    RaycastHit hitInfo_2;
    RaycastHit hitInfo_3;
    bool isHit2;
    bool isHit3;
    float diagonalRayAngle = 40;

    bool normalsNotSloped;
    #endregion

    #region Slope
    Ray ray2;
    float castDistance2 = 0.2f;
    float maxSlopeAngle = 55;
    float slopeForceMultiplier = 0.99f;
    public bool onSlopeMoving;
    [HideInInspector]public float normalAngle;
    [HideInInspector]public Vector3 crossProduct;

    RaycastHit hitInfoForSlope;

    #endregion
    void FixedUpdate()
    {
        CheckStair();
        CheckSlope();
    }

    public void CheckStair()
    {
        castCurrentHeight= 0;
        Vector3 rayDir = new Vector3(rb.velocity.x, 0, rb.velocity.z).normalized;
        Vector3 rayDir2 = RotateVecAroundVec(rayDir, Vector3.up, diagonalRayAngle);
        Vector3 rayDir3 = RotateVecAroundVec(rayDir, Vector3.up, -diagonalRayAngle);
        if(rb.velocity.magnitude < 0.01f)
        {
            /* If velocity is too low, then launch rays into the following directions */
            switch(character.animStatePriDir)
            {
                case AnimStatePriDir.front:
                    switch (character.animStateSecDir)
                    {
                        case AnimStateSecDir.none: rayDir = character.transform.forward; break;
                        case AnimStateSecDir.left: rayDir = (character.transform.forward - character.transform.right).normalized; break;
                        case AnimStateSecDir.right: rayDir = (character.transform.forward + character.transform.right).normalized; break;
                    } break;
                case AnimStatePriDir.back:
                    switch (character.animStateSecDir)
                    {
                        case AnimStateSecDir.none: rayDir = -character.transform.forward; break;
                        case AnimStateSecDir.left: rayDir = (-character.transform.forward - character.transform.right).normalized; break;
                        case AnimStateSecDir.right: rayDir = (-character.transform.forward + character.transform.right).normalized; break;
                    }
                    break;
                case AnimStatePriDir.none:
                    switch (character.animStateSecDir)
                    {
                        case AnimStateSecDir.left: rayDir = -character.transform.right; break;
                        case AnimStateSecDir.right: rayDir = character.transform.right; break;
                    }
                    break;
            }
        }
        Debug.DrawRay(transform.position, Vector3.up * stairMaxHeight, Color.magenta);
        Debug.DrawRay(transform.position + Vector3.up * stairMaxHeight, rayDir*castDistance, Color.magenta);
        
        do
        {
            /* Iteratively, launch a ray one by one. If it hits a surface in every launch until the
               stairMaxHeight, do nothing. If it gets NOT hit, thenincrease the y position of character
               by the height amount of stair by multiplying it with castCurrenHeight */
            ray = new Ray(transform.position + new Vector3(0,castCurrentHeight,0), rayDir);
            isHit = Physics.Raycast(ray, out hitInfo, castDistance, lMask, QueryTriggerInteraction.Ignore);
            Debug.DrawRay(transform.position + new Vector3(0, castCurrentHeight, 0), rayDir * castDistance, Color.cyan);

            ray_2 = new Ray(transform.position + new Vector3(0,castCurrentHeight,0), rayDir2);
            isHit2 = Physics.Raycast(ray_2, out hitInfo_2, castDistance, lMask, QueryTriggerInteraction.Ignore);
            Debug.DrawRay(transform.position + new Vector3(0, castCurrentHeight, 0), rayDir2 * castDistance, Color.cyan);

            ray_3 = new Ray(transform.position + new Vector3(0,castCurrentHeight,0), rayDir3);
            isHit3 = Physics.Raycast(ray_3, out hitInfo_3, castDistance, lMask, QueryTriggerInteraction.Ignore);
            Debug.DrawRay(transform.position + new Vector3(0, castCurrentHeight, 0), rayDir3 * castDistance, Color.cyan);

            float[] normalAngles = new float[3];
            normalAngles[0] = Vector3.Angle(hitInfo.normal, Vector3.up);
            normalAngles[1] = Vector3.Angle(hitInfo_2.normal, Vector3.up);
            normalAngles[2] = Vector3.Angle(hitInfo_3.normal, Vector3.up);

            normalsNotSloped = true;
            for(int i = normalAngles.Length - 1; i >= 0; i--)
            {
                if (normalAngles[i] < 80)
                {
                    normalsNotSloped = false;
                    break;
                }
            }

            if (!isHit && !isHit2 && !isHit3)
            {
                if(character.animStateSpeed != AnimStateSpeed.idle && normalsNotSloped){
                    character.transform.position = Vector3.Lerp(character.transform.position,
                                                  character.transform.position + new Vector3(0, castCurrentHeight, 0), 0.1f * stairJumpForce);
                }
                break;
            }
            castCurrentHeight += iterateIncrease;
        } while (castCurrentHeight < stairMaxHeight);

    }
    public void CheckSlope()
    {
        /* Launch a ray to downwards. If it is hit, calculate the normal angle to find slope. Find 
           the cross product to get a tangent line for the sruface. If normal angle is "5 < x < maxSlopeAngle"
           check speed anim state. If player is waiting idle, apply an opposite gravity to prevent sliding.
           And lerp the velocity to zero, or with zero gravity it fill float. If it is not waiting idle
           then set the onSlopeMoving true. If it is true, MoveChar and AccelerateChar will behave accordingly
           in the GeneralCharacter script */
        ray2 = new Ray(transform.position, Vector3.down);
        if (Physics.Raycast(ray2, out hitInfoForSlope, castDistance2, lMask, QueryTriggerInteraction.Ignore)){

            Debug.DrawRay(transform.position, Vector3.down * castDistance2, Color.yellow);
            normalAngle = Vector3.Angle(Vector3.up, hitInfoForSlope.normal);
            crossProduct = Vector3.Cross(hitInfoForSlope.normal, Vector3.up);
            Debug.DrawRay(transform.position + new Vector3(0,0.5f,0), crossProduct, Color.yellow);

            if (normalAngle < maxSlopeAngle && normalAngle > 5)
            {
                if (character.animStateSpeed == AnimStateSpeed.idle)
                {
                    rb.AddForce(Physics.gravity * rb.mass * -slopeForceMultiplier);
                    rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, 0.1f);
                    onSlopeMoving = false;
                }
                else
                {
                    onSlopeMoving = true;
                }
            }
        }

    }

    public static Vector3 RotateVecAroundVec(Transform tform,Vector3 vecToRotate, Vector3 vecToAxis , float angle)
    {
        Debug.DrawRay(tform.position + new Vector3(0,0.3f,0), Quaternion.AngleAxis(-angle, vecToAxis) * vecToRotate, Color.blue);
        return Quaternion.AngleAxis(-angle, vecToAxis) * vecToRotate;
    }
    public static Vector3 RotateVecAroundVec(Vector3 vecToRotate, Vector3 vecToAxis, float angle)
    {
        return Quaternion.AngleAxis(-angle, vecToAxis) * vecToRotate;
    }
    public static Vector3 RotateVecAroundVec(Vector3 vecToRotate, Vector3 vecToAxis, float angle, Vector3 vecToAxis2, float angle2)
    {
        return Quaternion.AngleAxis(- angle2, vecToAxis2)*(Quaternion.AngleAxis(-angle, vecToAxis)) * vecToRotate;
    }
}
