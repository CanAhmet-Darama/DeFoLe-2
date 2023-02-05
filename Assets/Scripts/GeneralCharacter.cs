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


    [Header("Not to meddle with")]
    #region Some Variables
    public bool charMoving;
    Vector3 refVelo = Vector3.zero;
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
        if (rb.velocity.magnitude < walkSpeed)
        {
            AccelerateChar(direction, walkAcceleration);
        }
        else
        {
            MoveChar(direction, walkSpeed);
        }
    }
    public void AccAndRun(Vector3 direction)
    {
        if (rb.velocity.magnitude < runSpeed)
        {
            AccelerateChar(direction, runAcceleration);
        }
        else
        {
            MoveChar(direction, runSpeed);
        }
    }
    public void RotateChar(Vector3 amount, float smoothTime)
    {
        transform.eulerAngles = Vector3.SmoothDamp(transform.eulerAngles, amount, ref refVelo, smoothTime);
    }
    protected enum Direction { forward, back, left, right, foLeft, baLeft, foRight, baRight }
}
