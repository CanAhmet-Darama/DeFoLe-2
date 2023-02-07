using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralCharacter : MonoBehaviour
{
    [Header("General")]
    public float walkSpeed;
    public float walkAcceleration;
    public float runSpeed;
    public float runAcceleration;
    public byte health;
    public Rigidbody rb;
   

    [Header("Animation")]
    public Animator animator;
    public AnimStateSpeed animStateSpeed;
    public AnimStatePriDir animStatePriDir;
    public AnimStateSecDir animStateSecDir;


    [Header("Not to meddle with")]
    #region Some Variables
    public bool charMoving;
    Vector3 refVelo = Vector3.zero;
    //float floRefVelo = 0;
    protected Vector3 targetRotation;
    protected Direction dirToMove;
    protected Vector3 direction = Vector3.zero;
    #endregion

    public void MoveChar(Vector3 direction, float speed)
    {
        rb.velocity = direction.normalized * speed;
    }
    public void AccelerateChar(Vector3 direction, float acc)
    {
        rb.AddForce(direction.normalized * acc, ForceMode.Impulse);
    }

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
    //public enum AnimState { idle, walkFo, walkBa, walkFoLeft, walkBaLeft, walkFoRight,walkBaRight, 
    //                        walkLeft, walkRight, runFo, runBa, runFoLeft, runBaLeft, runFoRight, runBaRight,
    //                        runLeft, runRight}
    //public enum AnimStateSpeed { idle, walk, run}
    //public enum AnimStatePriDir { front, back, none }
    //public enum AnimStateSecDir { left, right, none }
}
public enum AnimStateSpeed { idle, walk, run }
public enum AnimStatePriDir { front, back, none }
public enum AnimStateSecDir { left, right, none }
