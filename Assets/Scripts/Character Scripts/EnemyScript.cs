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
    public bool canSeeTarget;
    public float sqrDistFromPlayer;
    bool enteredNewState;
    Transform mainChar;
    MainCharacter mainCharScript;
    [Range(1,3)]public byte campOfEnemy;
    public byte enemyNumCode;

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
    [SerializeField][Range(0,1)]float alertRangeRate;
    [SerializeField] float waitBeforeAlertingAllDuration;
    Coroutine checkCoverCoroutine;
    #region Aiming and Fire
    bool shouldFire;
    byte numberOfShotsBeforeCrouch;
    byte shotsSinceLastCrouch;
    [SerializeField] float averageCrouchDuration;
    [HideInInspector] public bool isAiming;
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
    [Range(0,1)]public float shootingFrequency;
    [HideInInspector]public GeneralWeapon mainWeapon;


    void Start()
    {
        GeneralCharStart();
        NavAgentSetter();
        EnemyStart();
        ChangeWeapon(weapons[(int)mainWeapon.weaponType].GetComponent<GeneralWeapon>());
        EnemyManager.AddEnemyToList(campOfEnemy, this);
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
        lastPatrolIndex = 0;
        mainChar = GameManager.mainChar;
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
        }
    }
    void GeneralEnemyUpdate()
    {
        sqrDistFromPlayer = (mainChar.position - enemyEyes.position).sqrMagnitude;
        if(navAgent.destination != null)
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
    }

    #region State Functions
    void PatrolFunction()
    {
        if (enteredNewState)
        {
            navAgent.speed = walkSpeed;
            navAgent.acceleration = walkAcceleration;
            navAgent.isStopped = false;
            navAgent.SetDestination(patrolPoints[lastPatrolIndex]);
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
            enteredNewState = false;
            noticeCountdown = 0;
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
            Debug.Log(noticeDuratOnApply);
            if(noticeCountdown > noticeDuratOnApply)
            {
                ChangeEnemyAIState(EnemyAIState.Alerted);
            }
            else
            {
                RotateCharToLookAt(mainChar.position, 0.1f);
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
        if (enteredNewState)
        {
            StopNavMovement();
            navAgent.speed = runSpeed;
            navAgent.acceleration = runAcceleration;
            navAgent.SetDestination(CoverObjectsManager.GetCoverPoint(mainCharScript.closestCamp).worldPos);
            checkCoverCoroutine = StartCoroutine(AlertCoverCheckPeriodically(alertedCoverCheckCooldown));
            StartCoroutine(AlertEntireCamp());
            shouldFire = true;
            enteredNewState = false;
        }
        if (navAgent.remainingDistance < navAgent.stoppingDistance && !navAgent.isStopped)
        {
            StopNavMovement();
        }
        else if(navAgent.remainingDistance < navAgent.stoppingDistance && navAgent.isStopped)
        {
            RotateCharToLookAt(mainChar.position, 0.1f);
            OnCoverBehaviour();
        }
        else
        {
            navAgent.isStopped = false;
            if (isAiming)
            {
                EnemyFire(false);
            }
        }
    }
    IEnumerator AlertCoverCheckPeriodically(float frequency)
    {
        if (sqrDistFromPlayer > (visibleRange * visibleRange) / 4 && mainCharScript.closeToCampEnough)
        {
            if (isCrouching) CrouchOrStand();
            navAgent.SetDestination(CoverObjectsManager.GetCoverPoint(mainCharScript.closestCamp).worldPos);
            if(currentWeapon.currentAmmo < currentWeapon.maxAmmo && ammoCounts[(int)currentWeapon.weaponType] > 0 && canReload)
            {
                currentWeapon.Reload();
            }
        }
        yield return new WaitForSeconds(frequency);
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
        Vector2 targetVec = new Vector2(mainChar.position.x, mainChar.position.z) -
                    new Vector2(transform.position.x, transform.position.z);
        Vector2 enemyForward2 = new Vector2(enemyEyes.forward.x, enemyEyes.forward.z);
        if (Vector2.Angle(enemyForward2, targetVec) < 25)
        {
            if (!isAiming) { aimStarted = true;}
            else { aimStarted = false; }
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
            if (isAiming)
            {
                aimEnded = true;
            }
            else
            {
                aimEnded = false;
            }
            isAiming = false;
        }
        
        if(shotsSinceLastCrouch >= numberOfShotsBeforeCrouch && !isCrouching && canCrouch)
        {
            StartCoroutine(WaitOnCrouch());
        }
        EnemyReloadCheck();


    }
    void EnemyFire(bool onCover)
    {
        if (canShoot && !isCrouching)
        {
            currentWeapon.Fire();
            if (onCover) { shotsSinceLastCrouch++; }
            StartCoroutine(PickRandomFrequencyToFire(shootingFrequency));
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
        yield return new WaitForSeconds(Random.Range(averageCrouchDuration-1,averageCrouchDuration+1));
        if (isCrouching) CrouchOrStand();
        shotsSinceLastCrouch = 0;
    }

    void SearchingFunction()
    {
        if (enteredNewState)
        {
            StopNavMovement();
            navAgent.speed = runSpeed;
            navAgent.acceleration = runAcceleration;
            if(checkCoverCoroutine != null)
            {
                StopCoroutine(checkCoverCoroutine);
            }
            searchPositioningCoroutine=StartCoroutine(SearchForPlayerAroundLastSeenPos());
            timeSinceStartedSearch = 0;
            enteredNewState = false;
        }
        timeSinceStartedSearch += Time.deltaTime;

        if(timeSinceStartedSearch > searchDuration)
        {
            ChangeEnemyAIState(EnemyAIState.Patrol);
            if(searchPositioningCoroutine != null)
            {
                StopCoroutine(searchPositioningCoroutine);
            }
        }
        if(canSeeTarget)
        {
            ChangeEnemyAIState(EnemyAIState.Alerted);
            if (searchPositioningCoroutine != null)
            {
                StopCoroutine(searchPositioningCoroutine);
            }
        }
    }
    IEnumerator SearchForPlayerAroundLastSeenPos()
    {
        navAgent.SetDestination(PickPointAroundLastSeenPos());
        yield return new WaitForSeconds(lookElsewhereFrequency + Random.Range(-2f,2f));
        searchPositioningCoroutine = StartCoroutine(SearchForPlayerAroundLastSeenPos());
    }
    Vector3 PickPointAroundLastSeenPos()
    {
        Vector3 basePos = EnemyManager.lastSeenPosOfPlayer[campOfEnemy - 1];
        Vector3 pointToSearchOn;
        while (true)
        {
            float xPos = Random.Range(-searchRangeHorizontal, searchRangeHorizontal);
            float zPos = Random.Range(-searchRangeHorizontal, searchRangeHorizontal);
            float yPos = Random.Range(-searchRangeVertical, searchRangeVertical);
            pointToSearchOn = basePos+new Vector3(xPos,yPos,zPos);
            if (IsPointOnNavMesh(pointToSearchOn))
            {
                return pointToSearchOn;
            }
        }
    }
    public static bool IsPointOnNavMesh(Vector3 point)
    {
        if (NavMesh.SamplePosition(point, out NavMeshHit hit, 0.1f, NavMesh.AllAreas))
        {
            return true;
        }
        return false;
    }


    public void ChangeEnemyAIState(EnemyAIState newState)
    {
        enteredNewState = true;
        enemyState = newState;
        Debug.Log("Enemy is now : " + newState);
    }
    #endregion
    void CheckEyeSight()
    {
        #region Visiualize EyesightQ
        Debug.DrawRay(enemyEyes.position, StairCheckScript.RotateVecAroundVec(enemyEyes.forward, enemyEyes.up, visibleAngleX, enemyEyes.right, visibleAngleY) * visibleRange, Color.gray);
        Debug.DrawRay(enemyEyes.position, StairCheckScript.RotateVecAroundVec(enemyEyes.forward, enemyEyes.up, -visibleAngleX, enemyEyes.right, visibleAngleY) * visibleRange, Color.gray);
        Debug.DrawRay(enemyEyes.position, StairCheckScript.RotateVecAroundVec(enemyEyes.forward, enemyEyes.up, visibleAngleX, enemyEyes.right, -visibleAngleY) * visibleRange, Color.gray);
        Debug.DrawRay(enemyEyes.position, StairCheckScript.RotateVecAroundVec(enemyEyes.forward, enemyEyes.up, -visibleAngleX, enemyEyes.right, -visibleAngleY) * visibleRange, Color.gray);

        #endregion
        if (sqrDistFromPlayer < visibleRange * visibleRange)
        {
            #region Calculate Eyesight Angle
            Vector3 fromEyesToPlayer = enemyEyes.InverseTransformDirection((mainChar.position - enemyEyes.position).normalized);
            Vector3 fromEyesToPlayerY = new Vector3(0, fromEyesToPlayer.y,fromEyesToPlayer.z);
            Vector3 fromEyesToPlayerX = new Vector3(fromEyesToPlayer.x, 0, fromEyesToPlayer.z);

            float angleY = Mathf.Abs(Vector3.Angle(Vector3.forward, fromEyesToPlayerY));
            float angleX = Mathf.Abs(Vector3.Angle(Vector3.forward, fromEyesToPlayerX));

            #endregion

            if (angleX <= visibleAngleX && angleY <= visibleAngleY)
            {
                Ray rayToPlayer = new Ray(enemyEyes.position, mainCharScript.headOfChar.position - enemyEyes.position);
                if (Physics.Raycast(rayToPlayer, out RaycastHit hitInfo,visibleRange, ~0,QueryTriggerInteraction.Ignore))
                {
                    if (hitInfo.collider.CompareTag("Player"))
                    {
                        canSeeTarget = true;
                        Debug.DrawRay(rayToPlayer.origin, rayToPlayer.direction*hitInfo.distance, Color.green);
                    }
                    else
                    {
                        canSeeTarget = false;
                    }
                }
                else
                {
                    canSeeTarget = false;
                }
            }
            else
            {
                canSeeTarget = false;
            }
        }
    }
    void StopNavMovement()
    {
        navAgent.isStopped = true;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }
    public enum EnemyAIState { Patrol, SemiDetected, Alerted, Searching}
}
