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
                if (collision.gameObject.GetComponent<EnvObject>() != null)
                {
                    ImpactMarkManager.CallMark(collision.contacts[0].point + contact.normal.normalized * 0.01f, contact.normal,
                        collision.gameObject.GetComponent<EnvObject>().objectType);
                }
                else if(collision.collider.GetType() == typeof(WheelCollider))
                {
                    ImpactMarkManager.CallMark(collision.contacts[0].point + contact.normal.normalized * 0.01f, contact.normal,
                        EnvObjType.general);
                }
                else if (collision.collider.tag == "Vehicle")
                {
                    ImpactMarkManager.CallMark(collision.contacts[0].point + contact.normal.normalized * 0.01f, contact.normal,
                        EnvObjType.metal);
                }
                else if (collision.collider.tag == "Player" || collision.collider.tag == "Enemy")
                {
                    ImpactMarkManager.MakeBloodImpactAndSound(collision.contacts[0].point + contact.normal.normalized * 0.01f, contact.normal);
                }
                Debug.DrawLine(firedPos, contact.point, Color.cyan);
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
