using UnityEngine;

public class LevelOfDetailManager : MonoBehaviour
{
    #region Variables
    public static Vector3 mainCamPos;
    public bool objectOrEnemy = true;
    public float detailDiminishDistance;
    static public float detailDistanceMultiplier = 1;
    public float sqrDistancePlayer;
    public bool deactivateMeshOnLongDistance;
    public float longDistance;
    float perspectiveAngle;


    public Renderer[] meshesDetailed;
    public Renderer[] meshesNotDetailed;

    [Header("EXPERIMENT")]
    public bool increaseQuality;

    [Header("State Definers")]
    public MeshDetailLevel detailLevel;
    public DistanceInterval distanceInterval;

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
        sqrDistancePlayer = GameManager.SqrDistance(mainCamPos, transform.position);
        sqrRangeInUse = detailDiminishDistance * detailDiminishDistance * detailDistanceMultiplier * detailDistanceMultiplier; 
        sqrDeactivateRangeInUse = longDistance * longDistance * detailDistanceMultiplier * detailDistanceMultiplier;

        DistanceIntervalSetter();
        CheckRequiredLOD();

        if (increaseQuality)
        {
            DetermineMeshLevel(MeshDetailLevel.highQ);
            increaseQuality = false;
        }
    }


    void CheckRequiredLOD()
    {
        Vector2 toTargetVector2D = new Vector2((transform.position - mainCamPos).x, (transform.position - mainCamPos).z);
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
        if (sqrDistancePlayer < sqrRangeInUse)
        {
            distanceInterval = DistanceInterval.veryClose;
        }
        else
        {
            if(deactivateMeshOnLongDistance && sqrDistancePlayer > sqrDeactivateRangeInUse)
            {
                distanceInterval = DistanceInterval.tooFar;
            }
            else
            {
                distanceInterval = DistanceInterval.notClose;
            }
        }
    }
}
public enum MeshDetailLevel { noMesh, lowQ, highQ }
public enum DistanceInterval { tooFar, notClose, veryClose }