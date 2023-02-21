using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralWeapon : MonoBehaviour
{
    [Header("General")]
    public float damage;
    public float range;
    public float firingTime;
    public float reloadTime;
    public float inaccuracyDegree;
    public byte maxAmmo;
    public byte currentAmmo;
    [Space(2)]
    public GameObject bullet;
    public ParticleSystem muzzleFlash;
    public AudioClip firingSound;
    public AudioSource gunAudioSource;
    public AudioClip reloadSound;

    [Header("For Animation etc")]
    public Vector3 leftHandPos;


    public void Reload()
    {

    }
    public void Fire()
    {

    }

}
