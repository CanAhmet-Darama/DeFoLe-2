using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegionOptimizer : MonoBehaviour
{
    [Header("Collections")]
    public GameObject destroyableCollection;
    public GameObject interactableCollection;
    public GameObject enemiesCollection;
    [Header("Values")]
    public float sqrDistancePlayer;
    static float baseRange = 200;
    bool closeEnoughToPlayer = false;
    float sqrRangeInUse;

    CameraScript mainCamScr;

    float perspectiveAngle;


    void Start()
    {
        mainCamScr = GameManager.mainCam.GetComponent<CameraScript>();
        StartCoroutine(DeactivatingFarObjectsCoroutine());
    }

    void Update()
    {
        sqrDistancePlayer = GameManager.SqrDistance(GameManager.mainCam.position, transform.position);
        sqrRangeInUse = baseRange * baseRange * LevelOfDetailManager.detailDistanceMultiplier * LevelOfDetailManager.detailDistanceMultiplier;

        CheckRegion();
    }

    void CheckRegion()
    {
        //Vector2 toTargetVector2D = new Vector2((transform.position - mainCam.position).x, (transform.position - mainCam.position).z);
        //float AngleBetweenCam = Vector2.Angle(toTargetVector2D, LevelOfDetailManager.camForward2D);

        if (!closeEnoughToPlayer && (sqrDistancePlayer < sqrRangeInUse))
        {
            closeEnoughToPlayer = true;
            destroyableCollection.SetActive(true);
            interactableCollection.SetActive(true);
            enemiesCollection.SetActive(true);
            Destroy(this);
        }
        //else if (closeEnoughToPlayer && sqrDistancePlayer > sqrRangeInUse*2)
        //{
        //    closeEnoughToPlayer = false;
        //    destroyableCollection.SetActive(false);
        //    interactableCollection.SetActive(false);
        //}

    }
    
    IEnumerator DeactivatingFarObjectsCoroutine()
    {
        yield return new WaitForSeconds(0.1f);
        destroyableCollection.SetActive(false);
        interactableCollection.SetActive(false);
        enemiesCollection.SetActive(false);
    }
}
