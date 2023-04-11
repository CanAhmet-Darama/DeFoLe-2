using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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
    public NavMeshObstacle navObstacle;
    public bool isSubPartItself;
    bool hasDestructed = false;

    [Header("Destructable Index Things")]
    public short destObjIndex;
    public static short[] destObjIndexArray;

    public static EnvObject[] destroyableObjects;

    void Start()
    {
        if(destroyable)
        {
            if(breakable)
            {
                objRb = GetComponent<Rigidbody>();
                mainCollider = GetComponent<Collider>();
            }
            if (!isSubPartItself)
            {
                navObstacle = GetComponent<NavMeshObstacle>();
                AddDestroyableToList(this);


                if (breakable)
                {
                    for (int i = subPartObjs.Length - 1; i >= 0; i--)
                    {
                        subPartObjs[i].SetActive(false);
                    }
                }
            }
        }
    }

    public void ReduceObjHealth(short damage)
    {
        healthOfObject -= damage;
        if (healthOfObject <= 0 && destroyable && !hasDestructed)
        {
            DestroyEnvObject();
        }
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
            Destroy(mainObj);
            navObstacle.enabled = false;
        }
        else
        {
            Destroy(gameObject);
            volume = 0.5f;
        }
        ImpactMarkManager.MakeBreakingSound(transform.position, objectType, volume);
    }

    void AddDestroyableToList(EnvObject destroyObj)
    {
        GameManager.AddToArray(destroyObj, ref destroyableObjects);
        GameManager.AddToArray(destObjIndex, ref destObjIndexArray);
    }

    public static void SaveDestroyableObjects(GameData gameDataToUse)
    {
        gameDataToUse.envObjectsDestroyed = new bool[destroyableObjects.Length];
        gameDataToUse.envObjectsSubPartsDestroyed = new bool[destroyableObjects.Length][];
        bool[] destroyedArray = gameDataToUse.envObjectsDestroyed;
        for (int i = destroyableObjects.Length - 1; i >= 0; i--)
        {
            if (destroyableObjects[i].breakable)
            {
                gameDataToUse.envObjectsSubPartsDestroyed[destObjIndexArray[i]] 
                    = new bool[destroyableObjects[i].subPartObjs.Length];
                bool[] destSubArray = gameDataToUse.envObjectsSubPartsDestroyed[destObjIndexArray[i]];
                for(int j = destSubArray.Length - 1; j >= 0; j--)
                {
                    if (destroyableObjects[i].subPartObjs[j] == null)
                    {
                        destSubArray[j] = true;
                    }
                    else
                    {
                        destSubArray[j] = false;
                    }
                }

            }


            if (destroyableObjects[i].hasDestructed)
                destroyedArray[destObjIndexArray[i]] = true;
            else
            {
                destroyedArray[destObjIndexArray[i]] = false;
            }
        }
    }
    public static void LoadDestroyableObjects(GameData gameDataToUse)
    {
        for (int i = destroyableObjects.Length - 1; i >= 0; i--)
        {
            if (gameDataToUse.envObjectsDestroyed[destObjIndexArray[i]])
            {
                if (destroyableObjects[i] != null)
                {
                    destroyableObjects[i].mainCollider.enabled = false;
                    destroyableObjects[i].objRb.isKinematic = true;
                    destroyableObjects[i].hasDestructed = true;
                    Destroy(destroyableObjects[i].mainObj);
                    destroyableObjects[i].navObstacle.enabled = false;
                }

                if (gameDataToUse.envObjectsSubPartsDestroyed[destObjIndexArray[i]] != null)
                {
                    for(int j = gameDataToUse.envObjectsSubPartsDestroyed[destObjIndexArray[i]].Length - 1; j >= 0; j--)
                    {
                        if (gameDataToUse.envObjectsSubPartsDestroyed[destObjIndexArray[i]][j])
                        {
                            Destroy(destroyableObjects[i].subPartObjs[j]);
                        }
                        else
                        {
                            destroyableObjects[i].subPartObjs[j].SetActive(true);
                        }
                    }
                }
            }
        }
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