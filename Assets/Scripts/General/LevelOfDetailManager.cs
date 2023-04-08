using System.Collections;
using System.Collections.Generic;
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

    
    public MeshRenderer[] meshesDetailed;
    public MeshRenderer[] meshesNotDetailed;
    public bool isDetailedNow = false;

    [Header("Enemy Detail")]
    public SkinnedMeshRenderer skinnedMesh;
    public Transform mainCam;
    public bool enemyActivated = false;
    public EnemyScript enemyScr;
    public CameraScript mainCamScr;

    #endregion

    void Start()
    {
        mainCam = GameManager.mainCam;
        mainCamScr = mainCam.GetComponent<CameraScript>();
        EnableOrDisableMeshes(false);
    }


    void Update()
    {
        sqrDistancePlayer = GameManager.SqrDistance(mainCam.position, transform.position);
        CheckRequiredLOD();
    }

    void CheckRequiredLOD()
    {
        Vector2 toTargetVector2D = new Vector2((transform.position - mainCam.position).x, (transform.position - mainCam.position).z);
        Vector2 camForward2D = new Vector2((mainCam.forward).x, (mainCam.forward).z);
        float AngleBetweenCam = Vector2.Angle(toTargetVector2D,camForward2D);

        float perspectiveAngle = 75;
        if (mainCamScr.camOwnState == CamState.zoomScope)
        {
            perspectiveAngle = 30;
        }

        if (AngleBetweenCam < perspectiveAngle)
        {
            if(deactivateMeshOnLongDistance && sqrDistancePlayer > longDistance * longDistance * detailDistanceMultiplier * detailDistanceMultiplier)
            {
                if (isDetailedNow)
                {
                    EnableOrDisableMeshes(true, true);
                    isDetailedNow = false;
                    if (!objectOrEnemy)
                    {
                        EnemyManager.UndetailEnemy(enemyScr, true);
                    }
                }
            }
            else
            {
                if (!isDetailedNow && sqrDistancePlayer < detailDiminishDistance*detailDiminishDistance * detailDistanceMultiplier * detailDistanceMultiplier)
                {
                    EnableOrDisableMeshes(true);
                    isDetailedNow = true;
                    if (!objectOrEnemy)
                    {
                        if (!enemyActivated)
                        {
                            EnemyManager.ActivateEnemy(enemyScr, true);
                            enemyActivated = true;
                        }
                        else
                        {
                            EnemyManager.UndetailEnemy(enemyScr, false);
                        }
                    }
                }
                else if(isDetailedNow && sqrDistancePlayer > detailDiminishDistance * detailDiminishDistance * detailDistanceMultiplier * detailDistanceMultiplier)
                {
                    EnableOrDisableMeshes(false);
                    isDetailedNow = false;
                    if(!objectOrEnemy)
                    {
                        EnemyManager.UndetailEnemy(enemyScr, true);
                    }
                }
            }

        }
        else if(sqrDistancePlayer > 25)
        {
            if (isDetailedNow)
            {
                EnableOrDisableMeshes(true, true);
                isDetailedNow = false;
                if (!objectOrEnemy)
                {
                    EnemyManager.UndetailEnemy(enemyScr, true);
                }
            }
        }

    }
    void EnableOrDisableMeshes(bool enableDetail, bool disableAll = false)
    {
        if (!disableAll)
        {
            if (objectOrEnemy)
            {
                for (int i = meshesDetailed.Length - 1; i >= 0; i--)
                {
                    if(i == meshesDetailed.Length - 1 && !meshesDetailed[i].gameObject.activeInHierarchy)
                    {
                        break;
                    }
                    meshesDetailed[i].enabled = enableDetail;
                }
            }
            else
            {
                skinnedMesh.enabled = enableDetail;
            }

            for (int i = meshesNotDetailed.Length - 1; i >= 0; i--)
            {
                meshesNotDetailed[i].enabled = !enableDetail;
            }
        }
        else
        {
            if (objectOrEnemy)
            {
                for (int i = meshesDetailed.Length - 1; i >= 0; i--)
                {
                    meshesDetailed[i].enabled = false;
                }
            }
            else
            {
                skinnedMesh.enabled = false;
            }

            for (int i = meshesNotDetailed.Length - 1; i >= 0; i--)
            {
                meshesNotDetailed[i].enabled = false;
            }

        }
    }
}
