using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelOfDetailManager : MonoBehaviour
{
    #region Variables
    public bool objectOrEnemy = true;
    public float detailDiminishDistance;
    public float sqrDistancePlayer;

    
    public MeshRenderer[] meshesDetailed;
    public MeshRenderer[] meshesNotDetailed;
    public bool isDetailedNow = false;

    [Header("Enemy Detail")]
    public SkinnedMeshRenderer skinnedMesh;
    public Transform mainCam;
    public bool enemyActivated = false;
    public EnemyScript enemyScr;

    #endregion

    void Start()
    {
        mainCam = GameManager.mainCam;
        EnableOrDisableMeshes(false);
    }


    void Update()
    {
        sqrDistancePlayer = GameManager.SqrDistance(mainCam.position, transform.position);
        CheckRequiredLOD();
    }

    void CheckRequiredLOD()
    {
        if(!isDetailedNow && sqrDistancePlayer < detailDiminishDistance*detailDiminishDistance)
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
        else if(isDetailedNow && sqrDistancePlayer > detailDiminishDistance * detailDiminishDistance)
        {
            EnableOrDisableMeshes(false);
            isDetailedNow = false;
            if(!objectOrEnemy)
            {
                EnemyManager.UndetailEnemy(enemyScr, true);
            }
        }
    }
    void EnableOrDisableMeshes(bool enableDetail)
    {
        if (objectOrEnemy)
        {
            for (int i = meshesDetailed.Length - 1; i >= 0; i--)
            {
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
}