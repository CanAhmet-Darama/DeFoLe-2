using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;

public class GeneralCharacter : MonoBehaviour
{
    [Header("General")]
    public float walkSpeed;
    public float walkAcceleration;
    public float runSpeed;
    public float runAcceleration;
    public float jumpForce;
    public float jumpCooldown;
    public short health;
    public Rigidbody rb;
    public bool isEnemy;

    [Header("For Weapon")]
    public GeneralWeapon currentWeapon;
    public bool[] hasWeapons = new bool[5];
    public GameObject[] weapons = new GameObject[5];
    public GeneralWeapon[] weaponScripts = new GeneralWeapon[5];
    public short[] ammoCounts= new short[5];
    public byte magazineCount;
    public MeleeWeapon mainMelee;
    public bool canShoot;
    public bool canReload;
    public bool isReloading;
    public WeaponState weaponState;
    public enum WeaponState { ranged, melee, handsFree}

    #region Animation
    [Header("Animation")]
    public Animator animator;
    public AnimStateSpeed animStateSpeed;
    public AnimStatePriDir animStatePriDir;
    public AnimStateSecDir animStateSecDir;
    public GameObject aimTarget;
    public GameObject rightHBone;
    public GameObject leftHBone;
    public MultiAimConstraint rightHandConstraint;
    public MultiAimConstraint leftHandConstraint;
    public MultiAimConstraint multiAimCoBody;
    public TwoBoneIKConstraint rightHandTBIK;
    public TwoBoneIKConstraint leftHandTBIK;
    public GameObject leftHTarget;
    public GameObject rightHTarget;
    public GameObject skeleton;
    [SerializeField]
    AnimatorOverrideController[] animOverriders;
    #endregion

    [Header("For Audio")]
    public AudioClip[] stepSounds;
    public bool[] footTouchedGround; // First element is LEFT, second element is RIGHT
    public bool[] footTouchedGroundPrevious; // First element is LEFT, second element is RIGHT
    public AudioSource stepSoundMaker;
    public GameObject[] footBones; // First element is LEFT, second element is RIGHT
    float stepSoundMultiplier = 0.25f;

    [Header("Some Stuff")]
    public bool isGrounded;
    public bool isCrouching;
    public CapsuleCollider mainColl;
    [HideInInspector]public StairCheckScript stairSlopeChecker;
    [SerializeField] GameObject stairSlopeCheckerGO_;
    [HideInInspector] public CharColliderManager charColManager;

    [Header("Ragdoll")]
    protected Collider[] ragdollCols;
    protected Rigidbody[] ragdollRbs;

    [Header("Not to meddle with")]
    #region Some Variables
    public bool charMoving;
    Vector3 refVelo = Vector3.zero;
    protected Vector3 targetRotation;
    protected Direction dirToMove;
    protected Vector3 direction = Vector3.zero;

    protected bool canJump;
    public bool isJumping;

    public float blendAnimX;
    public float blendAnimY;

    Coroutine crouchLerper;
    protected bool canCrouch = true;

    public bool isShooting;

    #endregion

    public void MoveChar(Vector3 direction, float speed)
    {
        if(stairSlopeChecker.onSlopeMoving && !isJumping){
            rb.velocity = new Vector3(direction.normalized.x*speed,direction.normalized.y*speed,direction.normalized.z*speed);
        }
        else
        {
            rb.velocity = new Vector3(direction.normalized.x * speed, rb.velocity.y, direction.normalized.z * speed);
        }
    }
    public void AccelerateChar(Vector3 direction, float acc)
    {
        rb.AddForce(direction.normalized * acc, ForceMode.Impulse);
    }
    public void RotateChar(Vector3 target, float lerpSpeed)
    {
        Quaternion targetQua = Quaternion.Euler(transform.eulerAngles.x, target.y, transform.eulerAngles.z);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetQua, lerpSpeed);
    }
    public void RotateCharToLookAt(Vector3 targetPos, float lerpSpeed)
    {
        Vector3 targetVec = new Vector3(targetPos.x, 0, targetPos.z) -
                            new Vector3(transform.position.x, 0, transform.position.z);
        transform.forward = Vector3.Lerp(transform.forward, targetVec, lerpSpeed);
    }


    public void AccAndWalk(Vector3 direction)
    {
        if (stairSlopeChecker.onSlopeMoving)
        {
            direction = StairCheckScript.RotateVecAroundVec(direction, stairSlopeChecker.crossProduct, stairSlopeChecker.normalAngle);
        }

        if (rb.velocity.sqrMagnitude > walkSpeed*walkSpeed)
        {
            MoveChar(direction, walkSpeed);
        }
        else
        {
            AccelerateChar(direction, walkAcceleration);
        }
    }
    public void AccAndWalk(Vector3 direction, float speed)
    {
        if (stairSlopeChecker.onSlopeMoving)
        {
            direction = StairCheckScript.RotateVecAroundVec(direction, stairSlopeChecker.crossProduct, stairSlopeChecker.normalAngle);
        }

        if (rb.velocity.sqrMagnitude > speed*speed)
        {
            MoveChar(direction, speed);
        }
        else
        {
            AccelerateChar(direction, walkAcceleration);
        }
    }

    public void AccAndRun(Vector3 direction)
    {
        if (stairSlopeChecker.onSlopeMoving)
        {
            direction = StairCheckScript.RotateVecAroundVec(direction, stairSlopeChecker.crossProduct, stairSlopeChecker.normalAngle);
        }

        if (rb.velocity.sqrMagnitude > runSpeed * runSpeed)
        {
            MoveChar(direction, runSpeed);
        }
        else
        {
            AccelerateChar(direction, runAcceleration);
        }
    }
    public void Jump(float waitDurat)
    {
        StartCoroutine(WaitForJump(waitDurat));
        StartCoroutine(JumpCooldown());
    }
    #region Jump IEnumerators
    IEnumerator WaitForJump(float durat)
    {
        canJump = false;
        isJumping = true;
        yield return new WaitForSeconds(durat);
        rb.AddForce(transform.up*jumpForce, ForceMode.Impulse);
        yield return new WaitForSeconds(0.1f);
        isGrounded = false;
    }
    IEnumerator JumpCooldown()
    {
        yield return new WaitForSeconds(jumpCooldown);
        canJump = true;
    }
    #endregion

    public void CrouchOrStand()
    {
        if (crouchLerper != null) StopCoroutine(crouchLerper);
        crouchLerper = StartCoroutine(CrouchAnimLerp());
        isCrouching = !isCrouching;
    }
    IEnumerator CrouchAnimLerp()
    {
        float targetWeight;
        if(isCrouching)
        {
            targetWeight = 0;
            mainColl.height = 2.3f;
            mainColl.center = new Vector3(mainColl.center.x, -0.33f , mainColl.center.z);
        }
        else
        {
            targetWeight = 1;
            mainColl.height = 1.9f;
            mainColl.center = new Vector3(mainColl.center.x, -0.55f, mainColl.center.z);

        }
        canCrouch = false;

        while (Mathf.Abs(animator.GetLayerWeight(1) - targetWeight) >= 0.01f)
        {
            animator.SetLayerWeight(1,Mathf.Lerp(animator.GetLayerWeight(1),targetWeight,0.05f));
            yield return null;
        }
        animator.SetLayerWeight(1, targetWeight);
        canCrouch = true;
    }

    void CreateWeapons()
    {
        for (int i = weapons.Length - 1; i >= 0; i--)
        {
            if(!isEnemy || hasWeapons[i])
            {
                weapons[i] = Instantiate(GameManager.weaponPrefabs[i], rightHBone.transform);
                weaponScripts[i] = weapons[i].GetComponent<GeneralWeapon>();
                if (weaponScripts[i].weaponType == WeaponType.SR_1)
                { weapons[i].transform.parent = leftHBone.transform; }

                weapons[i].transform.localPosition = weaponScripts[i].weaponInternalPos;
                weapons[i].SetActive(false);

                weapons[i].transform.localScale *= 0.01f;
                if(weaponScripts[i].weaponType != WeaponType.SR_1)
                {
                    weapons[i].transform.localEulerAngles += new Vector3(-90, 90, 0);
                }
                else
                {
                    weapons[i].transform.localEulerAngles = new Vector3(-90, 0, 90);
                }

                weaponScripts[i].owner = this;
                //Debug.Log(gameObject.name + " has created : " + weapons[i].name);
                ammoCounts[i] = (short)(magazineCount * GameManager.weaponPrefabs[i].GetComponent<GeneralWeapon>().maxAmmo);
                weaponScripts[i].currentAmmo = (byte)(ammoCounts[i] / magazineCount);
            }
        }
        #region Melee Create
        mainMelee = Instantiate(GameManager.weaponPrefabs[5], rightHBone.transform).GetComponent<MeleeWeapon>();
        mainMelee.transform.localPosition = mainMelee.rightHandPos;
        mainMelee.transform.localEulerAngles = mainMelee.rightHandRot;
        mainMelee.gameObject.SetActive(false);
        mainMelee.transform.localScale *= 0.1f;
        mainMelee.owner = this;
        #endregion
    }
    public void ChangeWeapon(GeneralWeapon newWeapon)
    {
        if (weaponState != WeaponState.ranged)
        {
            GetMeleeWeaponOrHandsFree(WeaponState.ranged);
        }

        if (currentWeapon != null && currentWeapon != newWeapon) {        
            currentWeapon.gameObject.SetActive(false);
            if(currentWeapon.zoomedAlready)
            {
                currentWeapon.SniperZoom(false);
            }
        }

        currentWeapon = newWeapon;
        if(newWeapon == null)
        {
            Debug.Log("NULL");
        }
        currentWeapon.gameObject.SetActive(true);
        AnimationOverride(animOverriders[currentWeapon.animOverriderIndex]);
        if(currentWeapon.currentAmmo > 0)
        {
            canShoot = true;
        }
        ResetHandTargets(newWeapon);
        AimManager.ResetWeights(this);
        if (!isEnemy)
        {
            GameManager.uiManager.SetAmmoUI();
            GameManager.uiManager.weaponIcon.sprite = GameManager.uiManager.weaponIconSprites[(int)newWeapon.weaponType];
            GameManager.uiManager.ReduceOpacity(GameManager.uiManager.weaponIcon, 1, 1, true);
        }
    }
    public void ResetHandTargets(GeneralWeapon newWeapon)
    {
        rightHTarget.transform.localPosition = newWeapon.rightTargetPos;
        rightHTarget.transform.localEulerAngles = newWeapon.rightTargetRot;
        leftHTarget.transform.localPosition = newWeapon.leftTargetPos;
        leftHTarget.transform.localEulerAngles = newWeapon.leftTargetRot;
    }
    public void GetMeleeWeaponOrHandsFree(WeaponState stateX)
    {
        switch (stateX)
        {
            case WeaponState.melee:
                currentWeapon.gameObject.SetActive(false);
                mainMelee.gameObject.SetActive(true);
                weaponState = WeaponState.melee;
                AnimationOverride(mainMelee.overrideController);
                animator.SetLayerWeight(2, 1);
                mainMelee.audioSource.PlayOneShot(mainMelee.sheathSound);
                if (!isEnemy)
                {
                    GameManager.uiManager.SetAmmoUI();
                    GameManager.uiManager.weaponIcon.sprite = GameManager.uiManager.weaponIconSprites[5];
                    GameManager.uiManager.ReduceOpacity(GameManager.uiManager.weaponIcon, 1, 1, true);
                }
                break;
            case WeaponState.ranged:
                currentWeapon.gameObject.SetActive(true);
                mainMelee.gameObject.SetActive(false);
                weaponState = WeaponState.ranged;
                AimManager.ResetWeights(this);
                animator.SetLayerWeight(2, 1);
                break;
            case WeaponState.handsFree:
                currentWeapon.gameObject.SetActive(false);
                mainMelee.gameObject.SetActive(false);
                weaponState = WeaponState.handsFree;
                AimManager.ResetWeights(this);
                animator.SetLayerWeight(2, 0);
                break;
        }
        if (!isEnemy)
        {
            GameManager.uiManager.SetAmmoUI();
        }
    }
    public void AnimationOverride(AnimatorOverrideController overrider)
    {
        animator.runtimeAnimatorController = overrider;
    }

    protected enum Direction { forward, back, left, right, foLeft, baLeft, foRight, baRight, none }
    
    protected void GeneralCharStart()
    {
        canJump = true;
        canReload = true;
        if (GetComponent<MainCharacter>() == null)
        {
            isEnemy = true;
        }
        else
        {
            stairSlopeChecker = stairSlopeCheckerGO_.GetComponent<StairCheckScript>();
            isEnemy = false;
        }
        CreateWeapons();
        weaponState = WeaponState.ranged;

        footTouchedGround = new bool[2];
        footTouchedGroundPrevious = new bool[2];
        GetAndDisableRagdollParts();
    }
    protected void GeneralCharUpdate()
    {
        if(rb.velocity.y > 20)
        {
            rb.velocity = new Vector3(rb.velocity.x,20,rb.velocity.z);
        }
        else if (rb.velocity.y < -20)
        {
            rb.velocity = new Vector3(rb.velocity.x, -20, rb.velocity.z);
        }
        SteppedGroundCheck();
    }

    public IEnumerator CanShootAgain(float durat)
    {
        canShoot = false;
        yield return new WaitForSeconds(durat);
        canShoot = true;
    }
    public IEnumerator CanReloadAgain(float durat)
    {
        canReload = false;
        isReloading = true;
        yield return new WaitForSeconds(durat);
        canReload = true;
        isReloading = false;

    }

    public IEnumerator IsInFiring(float durat)
    {
        isShooting = true;
        yield return new WaitForSeconds(durat);
        isShooting = false;

    }
    public void ParentAndResetBullet(Transform bullet, Transform parent, GeneralBullet bScript)
    {
        if (this.isActiveAndEnabled)
        {
            StartCoroutine(AFrameThenParentAgain(bullet,parent, bScript));
        }
        else
        {
            bScript.trailRenderer.enabled = false;
            bullet.gameObject.SetActive(false);
            bullet.parent = parent;
            bullet.localPosition = currentWeapon.bulletLaunchOffset;
        }
    }
    IEnumerator AFrameThenParentAgain(Transform bullet, Transform parent, GeneralBullet bScript)
    {
        bScript.trailRenderer.enabled = false;
        bullet.gameObject.SetActive(false);
        yield return null;
        bullet.parent = parent;
        bullet.localPosition = currentWeapon.bulletLaunchOffset;
    }

    public static void GiveDamage(GeneralCharacter harmedChar, short damage)
    {
        if(harmedChar.health > 0)
        {
            harmedChar.health -= damage;
            harmedChar.DeathCheck();
            if (harmedChar.isEnemy)
            {
                EnemyScript enemyS = harmedChar.GetComponent<EnemyScript>();
                enemyS.ChangeEnemyAIState(EnemyScript.EnemyAIState.Alerted);
                EnemyScript.MakeEnemyVoice(enemyS, 1);
            }
            else
            {
                GameManager.uiManager.SetHealthUI();
            }
            //Debug.Log(harmedChar.gameObject.name + " got damage : " + damage);
        }
    }
    public void DeathCheck()
    {
        if(health <= 0)
        {
            KillCharacter();
        }
    }
    public void KillCharacter()
    {
        if (isEnemy)
        {
            EnemyScript enemyS = GetComponent<EnemyScript>();
            enemyS.StopAllCoroutines();
            enemyS.navAgent.enabled = false;
            enemyS.enabled = false;
            EnemyScript.MakeEnemyVoice(enemyS, 2);
            EnemyManager.enemiesDead[enemyS.campOfEnemy - 1][enemyS.enemyStaticIndex] = true;
            EnemyManager.CampClearedCheck(enemyS.campOfEnemy);
        }
        else
        {
            MainCharacter mainChar = GameManager.mainChar.GetComponent<MainCharacter>();
            mainChar.stairSlopeChecker.enabled = false;
            mainChar.StopAllCoroutines();
            mainChar.enabled = false;
            rb.velocity = Vector3.zero;
            GameManager.ChangeState(PlayerState.gameOver);
            GameManager.uiManager.GameOverDeathText();
        }
        animator.enabled = false;
        EnableRagdoll();
    }

    void SteppedGroundCheck()
    {
        float castDistance = 0.25f;
        int layerMask = 1;
        RaycastHit hitInfoStep;
        for (int i = footBones.Length - 1; i >= 0; i--)
        {
            Ray rayForStepCheck = new Ray(footBones[i].transform.position, -transform.up);
            footTouchedGround[i] = Physics.Raycast(rayForStepCheck, out hitInfoStep, castDistance, layerMask, QueryTriggerInteraction.Ignore);
            if (footTouchedGround[i])
            {
                Debug.DrawRay(rayForStepCheck.origin, hitInfoStep.point - rayForStepCheck.origin, Color.white);
                if (!footTouchedGroundPrevious[i] && (animStateSpeed != AnimStateSpeed.idle))
                {
                    byte soundIndex;
                    float stepSoundMultiplierInUse = stepSoundMultiplier;
                    if (isCrouching)
                    {
                        stepSoundMultiplierInUse *= 0.5f;
                    }
                    else if(animStateSpeed == AnimStateSpeed.run)
                    {
                        stepSoundMultiplierInUse *= 2;
                    }

                    if (hitInfoStep.collider.CompareTag("Ground"))
                    {
                        if(hitInfoStep.collider.gameObject == GameManager.mainTerrain.gameObject)
                        {
                            TerrainManager.GetTerrainTexture(hitInfoStep.point);
                            float[] textureValues = new float[TerrainManager.textureValues.Length];
                            GameManager.CopyArray(TerrainManager.textureValues, ref textureValues);
                            stepSoundMaker.transform.position = hitInfoStep.point;
                            for (short textureIndex = (short)(textureValues.Length - 1); textureIndex >= 0; textureIndex--)
                            {
                                if (textureValues[textureIndex] > 0)
                                {
                                    switch (textureIndex)
                                    {
                                        case 0:
                                            soundIndex = 1;
                                            break;
                                        case 1:
                                            soundIndex = 1;
                                            break;
                                        case 2:
                                            soundIndex = 0;
                                            break;
                                        default: soundIndex = 0; 
                                            break;
                                    }
                                    stepSoundMaker.PlayOneShot(stepSounds[soundIndex*2 + i], textureValues[textureIndex] * stepSoundMultiplierInUse);
                                }
                            }
                        }
                        else if (hitInfoStep.collider.GetComponent<EnvObject>() != null)
                        {
                            EnvObjType envObjType = hitInfoStep.collider.GetComponent<EnvObject>().objectType;
                            switch (envObjType)
                            {
                                case EnvObjType.dirt: soundIndex = 1;
                                    break;
                                case EnvObjType.metal: soundIndex = 2;
                                    break;
                                case EnvObjType.wood: soundIndex = 3;
                                    break;
                                case EnvObjType.concrete: soundIndex = 0;
                                    break;
                                default : soundIndex = 0;break;
                            }
                            stepSoundMaker.transform.position = hitInfoStep.point;
                            stepSoundMaker.PlayOneShot(stepSounds[soundIndex * 2 + i], stepSoundMultiplierInUse);
                        }
                    }
                    else if (hitInfoStep.collider.CompareTag("Vehicle"))
                    {
                        stepSoundMaker.transform.position = hitInfoStep.point;
                        stepSoundMaker.PlayOneShot(stepSounds[4 + i], stepSoundMultiplierInUse);
                    }

                }
            }

            footTouchedGroundPrevious[i] = footTouchedGround[i];
        }
    }
    void GetAndDisableRagdollParts()
    {
        ragdollCols = skeleton.GetComponentsInChildren<Collider>();
        ragdollRbs = skeleton.GetComponentsInChildren<Rigidbody>();
        DisableRagdoll();
    }
    public void EnableRagdoll()
    {
        for (int ragdollIndex = ragdollCols.Length - 1; ragdollIndex >= 0; ragdollIndex--)
        {
            ragdollCols[ragdollIndex].enabled = true;
        }
        for (int ragdollIndex = ragdollRbs.Length - 1; ragdollIndex >= 0; ragdollIndex--)
        {
            ragdollRbs[ragdollIndex].isKinematic = false;
        }
        mainColl.enabled = false;
        rb.isKinematic = true;
        if (isEnemy)
        {
            GetComponent<NavMeshAgent>().enabled = false;
        }
    }
    public void DisableRagdoll()
    {
        for (int ragdollIndex = ragdollCols.Length - 1; ragdollIndex >= 0; ragdollIndex--)
        {
            ragdollCols[ragdollIndex].enabled = false;
        }
        for (int ragdollIndex = ragdollRbs.Length - 1; ragdollIndex >= 0; ragdollIndex--)
        {
            ragdollRbs[ragdollIndex].isKinematic = true;
        }
        mainColl.enabled = true;
        rb.isKinematic = false;
        if (isEnemy)
        {
            GetComponent<NavMeshAgent>().enabled = true;
        }
    }

}
public enum AnimStateSpeed { idle, walk, run }
public enum AnimStatePriDir { front, back, none }
public enum AnimStateSecDir { left, right, none }
