using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCharacter : GeneralCharacter
{
    [SerializeField] Transform camFreeLookPivot;
    [SerializeField] Transform meshAndArmature;
    AnimatingClass playerAnimating;

    void Start()
    {
        GeneralCharStart();
        ChangeWeapon(weapons[0].GetComponent<GeneralWeapon>());
        playerAnimating = meshAndArmature.GetComponent<AnimatingClass>();
        GameManager.mainChar = transform;
    }
    void Update()
    {
        GeneralCharUpdate();
        ControlWeaponry();
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
        if (isGrounded)
        {
            if (Input.GetKey(KeyCode.Mouse1))
            {
                direction = Vector3.zero;
                if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)){
                    if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
                    {
                        direction = -transform.right + transform.forward;
                        playerAnimating.SetAnimStates(AnimStatePriDir.front, AnimStateSecDir.left);
                    }
                    else if(Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
                    {
                        direction = transform.right + transform.forward;
                        playerAnimating.SetAnimStates(AnimStatePriDir.front, AnimStateSecDir.right);
                    }
                    else
                    {
                        direction = transform.forward;
                        playerAnimating.SetAnimStates(AnimStatePriDir.front, AnimStateSecDir.none);
                    }
                }
                else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
                {
                    if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
                    {
                        direction = -transform.right - transform.forward;
                        playerAnimating.SetAnimStates(AnimStatePriDir.back, AnimStateSecDir.left);
                    }
                    else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
                    {
                        direction = transform.right - transform.forward;
                        playerAnimating.SetAnimStates(AnimStatePriDir.back, AnimStateSecDir.right);
                    }
                    else
                    {
                        direction = -transform.forward;
                        playerAnimating.SetAnimStates(AnimStatePriDir.back, AnimStateSecDir.none);
                    }
                }
                else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
                {
                    direction = -transform.right;
                    playerAnimating.SetAnimStates(AnimStatePriDir.none, AnimStateSecDir.left);

                }
                else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
                {
                    direction = transform.right;
                    playerAnimating.SetAnimStates(AnimStatePriDir.none, AnimStateSecDir.right);

                }

                if ((direction != Vector3.zero) && Input.GetKey(KeyCode.LeftShift))
                {
                    AccAndRun(direction);
                    playerAnimating.SetAnimStateSpeed(AnimStateSpeed.run);
                }
                else if (direction != Vector3.zero)
                {
                    AccAndWalk(direction);
                    playerAnimating.SetAnimStateSpeed(AnimStateSpeed.walk);

                }
                else
                {
                    playerAnimating.SetAnimStateSpeed(AnimStateSpeed.idle);
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
                            targetRotation = new Vector3(transform.eulerAngles.x, GameManager.mainCam.rotation.eulerAngles.y, transform.eulerAngles.z);
                            break;
                        case Direction.back:
                            targetRotation = new Vector3(transform.eulerAngles.x, GameManager.mainCam.rotation.eulerAngles.y + 180, transform.eulerAngles.z);
                            break;
                        case Direction.left:
                            targetRotation = new Vector3(transform.eulerAngles.x, GameManager.mainCam.rotation.eulerAngles.y - 90, transform.eulerAngles.z);
                            break;
                        case Direction.right:
                            targetRotation = new Vector3(transform.eulerAngles.x, GameManager.mainCam.rotation.eulerAngles.y + 90, transform.eulerAngles.z);
                            break;
                        case Direction.foLeft:
                            targetRotation = new Vector3(transform.eulerAngles.x, GameManager.mainCam.rotation.eulerAngles.y - 45, transform.eulerAngles.z);
                            break;
                        case Direction.foRight:
                            targetRotation = new Vector3(transform.eulerAngles.x, GameManager.mainCam.rotation.eulerAngles.y + 45, transform.eulerAngles.z);
                            break;
                        case Direction.baLeft:
                            targetRotation = new Vector3(transform.eulerAngles.x, GameManager.mainCam.rotation.eulerAngles.y - 135, transform.eulerAngles.z);
                            break;
                        case Direction.baRight:
                            targetRotation = new Vector3(transform.eulerAngles.x, GameManager.mainCam.rotation.eulerAngles.y + 135, transform.eulerAngles.z);
                            break;
                    }
                    if (targetRotation.y != transform.eulerAngles.y)
                    {
                        RotateChar(targetRotation, 0.20f);
                    }

                    if (Input.GetKey(KeyCode.LeftShift))
                    {
                        AccAndRun(transform.forward);
                        playerAnimating.SetAnimStates(AnimStateSpeed.run,AnimStatePriDir.front,AnimStateSecDir.none);
                    }
                    else
                    {
                        AccAndWalk(transform.forward);
                        playerAnimating.SetAnimStates(AnimStateSpeed.walk, AnimStatePriDir.front, AnimStateSecDir.none);
                    }
                }
                else
                {
                    playerAnimating.SetAnimStates(AnimStateSpeed.idle, AnimStatePriDir.none, AnimStateSecDir.none);
                }
            }

            if (Input.GetKeyUp(KeyCode.Mouse1))
            {
                rb.angularVelocity = Vector3.zero;
            }
            if(canJump && Input.GetKey(KeyCode.Space))
            {
                Jump(0.1f);
            }
        }
        else
        {
            if (isJumping)
            {
                if(rb.velocity.y > 0)
                {
                    if (!Input.GetKey(KeyCode.Space))
                    {
                        rb.AddForce((Physics.gravity * rb.mass));
                    }
                    else
                    {
                        rb.AddForce((Physics.gravity * rb.mass * 0.5f));
                    }
                }
                else
                {
                    rb.AddForce((Physics.gravity * rb.mass));
                }
            }
            else
            {
                rb.AddForce((Physics.gravity * rb.mass));
            }
        }

    }
    void ControlWeaponry()
    {
        if (Input.GetMouseButton(0) && canShoot)
        {
            currentWeapon.GetComponent<GeneralWeapon>().Fire();
            StartCoroutine(CanShootAgain());
        }

        if (Input.GetKeyDown(KeyCode.R) && canReload && ammoCounts[(int)currentWeapon.weaponType] > 0)
        {

        }


    }
    IEnumerator ReloadTimer()
    {
        canReload = false;
        canShoot = false;
        yield return new WaitForSeconds(currentWeapon.reloadTime);
        canReload = true;
        canShoot = true;
    }

    public void RegulateMainChar()
    {
        camFreeLookPivot = GameManager.mainCam.GetComponent<CameraScript>().freeLookPivotOnFoot;
    }
}
