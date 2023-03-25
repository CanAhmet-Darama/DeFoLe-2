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
    public Transform targetPlayer;
    public bool canSeeTarget;
    public float sqrDistFromPlayer;
    bool enteredNewState;
    Transform mainChar;
    MainCharacter mainCharScript;
    [Range(1,3)]public byte campOfEnemy;

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
    byte shouldFire;


    [Header("Searching")]
    [SerializeField]float searchDuration;
    Vector3 lastSeenPos;


    [Header("Using Weapons")]
    [HideInInspector]public GeneralWeapon mainWeapon;
    public float enemyInaccuracy;
    public float handShakeRate;
    public float shootingFrequency;

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
        sqrDistFromPlayer = (mainChar.position - enemyEyes.position).sqrMagnitude;
        if (Input.GetKeyDown(KeyCode.G))
        {
            navAgent.SetDestination(CoverObjectsManager.GetCoverPoint(campOfEnemy).worldPos);
        }
        else if (Input.GetKeyDown(KeyCode.J))
        {
            if (navAgent.speed != 0)
            {
                navAgent.speed = 0;
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
            else navAgent.speed = walkSpeed;
        }
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

    #region State Functions
    void PatrolFunction()
    {
        if (enteredNewState)
        {
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
            navAgent.speed = runSpeed;
            navAgent.acceleration = runAcceleration;
            StopNavMovement();
            InvokeRepeating("AlertCoverCheckPeriodically", 0.5f, alertedCoverCheckCooldown);
            StartCoroutine(AlertEntireCamp());
            enteredNewState = false;
        }
        if (navAgent.remainingDistance < navAgent.stoppingDistance && !navAgent.isStopped)
        {
            StopNavMovement();
        }
        else if(navAgent.remainingDistance < navAgent.stoppingDistance && navAgent.isStopped)
        {
            RotateCharToLookAt(mainChar.position, 0.1f);
        }
        else
        {
            navAgent.isStopped = false;
        }
        
    }
    void AlertCoverCheckPeriodically()
    {
        if(sqrDistFromPlayer > (visibleRange*visibleRange)/4)
        {
            navAgent.SetDestination(CoverObjectsManager.GetCoverPoint(campOfEnemy).worldPos);
        }
    }
    IEnumerator AlertEntireCamp()
    {
        yield return new WaitForSeconds(waitBeforeAlertingAllDuration);
        EnemyManager.AlertWholeCamp(campOfEnemy);
    }
    void OnCoverBehaviour()
    {

    }

    void SearchingFunction()
    {

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
                Ray rayToPlayer = new Ray(enemyEyes.position, mainChar.position - enemyEyes.position);
                if (Physics.Raycast(rayToPlayer, out RaycastHit hitInfo,visibleRange, ~0,QueryTriggerInteraction.Ignore))
                {
                    if (hitInfo.collider.CompareTag("Player"))
                    {
                        targetPlayer = hitInfo.transform;
                        canSeeTarget = true;
                        Debug.DrawRay(rayToPlayer.origin, rayToPlayer.direction*hitInfo.distance, Color.green);
                    }
                }
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
