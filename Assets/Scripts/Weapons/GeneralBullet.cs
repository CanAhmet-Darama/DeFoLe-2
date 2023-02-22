using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
[RequireComponent(typeof(Rigidbody))]

public class GeneralBullet : MonoBehaviour
{
    float maxLifespan = 3;
    public float duratPassed;
    Coroutine deactivateCor;
    public float bulletSpeed = 500;
    public GameObject itsHolder;

    void OnEnable()
    {
        if(deactivateCor != null)
        {
        StopCoroutine(deactivateCor);
        }
    }
    void OnDisable()
    {
    }
    void Update()
    {
        duratPassed += Time.deltaTime;
        if(duratPassed > maxLifespan)
        {
            gameObject.SetActive(false);
        }
    }
    void OnCollisionEnter(Collision collision)
    {
        gameObject.SetActive(false);
    }

}
