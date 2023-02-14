using System.Collections;
using System.Collections.Generic;
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
   

    [Header("Animation")]
    public Animator animator;
    public GroundCheckScript groundCheckScr;
    public AnimStateSpeed animStateSpeed;
    public AnimStatePriDir animStatePriDir;
    public AnimStateSecDir animStateSecDir;

    [Header("Some Stuff")]


    [Header("Not to meddle with")]
    #region Some Variables
    public bool charMoving;
    Vector3 refVelo = Vector3.zero;
    //float floRefVelo = 0;
    protected Vector3 targetRotation;
    protected Direction dirToMove;
    protected Vector3 direction = Vector3.zero;

    protected bool canJump;
    public bool hasJumped;
    public bool isFalling;
    public bool isGrounded;
    //protected bool previousFrameGrounded;

    public float blendAnimX;
    public float blendAnimY;

    #endregion

    public void MoveChar(Vector3 direction, float speed)
    {
        rb.velocity = new Vector3(direction.normalized.x*speed,rb.velocity.y,direction.normalized.z*speed);
    }
    public void AccelerateChar(Vector3 direction, float acc)
    {
        rb.AddForce(direction.normalized * acc, ForceMode.Impulse);
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
        hasJumped = true;
        yield return new WaitForSeconds(durat);
        Debug.Log("Thrust started");
        rb.AddForce(transform.up*jumpForce, ForceMode.Impulse);
    }
    IEnumerator JumpCooldown()
    {
        yield return new WaitForSeconds(jumpCooldown);
        Debug.Log("Can jump now");
        canJump = true;
    }
    #endregion

    public void AccAndWalk(Vector3 direction)
    {
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
        if (rb.velocity.magnitude > runSpeed)
        {
            MoveChar(direction, runSpeed);
        }
        else
        {
            AccelerateChar(direction, runAcceleration);
        }
    }
    public void RotateChar(Vector3 target, float smoothTime)
    {
        Quaternion targetQua = new Quaternion(0,0,0,0);
        targetQua = Quaternion.Euler(transform.eulerAngles.x, target.y, transform.eulerAngles.z);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetQua, smoothTime);
    }
    protected enum Direction { forward, back, left, right, foLeft, baLeft, foRight, baRight, none }
    
    protected void GeneralCharStart()
    {
        canJump = true;
        isGrounded = true;
        rb = GetComponent<Rigidbody>();
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
    protected void GeneralCharOnCollisionEnter()
    {

    }
    protected void OnTriggerEnterGC(Collider other)
    {

    }
    protected void OnTriggerExitGC(Collider other)
    {

    }

    protected bool GroundedChecker()
    {
        string[] tags = { "Ground", "Vehicle" };

        return true;



        //Ray ray = new Ray(transform.position + new Vector3(0,-1.2f,0), Vector3.down);
        //Vector3 extents = new Vector3(0.15f,0.05f,0.15f);
        //LayerMask mask = 0;
        //float castDistance = 0.65f;
        //RaycastHit hitInfo;
        //bool isHit = Physics.BoxCast(ray.origin,extents,ray.direction, out hitInfo,transform.rotation, castDistance, ~mask);
        //#region DrawRays
        //Debug.DrawRay(ray.origin + new Vector3(extents.x,0,extents.z), Vector3.down * castDistance, Color.green);
        //Debug.DrawRay(ray.origin + new Vector3(-extents.x, 0, extents.z), Vector3.down * castDistance, Color.green);
        //Debug.DrawRay(ray.origin + new Vector3(-extents.x, 0, -extents.z), Vector3.down * castDistance, Color.green);
        //Debug.DrawRay(ray.origin + new Vector3(extents.x, 0, -extents.z), Vector3.down * castDistance, Color.green);
        //#endregion


        //if (isHit)
        //{
        //    for (byte i = (byte)tags.Length; i > 0; i--)
        //    {
        //        if (hitInfo.collider.tag == tags[i - 1])
        //        {
        //            if (!previousFrameGrounded)
        //            {
        //                animator.SetBool("isJumping",true);
        //                animator.SetBool("isFalling", true);
        //                animator.SetBool("isGrounded", false);
        //            }
        //            else
        //            {
        //                animator.SetBool("isJumping", false);
        //                animator.SetBool("isFalling", false);
        //                animator.SetBool("isGrounded", true);
        //            }
        //            previousFrameGrounded = isHit;
        //            Debug.Log("GROUNDED");
        //            return true;
        //        }
        //    }

        //}
        //Debug.Log("AIRED");
        //return false;
    }
}
public enum AnimStateSpeed { idle, walk, run }
public enum AnimStatePriDir { front, back, none }
public enum AnimStateSecDir { left, right, none }
