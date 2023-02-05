using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCharacter : GeneralCharacter
{
    Transform camFreeLookPivot;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if(GameManager.mainState == PlayerState.onFoot)
        {
            ControlMovement();
        }
    }
    void ControlMovement()
    {
        if (Input.GetKey(KeyCode.Mouse1))
        {
            direction = Vector3.zero;
            if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)){
                if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
                {
                    direction = -transform.right + transform.forward;
                }
                else if(Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
                {
                    direction = transform.right + transform.forward;
                }
                else
                {
                    direction = transform.forward;
                }
            }
            else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            {
                if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
                {
                    direction = -transform.right - transform.forward;
                }
                else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
                {
                    direction = transform.right - transform.forward;
                }
                else
                {
                    direction = -transform.forward;
                }
            }
            else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                direction = -transform.right;
            }
            else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                direction = transform.right;
            }

            if((direction != Vector3.zero) && Input.GetKey(KeyCode.LeftShift))
            {
                AccAndRun(direction);
            }
            else if (direction != Vector3.zero)
            {
                AccAndWalk(direction);
            }

            targetRotation = new Vector3(transform.rotation.x,camFreeLookPivot.rotation.y, transform.rotation.z);
            transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, targetRotation, 0.3f);


        }
        else
        {
            bool moveYes = true;
            bool rotateYes = true;
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            {
                if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
                {
                    dirToMove = Direction.foLeft;
                }
                else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
                {
                    dirToMove = Direction.foRight;
                }
                else
                {
                    dirToMove = Direction.forward;
                }
            }
            else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            {
                if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
                {
                    dirToMove = Direction.baLeft;
                }
                else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
                {
                    dirToMove = Direction.baRight;
                }
                else
                {
                    dirToMove = Direction.back;
                }
            }
            else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                dirToMove = Direction.left;
            }
            else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                dirToMove = Direction.right;
            }
            else { moveYes = false; rotateYes = false; }

            switch (dirToMove)
            {
                case Direction.forward: 
                    targetRotation = new Vector3(transform.eulerAngles.x, camFreeLookPivot.eulerAngles.y, transform.eulerAngles.z);
                    break;
                case Direction.back:
                    targetRotation = new Vector3(transform.eulerAngles.x, camFreeLookPivot.eulerAngles.y + 180, transform.eulerAngles.z);
                    break;
                case Direction.left:
                    targetRotation = new Vector3(transform.eulerAngles.x, camFreeLookPivot.eulerAngles.y - 90, transform.eulerAngles.z);
                    break;
                case Direction.right:
                    targetRotation = new Vector3(transform.eulerAngles.x, camFreeLookPivot.eulerAngles.y + 90, transform.eulerAngles.z);
                    break;
                case Direction.foLeft:
                    targetRotation = new Vector3(transform.eulerAngles.x, camFreeLookPivot.eulerAngles.y -45, transform.eulerAngles.z);
                    break;
                case Direction.foRight:
                    targetRotation = new Vector3(transform.eulerAngles.x, camFreeLookPivot.eulerAngles.y +45, transform.eulerAngles.z);
                    break;
                case Direction.baLeft:
                    targetRotation = new Vector3(transform.eulerAngles.x, camFreeLookPivot.eulerAngles.y - 135, transform.eulerAngles.z);
                    break;
                case Direction.baRight:
                    targetRotation = new Vector3(transform.eulerAngles.x, camFreeLookPivot.eulerAngles.y + 135, transform.eulerAngles.z);
                    break;
                //default: rotateYes = false; break;
            }
            if (rotateYes)
            {
                transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, targetRotation, 0.4f);
            }

            if (moveYes)
            {
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    AccAndRun(transform.forward);
                }
                else
                {
                    AccAndWalk(transform.forward);
                }
            }
        }

        if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            rb.angularVelocity = Vector3.zero;
        }

    }
    public void RegulateMainChar()
    {
        camFreeLookPivot = GameManager.mainCam.GetComponent<CameraScript>().freeLookPivotOnFoot;

    }
}
