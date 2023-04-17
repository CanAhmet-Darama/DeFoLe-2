using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class AimManager : MonoBehaviour
{
    Camera mainCamera;

    [SerializeField]GeneralCharacter charToAim;
    MainCharacter mainCharToUse;
    EnemyScript enemyToUse;
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

    float lerpOrSnapSpeed = 0.1f;

    [SerializeField]bool userIsPlayer;

    bool aimStart;
    bool aimContinue;
    bool aimEnd;


    public void Start()
    {
        mainCamera = GameManager.mainCam.GetComponent<Camera>();
        mainCharToUse = GameManager.mainChar.GetComponent<MainCharacter>();
        if(charToAim.isEnemy)
        {
            enemyToUse = GetComponentInParent<EnemyScript>();
        }
        quitAimingCompletely = true;

        #region Assign Constraints
        animator = charToAim.animator;
        aimTarget = charToAim.aimTarget.transform;
        multiAimCoRHand = charToAim.rightHandConstraint;
        multiAimCoLHand = charToAim.leftHandConstraint;
        multiAimCoBody = charToAim.multiAimCoBody;
        rightHCoTBIK = charToAim.rightHandTBIK;
        leftHCoTBIK = charToAim.leftHandTBIK;
        #endregion
    }

    void Update()
    {
        if(!GameManager.isGamePaused)
        {
            if ((userIsPlayer && GameManager.mainState == PlayerState.onFoot) || !userIsPlayer)
            {
                SetBoolsOfAimer();
                AimTargetPositioner();
            }
        }
    }

    void AimTargetPositioner()
    {
        if(charToAim.weaponState == GeneralCharacter.WeaponState.ranged)
        {
            if(!(userIsPlayer && mainCharToUse.currentWeapon.weaponType == WeaponType.SR_1 
                && mainCharToUse.currentWeapon.zoomedAlready))
            {
                RightClickedOrNotManage(0.7f);
                isReloading = charToAim.isReloading;

            }

            if (quitAimingCompletely)
            {
                multiAimCoBody.weight = GameManager.LerpOrSnap(multiAimCoBody.weight, 0, lerpOrSnapSpeed);
                if(charToAim.currentWeapon != null)
                {
                    if(charToAim.currentWeapon.weaponType != WeaponType.SR_1)
                    {
                        multiAimCoRHand.weight = GameManager.LerpOrSnap(multiAimCoRHand.weight, 0, lerpOrSnapSpeed);
                    }
                    else
                    {
                        multiAimCoLHand.weight = GameManager.LerpOrSnap(multiAimCoLHand.weight, 0, lerpOrSnapSpeed);
                    }
                }
            }

        }
        else// if (charToAim.weaponState != GeneralCharacter.WeaponState.ranged)
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
        if (aimEnd)
        {
            willQuitAimingCompletely = StartCoroutine(WaitToQuitAimingCompletely(1));
            if (userIsPlayer)
            {
                GameManager.uiManager.crosshair.gameObject.SetActive(false);
                if(mainCharToUse.currentWeapon.weaponType == WeaponType.SR_1 && mainCharToUse.currentWeapon.zoomedAlready)
                {
                    mainCharToUse.currentWeapon.SniperZoom(false);
                }
            }
        }
        if (aimStart)
            {
                if(willQuitAimingCompletely != null)
                {
                    StopCoroutine(willQuitAimingCompletely);
                }
                quitAimingCompletely = false;
                animator.SetBool("isAiming", true);
            if (userIsPlayer && !(mainCharToUse.currentWeapon.weaponType == WeaponType.SR_1 && mainCharToUse.currentWeapon.zoomedAlready))
            {
                GameManager.uiManager.crosshair.gameObject.SetActive(true);
            }
        }

        if (aimContinue)
        {
            #region Raycasting for aiming
            if (!(userIsPlayer && mainCharToUse.currentWeapon.weaponType == WeaponType.SR_1 && mainCharToUse.currentWeapon.zoomedAlready))
            {
                Ray ray;
                if (userIsPlayer)
                {
                    ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

                }
                else
                {
                    float inaccEnemyX = Random.Range(-(enemyToUse.enemyInaccuracy), enemyToUse.enemyInaccuracy);
                    float inaccEnemyY = Random.Range(-(2*enemyToUse.enemyInaccuracy), enemyToUse.enemyInaccuracy);

                    Vector3 targetPos = enemyToUse.lastSeenPos;
                    if (GameManager.mainState == PlayerState.onFoot)
                    {
                        targetPos = mainCharToUse.centerPointBone.position + inaccEnemyX * enemyToUse.transform.right + inaccEnemyY * enemyToUse.transform.up;
                    }
                    else if(GameManager.mainState == PlayerState.inMainCar)
                    {
                        targetPos = GameManager.mainCar.position + Vector3.up + inaccEnemyX * enemyToUse.transform.right + inaccEnemyY * enemyToUse.transform.up;
                    }
                    ray = new Ray(enemyToUse.enemyEyes.position, targetPos - enemyToUse.enemyEyes.position);
                }
                targetHit = Physics.Raycast(ray, out hitInfo, aimCastDistance, ~(1 << 7), QueryTriggerInteraction.Ignore);

                Debug.DrawRay(ray.origin, ray.direction*hitInfo.distance, Color.gray);

                if (targetHit && hitInfo.distance > 3)
                {
                    aimTarget.position = GameManager.LerpOrSnap(aimTarget.position, hitInfo.point, 0.1f);
                }
                else
                {
                    aimTarget.localPosition = GameManager.LerpOrSnap(aimTarget.localPosition, new Vector3(0, 0, 15), 0.1f);
                }

            }
            else
            {
                aimTarget.localPosition = GameManager.LerpOrSnap(aimTarget.localPosition, new Vector3(0, 0, 500), 0.1f);
            }
            #endregion

            multiAimCoBody.weight = GameManager.LerpOrSnap(multiAimCoBody.weight, bodyTar, lerpOrSnapSpeed);
            if(charToAim.weaponState == GeneralCharacter.WeaponState.ranged)
            {
                if (!isReloading) {
                    if (charToAim.currentWeapon.weaponType != WeaponType.SR_1)
                    {
                        multiAimCoRHand.weight = GameManager.LerpOrSnap(multiAimCoRHand.weight, 1, lerpOrSnapSpeed);
                        leftHCoTBIK.weight = GameManager.LerpOrSnap(leftHCoTBIK.weight, 1, lerpOrSnapSpeed);
                    }
                    else
                    {
                        multiAimCoLHand.weight = GameManager.LerpOrSnap(multiAimCoLHand.weight, 1, lerpOrSnapSpeed);
                        if (!charToAim.isShooting) {
                            rightHCoTBIK.weight = GameManager.LerpOrSnap(rightHCoTBIK.weight, 1, lerpOrSnapSpeed);}
                    }

                }
                else
                {
                    if (charToAim.currentWeapon.weaponType != WeaponType.SR_1)
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
            if (CameraScript.mouseMoved || charToAim.animStateSpeed != AnimStateSpeed.idle)
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
    void SetBoolsOfAimer()
    {
        if (userIsPlayer)
        {
            aimStart = Input.GetMouseButtonDown(1);
            aimContinue = Input.GetMouseButton(1);
            aimEnd = Input.GetMouseButtonUp(1);
        }
        else
        {
            aimStart = enemyToUse.aimStarted;
            aimContinue = enemyToUse.isAiming;
            aimEnd = enemyToUse.aimEnded;
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
