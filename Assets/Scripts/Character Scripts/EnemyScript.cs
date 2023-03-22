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

    [Header("Enemy AI States")]
    bool enteredNewState;
    public bool hasPermanentPlace;
    public CoverTakeableObject permanentCoverObject;
    public Vector3[] patrolPoints;
    public byte lastPatrolIndex;
    public float patrolWaitDuration;
    Coroutine patrolWaitCoroutine;
    public float alertedCoverCheckCooldown;
    public float searchDuration;


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
        if (Input.GetKeyDown(KeyCode.G))
        {
            navAgent.SetDestination(CoverObjectsManager.GetCoverPoint(1, GameManager.mainChar.position).worldPos);
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
    }
    IEnumerator WaitForOtherPatrolPoint()
    {
        navAgent.isStopped = true;
        rb.velocity = Vector3.zero; rb.angularVelocity = Vector3.zero;
        yield return new WaitForSeconds(patrolWaitDuration);
        navAgent.isStopped = false;
        lastPatrolIndex = (byte)((lastPatrolIndex + 1) % patrolPoints.Length);
        navAgent.SetDestination(patrolPoints[lastPatrolIndex]);
        Debug.Log(navAgent.destination);
        patrolWaitCoroutine = null;
    }
    void SemiDetectedFunction()
    {

    }
    void AlertedFunction()
    {

    }
    void SearchingFunction()
    {

    }

    public void ChangeEnemyAIState(EnemyAIState newState)
    {
        enteredNewState = true;
        enemyState = newState;
    }
    #endregion
    void CheckEyeSight()
    {
        #region Visiualize EyesightQ
        Debug.DrawRay(enemyEyes.position, StairCheckScript.RotateVecAroundVec(enemyEyes.forward, enemyEyes.up, visibleAngleX) * visibleRange, Color.white);
        Debug.DrawRay(enemyEyes.position, StairCheckScript.RotateVecAroundVec(enemyEyes.forward, enemyEyes.up, -visibleAngleX) * visibleRange, Color.white);
        Debug.DrawRay(enemyEyes.position, StairCheckScript.RotateVecAroundVec(enemyEyes.forward, enemyEyes.right, visibleAngleY) * visibleRange, Color.white);
        Debug.DrawRay(enemyEyes.position, StairCheckScript.RotateVecAroundVec(enemyEyes.forward, enemyEyes.right, -visibleAngleY) * visibleRange, Color.white);
        #endregion
        float sqrDistBetweenPlayerEnemy = (GameManager.mainChar.position - enemyEyes.position).sqrMagnitude;
        if (sqrDistBetweenPlayerEnemy < visibleRange * visibleRange)
        {
            Ray rayToPlayer = new Ray(enemyEyes.position, GameManager.mainChar.position - enemyEyes.position);
            bool sawTarget = false;

            if (Physics.Raycast(rayToPlayer, out RaycastHit hitInfo,visibleRange, ~0,QueryTriggerInteraction.Ignore))
            {
                    if (hitInfo.collider.CompareTag("Player"))
                    {
                        #region Calculate Eyesight Angle
                        Vector3 relativeTargetPos = enemyEyes.InverseTransformVector(GameManager.mainChar.position);
                        float angleY = Mathf.Abs(relativeTargetPos.y - enemyEyes.position.y);
                        float angleX = Mathf.Abs(relativeTargetPos.x - enemyEyes.position.x);
                        if(angleX <= visibleAngleX && angleY <= visibleAngleY)
                        {
                            targetPlayer = hitInfo.transform;
                            canSeeTarget = true;
                        }
                        #endregion



                    }
            }
            if (!sawTarget)
            {
                    canSeeTarget = false;

            }

        }
    }
public enum EnemyAIState { Patrol, SemiDetected, Alerted, Searching}
}
