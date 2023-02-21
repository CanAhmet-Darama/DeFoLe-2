using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class AimManager : MonoBehaviour
{
    CameraScript mainCam;
    MainCharacter mainChar;
    Animator animator;
    [SerializeField]Transform aimTarget;
    [SerializeField] MultiAimConstraint multiAimCoRHand;
    [SerializeField] MultiAimConstraint multiAimCoBody;
    [SerializeField] TwoBoneIKConstraint leftHCo;




    bool targetHit;
    float aimCastDistance = 500;
    RaycastHit hitInfo;

    bool quitAimingCompletely = false;
    Coroutine willQuitAimingCompletely;


    public void Start()
    {
        mainCam = GameManager.mainCam.GetComponent<CameraScript>();
        mainChar = GameManager.mainChar.GetComponent<MainCharacter>();
        animator = mainChar.GetComponent<Animator>();
    }

    void Update()
    {
        AimTargetPositioner();
    }

    void AimTargetPositioner()
    {

        if (Input.GetMouseButtonUp(1))
        {
            willQuitAimingCompletely = StartCoroutine(WaitToQuitAimingCompletely(1));
        }
        if (Input.GetMouseButtonDown(1))
        {
            if(willQuitAimingCompletely != null)
            {
                StopCoroutine(willQuitAimingCompletely);
            }
            quitAimingCompletely = false;
        }
        if (Input.GetMouseButton(1))
        {
            #region Raycasting for aiming
            Ray ray = mainCam.GetComponent<Camera>().ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            targetHit = Physics.Raycast(ray, out hitInfo, aimCastDistance, ~0, QueryTriggerInteraction.Ignore);
            if (targetHit && hitInfo.distance > 3)
            {
                aimTarget.position = GameManager.LerpOrSnap(aimTarget.position, hitInfo.point, 0.03f);
            }
            else
            {
                aimTarget.localPosition = GameManager.LerpOrSnap(aimTarget.localPosition, new Vector3(0, 0, 15), 0.03f);
            }
            #endregion

            multiAimCoBody.weight = GameManager.LerpOrSnap(multiAimCoBody.weight, 0.7f, 0.025f);
            multiAimCoRHand.weight = GameManager.LerpOrSnap(multiAimCoRHand.weight, 1, 0.025f);
            leftHCo.weight = GameManager.LerpOrSnap(leftHCo.weight, 1, 0.025f);
        }
        else
        {
            if (CameraScript.mouseMoved || mainChar.animStateSpeed != AnimStateSpeed.idle)
            {
                if(willQuitAimingCompletely != null)
                {
                    if (willQuitAimingCompletely != null)
                    {
                        StopCoroutine(willQuitAimingCompletely);
                    }
                    quitAimingCompletely = true;
                }
            }
        }

        if(quitAimingCompletely)
        {
            multiAimCoBody.weight = GameManager.LerpOrSnap(multiAimCoBody.weight, 0, 0.025f);
            multiAimCoRHand.weight = GameManager.LerpOrSnap(multiAimCoRHand.weight, 0, 0.025f);
            leftHCo.weight = GameManager.LerpOrSnap(leftHCo.weight, 0, 0.025f);

        }
    }

    IEnumerator WaitToQuitAimingCompletely(float durat)
    {
        yield return new WaitForSeconds(durat);
        quitAimingCompletely = true;
    }
}
