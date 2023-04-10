using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoverObjectsManager : MonoBehaviour
{

    public static CoverPoint[][] coverPointsOfWorld;
    public static CoverTakeableObject[][] coverObjectsOfWorld;
    public static float sortByDistanceCooldown = 5;

    static MainCharacter mainChar;

    void Awake()
    {
        coverPointsOfWorld = new CoverPoint[GameManager.numberOfCamps][];
        coverObjectsOfWorld = new CoverTakeableObject[GameManager.numberOfCamps][];
        for(int i = coverObjectsOfWorld.Length-1; i >= 0; i--)
        {
            coverObjectsOfWorld[i] = new CoverTakeableObject[0];
        }
    }
    void Start()
    {
        mainChar = GameManager.mainChar.GetComponent<MainCharacter>();
        StartCoroutine(SortCoverObjectsByDistanceCoroutine(1, GameManager.mainChar.position));
    }

    /*IEnumerator CopySortedToUnsorted()
    {
        yield return null;
        for (int i = unSortedCoverObjectsOfWorld.Length - 1; i >= 0; i--)
        {
            unSortedCoverObjectsOfWorld[i] = new CoverTakeableObject[coverObjectsOfWorld[i].Length];
            Debug.Log("unsortedObject " + i + " is " + unSortedCoverObjectsOfWorld[i].Length + " but " + coverObjectsOfWorld[i].Length);
        }
        Array.Copy(coverObjectsOfWorld, unSortedCoverObjectsOfWorld, coverObjectsOfWorld.Length);

        for (int i = unSortedCoverObjectsOfWorld.Length - 1; i >= 0; i--)
        {
            for (int j = unSortedCoverObjectsOfWorld[i].Length - 1; j >= 0; j--)
            {
                coverObjectsOfWorld[i][j].unSortedIndex = (short)j;
            }
        }
    }*/


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

        coverObj.coveredObjectIndex = (short)(holderArray.Length);
    }

    public static Vector3 GetCoverPoint(byte campNumber, EnemyScript enemyScriptIns = null, bool stayOnSamePeekCover = false)
    {
        enemyScriptIns.currentCoveredIndexes[0] = (short)(campNumber - 1);
        if (!stayOnSamePeekCover)
        {
            short calculateTicket = 15;
            short coverIndex;
            while (calculateTicket > 0)
            {
                /* I don't let it to take a random object which is too close or far from enemy */
                coverIndex = (short)(Random.Range(3, coverObjectsOfWorld[campNumber - 1].Length / 2));
                //coverIndex = (short)(Random.Range(0, coverObjectsOfWorld[campNumber - 1].Length));

            
                CoverPoint[] cPointsOfObj;

                for(short i = coverIndex; i >= 0; i--)
                {
                    if(!coverObjectsOfWorld[campNumber - 1][i].forStaticUsage)
                    {
                        cPointsOfObj = coverObjectsOfWorld[campNumber - 1][i].coverPoints;
                        coverObjectsOfWorld[campNumber - 1][i].SortPointsByDistance(GameManager.mainChar.position);
                        for (short j = (short)(cPointsOfObj.Length - 1); j >= 0; j--)
                        {
                            if (!cPointsOfObj[j].isCoveredAlready)
                            {
                                enemyScriptIns.currentCoveredIndexes[1] = coverObjectsOfWorld[campNumber - 1][i].coveredObjectIndex;
                                enemyScriptIns.currentCoveredIndexes[2] = cPointsOfObj[j].coveredPointIndex;

                                cPointsOfObj[j].isCoveredAlready = true;

                                if (cPointsOfObj[j].crouchOrPeek)
                                {
                                    enemyScriptIns.currentCoverPoint = cPointsOfObj[j];
                                    return cPointsOfObj[j].worldPos;
                                }
                                else
                                {
                                    //Debug.Log("Picked a cover with peek");
                                    enemyScriptIns.currentCoverPoint = cPointsOfObj[j];
                                    Vector3 yResetPoint = new Vector3(cPointsOfObj[j].worldPos.x, 0, cPointsOfObj[j].worldPos.z);
                                    Vector3 yResetPlayer = new Vector3(GameManager.mainChar.position.x,0, GameManager.mainChar.position.z);

                                    enemyScriptIns.currentCoverPoint.coverForwardForPeek = (yResetPoint - yResetPlayer).normalized;
                                    return (cPointsOfObj[j].worldPos+ enemyScriptIns.currentCoverPoint.coverForwardForPeek
                                        * cPointsOfObj[j].peekCoverDistanceFromCenter + StairCheckScript.RotateVecAroundVec(
                                        enemyScriptIns.currentCoverPoint.coverForwardForPeek * enemyScriptIns.currentCoverPoint.peekCoverDistanceFromCenter, Vector3.up, 90));
                                }
                            }
                        }
                    }

                }
                calculateTicket--;
            }
            return Vector3.zero;
        }
        else
        {
            Vector3 yResetPoint = new Vector3(enemyScriptIns.currentCoverPoint.worldPos.x, 0, enemyScriptIns.currentCoverPoint.worldPos.z);
            Vector3 yResetPlayer = new Vector3(GameManager.mainChar.position.x, 0, GameManager.mainChar.position.z);

            enemyScriptIns.currentCoverPoint.coverForwardForPeek = (yResetPoint - yResetPlayer).normalized;
            return (enemyScriptIns.currentCoverPoint.worldPos + enemyScriptIns.currentCoverPoint.coverForwardForPeek
                * enemyScriptIns.currentCoverPoint.peekCoverDistanceFromCenter + StairCheckScript.RotateVecAroundVec(
                enemyScriptIns.currentCoverPoint.coverForwardForPeek * enemyScriptIns.currentCoverPoint.peekCoverDistanceFromCenter, Vector3.up, 90));

        }
    }

    public IEnumerator SortCoverObjectsByDistanceCoroutine(byte campNumber, Vector3 posToTakeDistance)
    {
        yield return new WaitForSeconds(sortByDistanceCooldown);
        SortCoverObjectsByDistance(campNumber, posToTakeDistance);
        yield return null;
        //AssignUnsortedCoveredVariables();
        yield return null;
        StartCoroutine(SortCoverObjectsByDistanceCoroutine(mainChar.closestCamp, GameManager.mainChar.position));
        yield return null;
        for (int i = GameManager.numberOfCamps - 1; i >= 0; i--)
        {
            SortCoverObjectsByDistance(campNumber, posToTakeDistance);
            yield return null;
        }
    }
    public IEnumerator SortCoverObjectsByDistanceCoroutine(Vector3 posToTakeDistance)
    {
        yield return null;
        for(short i = (short)(GameManager.numberOfCamps - 1); i >= 0; i--)
        {
            if(EnemyManager.campsAlerted[i])
            {
                SortCoverObjectsByDistance((byte)(i+1),posToTakeDistance);
                yield return null;
            }
        }
        yield return new WaitForSeconds(sortByDistanceCooldown);
        StartCoroutine(SortCoverObjectsByDistanceCoroutine(GameManager.mainChar.position));
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
