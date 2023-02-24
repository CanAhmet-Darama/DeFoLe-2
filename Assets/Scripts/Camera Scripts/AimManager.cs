using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class AimManager : MonoBehaviour
{
    CameraScript mainCam;
    MainCharacter mainChar;
    [SerializeField] Animator animator;
    [SerializeField] Transform aimTarget;
    [SerializeField] MultiAimConstraint multiAimCoRHand;
    [SerializeField] MultiAimConstraint multiAimCoBody;
    [SerializeField] TwoBoneIKConstraint leftHCo;




    bool targetHit;
    float aimCastDistance = 500;
    RaycastHit hitInfo;

    bool quitAimingCompletely = false;
    public bool isReloading;
    Coroutine willQuitAimingCompletely;

    float lerpOrSnapSpeed = 0.025f;


    public void Start()
    {
        mainCam = GameManager.mainCam.GetComponent<CameraScript>();
        mainChar = GameManager.mainChar.GetComponent<MainCharacter>();
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
            animator.SetBool("isAiming", true);
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

            multiAimCoBody.weight = GameManager.LerpOrSnap(multiAimCoBody.weight, 0.7f, lerpOrSnapSpeed);
            if (!isReloading) { 
                leftHCo.weight = GameManager.LerpOrSnap(leftHCo.weight, 1, lerpOrSnapSpeed);
                multiAimCoRHand.weight = GameManager.LerpOrSnap(multiAimCoRHand.weight, 1, lerpOrSnapSpeed);
            }
            else
            {
                leftHCo.weight = GameManager.LerpOrSnap(leftHCo.weight, 0, lerpOrSnapSpeed);
                multiAimCoRHand.weight = GameManager.LerpOrSnap(multiAimCoRHand.weight, 0, lerpOrSnapSpeed);
            }

        }
        else
        {
            if (CameraScript.mouseMoved || mainChar.animStateSpeed != AnimStateSpeed.idle)
            {
                if(willQuitAimingCompletely != null)
                {
                    StopCoroutine(willQuitAimingCompletely);
                    quitAimingCompletely = true;
                    animator.SetBool("isAiming", false);
                }
            }
        }

        if (Input.GetMouseButtonDown(0) && mainChar.canShoot)
        {
            animator.ResetTrigger("fire");
            animator.SetTrigger("fire");
        }
        if(Input.GetKeyDown(KeyCode.R))
        {
            animator.SetTrigger("reload");
            StartCoroutine(ReloadConstraintWeightLower(mainChar.currentWeapon.GetComponent<GeneralWeapon>().reloadTime));
        }

        if (quitAimingCompletely)
        {
            multiAimCoBody.weight = GameManager.LerpOrSnap(multiAimCoBody.weight, 0, lerpOrSnapSpeed);
            multiAimCoRHand.weight = GameManager.LerpOrSnap(multiAimCoRHand.weight, 0, lerpOrSnapSpeed);
            leftHCo.weight = GameManager.LerpOrSnap(leftHCo.weight, 0, lerpOrSnapSpeed);

        }

    }



    IEnumerator WaitToQuitAimingCompletely(float durat)
    {
        yield return new WaitForSeconds(durat);
        quitAimingCompletely = true;
        animator.SetBool("isAiming", false);
    }
    IEnumerator ReloadConstraintWeightLower(float durat)
    {
        isReloading = true;
        yield return new WaitForSeconds(durat);
        isReloading = false;
    }
}
