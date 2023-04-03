using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvObject : MonoBehaviour
{
    [Header("General Obj Properties")]
    public EnvObjType objectType;
    public bool destroyable;
    public bool breakable;
    public bool impactMarkable = true;
    public short healthOfObject;

    [Header("For Destruction")]
    public GameObject mainObj;
    public GameObject[] subPartObjs;
    public Collider mainCollider;
    public Rigidbody objRb;
    public bool isSubPartItself;
    bool hasDestructed = false;
    

    void Start()
    {
        if(destroyable && breakable)
        {
            objRb = GetComponent<Rigidbody>();
            mainCollider = GetComponent<Collider>();
        }
        if (!isSubPartItself && breakable)
        {
            for (int i = subPartObjs.Length - 1; i >= 0; i--)
            {
                subPartObjs[i].SetActive(false);
            }
        }
    }
    void Update()
    {
        if(healthOfObject <= 0 && destroyable && !hasDestructed)
        {
            DestroyEnvObject();
        }
    }
    public void ReduceObjHealth(short damage)
    {
        healthOfObject -= damage;
    }

    public void DestroyEnvObject()
    {
        if (breakable)
        {
            for (int i = subPartObjs.Length - 1; i >= 0; i--)
            {
                subPartObjs[i].SetActive(true);
            }
        }

        float volume = 1;
        if (!isSubPartItself)
        {
            mainCollider.enabled = false;
            objRb.isKinematic = true;
            hasDestructed = true;
            mainObj.SetActive(false);
        }
        else
        {
            gameObject.SetActive(false);
            volume = 0.5f;
        }
        ImpactMarkManager.MakeBreakingSound(transform.position, objectType, volume);
    }
}
public enum EnvObjType { general, dirt, metal, wood, concrete}

public class TerrainManager : MonoBehaviour
{
    public static Terrain mainTerrain;
    public static int posX;
    public static int posZ;
    public static float[] textureValues = new float[3];

    public static void GetTerrainTexture(Vector3 referencePos)
    {
        ConvertPositionToOnTerrain(referencePos);
        CheckTexture();
    }
    static void ConvertPositionToOnTerrain(Vector3 referencePos)
    {
        Vector3 terrainPosition = referencePos - mainTerrain.transform.position;
        Vector3 mapPosition = new Vector3
        (terrainPosition.x / mainTerrain.terrainData.size.x, 0,
        terrainPosition.z / mainTerrain.terrainData.size.z);
        float xCoord = mapPosition.x * mainTerrain.terrainData.alphamapWidth;
        float zCoord = mapPosition.z * mainTerrain.terrainData.alphamapHeight;
        posX = (int)xCoord;
        posZ = (int)zCoord;
    }
    static void CheckTexture()
    {
        float[,,] aMap = mainTerrain.terrainData.GetAlphamaps(posX, posZ, 1, 1);
        textureValues[0] = aMap[0, 0, 0];
        textureValues[1] = aMap[0, 0, 1];
        textureValues[2] = aMap[0, 0, 2];
    }

}