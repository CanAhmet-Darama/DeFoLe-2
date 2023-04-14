using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnvObject : MonoBehaviour
{
    public static float collideSoundVelocity = 1.5f;

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
    public Rigidbody[] subRbs;
    public NavMeshObstacle navObstacle;
    public bool isSubPartItself;
    bool hasDestructed = false;

    [Header("Destructable Index Things")]
    public short destObjIndex;

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

        if(gameObject.name == "Terrain")
        {
            StartCoroutine(AssignDestObjIndexCoroutine());
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
                subRbs[i].velocity = objRb.velocity;
            }
        }

        float volume = 1;
        if (!isSubPartItself)
        {
            mainCollider.enabled = false;
            objRb.isKinematic = true;
            hasDestructed = true;

            if(navObstacle != null)
            navObstacle.enabled = false;

            if (breakable)
            {
                mainObj.gameObject.SetActive(false);
            }
            else
            {
                Destroy(mainObj.transform.parent.gameObject);
            }
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
    }

    public static void SaveDestroyableObjects(GameData gameDataToUse)
    {
        gameDataToUse.envObjectsDestroyed = new bool[destroyableObjects.Length];
        gameDataToUse.envObjPoses = new float[destroyableObjects.Length][];
        gameDataToUse.envObjectsSubPartsDestroyed = new bool[destroyableObjects.Length][];
        bool[] destroyedArray = gameDataToUse.envObjectsDestroyed;
        for (int i = destroyableObjects.Length - 1; i >= 0; i--)
        {
            if (destroyableObjects[i] == null || destroyableObjects[i].hasDestructed)
            {
                destroyedArray[destroyableObjects[i].destObjIndex] = true;
                if (!destroyableObjects[i].breakable)
                {
                    continue;
                }
            }
            else
            {
                destroyedArray[destroyableObjects[i].destObjIndex] = false;
            }

            gameDataToUse.envObjPoses[destroyableObjects[i].destObjIndex] = new float[3]
            {
                destroyableObjects[i].transform.position.x,
                destroyableObjects[i].transform.position.y,
                destroyableObjects[i].transform.position.z
            };

            if (destroyableObjects[i].breakable)
            {
                gameDataToUse.envObjectsSubPartsDestroyed[destroyableObjects[i].destObjIndex] 
                    = new bool[destroyableObjects[i].subPartObjs.Length];
                bool[] destSubArray = gameDataToUse.envObjectsSubPartsDestroyed[destroyableObjects[i].destObjIndex];
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
        }
    }
    public static void LoadDestroyableObjects(GameData gameDataToUse)
    {
        for (int i = destroyableObjects.Length - 1; i >= 0; i--)
        {

            if (gameDataToUse.envObjectsDestroyed[destroyableObjects[i].destObjIndex])
            {
                if (destroyableObjects[i] != null)
                {
                    if (!destroyableObjects[i].breakable)
                    {
                        Destroy(destroyableObjects[i].gameObject, 0.05f);
                    }
                    else
                    {
                        destroyableObjects[i].mainCollider.enabled = false;
                        destroyableObjects[i].objRb.isKinematic = true;
                        destroyableObjects[i].hasDestructed = true;
                        Destroy(destroyableObjects[i].mainObj);
                        if(destroyableObjects[i].navObstacle != null)
                        {
                            destroyableObjects[i].navObstacle.enabled = false;
                        }
                    }

                    if (gameDataToUse.envObjectsSubPartsDestroyed[destroyableObjects[i].destObjIndex] != null)
                    {
                        bool noPartRemains = true;
                        for(int j = gameDataToUse.envObjectsSubPartsDestroyed[destroyableObjects[i].destObjIndex].Length - 1; j >= 0; j--)
                        {
                            if (gameDataToUse.envObjectsSubPartsDestroyed[destroyableObjects[i].destObjIndex][j])
                            {
                                Destroy(destroyableObjects[i].subPartObjs[j]);
                            }
                            else
                            {
                                noPartRemains = false;
                                destroyableObjects[i].subPartObjs[j].SetActive(true);
                            }
                        }
                        if (noPartRemains)
                        {
                            Destroy(destroyableObjects[i], 0.05f);
                        }
                    }

                }

            }

            if(destroyableObjects[i] != null && gameDataToUse.envObjPoses[destroyableObjects[i].destObjIndex] != null)
            {
                destroyableObjects[i].transform.position = new Vector3
                (gameDataToUse.envObjPoses[destroyableObjects[i].destObjIndex][0],
                gameDataToUse.envObjPoses[destroyableObjects[i].destObjIndex][1],
                gameDataToUse.envObjPoses[destroyableObjects[i].destObjIndex][2]);

                destroyableObjects[i].transform.eulerAngles = new Vector3(0, Random.Range(0, 180), 0);
            }

        }
    }

    public IEnumerator AssignDestObjIndexCoroutine()
    {
        yield return null;
        Transform[] destTransforms = new Transform[destroyableObjects.Length];
        for(int index = destroyableObjects.Length-1; index >= 0; index--)
        {
            destTransforms[index] = destroyableObjects[index].transform;
        }
        GameManager.SortObjectArrayByDistance(ref destroyableObjects, destTransforms, Vector3.zero);
        for(int index = destroyableObjects.Length - 1; index >= 0; index--)
        {
            destroyableObjects[index].destObjIndex = (short)index;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if(objRb != null && !collision.collider.CompareTag("Bullet") &&objRb.velocity.sqrMagnitude > collideSoundVelocity*collideSoundVelocity)
        {
            float sqrVelo = Mathf.Clamp(objRb.velocity.sqrMagnitude, collideSoundVelocity * collideSoundVelocity, collideSoundVelocity * collideSoundVelocity * 25);
            float veloRate = sqrVelo / (collideSoundVelocity * collideSoundVelocity * 25);
            ImpactMarkManager.MakeImpactSound(collision.contacts[0].point, objectType, veloRate);
            ReduceObjHealth((short)(sqrVelo * 0.6f));
        }
    }
}
public enum EnvObjType { general, dirt, metal, wood, concrete, glass}

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