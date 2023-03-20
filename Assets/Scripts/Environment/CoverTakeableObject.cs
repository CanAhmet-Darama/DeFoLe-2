using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoverTakeableObject : MonoBehaviour
{
    public CoverPoint[] coverPoints;
    public Vector3[] coverPositions;
    public byte campNumber;

    void Start()
    {
        SetCoverPointPositions();
        CoverObjectsManager.AddCoverPointsToList(campNumber,coverPoints);
    }
    void Update()
    {
        DrawRaysForCoverPositions();
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
    public static CoverPoint[][] coverPointsOfWorld = new CoverPoint[GameManager.numberOfCamps][];

    public static void AddCoverPointsToList(byte campNumber,CoverPoint[] cPoints)
    {
        if(coverPointsOfWorld[campNumber - 1] == null)
        {
            coverPointsOfWorld[campNumber - 1] = new CoverPoint[0];
        }
        CoverPoint[] holderArray = new CoverPoint[coverPointsOfWorld[campNumber - 1].Length];

        for (int i = holderArray.Length-1; i >= 0; i--)
        {
            holderArray[i] = coverPointsOfWorld[campNumber-1][i];
        }
        int lengthAll = coverPointsOfWorld[campNumber-1].Length, lengthForAdd = cPoints.Length;
        coverPointsOfWorld[campNumber - 1] = new CoverPoint[lengthAll + lengthForAdd];

        for (int currentIndex = 0; currentIndex < lengthAll + lengthForAdd - 1; currentIndex ++){
            if(currentIndex <= lengthAll - 1)
            {
                coverPointsOfWorld[campNumber - 1 ][currentIndex] = holderArray[currentIndex];
            }
            else
            {
                coverPointsOfWorld[campNumber - 1][currentIndex] = cPoints[currentIndex - lengthAll];
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

