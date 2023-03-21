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
        CoverObjectsManager.AddCoverTakeableObjectToList(campNumber, this);
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
            coverPoints[i].owner = transform;
            coverPoints[i].worldPos = transform.TransformPoint(coverPoints[i].relativePos);
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

public struct CoverPoint
{
    public Transform owner;
    public Vector3 relativePos;
    public Vector3 worldPos;
    public float visibleAngle;
    public bool crouchOrPeek;
    public bool isCoveredAlready;
}

