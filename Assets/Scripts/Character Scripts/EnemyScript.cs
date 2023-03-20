using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyScript : GeneralCharacter
{
    [Header("Enemy AI")]
    public NavMeshAgent navAgent;
    public EnemyAIState enemyState;
    public float visibleAngleX;
    public float visibleAngleY;
    public float visibleRange;

    [Header("Using Weapons")]
    public float enemyInaccuracy;

    void Start()
    {
        NavAgentSetter();
    }

    void Update()
    {
        AnimStateManage();
    }

    void NavAgentSetter()
    {
        navAgent.speed = walkSpeed;
        navAgent.acceleration = walkAcceleration;
    }
    void NavTarget()
    {
        navAgent.SetDestination(GameManager.mainCar.transform.position + new Vector3(0, 0, -5));
    }
    void AnimStateManage()
    {
        float forwardSpeed = transform.InverseTransformVector(navAgent.velocity).z;
        if (forwardSpeed > 0.03f)
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

    }
    public enum EnemyAIState { Patrol, SemiDetected, Alerted, Searching}
}
