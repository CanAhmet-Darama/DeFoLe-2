using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeapSubAnimer : MonoBehaviour
{

    public static void WeapunSubAnimReload(Animator animator)
    {
        animator.SetTrigger("reloading");
    }
    public static void WeaponSubAnimFire(Animator animator)
    {
        animator.SetTrigger("firing");

    }

}
