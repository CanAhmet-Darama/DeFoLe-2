using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyScript : GeneralCharacter
{
    [Header("Enemy AI")]
    public NavMeshAgent navAgent;
    public EnemyAIState enemyState;
    public Transform enemyEyes;
    public float visibleAngleX;
    public float visibleAngleY;
    public float visibleRange;
    float visibleRangeInUse;
    public bool canSeeTarget;
    public float sqrDistFromPlayer;
    bool enteredNewState;
    Transform targetTransform;
    Transform mainChar;
    Transform mainCar;
    MainCharacter mainCharScript;
    [Range(1, 3)] public byte campOfEnemy;
    [HideInInspector]public byte enemyNumCode;
    public short enemyStaticIndex;

    [Header("Patrol")]
    public bool hasPermanentPlace;
    public CoverTakeableObject permanentCoverObject;
    public Vector3[] patrolPoints;
    public byte lastPatrolIndex;
    [SerializeField] float patrolWaitDuration;
    Coroutine patrolWaitCoroutine;

    [Header("SemiDetected")]
    [SerializeField] float noticeDuration;
    float noticeCountdown;
    Coroutine cancelSemiDetectCoroutine;
    [SerializeField] float suspectedDuration;
    float realDistFromPlayer;


    [Header("Alerted")]
    [SerializeField] float alertedCoverCheckCooldown;
    [SerializeField][Range(0, 1)] float alertRangeRate;
    [SerializeField] float waitBeforeAlertingAllDuration;
    Coroutine checkCoverCoroutine;
    Coroutine peekableCoverCoroutine;
    public CoverPoint currentCoverPoint;
    public short[] currentCoveredIndexes = new short[3];
    short[] previousCoverIndexes = new short[3];
    #region Aiming and Fire
    bool shouldFire;
    byte numberOfShotsBeforeCrouch;
    byte shotsSinceLastCrouch;
    float angleX;
    float angleY;
    [SerializeField] float averageCrouchDuration;
    [HideInInspector] public bool isAiming;
    [HideInInspector] public bool previousAiming;
    [HideInInspector] public bool aimStarted;
    [HideInInspector] public bool aimEnded;
    #endregion


    [Header("Searching")]
    [SerializeField]float searchDuration;
    [SerializeField] float searchRangeHorizontal;
    [SerializeField] float searchRangeVertical;
    [SerializeField] float lookElsewhereFrequency;
    float timeSinceStartedSearch;
    public Vector3 lastSeenPos;
    Coroutine searchPositioningCoroutine;


    [Header("Using Weapons")]
    public float enemyInaccuracy;
    [Range(0,1)] public float shootingFrequency;
    [HideInInspector] public GeneralWeapon mainWeapon;
    [SerializeField] float meleeRange;
    public AimManager enemyAimer;

    [Header("Ragdoll")]
    Collider[] ragdollCols;
    Rigidbody[] ragdollRbs;

    [Header("Enemy Instance References")]
    public GameObject enemyHitboxes;
    public GameObject groundChecker;
    [HideInInspector] public GameObject[] weaponMeshes;  


    void Start()
    {
        GeneralCharStart();
        NavAgentSetter();
        EnemyStart();
        ChangeWeapon(weapons[(int)mainWeapon.weaponType].GetComponent<GeneralWeapon>());
        EnemyManager.AddEnemyToList(campOfEnemy, this);
        EnemyManager.ActivateEnemy(this, false);
    }

    void Update()
    {
        GeneralCharUpdate();
        GeneralEnemyUpdate();
        CheckEyeSight();
        EnemyStateManager();
        AnimStateManage();
    }

    void EnemyStart()
    {
        GetAndDisableRagdollParts();
        lastPatrolIndex = 0;
        mainChar = GameManager.mainChar;
        mainCar = GameManager.mainCar;
        mainCharScript = mainChar.GetComponent<MainCharacter>();
        for (short i = (short)(hasWeapons.Length - 1); i >= 0; i--)
        {
            if (hasWeapons[i] && i != (short)(WeaponType.Pistol))
            {
                mainWeapon = weapons[i].GetComponent<GeneralWeapon>();
            }
        }
        numberOfShotsBeforeCrouch = mainWeapon.recommendedShotsBeforeCrouch;
        shotsSinceLastCrouch = 0;
        ChangeEnemyAIState(EnemyAIState.Patrol);
        if (hasPermanentPlace)
        {
            StartCoroutine(PermanentPlaceCoverCheck());
            patrolPoints = new Vector3[permanentCoverObject.coverPositions.Length];
            for (int i = patrolPoints.Length - 1; i >= 0; i--)
            {
                patrolPoints[i] = permanentCoverObject.transform.TransformVector((permanentCoverObject.coverPositions[i]));
            }
        }
        if (mainWeapon.weaponType == WeaponType.SR_1)
        {
            visibleRange = visibleRange * 2;
        }
        weaponMeshes = mainWeapon.meshedPartOfWeapon;
    }
    void NavAgentSetter()
    {
        navAgent.speed = walkSpeed;
        navAgent.acceleration = walkAcceleration;
    }
    void AnimStateManage()
    {
        float forwardSpeed = transform.InverseTransformVector(navAgent.velocity).z;
        if (forwardSpeed > 0.25f)
        {
            if(forwardSpeed < (runSpeed + walkSpeed) / 2)
            {
                animStateSpeed = AnimStateSpeed.walk;
            }
            else
            {
                animStateSpeed = AnimStateSpeed.run;
            }
                animStatePriDir = AnimStatePriDir.front;

        }
        else
        {
            animStateSpeed = AnimStateSpeed.idle;

        }
    }
    void EnemyStateManager()
    {
        SetAimingBools();
        switch (enemyState)
        {
            case EnemyAIState.Patrol:
                PatrolFunction();
                break;
            case EnemyAIState.SemiDetected:
                SemiDetectedFunction();
                break;
            case EnemyAIState.Alerted:
                AlertedFunction();
                break;
            case EnemyAIState.Searching:
                SearchingFunction();
                break;
            case EnemyAIState.GameOver: 
                GameOverFunction();
                break;
        }
    }
    void GeneralEnemyUpdate()
    {
        visibleRangeInUse = visibleRange;
        targetTransform = mainCharScript.headOfChar;
        if (GameManager.mainState == PlayerState.inMainCar)
        {
            visibleRangeInUse *= 2;
            targetTransform = mainCar;
        }


        sqrDistFromPlayer = (targetTransform.position - enemyEyes.position).sqrMagnitude;
        if (navAgent.destination != null)
        {
            Vector3 offset = new Vector3(0,1.5f,0);
            Debug.DrawRay(navAgent.destination + offset,
                (new Vector3(0.6f, 0, 0)), Color.red);
            Debug.DrawRay(navAgent.destination + offset,
                    (new Vector3(-0.6f, 0, 0)), Color.red);
            Debug.DrawRay(navAgent.destination + offset,
                    (new Vector3(0, 0, 0.6f)), Color.red);
            Debug.DrawRay(navAgent.destination + offset,
                    (new Vector3(0, 0, -0.6f)), Color.red);
            Debug.DrawRay(navAgent.destination + offset,
                (new Vector3(0, -1, 0)), Color.green);

        }
        EnemyManager.enemiesCanSee[campOfEnemy - 1][enemyNumCode] = canSeeTarget;

        if(GameManager.mainState == PlayerState.gameOver && enemyState != EnemyAIState.GameOver)
        {
            ChangeEnemyAIState(EnemyAIState.GameOver);
        }
    }
    IEnumerator PermanentPlaceCoverCheck()
    {
        yield return new WaitForSeconds(alertedCoverCheckCooldown);
        if(enemyState == EnemyAIState.Alerted)
        {
            permanentCoverObject.SortPointsByDistance(targetTransform.position);
        }
        StartCoroutine(PermanentPlaceCoverCheck());
    }

    #region State Functions
    void PatrolFunction()
    {
        if (enteredNewState)
        {
            navAgent.speed = walkSpeed;
            navAgent.acceleration = walkAcceleration;
            navAgent.isStopped = false;
            if(lastPatrolIndex != 0)
            {
                navAgent.SetDestination(patrolPoints[0]);
            }
            else
            {
                navAgent.SetDestination(patrolPoints[lastPatrolIndex]);
            }
            isAiming = false;
            enteredNewState = false;
        }
        if(navAgent.remainingDistance < navAgent.stoppingDistance && !navAgent.isStopped && patrolWaitCoroutine == null)
        {
            //Debug.Log("Remaining Distance : "+ navAgent.remainingDistance);
            //Debug.Log("Stopping Distance : "+ navAgent.stoppingDistance);
            //Debug.Log("Started Coroutine");
            patrolWaitCoroutine = StartCoroutine(WaitForOtherPatrolPoint());
        }
        if(canSeeTarget)
        {
            if (sqrDistFromPlayer < ((visibleRange * visibleRange) * (alertRangeRate* alertRangeRate)))
            {
                ChangeEnemyAIState(EnemyAIState.Alerted);
                return;
            }
            else
            {
                ChangeEnemyAIState(EnemyAIState.SemiDetected);
            }
        }
    }
    IEnumerator WaitForOtherPatrolPoint()
    {
        StopNavMovement();
        yield return new WaitForSeconds(patrolWaitDuration);
        navAgent.isStopped = false;
        lastPatrolIndex = (byte)((lastPatrolIndex + 1) % patrolPoints.Length);
        navAgent.SetDestination(patrolPoints[lastPatrolIndex]);
        //Debug.Log(navAgent.destination);
        patrolWaitCoroutine = null;
    }

    void SemiDetectedFunction()
    {
        if (enteredNewState)
        {
            StopNavMovement();
            if (patrolWaitCoroutine != null) StopCoroutine(patrolWaitCoroutine);
            noticeCountdown = 0;
            enteredNewState = false;
        }
        noticeCountdown += Time.deltaTime;
        if(canSeeTarget)
        {
            if (cancelSemiDetectCoroutine != null) StopCoroutine(cancelSemiDetectCoroutine);
            if (sqrDistFromPlayer < ((visibleRange * visibleRange) * (alertRangeRate * alertRangeRate)))
            {
                ChangeEnemyAIState(EnemyAIState.Alerted);
                return;
            }

            realDistFromPlayer = Mathf.Sqrt(sqrDistFromPlayer);
            float noticeDuratOnApply = noticeDuration + noticeDuration * 
                                       ((realDistFromPlayer - (visibleRange* alertRangeRate))/(visibleRange - visibleRange*alertRangeRate));
            //Debug.Log(noticeDuratOnApply);
            if(noticeCountdown > noticeDuratOnApply)
            {
                ChangeEnemyAIState(EnemyAIState.Alerted);
            }
            else
            {
                RotateCharToLookAt(targetTransform.position, 0.1f);
            }
        }
        else if(!canSeeTarget)
        {
            if (cancelSemiDetectCoroutine == null)
                cancelSemiDetectCoroutine = StartCoroutine(IfCantSeeBackToPatrol());

            if(noticeCountdown > noticeDuration/2)
            {

            }
        }

    }
    IEnumerator IfCantSeeBackToPatrol()
    {
        yield return new WaitForSeconds(suspectedDuration);
        ChangeEnemyAIState(EnemyAIState.Patrol);
    }

    void AlertedFunction()
    {
        if(weaponState == WeaponState.ranged)
        {
            if (enteredNewState)
            {
                navAgent.ResetPath();
                StopNavMovement();
                navAgent.speed = runSpeed;
                navAgent.acceleration = runAcceleration;
                if (!hasPermanentPlace)
                {
                    StartCoroutine(IsCoveredSettingCoroutine());
                    navAgent.SetDestination(CoverObjectsManager.GetCoverPoint(mainCharScript.closestCamp,this));
                }
                else
                {
                    navAgent.SetDestination(permanentCoverObject.coverPoints[0].worldPos);
                }
                checkCoverCoroutine = StartCoroutine(AlertCoverCheckPeriodically(alertedCoverCheckCooldown));
                StartCoroutine(AlertEntireCamp());
                shouldFire = true;
                if (patrolWaitCoroutine != null) StopCoroutine(patrolWaitCoroutine);
                enteredNewState = false;
            }
            if (navAgent.remainingDistance < navAgent.stoppingDistance && !navAgent.isStopped)
            {
                StopNavMovement();
            }
            else if(navAgent.remainingDistance < navAgent.stoppingDistance && navAgent.isStopped)
            {
                RotateCharToLookAt(targetTransform.position, 0.1f);
                OnCoverBehaviour();
            }
            else
            {
                isAiming = false;
                navAgent.isStopped = false;
                if (angleX < 15 && angleY < 15 && canShoot)
                {
                    isAiming = true;
                    EnemyFire(false);
                }
                if (isCrouching) CrouchOrStand();
            }
            if (currentWeapon.currentAmmo == 0 && weaponState == WeaponState.ranged)
            {
                if (ammoCounts[(int)currentWeapon.weaponType] > 0)
                {
                    currentWeapon.Reload();
                }
                else
                {
                    if (currentWeapon == mainWeapon)
                    {
                        if(mainWeapon.weaponType == WeaponType.SR_1)
                        {
                            visibleRange /= 2;
                        }
                        ChangeWeapon(weaponScripts[2]);
                    }
                    else if(currentWeapon.weaponType == WeaponType.Pistol)
                    {
                        GetMeleeWeaponOrHandsFree(WeaponState.melee);
                        if (navAgent.isStopped) navAgent.isStopped = false;
                        if (checkCoverCoroutine != null)
                        {
                            StopCoroutine(checkCoverCoroutine);
                        }
                        if (isCrouching) CrouchOrStand();
                        navAgent.stoppingDistance = 0.7f;
                        IsCoveredSetter(true);
                    }
                }

            }

        }
        else
        {
            navAgent.SetDestination(targetTransform.position);
            RotateCharToLookAt(targetTransform.position, 0.05f);
            if(sqrDistFromPlayer < meleeRange*meleeRange)
            {
                EnemyFire();
            }
        }




    }
    IEnumerator AlertCoverCheckPeriodically(float frequency)
    {
        if (sqrDistFromPlayer > (visibleRange * visibleRange) / 4 && mainCharScript.closeToCampEnough)
        {
            if (isCrouching) { CrouchOrStand(); }
            StartCoroutine(IsCoveredSettingCoroutine());
            navAgent.SetDestination(CoverObjectsManager.GetCoverPoint(mainCharScript.closestCamp, this));
            if(currentWeapon.currentAmmo < currentWeapon.maxAmmo && ammoCounts[(int)currentWeapon.weaponType] > 0 && canReload)
            {
                currentWeapon.Reload();
            }
        }
        else if (!currentCoverPoint.crouchOrPeek && Mathf.Abs(Vector3.Angle(-currentCoverPoint.coverForwardForPeek, 
            currentCoverPoint.worldPos - new Vector3(targetTransform.position.x, currentCoverPoint.worldPos.y, targetTransform.position.z))) > 15)
        {
            StartCoroutine(IsCoveredSettingCoroutine());
            navAgent.SetDestination(CoverObjectsManager.GetCoverPoint(mainCharScript.closestCamp, this, true));
        }
        yield return new WaitForSeconds(Random.Range(frequency, frequency + 1));
        if(enemyState == EnemyAIState.Alerted)
        {
            checkCoverCoroutine = StartCoroutine(AlertCoverCheckPeriodically(frequency));
        }
    }
    IEnumerator AlertEntireCamp()
    {
        yield return new WaitForSeconds(waitBeforeAlertingAllDuration);
        EnemyManager.AlertWholeCamp(campOfEnemy);
    }
    void OnCoverBehaviour()
    {
        Vector2 targetVec = new Vector2(targetTransform.position.x, targetTransform.position.z) -
                    new Vector2(transform.position.x, transform.position.z);
        Vector2 enemyForward2 = new Vector2(enemyEyes.forward.x, enemyEyes.forward.z);

        if (Vector2.Angle(enemyForward2, targetVec) < 50)
        {
            isAiming = true;
            if (shouldFire && canSeeTarget)
            {
                if (!Physics.Raycast(currentWeapon.transform.position + currentWeapon.transform.TransformVector(currentWeapon.bulletLaunchOffset), 
                                    currentWeapon.transform.forward, 1))
                {
                    EnemyFire(true);
                }
            }
        }
        else
        {
            isAiming = false;
        }



        if (currentCoverPoint.crouchOrPeek)
        {
            if(peekableCoverCoroutine != null)
            {
                StopCoroutine(peekableCoverCoroutine);
            }
            if(shotsSinceLastCrouch >= numberOfShotsBeforeCrouch && !isCrouching && canCrouch)
            {
                StartCoroutine(WaitOnCrouch());
            }
        }
        else
        {
            if (shotsSinceLastCrouch >= numberOfShotsBeforeCrouch && peekableCoverCoroutine == null)
            {
                peekableCoverCoroutine = StartCoroutine(WaitBehindCover());
            }
        }


    }
    void EnemyFire(bool onCover = false)
    {
        if(weaponState == WeaponState.ranged)
        {
            if (canShoot && !isCrouching && currentWeapon.currentAmmo > 0)
            {
                currentWeapon.Fire();
                if (onCover) { shotsSinceLastCrouch++; }
                StartCoroutine(PickRandomFrequencyToFire(shootingFrequency));
            }
        }
        else if(weaponState == WeaponState.melee && mainMelee.canSwing)
        {
            mainMelee.Swing();
        }
    }
    IEnumerator PickRandomFrequencyToFire(float frequencyLimit)
    {
        float extraWait = Random.Range(0, frequencyLimit);
        shouldFire = false;
        yield return new WaitForSeconds(currentWeapon.firingTime + currentWeapon.firingTime*extraWait);
        shouldFire = true;

    }
    void EnemyReloadCheck()
    {
        if (currentWeapon.currentAmmo == 0)
        {
            canShoot = false;
            if (ammoCounts[(int)currentWeapon.weaponType] > 0 && canReload)
            {
                currentWeapon.Reload();
            }
        }

    }
    IEnumerator WaitOnCrouch()
    {
        CrouchOrStand();
        if (isCrouching)
        {
            EnemyReloadCheck();
        }
        yield return new WaitForSeconds(Random.Range(averageCrouchDuration-1,averageCrouchDuration+1));
        if (isCrouching) CrouchOrStand();
        shotsSinceLastCrouch = 0;
    }
    IEnumerator WaitBehindCover()
    {
        navAgent.SetDestination(currentCoverPoint.worldPos + currentCoverPoint.coverForwardForPeek * currentCoverPoint.peekCoverDistanceFromCenter);
        EnemyReloadCheck();
        yield return new WaitForSeconds(Random.Range(averageCrouchDuration - 1, averageCrouchDuration + 1));
        shotsSinceLastCrouch = 0;
        navAgent.SetDestination(currentCoverPoint.worldPos + currentCoverPoint.coverForwardForPeek * currentCoverPoint.peekCoverDistanceFromCenter * 0.75f + 
            StairCheckScript.RotateVecAroundVec(currentCoverPoint.coverForwardForPeek* currentCoverPoint.peekCoverDistanceFromCenter * 0.75f, Vector3.up, 90));
        peekableCoverCoroutine = null;
    }

    IEnumerator IsCoveredSettingCoroutine()
    {
        previousCoverIndexes = GameManager.CopyArray(currentCoveredIndexes, previousCoverIndexes);
        yield return null;
        IsCoveredSetter();
    }
    void IsCoveredSetter(bool deleteCurrent = false)
    {
        if(!GameManager.CompareArray(previousCoverIndexes, currentCoveredIndexes) && !deleteCurrent)
        {
            for(int i = CoverObjectsManager.coverObjectsOfWorld[previousCoverIndexes[0]].Length - 1; i >= 0; i--)
            {
                CoverTakeableObject coverObj = CoverObjectsManager.coverObjectsOfWorld[previousCoverIndexes[0]][i];
                if(coverObj.coveredObjectIndex == previousCoverIndexes[1])
                {
                    for (int j = coverObj.coverPoints.Length - 1; j >= 0; j--)
                    {
                        if (coverObj.coverPoints[j].coveredPointIndex == previousCoverIndexes[2])
                        {
                            coverObj.coverPoints[j].isCoveredAlready = false;
                            break;
                        }
                    }
                    break;
                }
            }
        }
        else if (deleteCurrent)
        {
            for (int i = CoverObjectsManager.coverObjectsOfWorld[currentCoveredIndexes[0]].Length - 1; i >= 0; i--)
            {
                CoverTakeableObject coverObj = CoverObjectsManager.coverObjectsOfWorld[currentCoveredIndexes[0]][i];
                if (coverObj.coveredObjectIndex == currentCoveredIndexes[1])
                {
                    for (int j = coverObj.coverPoints.Length - 1; j >= 0; j--)
                    {
                        if (coverObj.coverPoints[j].coveredPointIndex == currentCoveredIndexes[2])
                        {
                            coverObj.coverPoints[j].isCoveredAlready = false;
                            break;
                        }
                    }
                    break;
                }
            }

        }
    }

    void SearchingFunction()
    {
        if (enteredNewState)
        {
            navAgent.speed = runSpeed;
            navAgent.acceleration = runAcceleration;
            if(checkCoverCoroutine != null)
            {
                StopCoroutine(checkCoverCoroutine);
            }
            if (peekableCoverCoroutine != null)
            {
                StopCoroutine(peekableCoverCoroutine);
            }
            if (!hasPermanentPlace)
            {
                searchPositioningCoroutine = StartCoroutine(SearchForPlayerAroundLastSeenPos());
            }
            timeSinceStartedSearch = 0;
            isAiming = false;

            enteredNewState = false;
        }

        if (!hasPermanentPlace)
        {
            timeSinceStartedSearch += Time.deltaTime;

            if(timeSinceStartedSearch > searchDuration)
            {
                if(searchPositioningCoroutine != null)
                {
                    StopCoroutine(searchPositioningCoroutine);
                }
                ChangeEnemyAIState(EnemyAIState.Patrol);
                return;
            }
            if(canSeeTarget)
            {
                if (searchPositioningCoroutine != null)
                {
                    StopCoroutine(searchPositioningCoroutine);
                }
                ChangeEnemyAIState(EnemyAIState.Alerted);
            }

        }
        else
        {
            RotateCharToLookAt(EnemyManager.lastSeenPosOfPlayer[campOfEnemy - 1], 0.05f);
        }
    }
    IEnumerator SearchForPlayerAroundLastSeenPos()
    {
        navAgent.SetDestination(PickPointAroundLastSeenPos());
        while (true)
        {
            yield return null;
            if (navAgent.remainingDistance < navAgent.stoppingDistance)
            {
                StopNavMovement();
                break;
            }
        }
        yield return new WaitForSeconds(lookElsewhereFrequency + Random.Range(-2f,2f));
        navAgent.isStopped = false;
        searchPositioningCoroutine = StartCoroutine(SearchForPlayerAroundLastSeenPos());
    }
    Vector3 PickPointAroundLastSeenPos()
    {
        Vector3 basePos = EnemyManager.lastSeenPosOfPlayer[campOfEnemy - 1];
        Vector3 pointToSearchOn;
        byte countTimes = 0;
        while (countTimes < 30)
        {
            float xPos = Random.Range(-searchRangeHorizontal + targetTransform.position.x, searchRangeHorizontal + targetTransform.position.x);
            float zPos = Random.Range(-searchRangeHorizontal+ targetTransform.position.z, searchRangeHorizontal + targetTransform.position.z);
            float yPos = Random.Range(-searchRangeVertical + targetTransform.position.y, searchRangeVertical + targetTransform.position.y);
            pointToSearchOn = basePos+new Vector3(xPos,yPos,zPos);
            if (IsPointOnNavMesh(pointToSearchOn))
            {
                //Debug.Log("Point to search is : " + pointToSearchOn);
                return pointToSearchOn;
            }
            countTimes++;
        }
        return basePos;
    }
    public static bool IsPointOnNavMesh(Vector3 point)
    {
        if (NavMesh.SamplePosition(point, out NavMeshHit hit, 0.5f, NavMesh.AllAreas))
        {
            return true;
        }
        return false;
    }

    void GameOverFunction()
    {
        if (enteredNewState)
        {
            StopNavMovement();
            isAiming = false;
            shouldFire= false;

            enteredNewState = false;
        }
    }


    public void ChangeEnemyAIState(EnemyAIState newState)
    {
        enteredNewState = true;
        enemyState = newState;
        //Debug.Log("Enemy is now : " + newState);
    }
    #endregion
    void SetAimingBools()
    {
        if((previousAiming && isAiming)||(!previousAiming && !isAiming))
        {
            aimStarted = false;
            aimEnded = false;
        }
        else if(!previousAiming && isAiming)
        {
            aimStarted = true;
            aimEnded = false;
        }
        else if(previousAiming && !isAiming)
        {
            aimStarted = false;
            aimEnded = true;
        }
        previousAiming = isAiming;
    }
    void CheckEyeSight()
    {
        #region Visiualize EyesightQ
        Debug.DrawRay(enemyEyes.position, StairCheckScript.RotateVecAroundVec(enemyEyes.forward, enemyEyes.up, visibleAngleX, enemyEyes.right, visibleAngleY) * visibleRange, Color.gray);
        Debug.DrawRay(enemyEyes.position, StairCheckScript.RotateVecAroundVec(enemyEyes.forward, enemyEyes.up, -visibleAngleX, enemyEyes.right, visibleAngleY) * visibleRange, Color.gray);
        Debug.DrawRay(enemyEyes.position, StairCheckScript.RotateVecAroundVec(enemyEyes.forward, enemyEyes.up, visibleAngleX, enemyEyes.right, -visibleAngleY) * visibleRange, Color.gray);
        Debug.DrawRay(enemyEyes.position, StairCheckScript.RotateVecAroundVec(enemyEyes.forward, enemyEyes.up, -visibleAngleX, enemyEyes.right, -visibleAngleY) * visibleRange, Color.gray);
        #endregion

        if (sqrDistFromPlayer < visibleRangeInUse * visibleRangeInUse)
        {
            #region Calculate Eyesight Angle
            Vector3 fromEyesToPlayer = enemyEyes.InverseTransformVector((targetTransform.position - enemyEyes.position).normalized);
            Vector3 fromEyesToPlayerY = new Vector3(0, fromEyesToPlayer.y, fromEyesToPlayer.z);
            Vector3 fromEyesToPlayerX = new Vector3(fromEyesToPlayer.x, 0, fromEyesToPlayer.z);

            angleY = Mathf.Abs(Vector3.Angle(Vector3.forward, fromEyesToPlayerY));
            angleX = Mathf.Abs(Vector3.Angle(Vector3.forward, fromEyesToPlayerX));

            #endregion

            if (angleX <= visibleAngleX && angleY <= visibleAngleY)
            {
                Ray rayToPlayer = new Ray(enemyEyes.position, targetTransform.position - enemyEyes.position);
                if (Physics.Raycast(rayToPlayer, out RaycastHit hitInfo, visibleRangeInUse, ~0, QueryTriggerInteraction.Ignore))
                {
                    if (hitInfo.collider.CompareTag("Player") || (hitInfo.collider.CompareTag("Vehicle")))
                    {
                        canSeeTarget = true;
                        lastSeenPos = targetTransform.position;
                        Debug.DrawRay(rayToPlayer.origin, rayToPlayer.direction*hitInfo.distance, Color.green);
                    }
                    else
                    {
                        //Debug.Log("canSeeTarget falsed by different collider");
                        canSeeTarget = false;
                    }
                }
                else
                {
                    //Debug.Log("canSeeTarget falsed by not hitting");
                    canSeeTarget = false;
                }
            }
            else
            {
                //Debug.Log("canSeeTarget falsed by being out of field of view");
                canSeeTarget = false;
            }
        }
        else
        {
            //Debug.Log("canSeeTarget falsed by distance");
            canSeeTarget = false;
        }
    }
    void StopNavMovement()
    {
        navAgent.isStopped = true;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }
    public enum EnemyAIState { Patrol, SemiDetected, Alerted, Searching, GameOver}

    void GetAndDisableRagdollParts()
    {
        ragdollCols = skeleton.GetComponentsInChildren<Collider>();
        ragdollRbs = skeleton.GetComponentsInChildren<Rigidbody>();
        DisableRagdoll();
    }
    public void EnableRagdoll()
    {
        for (int ragdollIndex = ragdollCols.Length - 1; ragdollIndex >= 0; ragdollIndex--)
        {
            ragdollCols[ragdollIndex].enabled = true;
        }
        for (int ragdollIndex = ragdollRbs.Length - 1; ragdollIndex >= 0; ragdollIndex--)
        {
            ragdollRbs[ragdollIndex].isKinematic = false;
        }
        mainColl.enabled = false;
        rb.isKinematic = true;
        navAgent.enabled = false;
    }
    public void DisableRagdoll()
    {
        for(int ragdollIndex = ragdollCols.Length-1;ragdollIndex>= 0;ragdollIndex--)
        {
            ragdollCols[ragdollIndex].enabled = false;
        }
        for(int ragdollIndex = ragdollRbs.Length-1;ragdollIndex>= 0;ragdollIndex--)
        {
            ragdollRbs[ragdollIndex].isKinematic = true;
        }
        mainColl.enabled = true;
        rb.isKinematic = false;
        navAgent.enabled = true;
    }
}
