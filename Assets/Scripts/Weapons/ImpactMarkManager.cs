using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ImpactMarkManager : MonoBehaviour
{
    [Header("Bullet Marks")]
    public static GameObject bulletMarkPrefab;
    [SerializeField] GameObject bMP_isThis;
    float markLife = 10;

    public static GameObject[] bulletMarks;
    public static bool firstOrSecondCycle = false;
    public static bool[] cyclesStates;
    public static short bulletMarksCount = 75;
    public static short lastCalledIndex;
    static ImpactMarkManager impactManagerIns;

    [Header("Bullet Impacts")]
    public static ParticleSystem bulletImpactPrefab;
    [SerializeField] ParticleSystem bImpact_isThis;
    public static ParticleSystem[] bulletImpacts;
    public static short impactsCount = 20;




    void Start()
    {
        impactManagerIns= GetComponent<ImpactMarkManager>();
        bulletMarkPrefab = bMP_isThis;
        bulletMarks = new GameObject[bulletMarksCount];
        cyclesStates = new bool[bulletMarksCount];
        lastCalledIndex = 0;

        bulletImpacts = new ParticleSystem[impactsCount];
        bulletImpactPrefab = bImpact_isThis;
        InstantiateBulletMarks();
        InstantiateBulletImpacts();
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

        ParticleSystem impact = GetImpactReady();
        impact.transform.position = pos;
        impact.transform.forward = rot;
        impact.Play();
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
    void InstantiateBulletImpacts()
    {
        for (short i = (short)(impactsCount - 1); i >= 0; i--)
        {
            ParticleSystem bImpact = Instantiate(bulletImpactPrefab, transform);
            bulletImpacts[i] = bImpact;
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
    static ParticleSystem GetImpactReady()
    {
        for (short i = (short)(bulletImpacts.Length - 1); i >= 0; i--)
        {
            if (!bulletImpacts[i].gameObject.activeInHierarchy)
            {
                return bulletImpacts[i];
            }
        }
        return bulletImpacts[bulletImpacts.Length - 1];
    }
}
