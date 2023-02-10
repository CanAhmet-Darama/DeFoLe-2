using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundChecker : MonoBehaviour
{
    [SerializeField] GeneralCharacter genChar;
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Ground")
        {
            genChar.isGround= true;
            genChar.animator.SetBool("isGrounded", true);
            genChar.animator.SetBool("isJumping",false);
            genChar.animator.SetBool("isFalling", false);
            genChar.animator.Play("Landing");
        }
    }
    void OnTriggerStay(Collider other)
    {
        if(other.tag == "Ground")
        {
            genChar.isGround= true;
            genChar.animator.SetBool("isGrounded", true);
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Ground")
        {
            genChar.isGround = false;
            genChar.animator.SetBool("isFalling",true);
        }

    }
}
