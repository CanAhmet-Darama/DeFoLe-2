using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    [SerializeField] Vector3 offset;
    [SerializeField] Transform objectTransform;
    [SerializeField] Transform camPointTransform;
    float smoothTime = 10f;
    Vector3 velocity = Vector3.zero;
    Vector3 behindPoint;

    void Start()
    {
        
    }

    void Update()
    {
        
    }
    void LateUpdate()
    {
        behindPoint = ((camPointTransform.position - objectTransform.position).normalized * offset.x) + objectTransform.position;
        transform.position = Vector3.SmoothDamp(transform.position,behindPoint + offset, ref velocity, smoothTime*Time.deltaTime);
        camPointTransform.position = objectTransform.position + new Vector3(0,5,0);
        //gameObject.transform.LookAt(camPointTransform);
    }
}
