using System.Collections;
using System.Collections.Generic;
//using System.Diagnostics;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using static UnityEngine.ParticleSystem;

public class GeneralCharacter : MonoBehaviour
{
    [Header("General")]
    public float walkSpeed;
    public float walkAcceleration;
    public float runSpeed;
    public float runAcceleration;
    public float jumpForce;
    public float jumpCooldown;
    public byte health;
    public Rigidbody rb;
    public bool isEnemy;

    [Header("For Weapon")]
    public GeneralWeapon currentWeapon;
    public bool[] hasWeapons = new bool[5];
    public GameObject[] weapons = new GameObject[5];
    public short[] ammoCounts= new short[5];
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
    [SerializeField]
    AnimatorOverrideController[] animOverriders;
    #endregion

    [Header("Some Stuff")]
    public bool isGrounded;
    public bool isCrouching;
    [SerializeField]CapsuleCollider mainColl;
    [HideInInspector]public StairCheckScript stairSlopeChecker;
    [SerializeField] GameObject stairSlopeCheckerGO_;

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
    public void RotateChar(Vector3 target, float smoothTime)
    {
        Quaternion targetQua = new Quaternion(0,0,0,0);
        targetQua = Quaternion.Euler(transform.eulerAngles.x, target.y, transform.eulerAngles.z);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetQua, smoothTime);
    }

    public void AccAndWalk(Vector3 direction)
    {
        if (stairSlopeChecker.onSlopeMoving)
        {
            direction = stairSlopeChecker.RotateVecAroundVec(direction, stairSlopeChecker.crossProduct, stairSlopeChecker.normalAngle);
        }

        if (rb.velocity.magnitude > walkSpeed)
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
            direction = stairSlopeChecker.RotateVecAroundVec(direction, stairSlopeChecker.crossProduct, stairSlopeChecker.normalAngle);
        }

        if (rb.velocity.magnitude > speed)
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
            direction = stairSlopeChecker.RotateVecAroundVec(direction, stairSlopeChecker.crossProduct, stairSlopeChecker.normalAngle);
        }

        if (rb.velocity.magnitude > runSpeed)
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
        byte magazineCount = 5;
        //weapons = new GameObject[5];
        for (int i = weapons.Length - 1; i >= 0; i--)
        {
            if(!isEnemy || hasWeapons[i])
            {
                weapons[i] = Instantiate(GameManager.weaponPrefabs[i], rightHBone.transform);
                GeneralWeapon weaponScript = weapons[i].GetComponent<GeneralWeapon>();
                if (weaponScript.weaponType == WeaponType.SR_1)
                { weapons[i].transform.parent = leftHBone.transform; }

                weapons[i].transform.localPosition = weapons[i].GetComponent<GeneralWeapon>().rightHandPosOffset;
                weapons[i].SetActive(false);

                weapons[i].transform.localScale *= 0.01f;
                weapons[i].transform.localPosition = weapons[i].GetComponent<GeneralWeapon>().rightHandPosOffset;
                if(weaponScript.weaponType != WeaponType.SR_1)
                {
                    weapons[i].transform.localEulerAngles += new Vector3(-90, 90, 0);
                }
                else
                {
                    weapons[i].transform.localEulerAngles = new Vector3(-90, 0, 90);
                }
                weaponScript.owner = this;
                Debug.Log(gameObject.name + " has created : " + weapons[i].name);
                ammoCounts[i] = (short)(magazineCount * GameManager.weaponPrefabs[i].GetComponent<GeneralWeapon>().maxAmmo);
                weaponScript.currentAmmo = (byte)(ammoCounts[i] / magazineCount);
            }
            //else if (hasWeapons[i])
            //{
            //    weapons[i] = Instantiate(GameManager.weaponPrefabs[i], rightHBone.transform);
            //    weapons[i].transform.localPosition = weapons[i].GetComponent<GeneralWeapon>().rightHandPosOffset;
            //    weapons[i].SetActive(false);
            //}
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
        if(weaponState != WeaponState.ranged)
        {
            GetMeleeWeaponOrHandsFree(WeaponState.ranged);
        }

        if (currentWeapon != null) {
            
            if (currentWeapon != newWeapon)
            {
                currentWeapon.gameObject.SetActive(false);

            }
         }
        currentWeapon = newWeapon;
        currentWeapon.gameObject.SetActive(true);
        AnimationOverride(animOverriders[currentWeapon.animOverriderIndex]);
        if(currentWeapon.currentAmmo > 0)
        {
            canShoot = true;
        }

        if(newWeapon.weaponType == WeaponType.SR_1)
        {
            rightHTarget.transform.localPosition = newWeapon.leftHandPos;
            rightHTarget.transform.localEulerAngles = newWeapon.leftHandRot;
        }
        else
        {
            leftHTarget.transform.localPosition = newWeapon.leftHandPos;
            leftHTarget.transform.localEulerAngles = newWeapon.leftHandRot;
        }

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
    }
    public void AnimationOverride(AnimatorOverrideController overrider)
    {
        animator.runtimeAnimatorController = overrider;
    }

    protected enum Direction { forward, back, left, right, foLeft, baLeft, foRight, baRight, none }
    
    protected void GeneralCharStart()
    {
        canJump = true;
        stairSlopeChecker = stairSlopeCheckerGO_.GetComponent<StairCheckScript>();
        if (GetComponent<MainCharacter>() == null)
        {
            isEnemy = true;
        }
        else
        {
            isEnemy = false;
        }
        CreateWeapons();
        weaponState = WeaponState.ranged;
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


}
public enum AnimStateSpeed { idle, walk, run }
public enum AnimStatePriDir { front, back, none }
public enum AnimStateSecDir { left, right, none }
