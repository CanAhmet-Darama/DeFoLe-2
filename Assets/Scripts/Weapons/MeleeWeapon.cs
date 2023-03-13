using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : MonoBehaviour
{
    [Header("General")]
    public float damage;
    public float swingCooldown;
    public GeneralCharacter owner;
    public bool canSwing = true;

    [Header("For Animation etc...")]
    public Vector3 rightHandPos;
    public Vector3 rightHandRot;
    public AnimatorOverrideController overrideController;

    public void Swing()
    {
        owner.animator.SetTrigger("fire");
        StartCoroutine(WaitToHitAgain());
    }
    IEnumerator WaitToHitAgain()
    {
        canSwing = false;
        yield return new WaitForSeconds(swingCooldown);
        canSwing = true;
    }
}
