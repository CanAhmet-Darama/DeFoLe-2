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
                    /*if (collision.gameObject == GameManager.mainTerrain.gameObject)
                    {
                        Terrain terrain = GameManager.mainTerrain;

                        Vector3 terrainLocalPos = terrain.transform.InverseTransformPoint(transform.position);
                        float terrainHeight = terrain.SampleHeight(terrainLocalPos);
                        Vector3 closestPoint = terrain.transform.TransformPoint(new Vector3(terrainLocalPos.x, terrainHeight, terrainLocalPos.z));

                        int x = Mathf.RoundToInt(closestPoint.x / terrain.terrainData.size.x * terrain.terrainData.alphamapWidth);
                        int y = Mathf.RoundToInt(closestPoint.z / terrain.terrainData.size.z * terrain.terrainData.alphamapHeight);

                        if (x < 0 || x >= terrain.terrainData.alphamapWidth || y < 0 || y >= terrain.terrainData.alphamapHeight)
                        {
                            Debug.Log("Invalid position: " + closestPoint);
                        }
                        else
                        {
                            float[,,] alphamaps = terrain.terrainData.GetAlphamaps(x, y, 1, 1);

                            float maxAlpha = 0f;
                            int maxIndex = 0;
                            for (int i = alphamaps.GetLength(2) - 1; i >= 0; i--)
                            {
                                if (alphamaps[0, 0, i] > maxAlpha)
                                {
                                    maxAlpha = alphamaps[0, 0, i];
                                    maxIndex = i;
                                }
                            }

                            string textureName = terrain.terrainData.terrainLayers[maxIndex].diffuseTexture.name;
                            Debug.Log("Texture at closest point: " + textureName);

                        }

                    }*/

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
                    ImpactMarkManager.MakeBulletImpactWithoutMark(collision.contacts[0].point + contact.normal.normalized * 0.01f, contact.normal,
                        EnvObjType.metal);
                }
                else if (collision.collider.tag == "Player" || collision.collider.tag == "Enemy")
                {
                    if (collision.collider.gameObject.name == "Helmet Holder")
                        ImpactMarkManager.MakeBulletImpactWithoutMark(collision.contacts[0].point + contact.normal.normalized * 0.01f, contact.normal, EnvObjType.metal);
                    else
                        ImpactMarkManager.MakeBloodImpactAndSound(collision.contacts[0].point + contact.normal.normalized * 0.01f, contact.normal, true);

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
