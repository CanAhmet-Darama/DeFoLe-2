using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ImpactMarkManager : MonoBehaviour
{
    public static GameObject bulletMarkPrefab;
    [SerializeField] GameObject bMP_isThis;
    float markLife = 10;

    public static GameObject[] bulletMarks;
    public static bool firstOrSecondCycle = false;
    public static bool[] cyclesStates;
    public static short bulletMarksCount = 75;
    public static short lastCalledIndex;
    static ImpactMarkManager impactManagerIns;

    void Start()
    {
        impactManagerIns= GetComponent<ImpactMarkManager>();
        bulletMarkPrefab = bMP_isThis;
        bulletMarks = new GameObject[bulletMarksCount];
        cyclesStates = new bool[bulletMarksCount];
        lastCalledIndex = 0;
        InstantiateBulletMarks();
    }

    void Update()
    {
        
    }
    public static void CallMark(Vector3 pos, Vector3 rot)
    {
        GameObject mark = GetMarkReady();
        mark.transform.position = pos;
        Debug.Log(rot);
        mark.transform.forward = rot;
        mark.SetActive(true);
        impactManagerIns.DeleteBulletMark(mark, lastCalledIndex);
    }
    void InstantiateBulletMarks()
    {
        for (short i = (short)(bulletMarksCount - 1); i >= 0; i--)
        {
            GameObject bMark = Instantiate(bulletMarkPrefab, transform);
            bulletMarks[i] = bMark;
            bMark.SetActive(false);
        }
    }
    static GameObject GetMarkReady()
    {
        for(short i = (short)(bulletMarks.Length - 1); i >= 0; i--) {
            if (!bulletMarks[i].activeInHierarchy)
            {
                cyclesStates[i] = !cyclesStates[i];
                lastCalledIndex= i;
                return bulletMarks[i];
            }
        }
        for (short i = (short)(bulletMarks.Length - 1); i >= 0; i--)
        {
            if (cyclesStates[i] == firstOrSecondCycle)
            {
                cyclesStates[i] = !cyclesStates[i];
                lastCalledIndex = i;
                return bulletMarks[i];
            }
        }
        firstOrSecondCycle = !firstOrSecondCycle;
        return GetMarkReady();
    }
    IEnumerator DeleteBulletMark(GameObject mark, short index)
    {
        bool previousValue = cyclesStates[index];
        yield return new WaitForSeconds(markLife);

        if(previousValue == cyclesStates[index])
        {
            mark.SetActive(false);
        }
    }
}
