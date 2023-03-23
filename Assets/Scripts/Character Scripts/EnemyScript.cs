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
    bool noticingComplete;


    [Header("Alerted")]
    [SerializeField] float alertedCoverCheckCooldown;

    [Header("Searching")]
    Vector3 lastSeenPos;
    [SerializeField]float searchDuration;


    [Header("Using Weapons")]
    public float enemyInaccuracy;
    public float handShakeRate;
    public float shootingFrequency;

    void Start()
    {
        GeneralCharStart();
        NavAgentSetter();
        EnemyStart();
        ChangeWeapon(weapons[1].GetComponent<GeneralWeapon>());
    }

    void Update()
    {
        GeneralCharUpdate();
        sqrDistFromPlayer = (mainChar.position - enemyEyes.position).sqrMagnitude;
        if (Input.GetKeyDown(KeyCode.G))
        {
            navAgent.SetDestination(CoverObjectsManager.GetCoverPoint(1, mainChar.position).worldPos);
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
        enteredNewState = true;
        lastPatrolIndex = 0;
        mainChar = GameManager.mainChar;
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
            Debug.Log("Saw target");
            if (sqrDistFromPlayer < ((visibleRange * visibleRange) / 4))
            {
                ChangeEnemyAIState(EnemyAIState.Alerted);
            }
            else
            {
                ChangeEnemyAIState(EnemyAIState.SemiDetected);
            }
        }
    }
    IEnumerator WaitForOtherPatrolPoint()
    {
        navAgent.isStopped = true;
        rb.velocity = Vector3.zero; rb.angularVelocity = Vector3.zero;
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
            navAgent.isStopped = true;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            enteredNewState = false;
            noticingComplete= false;
            noticeCountdown = 0;
        }
        noticeCountdown += Time.deltaTime;
        if(canSeeTarget)
        {
            if(noticeCountdown > noticeDuration)
            {
                ChangeEnemyAIState(EnemyAIState.Alerted);
            }
            else
            {
                Vector3 targetRotation = new Vector3(mainChar.eulerAngles.x, 0, mainChar.eulerAngles.z) 
                                        - new Vector3(transform.eulerAngles.x, 0, transform.eulerAngles.z);
                RotateChar(targetRotation, 0.2f);
            }
        }
        else if(!canSeeTarget)
        {

        }

    }
    void AlertedFunction()
    {
        if (enteredNewState)
        {
            navAgent.isStopped = true;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            enteredNewState = false;
        }

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
public enum EnemyAIState { Patrol, SemiDetected, Alerted, Searching}
}
