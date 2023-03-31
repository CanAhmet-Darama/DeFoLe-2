using System;
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
            if ((transform.position - firedPos).sqrMagnitude < 50 || (transform.position - GameManager.mainCam.position).sqrMagnitude < 2500)
            {
                ContactPoint contact = collision.contacts[0];
                //if (collision.gameObject.GetComponent<EnvObject>() != null || collision.collider.GetType() == typeof(TerrainCollider))
                //{
                if (collision.gameObject.CompareTag("Ground"))
                {
                    if (collision.gameObject == GameManager.mainTerrain.gameObject)
                    {
                        TerrainManager.GetTerrainTexture(transform.position);
                        float[] textureValues = new float[TerrainManager.textureValues.Length];
                        Array.Copy(TerrainManager.textureValues, textureValues, TerrainManager.textureValues.Length);
                        for(short textureIndex = (short)(textureValues.Length - 1); textureIndex >= 0; textureIndex--)
                        {
                            if (textureValues[textureIndex] > 0)
                            {
                                EnvObjType terrainPointType = EnvObjType.general;
                                switch (textureIndex)
                                {
                                    case 0:
                                        terrainPointType = EnvObjType.dirt;
                                        break;
                                    case 1:
                                        terrainPointType = EnvObjType.dirt;
                                        break;
                                    case 2:
                                        terrainPointType = EnvObjType.concrete;
                                        break;
                                }

                                ImpactMarkManager.MakeImpactSound(collision.contacts[0].point, terrainPointType, textureValues[textureIndex]);
                            }
                        }
                        EnvObjType terrainPointTypeForMark = EnvObjType.general;
                        Array.Sort(textureValues);
                        for (short i = (short)(textureValues.Length - 1); i >= 0;i--)
                        {
                            if (TerrainManager.textureValues[i] == textureValues[textureValues.Length-1])
                            {
                                switch (i)
                                {
                                    case 0:
                                        terrainPointTypeForMark = EnvObjType.dirt;
                                        break;
                                    case 1:
                                        terrainPointTypeForMark = EnvObjType.dirt;
                                        break;
                                    case 2:
                                        terrainPointTypeForMark = EnvObjType.concrete;
                                        break;
                                }
                                ImpactMarkManager.CallMark(collision.contacts[0].point + contact.normal.normalized * 0.01f, contact.normal,
                                    terrainPointTypeForMark,0);
                                break;
                            }
                        }
                    }
                    else
                    {
                        ImpactMarkManager.CallMark(collision.contacts[0].point + contact.normal.normalized * 0.01f, contact.normal,
                                collision.gameObject.GetComponent<EnvObject>().objectType);
                    }
                }
                else if(collision.collider.GetType() == typeof(WheelCollider))
                {
                    ImpactMarkManager.MakeBulletImpactWithoutMark(collision.contacts[0].point + contact.normal.normalized * 0.01f, contact.normal,
                        EnvObjType.general);
                }
                else if (collision.collider.tag == "Vehicle")
                {
                    ImpactMarkManager.MakeBulletImpactWithoutMark(collision.contacts[0].point + contact.normal.normalized * 0.01f, contact.normal,
                        EnvObjType.metal);
                }
                else if (collision.collider.tag == "Player" || collision.collider.tag == "Enemy")
                {
                    if (collision.collider.gameObject.name == "Helmet Holder")
                        ImpactMarkManager.MakeBulletImpactWithoutMark(collision.contacts[0].point + contact.normal.normalized * 0.01f, contact.normal, EnvObjType.metal);
                    else
                        ImpactMarkManager.MakeBloodImpactAndSound(collision.contacts[0].point + contact.normal.normalized * 0.01f, contact.normal, true);


                    CharColliderManager usedChar = contact.otherCollider.GetComponentInParent<CharColliderManager>();
                    byte bodyPartIndex = CharColliderManager.ReturnBodyPartTypeIndex(contact.otherCollider.gameObject, usedChar);
                    GeneralCharacter.GiveDamage(usedChar.ownerCharacter, (short)(itsOwnerWeapon.damage * CharColliderManager.damageMultipliers[bodyPartIndex]));
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
