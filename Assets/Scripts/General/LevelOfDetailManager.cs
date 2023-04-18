using UnityEngine;

public class LevelOfDetailManager : MonoBehaviour
{
    #region Variables
    public bool objectOrEnemy = true;
    public float detailDiminishDistance;
    static public float detailDistanceMultiplier = 1;
    public float sqrDistancePlayer;
    public bool deactivateMeshOnLongDistance;
    public float longDistance;
    float perspectiveAngle;


    public Renderer[] meshesDetailed;
    public Renderer[] meshesNotDetailed;

    [Header("State Definers")]
    public MeshDetailLevel detailLevel;
    public DistanceInterval distanceInterval;
    //public bool isDetailedNow = false;
    //public bool isDeactivated = false;

    [Header("Enemy Detail")]
    public bool enemyActivated = false;
    public EnemyScript enemyScr;
    public CameraScript mainCamScr;

    [Header("Various")]
    public static Vector2 camForward2D;
    float sqrRangeInUse;
    float sqrDeactivateRangeInUse;

    #endregion

    void Start()
    {
        mainCamScr = GameManager.mainCam.GetComponent<CameraScript>();

        DetermineMeshLevel(MeshDetailLevel.highQ);
    }


    void Update()
    {
        sqrDistancePlayer = GameManager.SqrDistance(GameManager.mainCam.position, transform.position);
        sqrRangeInUse = detailDiminishDistance * detailDiminishDistance * detailDistanceMultiplier * detailDistanceMultiplier; 
        sqrDeactivateRangeInUse = longDistance * longDistance;


        DistanceIntervalSetter();
        CheckRequiredLOD();
        if(sqrDistancePlayer < sqrRangeInUse && distanceInterval == DistanceInterval.notClose)
        {
            Debug.Log("Cam : " + GameManager.mainCam.position + ", Distance : " + sqrDistancePlayer);
        }
    }


    /*void CheckRequiredLOD()
    {
        Vector2 toTargetVector2D = new Vector2((transform.position - mainCam.position).x, (transform.position - mainCam.position).z);
        float AngleBetweenCam = Vector2.Angle(toTargetVector2D, camForward2D);

        perspectiveAngle = 80;
        if(mainCamScr.camOwnState == CamState.zoomScope)
        {
            perspectiveAngle = 30;
        }

        if(AngleBetweenCam > perspectiveAngle && sqrDistancePlayer > 400)
        {
            if(detailLevel != MeshDetailLevel.noMesh && previousDetailLevel != MeshDetailLevel.noMesh)
            {
                DetermineMeshLevel(MeshDetailLevel.noMesh);
            }
        }
        else
        {
            if (deactivateMeshOnLongDistance && distanceInterval == DistanceInterval.tooFar)
            {
                if (detailLevel != MeshDetailLevel.noMesh && previousDetailLevel != MeshDetailLevel.noMesh)
                {
                    DetermineMeshLevel(MeshDetailLevel.noMesh);
                }
            }
            else if (detailLevel != MeshDetailLevel.lowQ && previousDetailLevel != MeshDetailLevel.lowQ && distanceInterval == DistanceInterval.notClose)
            {
                DetermineMeshLevel(MeshDetailLevel.lowQ);
            }
            else if (detailLevel != MeshDetailLevel.highQ && previousDetailLevel != MeshDetailLevel.highQ && distanceInterval == DistanceInterval.veryClose)
            {
                DetermineMeshLevel(MeshDetailLevel.highQ);
            }
            //if(deactivateMeshOnLongDistance && detailLevel != MeshDetailLevel.noMesh && previousDetailLevel != MeshDetailLevel.noMesh && distanceInterval == DistanceInterval.tooFar)
            //{
            //    DetermineMeshLevel(MeshDetailLevel.noMesh);
            //}
            //else if(detailLevel != MeshDetailLevel.lowQ && previousDetailLevel != MeshDetailLevel.lowQ && distanceInterval == DistanceInterval.notClose)
            //{
            //    DetermineMeshLevel(MeshDetailLevel.lowQ);
            //}
            //else if(detailLevel != MeshDetailLevel.highQ && previousDetailLevel != MeshDetailLevel.highQ && distanceInterval == DistanceInterval.veryClose)
            //{
            //    DetermineMeshLevel(MeshDetailLevel.highQ);
            //}
        }

        if (!objectOrEnemy && !enemyActivated && sqrDistancePlayer < EnemyManager.enemyActivateRange * EnemyManager.enemyActivateRange)
        {
            EnemyManager.ActivateEnemy(enemyScr, true);
            enemyActivated = true;
        }

        previousDetailLevel = detailLevel;
    }*/
    void CheckRequiredLOD()
    {
        Vector2 toTargetVector2D = new Vector2((transform.position - GameManager.mainCam.position).x, (transform.position - GameManager.mainCam.position).z);
        float AngleBetweenCam = Vector2.Angle(toTargetVector2D, camForward2D);

        perspectiveAngle = 80;
        if(mainCamScr.camOwnState == CamState.zoomScope)
        {
            perspectiveAngle = 30;
        }

        if(AngleBetweenCam > perspectiveAngle && sqrDistancePlayer > 400)
        {
            DetermineMeshLevel(MeshDetailLevel.noMesh);
        }
        else
        {
            if (distanceInterval == DistanceInterval.veryClose)
            {
                DetermineMeshLevel(MeshDetailLevel.highQ);
            }
            else if (distanceInterval == DistanceInterval.notClose)
            {
                DetermineMeshLevel(MeshDetailLevel.lowQ);
            }
            else
            {
                DetermineMeshLevel(MeshDetailLevel.noMesh);
            }
        }

        if (!objectOrEnemy && !enemyActivated && sqrDistancePlayer < EnemyManager.enemyActivateRange * EnemyManager.enemyActivateRange)
        {
            EnemyManager.ActivateEnemy(enemyScr, true);
            enemyActivated = true;
        }
    }

    void DetermineMeshLevel(MeshDetailLevel level)
    {
        bool lowMeshEnable = false;
        bool highMeshEnable = false;
        switch(level)
        {
            case MeshDetailLevel.noMesh:
            {
                detailLevel = MeshDetailLevel.noMesh;
                break;
            }
            case MeshDetailLevel.lowQ:
            {
                lowMeshEnable = true;
                highMeshEnable = false;
                detailLevel = MeshDetailLevel.lowQ;
                break;
            }
            case MeshDetailLevel.highQ:
            {
                lowMeshEnable = false;
                highMeshEnable = true;
                detailLevel = MeshDetailLevel.highQ;
                break;
            }
        }
        for (int i = meshesNotDetailed.Length - 1; i >= 0; i--)
        {
            if (meshesNotDetailed[i] != null)
            {
                if (i == meshesNotDetailed.Length-1 && meshesNotDetailed[i].enabled == lowMeshEnable)
                {
                    break;
                }

                meshesNotDetailed[i].enabled = lowMeshEnable;
            }
        }
        for (int i = meshesDetailed.Length - 1; i >= 0; i--)
        {
            if (meshesDetailed[i] != null)
            {
                if (i == meshesDetailed.Length - 1 && meshesDetailed[i].enabled == highMeshEnable)
                {
                    break;
                }

                meshesDetailed[i].enabled = highMeshEnable;
            }
        }

    }
    void DistanceIntervalSetter()
    {
        if(deactivateMeshOnLongDistance && sqrDistancePlayer > sqrDeactivateRangeInUse)
        {
            distanceInterval = DistanceInterval.tooFar;
        }
        else if(sqrDistancePlayer > sqrRangeInUse)
        {
            distanceInterval = DistanceInterval.notClose;
        }
        else
        {
            distanceInterval = DistanceInterval.veryClose;
        }
    }

    //void DetermineMeshLevel(byte meshLevel)
    //{
    //    switch (meshLevel)
    //    {
    //        case 0:
    //            isDeactivated = true;
    //            isDetailedNow = false;
    //            if (!objectOrEnemy)
    //            {
    //                EnemyManager.UndetailEnemy(enemyScr, true);

    //            }
    //            for (int i = meshesDetailed.Length - 1; i >= 0; i--)
    //            {
    //                //if (i == meshesDetailed.Length - 1 && !meshesDetailed[i].gameObject.activeInHierarchy)
    //                //{
    //                //    break;
    //                //}
    //                if (meshesDetailed[i] != null)
    //                {
    //                    meshesDetailed[i].enabled = false;
    //                }
    //            }

    //            for (int i = meshesNotDetailed.Length - 1; i >= 0; i--)
    //            {
    //                //if (i == meshesNotDetailed.Length - 1 && !meshesNotDetailed[i].gameObject.activeInHierarchy)
    //                //{
    //                //    break;
    //                //}
    //                if (meshesNotDetailed[i] != null)
    //                    meshesNotDetailed[i].enabled = false;
    //            }

    //            break;
    //        case 1:
    //            isDeactivated = false;
    //            isDetailedNow = false;
    //            if (!objectOrEnemy)
    //            {
    //                EnemyManager.UndetailEnemy(enemyScr, true);

    //            }
    //            for (int i = meshesDetailed.Length - 1; i >= 0; i--)
    //            {
    //                //if (i == meshesDetailed.Length - 1 && !meshesDetailed[i].gameObject.activeInHierarchy)
    //                //{
    //                //    break;
    //                //}
    //                if (meshesDetailed[i] != null)
    //                    meshesDetailed[i].enabled = false;
    //            }

    //            for (int i = meshesNotDetailed.Length - 1; i >= 0; i--)
    //            {
    //                if (meshesNotDetailed[i] != null)
    //                    meshesNotDetailed[i].enabled = true;
    //            }

    //            break;
    //        case 2:
    //            isDeactivated = false;
    //            isDetailedNow = true;
    //            if (!objectOrEnemy)
    //            {
    //                EnemyManager.UndetailEnemy(enemyScr, false);
    //            }
    //            for (int i = meshesDetailed.Length - 1; i >= 0; i--)
    //            {
    //                //if (i == meshesDetailed.Length - 1 && !meshesDetailed[i].gameObject.activeInHierarchy)
    //                //{
    //                //    break;
    //                //}
    //                if(meshesDetailed[i] != null)
    //                    meshesDetailed[i].enabled = true;
    //            }

    //            for (int i = meshesNotDetailed.Length - 1; i >= 0; i--)
    //            {
    //                if(meshesNotDetailed[i] != null)
    //                    meshesNotDetailed[i].enabled = false;
    //            }
    //            break;
    //    }
    //}

}
public enum MeshDetailLevel { noMesh, lowQ, highQ }
public enum DistanceInterval { tooFar, notClose, veryClose }