using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ImpactMarkManager : MonoBehaviour
{
    public static GameObject bulletMarkPrefab;
    [SerializeField] GameObject bMP_isThis;

    public static GameObject[] bulletMarks;
    public static short bulletMarksCount = 75;

    void Start()
    {
        bulletMarkPrefab = bMP_isThis;
        bulletMarks = new GameObject[bulletMarksCount];
        InstantiateBulletMarks();
    }

    void Update()
    {
        
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
    GameObject GetMarkReady()
    {
        for(short i = (short)bulletMarks.Length; i >= 0; i--) {
            if (bulletMarks[i] == null)
            {
                return bulletMarks[i];
            }
        }
        return null;
    }
}
