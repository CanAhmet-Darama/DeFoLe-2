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

    public void MoveChar(Vector3 direction, float speed)
    {
        rb.velocity = direction.normalized * speed;
        Debug.Log(rb.velocity.magnitude);
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
            Debug.Log("3rd working");

        }
        else
        {
            MoveChar(direction, walkSpeed);
            Debug.Log("4th working");

        }
    }
    public void AccAndRun(Vector3 direction)
    {
        if (rb.velocity.magnitude < runSpeed)
        {
            AccelerateChar(direction, runAcceleration);
            Debug.Log("5th working");

        }
        else
        {
            MoveChar(direction, runSpeed);
            Debug.Log("6th working");

        }
    }


}
