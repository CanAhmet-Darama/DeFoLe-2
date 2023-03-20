using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoverTakeableObject : MonoBehaviour
{
    public CoverPoint[] coverPoints;
    public Vector3[] coverPositions;

    void Start()
    {
        SetCoverPointPositions();
        CoverObjectsManager.AddCoverPointsToList(coverPoints);
    }
    void Update()
    {
        //DrawRaysForCoverPositions();
        DrawRaysForCoverPositions2();
    }
    void SetCoverPointPositions()
    {
        coverPoints = new CoverPoint[coverPositions.Length];
        for(int i = coverPoints.Length - 1; i >= 0; i--)
        {
            coverPoints[i].relativePos = coverPositions[i];
        }
    }
    void DrawRaysForCoverPositions()
    {
        for (int i = coverPoints.Length - 1; i >= 0; i--)
        {
            Debug.DrawRay(transform.TransformPoint(coverPoints[i].relativePos),
                (new Vector3(0.5f, 0, 0)), Color.magenta);
            Debug.DrawRay(transform.TransformPoint(coverPoints[i].relativePos),
                (new Vector3(-0.5f, 0, 0)), Color.magenta);
            Debug.DrawRay(transform.TransformPoint(coverPoints[i].relativePos),
                (new Vector3(0, 0, 0.5f)), Color.magenta);
            Debug.DrawRay(transform.TransformPoint(coverPoints[i].relativePos),
                (new Vector3(0, 0, -0.5f)), Color.magenta);
            Debug.DrawRay(transform.TransformPoint(coverPoints[i].relativePos),
                (new Vector3(0, 0.5f, 0)), Color.yellow);
        }
    }
    void DrawRaysForCoverPositions2()
    {
        Quaternion rayRotationY = Quaternion.AngleAxis(transform.eulerAngles.y, Vector3.up);
        for (int i = coverPoints.Length - 1; i >= 0; i--)
        {
            Debug.DrawRay(transform.TransformPoint(coverPoints[i].relativePos),
                rayRotationY*(new Vector3(0.5f, 0, 0)), Color.magenta);
            Debug.DrawRay(transform.TransformPoint(coverPoints[i].relativePos),
                rayRotationY*(new Vector3(-0.5f, 0, 0)), Color.magenta);
            Debug.DrawRay(transform.TransformPoint(coverPoints[i].relativePos),
                rayRotationY*(new Vector3(0, 0, 0.5f)), Color.magenta);
            Debug.DrawRay(transform.TransformPoint(coverPoints[i].relativePos),
                rayRotationY*(new Vector3(0, 0, -0.5f)), Color.magenta);
            Debug.DrawRay(transform.TransformPoint(coverPoints[i].relativePos),
                (new Vector3(0, 0.5f, 0)), Color.yellow);
        }

    }
}
public class CoverObjectsManager : MonoBehaviour
{
    public static CoverPoint[] coverPointsOfWorld = new CoverPoint[0];

    public static void AddCoverPointsToList(CoverPoint[] cPoints)
    {
        CoverPoint[] holderArray = new CoverPoint[coverPointsOfWorld.Length];
        for(int i = holderArray.Length-1; i >= 0; i--)
        {
            holderArray[i] = coverPointsOfWorld[i];
        }
        int lengthAll = coverPointsOfWorld.Length, lengthForAdd = cPoints.Length;
        coverPointsOfWorld = new CoverPoint[lengthAll + lengthForAdd];

        for (int currentIndex = 0; currentIndex < lengthAll + lengthForAdd - 1; currentIndex ++){
            if(currentIndex <= lengthAll - 1)
            {
                coverPointsOfWorld[currentIndex] = holderArray[currentIndex];
            }
            else
            {
                coverPointsOfWorld[currentIndex] = cPoints[currentIndex - lengthAll];
            }
        }
    }
}

public struct CoverPoint
{
    public Vector3 relativePos;
    public float visibleAngle;
    public bool crouchOrPeek;
    public bool isCoveredAlready;
}

