using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GroundCheckScript : MonoBehaviour
{
    GeneralCharacter Char;
    Coroutine groundedCounting;
    public bool touchingGround;
    Rigidbody rb;

    void Start()
    {
        Char = transform.parent.GetComponent<GeneralCharacter>();
        rb = Char.GetComponent<Rigidbody>();
    }
    void Update()
    {
        if (!Char.isJumping && !touchingGround && rb.velocity.y > 0)
        {
            rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y * 0.9f, rb.velocity.z);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Ground" || other.tag == "Vehicle")
        {
            Char.isGrounded= true;
            Char.isJumping = false;
            if(groundedCounting != null)
            {
                StopCoroutine(groundedCounting);
            }
        }
    }
    void OnTriggerStay(Collider other)
    {
        if (other.tag == "Ground" || other.tag == "Vehicle")
        {
            Char.isGrounded = true;
            touchingGround = true;
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Ground" || other.tag == "Vehicle")
        {
            groundedCounting = StartCoroutine(CountForNotGrounded());
            touchingGround = false;
        }
    }
    IEnumerator CountForNotGrounded()
    {
        yield return new WaitForSeconds(0.5f);
        if(!touchingGround)
        {
            Char.isGrounded = false;
        }
    }

}
