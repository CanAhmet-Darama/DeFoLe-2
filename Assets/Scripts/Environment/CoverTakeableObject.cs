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
        if (Input.GetKeyDown(KeyCode.G))
        {
            SortPointsByDistance(GameManager.mainChar.position);
        }
    }
    void SetCoverPointPositions()
    {
        coverPoints = new CoverPoint[coverPositions.Length];
        for(int i = coverPoints.Length - 1; i >= 0; i--)
        {
            coverPoints[i].relativePos = coverPositions[i];
            coverPoints[i].owner = transform;
            coverPoints[i].worldPos = transform.TransformPoint(coverPoints[i].relativePos);
            coverPoints[i].isCoveredAlready = false;
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
    void SortPointsByDistance(Vector3 referencePos)
    {
        bool hasSwapped = true;
        CoverPoint holderCoverObj;
        while (hasSwapped)
        {
            hasSwapped = false;
            for (short index = 0, limit = (short)(coverPoints.Length - 1); index < limit; index++)
            {
                if ((coverPoints[index].worldPos - referencePos).sqrMagnitude >
                    (coverPoints[index + 1].worldPos - referencePos).sqrMagnitude)
                {
                    holderCoverObj = coverPoints[index];
                    coverPoints[index] = coverPoints[index + 1];
                    coverPoints[index + 1] = holderCoverObj;
                    hasSwapped = true;
                }
            }
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
