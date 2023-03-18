using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyScript : GeneralCharacter
{
    [Header("Enemy AI")]
    public NavMeshAgent navAgent;

    void Start()
    {
        NavAgentSetter();
        Invoke("NavTarget", 5);
    }

    void Update()
    {
        
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
        if (transform.InverseTransformVector(navAgent.velocity).z > 0)
        {
            animStateSpeed = AnimStateSpeed.walk;
        }
    }
}
