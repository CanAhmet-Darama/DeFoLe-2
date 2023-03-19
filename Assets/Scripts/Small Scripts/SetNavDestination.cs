using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SetNavDestination : MonoBehaviour
{
    [SerializeField] NavMeshAgent agent;
    [SerializeField] bool setTarget;
    bool hasSetTarget;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            setTarget = !setTarget;
            if (setTarget == true)
            {
                hasSetTarget = false;
            }    
        }
        if (setTarget && !hasSetTarget)
        {
            hasSetTarget = true;
            agent.SetDestination(transform.position);
        }
    }
}
