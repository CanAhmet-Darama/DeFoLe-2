using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
[RequireComponent(typeof(Rigidbody))]

public class GeneralBullet : MonoBehaviour
{
    float maxRange;
    [HideInInspector]public Vector3 firedPos;
    public float duratPassed;
    public float bulletSpeed = 500;
    public GameObject itsHolder;
    public GeneralWeapon itsOwnerWeapon;

    public TrailRenderer trailRenderer;
    public byte index;


    void Start()
    {
        maxRange = itsOwnerWeapon.range;
    }

    void OnEnable()
    {
        if(trailRenderer != null)trailRenderer.Clear();
    }
    void OnDisable()
    {

        trailRenderer.enabled = false;

    }
    void Update()
    {
        duratPassed += Time.deltaTime;
        if((transform.position - firedPos).magnitude > maxRange)
        {
            itsOwnerWeapon.owner.ParentAndResetBullet(transform, itsHolder.transform, this);
        }
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag != "Bullet")
        {
            if ((transform.position - firedPos).magnitude < 50)
            {
                ContactPoint contact = collision.contacts[0];
                ImpactMarkManager.CallMark(contact.point + contact.normal.normalized * 0.02f, contact.normal);
                Debug.DrawRay(firedPos, contact.point - firedPos, Color.cyan);
                Debug.Log(Vector3.Angle(contact.normal, firedPos - contact.point));
            }
            itsOwnerWeapon.owner.ParentAndResetBullet(transform, itsHolder.transform, this);
        }
    }

    //void OnCollisionEnter(Collision collision)
    //{
    //    if(collision.gameObject.tag != "Bullet")
    //    {
    //        if((transform.position - firedPos).magnitude < 50)
    //        {
    //            int layerM = ~(1 << 7);
    //            Ray ray = new Ray(firedPos,(transform.position - firedPos).normalized);
    //            Physics.Raycast(ray, out RaycastHit hitInfo,50, layerM, QueryTriggerInteraction.Ignore);
    //            ImpactMarkManager.CallMark(hitInfo.point + hitInfo.normal.normalized*0.02f, hitInfo.normal);
    //            Debug.DrawRay(ray.origin, ray.direction*hitInfo.distance,Color.cyan);
    //        }
    //        itsOwnerWeapon.owner.ParentAndResetBullet(transform, itsHolder.transform, this);

    //    }
    //}

}
