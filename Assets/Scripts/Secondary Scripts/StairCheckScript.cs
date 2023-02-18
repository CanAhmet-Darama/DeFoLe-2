using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class StairCheckScript : MonoBehaviour
{
    [SerializeField] Rigidbody rb;
    [SerializeField] MainCharacter mainChar;
    #region Stair
    Ray ray;
    float castDistance = 0.225f;
    float stairMaxHeight = 0.6f;
    float castCurrentHeight;
    float iterateIncrease = 0.05f;
    float stairJumpForce = 3;
    LayerMask lMask = ~128;

    RaycastHit hitInfo;
    bool isHit;
    #endregion

    #region Slope
    Ray ray2;
    float castDistance2 = 0.2f;
    float maxSlopeAngle = 55;
    float slopeForceMultiplier = 0.99f;
    public bool onSlopeMoving;
    public float normalAngle;
    public Vector3 crossProduct;

    RaycastHit hitInfo2;

    #endregion


    void Start()
    {
    }
    void FixedUpdate()
    {
        CheckStair();
        CheckSlope();
    }

    public void CheckStair()
    {
        castCurrentHeight= 0;
        Vector3 rayDir = new Vector3(rb.velocity.x, 0, rb.velocity.z).normalized;
        if(rb.velocity.magnitude < 0.01f)
        {
            switch(mainChar.animStatePriDir)
            {
                case AnimStatePriDir.front:
                    switch (mainChar.animStateSecDir)
                    {
                        case AnimStateSecDir.none: rayDir = mainChar.transform.forward; break;
                        case AnimStateSecDir.left: rayDir = (mainChar.transform.forward - mainChar.transform.right).normalized; break;
                        case AnimStateSecDir.right: rayDir = (mainChar.transform.forward + mainChar.transform.right).normalized; break;
                    } break;
                case AnimStatePriDir.back:
                    switch (mainChar.animStateSecDir)
                    {
                        case AnimStateSecDir.none: rayDir = -mainChar.transform.forward; break;
                        case AnimStateSecDir.left: rayDir = (-mainChar.transform.forward - mainChar.transform.right).normalized; break;
                        case AnimStateSecDir.right: rayDir = (-mainChar.transform.forward + mainChar.transform.right).normalized; break;
                    }
                    break;
                case AnimStatePriDir.none:
                    switch (mainChar.animStateSecDir)
                    {
                        case AnimStateSecDir.left: rayDir = -mainChar.transform.right; break;
                        case AnimStateSecDir.right: rayDir = mainChar.transform.right; break;
                    }
                    break;
            }
        }
        Debug.DrawRay(transform.position, Vector3.up * stairMaxHeight, Color.magenta);
        Debug.DrawRay(transform.position + Vector3.up * stairMaxHeight, rayDir*castDistance, Color.magenta);
        
        do
        {
            ray = new Ray(transform.position + new Vector3(0,castCurrentHeight,0), rayDir);
            isHit = Physics.Raycast(ray, out hitInfo, castDistance, lMask, QueryTriggerInteraction.Ignore);
            Debug.DrawRay(transform.position + new Vector3(0, castCurrentHeight, 0), rayDir * castDistance, Color.cyan);

            if (!isHit)
            {
                if(mainChar.animStateSpeed != AnimStateSpeed.idle){
                    mainChar.transform.position = Vector3.Lerp(mainChar.transform.position,
                                                  mainChar.transform.position + new Vector3(0, castCurrentHeight, 0), 0.1f * stairJumpForce);
                }
                break;
            }
            castCurrentHeight += iterateIncrease;
        } while (castCurrentHeight < stairMaxHeight);

    }
    public void CheckSlope()
    {
        ray2 = new Ray(transform.position, Vector3.down);
        if (Physics.Raycast(ray2, out hitInfo2, castDistance2, lMask, QueryTriggerInteraction.Ignore)){

            Debug.DrawRay(transform.position, Vector3.down * castDistance2, Color.yellow);
            normalAngle = Vector3.Angle(Vector3.up, hitInfo2.normal);
            crossProduct = Vector3.Cross(hitInfo2.normal, Vector3.up);
            Debug.DrawRay(transform.position + new Vector3(0,0.5f,0), crossProduct, Color.yellow);

            if (normalAngle < maxSlopeAngle && normalAngle > 5)
            {
                if (mainChar.animStateSpeed == AnimStateSpeed.idle)
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

    public Vector3 RotateVecAroundVec(Vector3 vecToRotate, Vector3 vecToAxis , float angle)
    {
        Debug.DrawRay(transform.position + new Vector3(0,0.3f,0), Quaternion.AngleAxis(angle, vecToAxis) * vecToRotate, Color.blue);
        return Quaternion.AngleAxis(-angle, vecToAxis) * vecToRotate;
    }
}
