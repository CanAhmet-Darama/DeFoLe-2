using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StairCheckScript : MonoBehaviour
{
    [SerializeField] Rigidbody rb;
    Ray ray;
    float castDistance = 0.225f;
    float stairMaxHeight = 0.6f;
    float castCurrentHeight;
    float iterateIncrease = 0.025f;
    LayerMask lMask = ~128;

    RaycastHit hitInfo;
    bool isHit;

    void Start()
    {
        
    }
    void Update()
    {
        CheckStair();
    }

    public bool CheckStair()
    {
        castCurrentHeight= 0;
        Vector3 rayDir = new Vector3(rb.velocity.x, 0, rb.velocity.z).normalized;
        Debug.DrawRay(transform.position, Vector3.up * stairMaxHeight, Color.magenta);
        Debug.DrawRay(transform.position + Vector3.up * stairMaxHeight, rayDir*castDistance, Color.magenta);
        
        do
        {
            ray = new Ray(transform.position + new Vector3(0,castCurrentHeight,0), rayDir);
            isHit = Physics.Raycast(ray, out hitInfo, castDistance, lMask, QueryTriggerInteraction.Ignore);
            Debug.DrawRay(transform.position, rayDir * castDistance, Color.cyan);
            if (!isHit)
            {
                rb.AddForce(Vector3.up * (rb.mass * -Physics.gravity.y + castCurrentHeight) * Time.deltaTime * 45);
                //Debug.Log("FOUND STAIR LOW ENOUGH");
                break;
            }
            //Debug.Log("Can't step this high");
            //Debug.Log("Col name : " + hitInfo.collider.name);
            castCurrentHeight += iterateIncrease;
        } while (castCurrentHeight < stairMaxHeight);
        Debug.Log("Cast Current Height : "+ castCurrentHeight);

        return false;
    }
}
