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

    public static Collider[] nearbyColliders;
    public float collisionAlertRange;


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
        if((transform.position - firedPos).sqrMagnitude > maxRange*maxRange)
        {
            itsOwnerWeapon.owner.ParentAndResetBullet(transform, itsHolder.transform, this);
        }
    }
    void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Bullet"))
        {
            ContactPoint contact = collision.contacts[0];

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
                    EnvObject envObj = collision.gameObject.GetComponent<EnvObject>();
                    if (envObj.impactMarkable)
                    {
                        ImpactMarkManager.CallMark(collision.contacts[0].point + contact.normal.normalized * 0.01f, contact.normal,
                                collision.gameObject.GetComponent<EnvObject>().objectType);
                    }
                    else
                    {
                        ImpactMarkManager.MakeBulletImpactWithoutMark(collision.contacts[0].point + contact.normal.normalized * 0.01f, contact.normal,
                                collision.gameObject.GetComponent<EnvObject>().objectType);
                    }

                    if (envObj.destroyable)
                    {
                        envObj.ReduceObjHealth(itsOwnerWeapon.damage);
                    }
                }
            }
            else if(collision.collider.tag == "Vehicle")
            {
                GeneralVehicle shotVehicle = collision.gameObject.GetComponentInParent<GeneralVehicle>();
                shotVehicle.DamageVehicle(itsOwnerWeapon.damage);
            if (collision.collider.GetType() == typeof(WheelCollider))
                {
                    ImpactMarkManager.MakeBulletImpactWithoutMark(collision.contacts[0].point + contact.normal.normalized * 0.01f, contact.normal,
                        EnvObjType.general);
                        
                }
                else
                {
                    ImpactMarkManager.MakeBulletImpactWithoutMark(collision.contacts[0].point + contact.normal.normalized * 0.01f, contact.normal,
                        EnvObjType.metal);
                }
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

            nearbyColliders = Physics.OverlapSphere(contact.point, collisionAlertRange, LayerMask.GetMask("Player"));
            for(int i = nearbyColliders.Length - 1; i >= 0;i--)
            {
                if (nearbyColliders[i].CompareTag("Enemy"))
                {
                    nearbyColliders[i].GetComponent<EnemyScript>().ChangeEnemyAIState(EnemyScript.EnemyAIState.Alerted);
                }
            }

            Debug.DrawLine(firedPos, contact.point, Color.cyan);
            itsOwnerWeapon.owner.ParentAndResetBullet(transform, itsHolder.transform, this);
        }
    }


}
