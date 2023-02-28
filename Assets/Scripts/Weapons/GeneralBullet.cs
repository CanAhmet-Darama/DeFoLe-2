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

    void Start()
    {
        maxRange = itsOwnerWeapon.range;
    }

    void OnEnable()
    {
    }
    void OnDisable()
    {
    }
    void Update()
    {
        duratPassed += Time.deltaTime;
        if((transform.position - firedPos).magnitude > maxRange)
        {
            itsOwnerWeapon.owner.ParentAndResetBullet(transform, itsHolder.transform);
        }
    }
    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag != "Bullet")
        {
            itsOwnerWeapon.owner.ParentAndResetBullet(transform, itsHolder.transform);
        }
    }

}
