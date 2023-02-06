using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCharacter : GeneralCharacter
{
    Transform camFreeLookPivot;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator.Play("Idle");
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
            targetRotation = new Vector3(transform.eulerAngles.x,camFreeLookPivot.eulerAngles.y, transform.eulerAngles.z);

            if(Mathf.Abs((targetRotation.y - transform.eulerAngles.y)) > 180)
            {
                transform.eulerAngles = targetRotation;
            }
            else
            {
            transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, targetRotation, 0.35f);
            }


        }
        else
        {
            bool moveYes = true;
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
            else { moveYes = false;}


            if (moveYes)
            {
                switch (dirToMove)
                {
                    case Direction.forward: 
                        targetRotation = new Vector3(transform.eulerAngles.x, (GameManager.mainCam.rotation.eulerAngles.y), transform.eulerAngles.z);
                        break;
                    case Direction.back:
                        targetRotation = new Vector3(transform.eulerAngles.x, GameManager.mainCam.rotation.eulerAngles.y + 180, transform.eulerAngles.z);
                        break;
                    case Direction.left:
                        targetRotation = new Vector3(transform.eulerAngles.x, GameManager.mainCam.rotation.eulerAngles.y + 270, transform.eulerAngles.z);
                        break;
                    case Direction.right:
                        targetRotation = new Vector3(transform.eulerAngles.x, GameManager.mainCam.rotation.eulerAngles.y + 90, transform.eulerAngles.z);
                        break;
                    case Direction.foLeft:
                        targetRotation = new Vector3(transform.eulerAngles.x, GameManager.mainCam.rotation.eulerAngles.y + 315, transform.eulerAngles.z);
                        break;
                    case Direction.foRight:
                        targetRotation = new Vector3(transform.eulerAngles.x, GameManager.mainCam.rotation.eulerAngles.y + 45, transform.eulerAngles.z);
                        break;
                    case Direction.baLeft:
                        targetRotation = new Vector3(transform.eulerAngles.x, GameManager.mainCam.rotation.eulerAngles.y + 225, transform.eulerAngles.z);
                        break;
                    case Direction.baRight:
                        targetRotation = new Vector3(transform.eulerAngles.x, GameManager.mainCam.rotation.eulerAngles.y + 135, transform.eulerAngles.z);
                        break;
                        #region cases 2
                        //case Direction.forward:
                        //    targetRotation = new Vector3(transform.eulerAngles.x, (GameManager.mainCam.eulerAngles.y), transform.eulerAngles.z);
                        //    break;
                        //case Direction.back:
                        //    targetRotation = new Vector3(transform.eulerAngles.x, GameManager.mainCam.eulerAngles.y + 360, transform.eulerAngles.z);
                        //    break;
                        //case Direction.left:
                        //    targetRotation = new Vector3(transform.eulerAngles.x, GameManager.mainCam.eulerAngles.y - 90, transform.eulerAngles.z);
                        //    break;
                        //case Direction.right:
                        //    targetRotation = new Vector3(transform.eulerAngles.x, GameManager.mainCam.eulerAngles.y + 90, transform.eulerAngles.z);
                        //    break;
                        //case Direction.foLeft:
                        //    targetRotation = new Vector3(transform.eulerAngles.x, GameManager.mainCam.eulerAngles.y - 45, transform.eulerAngles.z);
                        //    break;
                        //case Direction.foRight:
                        //    targetRotation = new Vector3(transform.eulerAngles.x, GameManager.mainCam.eulerAngles.y + 45, transform.eulerAngles.z);
                        //    break;
                        //case Direction.baLeft:
                        //    targetRotation = new Vector3(transform.eulerAngles.x, GameManager.mainCam.eulerAngles.y - 135, transform.eulerAngles.z);
                        //    break;
                        //case Direction.baRight:
                        //    targetRotation = new Vector3(transform.eulerAngles.x, GameManager.mainCam.eulerAngles.y + 135, transform.eulerAngles.z);
                        //    break;
                        #endregion
                }
                if (targetRotation.y != transform.eulerAngles.y)
                {
                    RotateChar(targetRotation, 0.25f);
                }
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
