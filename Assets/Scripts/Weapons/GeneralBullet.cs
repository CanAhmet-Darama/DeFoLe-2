using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
[RequireComponent(typeof(Rigidbody))]

public class GeneralBullet : MonoBehaviour
{
    float maxLifespan = 4;
    public float duratPassed;
    Coroutine deactivateCor;
    public float bulletSpeed = 500;
    public GameObject itsHolder;

    void OnEnable()
    {
        duratPassed = 0;
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
    void OnCollisionEnter()
    {
        gameObject.SetActive(false);
    }

    //    IEnumerator DeactivateBullet(float durat)
    //    {
    //        yield return new WaitForSeconds(durat);
    //        gameObject.SetActive(false);
    //    }
}
