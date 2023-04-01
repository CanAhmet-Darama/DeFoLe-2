using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : MonoBehaviour
{
    [Header("General")]
    public float damage;
    public float swingCooldown;
    public GeneralCharacter owner;
    public bool canSwing;
    public bool isSwinging;
    public BoxCollider meleeCollider;

    [Header("For Animation etc...")]
    public Vector3 rightHandPos;
    public Vector3 rightHandRot;
    public AnimatorOverrideController overrideController;

    [Header("Sound Stuff")]
    public AudioSource audioSource;
    public AudioClip sheathSound;
    public AudioClip swingSound;

    void Start()
    {
        canSwing = true;
        isSwinging = false;
    }
    public void Swing()
    {
        meleeCollider.enabled = true;
        owner.animator.SetTrigger("fire");
        StartCoroutine(WaitToHitAgain());
    }
    IEnumerator WaitToHitAgain()
    {
        canSwing = false;
        yield return new WaitForSeconds(swingCooldown/3);
        isSwinging = true;
        audioSource.Play();
        yield return new WaitForSeconds(swingCooldown/3);
        isSwinging= false;
        yield return new WaitForSeconds(swingCooldown/3);
        yield return null;
        canSwing = true;
        meleeCollider.enabled = false;
    }
    void OnTriggerEnter(Collider other)
    {
        if (isSwinging)
        {
            Vector3 contactPoint;
            if (other.GetType() == typeof(MeshCollider))
            {
                Ray ray = new Ray(transform.position, GameManager.mainCam.forward);
                if(other.Raycast(ray, out RaycastHit hitInfo, 1))
                {
                    contactPoint = hitInfo.point;
                }
                else
                {
                    contactPoint = transform.position;
                }
            }
            else
            {
                contactPoint = other.ClosestPoint(transform.position);
            }

            if (other.CompareTag("Ground"))
            {
                if (other.gameObject == GameManager.mainTerrain.gameObject)
                {
                    TerrainManager.GetTerrainTexture(transform.position);
                    float[] textureValues = new float[TerrainManager.textureValues.Length];
                    Array.Copy(TerrainManager.textureValues, textureValues, TerrainManager.textureValues.Length);
                    for (short textureIndex = (short)(textureValues.Length - 1); textureIndex >= 0; textureIndex--)
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

                            ImpactMarkManager.MakeImpactSound(contactPoint, terrainPointType, textureValues[textureIndex]);
                        }
                    }
                    EnvObjType terrainPointTypeForMark = EnvObjType.general;
                    Array.Sort(textureValues);
                    for (short i = (short)(textureValues.Length - 1); i >= 0; i--)
                    {
                        if (TerrainManager.textureValues[i] == textureValues[textureValues.Length - 1])
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
                            ImpactMarkManager.CallBladeMark(contactPoint + (transform.position - contactPoint).normalized * 0.01f, 
                                (contactPoint - transform.position).normalized, terrainPointTypeForMark, 0);
                            break;
                        }
                    }
                }
                else
                {
                    ImpactMarkManager.CallBladeMark(contactPoint + (transform.position - contactPoint).normalized*0.01f, (contactPoint - transform.position).normalized, other.GetComponent<EnvObject>().objectType);
                }
            }
            else if((other.CompareTag("Player") || other.CompareTag("Enemy")) && other.gameObject != owner.gameObject)
            {
                if (other.gameObject.name == "Helmet Holder")
                    ImpactMarkManager.MakeBulletImpactWithoutMark(contactPoint + (transform.position - contactPoint).normalized * 0.01f, (contactPoint - transform.position).normalized + new Vector3(0,90,0), EnvObjType.metal);
                else
                    ImpactMarkManager.MakeBloodImpactAndSound(contactPoint + (transform.position - contactPoint).normalized * 0.01f, (contactPoint - transform.position).normalized, false);
                
                CharColliderManager usedChar = other.GetComponentInParent<CharColliderManager>();
                if(usedChar.enemySc != null && usedChar.enemySc.enemyState != EnemyScript.EnemyAIState.Alerted)
                {
                    GeneralCharacter.GiveDamage(usedChar.ownerCharacter, usedChar.ownerCharacter.health);
                }
                else
                {
                    byte bodyPartIndex = CharColliderManager.ReturnBodyPartTypeIndex(other.gameObject, usedChar);
                    GeneralCharacter.GiveDamage(usedChar.ownerCharacter, (short)(damage * CharColliderManager.damageMultipliers[bodyPartIndex]));
                }
            }
            else if (other.GetType() == typeof(WheelCollider))
            {
                ImpactMarkManager.MakeBulletImpactWithoutMark(contactPoint + (contactPoint - transform.position).normalized * 0.01f, (contactPoint - transform.position).normalized,
                    EnvObjType.general);
            }
            else if (other.tag == "Vehicle")
            {
                ImpactMarkManager.MakeBulletImpactWithoutMark(contactPoint + (contactPoint - transform.position).normalized * 0.01f, (contactPoint - transform.position).normalized,
                    EnvObjType.metal);
            }

        }
    }
}
