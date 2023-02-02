using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCharacter : GeneralCharacter
{
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if(GameManager.mainState == PlayerState.onFoot)
        {
            ControlMovement();
        }
    }
    void ControlMovement()
    {
        Vector3 direction = Vector3.zero;
        if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)){
            if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                direction = Vector3.left + Vector3.back;
            }
            else if(Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                direction = Vector3.left + Vector3.forward;
            }
            else
            {
                direction = Vector3.left;
            }
        }
        else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                direction = Vector3.right + Vector3.back;
            }
            else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                direction = Vector3.right + Vector3.forward;
            }
            else
            {
                direction = Vector3.right;
            }
        }
        else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            direction = Vector3.back;
        }
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            direction = Vector3.forward;
        }

        if (Input.GetKey(KeyCode.Z))
        {
            transform.Rotate(0,5,0);
        }


        Debug.Log(direction);
        if((direction != Vector3.zero) && Input.GetKey(KeyCode.LeftShift))
        {
            AccAndRun(direction);
            Debug.Log("1st working");
        }
        else if (direction != Vector3.zero)
        {
            AccAndWalk(direction);
            Debug.Log("2nd working");
        }

    }
}
