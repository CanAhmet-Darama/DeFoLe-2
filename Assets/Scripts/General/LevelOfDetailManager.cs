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
    public bool isDetailedNow = false;
    public bool isDeactivated = false;

    [Header("Enemy Detail")]
    public Transform mainCam;
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
        mainCam = GameManager.mainCam;
        mainCamScr = mainCam.GetComponent<CameraScript>();
    }


    void Update()
    {
        sqrDistancePlayer = GameManager.SqrDistance(mainCam.position, transform.position);
        sqrRangeInUse = detailDiminishDistance * detailDiminishDistance * detailDistanceMultiplier * detailDistanceMultiplier; 
        sqrDeactivateRangeInUse = longDistance * longDistance; 
        CheckRequiredLOD();
    }


    void CheckRequiredLOD()
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
            if(!isDeactivated)
            {
                DetermineMeshLevel(0);
            }
        }
        else
        {
            if(!isDeactivated && deactivateMeshOnLongDistance &&sqrDistancePlayer > sqrDeactivateRangeInUse)
            {
                DetermineMeshLevel(0);
            }
            else if((isDetailedNow || isDeactivated) && sqrDistancePlayer > sqrRangeInUse)
            {
                DetermineMeshLevel(1);
            }
            else if(!isDetailedNow && sqrDistancePlayer < sqrRangeInUse)
            {
                DetermineMeshLevel(2);
            }
        }

        if(!objectOrEnemy && !enemyActivated && sqrDistancePlayer < EnemyManager.enemyActivateRange * EnemyManager.enemyActivateRange)
        {
            EnemyManager.ActivateEnemy(enemyScr, true);
            enemyActivated = true;
        }

    }

    void DetermineMeshLevel(byte meshLevel)
    {
        switch (meshLevel)
        {
            case 0:
                isDeactivated = true;
                isDetailedNow = false;
                if (!objectOrEnemy)
                {
                    EnemyManager.UndetailEnemy(enemyScr, true);

                }
                for (int i = meshesDetailed.Length - 1; i >= 0; i--)
                {
                    if (i == meshesDetailed.Length - 1 && !meshesDetailed[i].gameObject.activeInHierarchy)
                    {
                        break;
                    }
                    if (meshesDetailed[i] != null)
                    {
                        meshesDetailed[i].enabled = false;
                    }
                }

                for (int i = meshesNotDetailed.Length - 1; i >= 0; i--)
                {
                    if (i == meshesNotDetailed.Length - 1 && !meshesNotDetailed[i].gameObject.activeInHierarchy)
                    {
                        break;
                    }
                    meshesNotDetailed[i].enabled = false;
                }

                break;
            case 1:
                isDeactivated = false;
                isDetailedNow = false;
                if (!objectOrEnemy)
                {
                    EnemyManager.UndetailEnemy(enemyScr, true);

                }
                for (int i = meshesDetailed.Length - 1; i >= 0; i--)
                {
                    if (i == meshesDetailed.Length - 1 && !meshesDetailed[i].gameObject.activeInHierarchy)
                    {
                        break;
                    }
                    meshesDetailed[i].enabled = false;
                }

                for (int i = meshesNotDetailed.Length - 1; i >= 0; i--)
                {
                    meshesNotDetailed[i].enabled = true;
                }

                break;
            case 2:
                isDeactivated = false;
                isDetailedNow = true;
                if (!objectOrEnemy)
                {
                    EnemyManager.UndetailEnemy(enemyScr, false);
                }
                for (int i = meshesDetailed.Length - 1; i >= 0; i--)
                {
                    if (i == meshesDetailed.Length - 1 && !meshesDetailed[i].gameObject.activeInHierarchy)
                    {
                        break;
                    }
                    meshesDetailed[i].enabled = true;
                }

                for (int i = meshesNotDetailed.Length - 1; i >= 0; i--)
                {
                    meshesNotDetailed[i].enabled = false;
                }
                break;
        }
    }

}
