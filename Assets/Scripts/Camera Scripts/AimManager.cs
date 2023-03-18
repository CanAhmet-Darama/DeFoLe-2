using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class AimManager : MonoBehaviour
{
    CameraScript mainCam;
    MainCharacter mainChar;
    Animator animator;
    Transform aimTarget;
    MultiAimConstraint multiAimCoRHand;
    MultiAimConstraint multiAimCoLHand;
    MultiAimConstraint multiAimCoBody;
    TwoBoneIKConstraint rightHCoTBIK;
    TwoBoneIKConstraint leftHCoTBIK;




    bool targetHit;
    float aimCastDistance = 500;
    RaycastHit hitInfo;

    bool quitAimingCompletely;
    public bool isReloading;
    Coroutine willQuitAimingCompletely;

    float lerpOrSnapSpeed = 0.025f;


    public void Start()
    {
        mainCam = GameManager.mainCam.GetComponent<CameraScript>();
        mainChar = GameManager.mainChar.GetComponent<MainCharacter>();
        quitAimingCompletely = true;
        #region Assign Constraints
        animator = mainChar.animator;
        aimTarget = mainChar.aimTarget.transform;
        multiAimCoRHand = mainChar.rightHandConstraint;
        multiAimCoLHand = mainChar.leftHandConstraint;
        multiAimCoBody = mainChar.multiAimCoBody;
        rightHCoTBIK = mainChar.rightHandTBIK;
        leftHCoTBIK = mainChar.leftHandTBIK;
        #endregion
    }

    void Update()
    {
        if(GameManager.mainState == PlayerState.onFoot)
        AimTargetPositioner();
    }

    void AimTargetPositioner()
    {
        if(mainChar.weaponState == GeneralCharacter.WeaponState.ranged)
        {
            isReloading = mainChar.isReloading;
            RightClickedOrNotManage(0.7f);

            if (quitAimingCompletely)
            {
                multiAimCoBody.weight = GameManager.LerpOrSnap(multiAimCoBody.weight, 0, lerpOrSnapSpeed);
                if(mainChar.currentWeapon.weaponType != WeaponType.SR_1)
                {
                    multiAimCoRHand.weight = GameManager.LerpOrSnap(multiAimCoRHand.weight, 0, lerpOrSnapSpeed);
                }
                else
                {
                    multiAimCoLHand.weight = GameManager.LerpOrSnap(multiAimCoLHand.weight, 0, lerpOrSnapSpeed);
                }
            }

        }
        else// if (mainChar.weaponState != GeneralCharacter.WeaponState.ranged)
        {
            RightClickedOrNotManage(0.8f);
            if (quitAimingCompletely)
            {
                multiAimCoBody.weight = GameManager.LerpOrSnap(multiAimCoBody.weight, 0, lerpOrSnapSpeed);
            }
        }

    }
    void RightClickedOrNotManage(float bodyTar)
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

                multiAimCoBody.weight = GameManager.LerpOrSnap(multiAimCoBody.weight, bodyTar, lerpOrSnapSpeed);
                if(mainChar.weaponState == GeneralCharacter.WeaponState.ranged)
                {
                    if (!isReloading) {
                        if (mainChar.currentWeapon.weaponType != WeaponType.SR_1)
                        {
                            multiAimCoRHand.weight = GameManager.LerpOrSnap(multiAimCoRHand.weight, 1, lerpOrSnapSpeed);
                            leftHCoTBIK.weight = GameManager.LerpOrSnap(leftHCoTBIK.weight, 1, lerpOrSnapSpeed);
                        }
                        else
                        {
                            multiAimCoLHand.weight = GameManager.LerpOrSnap(multiAimCoLHand.weight, 1, lerpOrSnapSpeed);
                            if (!mainChar.isShooting) {
                                rightHCoTBIK.weight = GameManager.LerpOrSnap(rightHCoTBIK.weight, 1, lerpOrSnapSpeed);}
                        }

                    }
                    else
                    {
                        if (mainChar.currentWeapon.weaponType != WeaponType.SR_1)
                        {
                            multiAimCoRHand.weight = GameManager.LerpOrSnap(multiAimCoRHand.weight, 1, lerpOrSnapSpeed);
                            leftHCoTBIK.weight = 0;
                        }
                        else
                        {
                            multiAimCoLHand.weight = GameManager.LerpOrSnap(multiAimCoLHand.weight, 1, lerpOrSnapSpeed);
                            rightHCoTBIK.weight = 0;
                        }

                    }
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

    }
    public static void ResetWeights(GeneralCharacter character)
    {
        character.rightHandConstraint.weight = 0;
        character.leftHandConstraint.weight = 0;
        character.multiAimCoBody.weight = 0;
        character.rightHandTBIK.weight = 0;
        character.leftHandTBIK.weight = 0;
    }


    IEnumerator WaitToQuitAimingCompletely(float durat)
    {
        yield return new WaitForSeconds(durat);
        quitAimingCompletely = true;
        animator.SetBool("isAiming", false);
    }
}
