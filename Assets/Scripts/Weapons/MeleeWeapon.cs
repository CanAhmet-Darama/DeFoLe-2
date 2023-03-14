using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : MonoBehaviour
{
    [Header("General")]
    public float damage;
    public float swingCooldown;
    public GeneralCharacter owner;
    public bool canSwing;
    public bool isSwinging;

    [Header("For Animation etc...")]
    public Vector3 rightHandPos;
    public Vector3 rightHandRot;
    public AnimatorOverrideController overrideController;

    [Header("Sound Stuff")]
    public AudioSource audioSource;
    public AudioClip sheathSound;
    public AudioClip swingSound;

    void Start()
    {
        canSwing = true;
        isSwinging = false;
    }
    public void Swing()
    {
        owner.animator.SetTrigger("fire");
        StartCoroutine(WaitToHitAgain());
    }
    IEnumerator WaitToHitAgain()
    {
        canSwing = false;
        yield return new WaitForSeconds(swingCooldown/3);
        isSwinging = true;
        audioSource.Play();
        yield return new WaitForSeconds(swingCooldown/3);
        isSwinging= false;
        yield return new WaitForSeconds(swingCooldown/3);
        yield return null;
        canSwing = true;
    }
    void OnTriggerEnter(Collider other)
    {
        if (isSwinging)
        {
            Vector3 contactPoint = other.ClosestPoint(transform.position);
            if (other.CompareTag("Ground"))
            {
                ImpactMarkManager.CallBladeMark(contactPoint + (transform.position - contactPoint).normalized*0.01f, (contactPoint - transform.position).normalized, other.GetComponent<EnvObject>().objectType);
            }
            else if(other.CompareTag("Player") || other.CompareTag("Enemy"))
            {
                ImpactMarkManager.MakeBloodImpactAndSound(contactPoint + (transform.position - contactPoint).normalized * 0.01f, (contactPoint - transform.position).normalized , false);
            }

        }
    }
}
