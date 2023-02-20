using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GroundCheckScript : MonoBehaviour
{
    GeneralCharacter Char;
    Coroutine groundedCounting;

    void Start()
    {
        Char = transform.parent.GetComponent<GeneralCharacter>();
        //Char.groundCheckScr = GetComponent<GroundCheckScript>();
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
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Ground" || other.tag == "Vehicle")
        {
            groundedCounting = StartCoroutine(CountForNotGrounded());
        }
    }
    IEnumerator CountForNotGrounded()
    {
        yield return new WaitForSeconds(0.5f);
        Char.isGrounded = false;
    }

}
