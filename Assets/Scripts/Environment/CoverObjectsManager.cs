using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoverObjectsManager : MonoBehaviour
{

    public static CoverPoint[][] coverPointsOfWorld;
    public static CoverTakeableObject[][] coverObjectsOfWorld;

    void Awake()
    {
        coverPointsOfWorld = new CoverPoint[GameManager.numberOfCamps][];
        coverObjectsOfWorld = new CoverTakeableObject[GameManager.numberOfCamps][];
    }
    void Start()
    {
        StartCoroutine(SortCoverObjectsByDistanceCoroutine(1, GameManager.mainChar.position));
    }

    public static void AddCoverPointsToList(byte campNumber, CoverPoint[] cPoints)
    {
        if (coverPointsOfWorld[campNumber - 1] == null)
        {
            coverPointsOfWorld[campNumber - 1] = new CoverPoint[0];
        }
        CoverPoint[] holderArray = new CoverPoint[coverPointsOfWorld[campNumber - 1].Length];

        for (int i = holderArray.Length - 1; i >= 0; i--)
        {
            holderArray[i] = coverPointsOfWorld[campNumber - 1][i];
        }
        int lengthAll = coverPointsOfWorld[campNumber - 1].Length, lengthForAdd = cPoints.Length;
        coverPointsOfWorld[campNumber - 1] = new CoverPoint[lengthAll + lengthForAdd];

        for (int currentIndex = 0; currentIndex < lengthAll + lengthForAdd - 1; currentIndex++)
        {
            if (currentIndex <= lengthAll - 1)
            {
                coverPointsOfWorld[campNumber - 1][currentIndex] = holderArray[currentIndex];
            }
            else
            {
                coverPointsOfWorld[campNumber - 1][currentIndex] = cPoints[currentIndex - lengthAll];
            }
        }
    }
    public static void AddCoverTakeableObjectToList(byte campNumber, CoverTakeableObject coverObj)
    {
        if (coverObjectsOfWorld[campNumber - 1] == null)
        {
            coverObjectsOfWorld[campNumber - 1] = new CoverTakeableObject[0];
        }
        CoverTakeableObject[] holderArray = coverObjectsOfWorld[campNumber - 1];
        coverObjectsOfWorld[campNumber - 1] = new CoverTakeableObject[holderArray.Length + 1];
        for (short i = (short)(holderArray.Length - 1); i >= 0; i--)
        {
            coverObjectsOfWorld[campNumber - 1][i] = holderArray[i];
        }
        coverObjectsOfWorld[campNumber - 1][holderArray.Length] = coverObj;
    }

    public static CoverPoint GetCoverPoint(byte campNumber, float weaponRange, Vector3 ownPos, Vector3 enemyPos)
    {
        short coverIndex;
        while (true)
        {
            /* I subtract 2 from the length of array and increase the coverIndex 
               so the enemy won't pick a point which is too close to player */
            coverIndex = (short)(Random.Range(0, coverObjectsOfWorld[campNumber - 1].Length - 2) + 2);
            Transform ownerTransform = coverObjectsOfWorld[campNumber - 1][coverIndex].transform;

            CoverPoint[] cPointsOfObj = coverObjectsOfWorld[campNumber - 1][coverIndex].coverPoints;
            CoverPoint holderCoverObj;
            bool hasSwapped = true;
            while (hasSwapped)
            {
                hasSwapped = false;
                for (short index = 0, limit = (short)(cPointsOfObj.Length - 1); index < limit; index++)
                {
                    if ((cPointsOfObj[index].worldPos - enemyPos).sqrMagnitude >
                        (cPointsOfObj[index + 1].worldPos - enemyPos).sqrMagnitude)
                    {
                        holderCoverObj = cPointsOfObj[index];
                        cPointsOfObj[index] = cPointsOfObj[index + 1];
                        cPointsOfObj[index + 1] = holderCoverObj;
                        hasSwapped = true;
                    }
                }
            }
            for(short i = (short)(cPointsOfObj.Length - 1); i >= 0; i--)
            {
                if (cPointsOfObj[i].isCoveredAlready!)
                {
                    return cPointsOfObj[i];
                }
            }
        }
    }
    public IEnumerator SortCoverObjectsByDistanceCoroutine(byte campNumber, Vector3 posToTakeDistance)
    {
        yield return null;
        SortCoverObjectsByDistance(campNumber, posToTakeDistance);
    }
    public static void SortCoverObjectsByDistance(byte campNumber, Vector3 posToTakeDistance)
    {
        bool hasSwapped = true;
        CoverTakeableObject[] arrayToSort = coverObjectsOfWorld[campNumber - 1];
        CoverTakeableObject holderCoverObj;
        while (hasSwapped)
        {
            hasSwapped = false;
            for (short index = 0, limit = (short)(arrayToSort.Length - 1); index < limit; index++)
            {
                if ((arrayToSort[index].transform.position - posToTakeDistance).sqrMagnitude > 
                    (arrayToSort[index + 1].transform.position - posToTakeDistance).sqrMagnitude)
                {
                    holderCoverObj = arrayToSort[index];
                    arrayToSort[index] = arrayToSort[index + 1];
                    arrayToSort[index + 1] = holderCoverObj;
                    hasSwapped = true;
                }
            }
        }

        /*for(int i = arrayToSort.Length -1; i >= 0; i--)
        {
            Debug.Log((arrayToSort[i].name + " : " + (arrayToSort[i].transform.position - posToTakeDistance).magnitude));
        }*/
    }
}
