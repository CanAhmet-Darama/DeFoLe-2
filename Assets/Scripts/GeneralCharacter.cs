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
   

    [Header("Animation")]
    public Animator animator;
    public GroundCheckScript groundCheckScr;
    public AnimStateSpeed animStateSpeed;
    public AnimStatePriDir animStatePriDir;
    public AnimStateSecDir animStateSecDir;

    [Header("Some Stuff")]
    public bool isGrounded;

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
        isJumping = true;
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

    /*protected void DetectSlope()
    { 
        float castPosY = ( - GetComponent<CapsuleCollider>().height / 2) + 0.1f;
        Vector3 castPos = new Vector3(transform.position.x, transform.position.y - castPosY, transform.position.z);
        float castDistance = 0.5f;
        float castRadius = 0.3f;
        RaycastHit hitFront, hitBack, hitLeft, hitRight;
        Physics.Raycast(castPos + transform.forward * castRadius, Vector3.down, out hitFront,castDistance);
        Physics.Raycast(castPos + transform.right * castRadius, Vector3.down, out hitRight, castDistance);
        Physics.Raycast(castPos - transform.forward * castRadius, Vector3.down, out hitBack, castDistance);
        Physics.Raycast(castPos - transform.right * castRadius, Vector3.down, out hitLeft, castDistance);

        Debug.DrawLine(castPos + transform.forward * castRadius, hitFront.distance* Vector3.down, Color.magenta);
        Debug.DrawRay(castPos + transform.right * castRadius, hitRight.distance * Vector3.down, Color.magenta);
        Debug.DrawRay(castPos - transform.forward * castRadius, hitBack.distance * Vector3.down, Color.magenta);
        Debug.DrawRay(castPos - transform.right * castRadius, hitLeft.distance * Vector3.down, Color.magenta);

        Debug.Log(hitFront.normal);
        Debug.Log(hitBack.normal);
        Debug.Log(hitLeft.normal);
        Debug.Log(hitRight.normal);

    }*/

}
public enum AnimStateSpeed { idle, walk, run }
public enum AnimStatePriDir { front, back, none }
public enum AnimStateSecDir { left, right, none }
