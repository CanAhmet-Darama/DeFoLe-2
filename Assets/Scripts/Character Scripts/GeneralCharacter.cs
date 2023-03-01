using System.Collections;
using System.Collections.Generic;
//using System.Diagnostics;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;
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

    [Header("For Weapon")]
    public GeneralWeapon currentWeapon;
    public bool[] hasWeapons = new bool[5];
    public GameObject[] weapons = new GameObject[5];
    public short[] ammoCounts= new short[5];
    public bool canShoot;
    public bool canReload;
    public bool isReloading;


    [Header("Animation")]
    public Animator animator;
    public GroundCheckScript groundCheckScr;
    public AnimStateSpeed animStateSpeed;
    public AnimStatePriDir animStatePriDir;
    public AnimStateSecDir animStateSecDir;
    public GameObject rightHBone;
    public GameObject leftHBone;
    [SerializeField]
    AnimatorOverrideController[] animOverriders;

    [Header("Some Stuff")]
    public bool isGrounded;
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

    void CreateWeapons()
    {
        for(int i = weapons.Length - 1; i >= 0; i--)
        {
            if(GetComponent<MainCharacter>() != null)
            {
                weapons[i] = Instantiate(GameManager.weaponPrefabs[i], rightHBone.transform);
                weapons[i].transform.localPosition = weapons[i].GetComponent<GeneralWeapon>().rightHandPosOffset;
                weapons[i].SetActive(false);
            }
            else if (hasWeapons[i - 1])
            {
                weapons[i] = Instantiate(GameManager.weaponPrefabs[i], rightHBone.transform);
                weapons[i].transform.localPosition = weapons[i].GetComponent<GeneralWeapon>().rightHandPosOffset;
                weapons[i].SetActive(false);
            }
            weapons[i].transform.localScale *= 0.01f;
            weapons[i].transform.localPosition = weapons[i].GetComponent<GeneralWeapon>().rightHandPosOffset;
            weapons[i].transform.localEulerAngles += new Vector3(-90,90,0);
            //weapons[i].transform.localEulerAngles = weapons[i].GetComponent<GeneralWeapon>().rightHandRotOffset;
            weapons[i].GetComponent<GeneralWeapon>().owner = this;
        }
    }
    public void ChangeWeapon(GeneralWeapon newWeapon)
    {
        if(currentWeapon != null)
        {
            currentWeapon.gameObject.SetActive(false);
        }
        currentWeapon = newWeapon;
        currentWeapon.gameObject.SetActive(true);
        AnimationOverride(animOverriders[currentWeapon.animOverriderIndex]);
        if(currentWeapon.currentAmmo > 0)
        {
            canShoot = true;
        }
        //currentWeapon.transform.localPosition = currentWeapon.rightHandPosOffset;
    }
    public void AnimationOverride(AnimatorOverrideController overrider)
    {
        animator.runtimeAnimatorController = overrider;
    }

    protected enum Direction { forward, back, left, right, foLeft, baLeft, foRight, baRight, none }
    
    protected void GeneralCharStart()
    {
        canJump = true;
        rb = GetComponent<Rigidbody>();
        stairSlopeChecker = stairSlopeCheckerGO_.GetComponent<StairCheckScript>();
        CreateWeapons();
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
    public void ParentAndResetBullet(Transform bullet, Transform parent, GeneralBullet bScript)
    {
        StartCoroutine(AFrameThenParentAgain(bullet,parent, bScript));
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
