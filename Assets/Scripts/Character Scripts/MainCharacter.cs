using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCharacter : GeneralCharacter
{
    [Header("Main Char Special")]
    [SerializeField] Transform camFreeLookPivot;
    [SerializeField] Transform meshAndArmature;
    public byte closestCamp;
    public bool closeToCampEnough;
    public Transform centerPointBone;
    AnimatingClass playerAnimating;
    public Transform headOfChar;
    public Transform playerTransform;

    #region Some Stuff
    Vector3 middleScreen = new Vector3(.5f,.5f, 0f);
    #endregion

    void Start()
    {
        GeneralCharStart();
        ChangeWeapon(weapons[0].GetComponent<GeneralWeapon>());
        closestCamp = 1;
        playerAnimating = meshAndArmature.GetComponent<AnimatingClass>();
        GameManager.mainChar = transform;
        playerTransform = transform;
        InvokeRepeating("CalculateClosestCamp",0,3);
    }
    void Update()
    {
        GeneralCharUpdate();
        ControlWeaponry();
        SetUIStuff();
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

                if (isCrouching)
                {
                    if (direction != Vector3.zero)
                    {
                        AccAndWalk(direction, walkSpeed / 2);
                        playerAnimating.SetAnimStateSpeed(AnimStateSpeed.walk);
                    }
                    else
                    {
                        playerAnimating.SetAnimStateSpeed(AnimStateSpeed.idle);
                    }
                }
                else
                {
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
                        RotateChar(targetRotation, 0.2f);
                    }

                    if (isCrouching)
                    {
                        AccAndWalk(transform.forward, walkSpeed / 2);
                        playerAnimating.SetAnimStates(AnimStateSpeed.walk, AnimStatePriDir.front, AnimStateSecDir.none);
                    }
                    else
                    {
                        if (Input.GetKey(KeyCode.LeftShift))
                        {
                            AccAndRun(transform.forward);
                            playerAnimating.SetAnimStates(AnimStateSpeed.run, AnimStatePriDir.front, AnimStateSecDir.none);
                        }
                        else
                        {
                            AccAndWalk(transform.forward);
                            playerAnimating.SetAnimStates(AnimStateSpeed.walk, AnimStatePriDir.front, AnimStateSecDir.none);
                        }
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
            if(canJump && !isCrouching &&Input.GetKey(KeyCode.Space))
            {
                Jump(0.1f);
            }
            if (Input.GetKey(KeyCode.C) && canCrouch)
            {
                CrouchOrStand();
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

        if(weaponState == WeaponState.ranged)
        {
            if(currentWeapon.currentAmmo == 0)
            {
                canShoot = false;
                if (ammoCounts[(int)currentWeapon.weaponType] > 0 && canReload)
                {
                    currentWeapon.Reload();
                }
            }
            if (Input.GetMouseButton(0) && canShoot && !isReloading && Input.GetMouseButton(1))
            {
                currentWeapon.Fire();
            }

            if (Input.GetKeyDown(KeyCode.R) && canReload && ammoCounts[(int)currentWeapon.weaponType] > 0 && (currentWeapon.currentAmmo < currentWeapon.maxAmmo))
            {
                currentWeapon.Reload();
            }
        }
        else if(weaponState == WeaponState.melee) {
            if(Input.GetMouseButton(0) && mainMelee.canSwing)
            {
                mainMelee.Swing();
            }
        }

        if (!isReloading && !isShooting && mainMelee.canSwing)
        {
            SwitchingBetweenWeapons();
        }

    }
    void SetUIStuff()
    {
        int defaultLayerMask = 1 << 0;
        if (Physics.Raycast(GameManager.mainCam.GetComponent<Camera>().ViewportPointToRay(middleScreen), out RaycastHit hitInfo, 5, defaultLayerMask))
        {
            Debug.DrawLine(GameManager.mainCam.position,hitInfo.point);
            //Debug.Log(hitInfo.collider.gameObject);
            if (GameManager.SqrDistance(hitInfo.point, transform.position) < 9 && hitInfo.collider.CompareTag("Vehicle"))
            {
                if (UI_Manager.isLookingInteractable == false)
                {
                    GameManager.uiManager.interactionText.gameObject.SetActive(true);
                }
                UI_Manager.isLookingInteractable= true;
                UI_Manager.interactionType = UI_Manager.InteractableForUI.mainCar;
                if(Input.GetKeyDown(KeyCode.F)) {
                    GameManager.ChangeState(PlayerState.inMainCar);
                }
            }
            else
            {
                if (UI_Manager.isLookingInteractable == true)
                {
                    GameManager.uiManager.interactionText.gameObject.SetActive(false);
                }
                UI_Manager.isLookingInteractable = false;
            }
        }

    }

    public void RegulateMainChar()
    {
        camFreeLookPivot = GameManager.mainCam.GetComponent<CameraScript>().freeLookPivotOnFoot;
    }
    void SwitchingBetweenWeapons()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ChangeWeapon(weapons[0].GetComponent<GeneralWeapon>());
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ChangeWeapon(weapons[1].GetComponent<GeneralWeapon>());
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ChangeWeapon(weapons[2].GetComponent<GeneralWeapon>());
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            ChangeWeapon(weapons[3].GetComponent<GeneralWeapon>());
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            ChangeWeapon(weapons[4].GetComponent<GeneralWeapon>());
        }

        else if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            GetMeleeWeaponOrHandsFree(WeaponState.melee);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            GetMeleeWeaponOrHandsFree(WeaponState.handsFree);
        }

    }
    void CalculateClosestCamp()
    {
        if(GameManager.mainState == PlayerState.onFoot)
        {
            playerTransform = transform;
        }
        else if(GameManager.mainState == PlayerState.inMainCar)
        {
            playerTransform = GameManager.mainCar.transform;
        }

        for(int i = GameManager.enemyCamps.Length - 1; i >= 0; i--)
        {
            float newCampDist =GameManager.SqrDistance(GameManager.enemyCamps[i].transform.position, playerTransform.position);
            if (newCampDist < GameManager.SqrDistance(GameManager.enemyCamps[closestCamp].transform.position, playerTransform.position))
            {
                closestCamp = (byte)(i+1);
                if(newCampDist < 150 * 150)
                {
                    closeToCampEnough= true;
                }
                else
                {
                    closeToCampEnough = false;
                }
            }
        }
    }
}
