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
    float perspectiveAngle;


    public MeshRenderer[] meshesDetailed;
    public MeshRenderer[] meshesNotDetailed;
    public bool isDetailedNow = false;
    public bool isDeactivated = false;

    [Header("Enemy Detail")]
    public SkinnedMeshRenderer skinnedMesh;
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
        sqrDeactivateRangeInUse = longDistance * longDistance * detailDistanceMultiplier * detailDistanceMultiplier; 
        CheckRequiredLOD();
    }

    /*void CheckRequiredLOD()
    {
        Vector2 toTargetVector2D = new Vector2((transform.position - mainCam.position).x, (transform.position - mainCam.position).z);
        Vector2 camForward2D = new Vector2(mainCam.forward.x, mainCam.forward.z);
        float AngleBetweenCam = Vector2.Angle(toTargetVector2D, camForward2D);
        if(gameObject.name == "Metal Cube (14)")
        {
            //Debug.Log(AngleBetweenCam + " _ " + Mathf.Sqrt(sqrDistancePlayer));
            //Debug.DrawLine(mainCam.position, transform.position);
        }

        float perspectiveAngle = 75;
        if (mainCamScr.camOwnState == CamState.zoomScope)
        {
            perspectiveAngle = 30;
        }

        if (AngleBetweenCam < perspectiveAngle)
        {
            if(deactivateMeshOnLongDistance && sqrDistancePlayer > longDistance * longDistance * detailDistanceMultiplier * detailDistanceMultiplier)
            {
                if(!isDeactivated)
                {
                    EnableOrDisableMeshes(true, true);
                    isDeactivated = true;
                    if (!objectOrEnemy)
                    {
                        EnemyManager.UndetailEnemy(enemyScr, true);
                    }
                    if (gameObject.name == "Metal Cube (14)")
                    {
                        Debug.Log("DEACTIVATED");
                    }
                }
            }
            else if(!deactivateMeshOnLongDistance)
            {
                if (!isDetailedNow && sqrDistancePlayer < detailDiminishDistance*detailDiminishDistance * detailDistanceMultiplier * detailDistanceMultiplier)
                {
                    EnableOrDisableMeshes(true);
                    isDetailedNow = true;
                    isDeactivated = false;
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
                    if (gameObject.name == "Metal Cube (14)")
                    {
                        Debug.Log("DETAILED");
                    }

                }
                else if(isDetailedNow && sqrDistancePlayer > detailDiminishDistance * detailDiminishDistance * detailDistanceMultiplier * detailDistanceMultiplier)
                {
                    EnableOrDisableMeshes(false);
                    isDetailedNow = false;
                    isDeactivated = false;
                    if (!objectOrEnemy)
                    {
                        EnemyManager.UndetailEnemy(enemyScr, true);
                    }
                    if (gameObject.name == "Metal Cube (14)")
                    {
                        Debug.Log("NOT DETAILED");
                    }

                }
            }

        }
        else if(sqrDistancePlayer > 64)
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

    }*/

    void CheckRequiredLOD()
    {
        Vector2 toTargetVector2D = new Vector2((transform.position - mainCam.position).x, (transform.position - mainCam.position).z);
        float AngleBetweenCam = Vector2.Angle(toTargetVector2D, camForward2D);

        perspectiveAngle = 80;
        if(mainCamScr.camOwnState == CamState.zoomScope)
        {
            perspectiveAngle = 30;
        }

        if(AngleBetweenCam > perspectiveAngle && sqrDistancePlayer > 225)
        {
            if(!isDeactivated)
            {
                DetermineMeshLevel(0);
            }
        }
        else
        {
            if(deactivateMeshOnLongDistance &&sqrDistancePlayer > sqrDeactivateRangeInUse)
            {
                if (!isDeactivated)
                {
                    DetermineMeshLevel(0);
                }
            }
            else if( sqrDistancePlayer > sqrRangeInUse)
            {
                if (isDetailedNow || isDeactivated)
                {
                    DetermineMeshLevel(1);
                }
            }
            else
            {
                if (!isDetailedNow || isDeactivated)
                {
                    DetermineMeshLevel(2);
                }
            }
        }
    }

    void DetermineMeshLevel(byte meshLevel)
    {
        switch (meshLevel)
        {
            case 0:
                isDeactivated = true;
                isDetailedNow = false;
                if (objectOrEnemy)
                {
                    for (int i = meshesDetailed.Length - 1; i >= 0; i--)
                    {
                        if (i == meshesDetailed.Length - 1 && !meshesDetailed[i].gameObject.activeInHierarchy)
                        {
                            break;
                        }
                        meshesDetailed[i].enabled = false;
                    }
                }
                else
                {
                    skinnedMesh.enabled = false;
                    EnemyManager.UndetailEnemy(enemyScr, true);
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
                if (objectOrEnemy)
                {
                    for (int i = meshesDetailed.Length - 1; i >= 0; i--)
                    {
                        if (i == meshesDetailed.Length - 1 && !meshesDetailed[i].gameObject.activeInHierarchy)
                        {
                            break;
                        }
                        meshesDetailed[i].enabled = false;
                    }
                }
                else
                {
                    skinnedMesh.enabled = false;
                    EnemyManager.UndetailEnemy(enemyScr, true);
                }

                for (int i = meshesNotDetailed.Length - 1; i >= 0; i--)
                {
                    meshesNotDetailed[i].enabled = true;
                }

                break;
            case 2:
                isDeactivated = false;
                isDetailedNow = true;
                if (objectOrEnemy)
                {
                    for (int i = meshesDetailed.Length - 1; i >= 0; i--)
                    {
                        if (i == meshesDetailed.Length - 1 && !meshesDetailed[i].gameObject.activeInHierarchy)
                        {
                            break;
                        }
                        meshesDetailed[i].enabled = true;
                    }
                }
                else
                {
                    skinnedMesh.enabled = true;
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

                for (int i = meshesNotDetailed.Length - 1; i >= 0; i--)
                {
                    meshesNotDetailed[i].enabled = false;
                }
                break;
        }
    }

    /*void EnableOrDisableMeshes(bool enableDetail, bool disableAll = false)
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
    }*/
}
