using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvObject : MonoBehaviour
{
    public EnvObjType objectType;
    public bool destroyable;
    public short healthOfObject;
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