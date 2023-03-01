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
<<<<<<< HEAD
    public LineRenderer lineRenderer;
    public byte index;
=======
>>>>>>> parent of ef6e371 (Commit 17_3)

    void Start()
    {
        maxRange = itsOwnerWeapon.range;
<<<<<<< HEAD
        lineRenderer = GetComponent<LineRenderer>();
=======
>>>>>>> parent of ef6e371 (Commit 17_3)
    }

    void OnEnable()
    {
    }
    void OnDisable()
    {
<<<<<<< HEAD
        lineRenderer.enabled = false;
=======
>>>>>>> parent of ef6e371 (Commit 17_3)
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
        if(collision.gameObject.tag != "Bullet")
        {
            itsOwnerWeapon.owner.ParentAndResetBullet(transform, itsHolder.transform, this);
        }
    }

}
