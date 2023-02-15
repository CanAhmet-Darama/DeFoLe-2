using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GroundCheckScript : MonoBehaviour
{
    GeneralCharacter Char;

    void Start()
    {
        Char = transform.parent.GetComponent<GeneralCharacter>();
        Char.groundCheckScr = GetComponent<GroundCheckScript>();
    }
    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Ground" || other.tag == "Vehicle")
        {
            Char.isGrounded= true;
            Char.isJumping = false;
        }
    }
    void OnTriggerStay(Collider other)
    {
        if (other.tag == "Ground" || other.tag == "Vehicle")
        {
            Char.isGrounded = true;
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Ground" || other.tag == "Vehicle")
        {
            Char.isGrounded = false;

        }
    }


}
