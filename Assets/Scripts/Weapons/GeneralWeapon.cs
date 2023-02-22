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
    public WeaponType weaponType;
    [Space(2)]
    public GameObject bullet;
    public GameObject[] bulletPool;
    public GameObject bulletPoolHolder;
    public ParticleSystem muzzleFlash;
    public AudioClip firingSound;
    public AudioSource gunAudioSource;
    public AudioClip reloadSound;

    [Header("For Animation etc")]
    public Vector3 leftHandPos;
    public Vector3 bulletLaunchOffset;


    protected void Start()
    {
        if(weaponType == WeaponType.AR_1 || weaponType == WeaponType.TR_1)
        {
            bulletPool = new GameObject[15];
        }
        else
        {
            bulletPool = new GameObject[6];
        }
        for(int i = bulletPool.Length; i > 0; i--)
        {
            bulletPool[i - 1] = Instantiate(bullet, Vector3.zero, transform.rotation, bulletPoolHolder.transform);
            bulletPool[i - 1].transform.localPosition = Vector3.zero;
            bulletPool[i - 1].SetActive(false);
            bulletPool[i - 1].GetComponent<GeneralBullet>().itsHolder = bulletPoolHolder;
        }
        gunAudioSource.volume = 0.05f;
    }

    public void Reload()
    {

    }
    public void Fire()
    {
        GameObject bulletToShoot = GetAmmo();
        GeneralBullet gBullet = bulletToShoot.GetComponent<GeneralBullet>();
        bulletToShoot.GetComponent<TrailRenderer>().enabled = false;
        bulletToShoot.transform.parent = bulletPoolHolder.transform;
        bulletToShoot.transform.localPosition = bulletLaunchOffset;
        gBullet.itsHolder = bulletPoolHolder;
        gBullet.duratPassed = 0;
        bulletToShoot.transform.parent = null;
        bulletToShoot.GetComponent<TrailRenderer>().enabled = true;
        bulletToShoot.SetActive(true);

        bulletToShoot.GetComponent<Rigidbody>().velocity = bulletToShoot.GetComponent<GeneralBullet>().bulletSpeed * transform.forward;
        if (gunAudioSource.clip != firingSound) gunAudioSource.clip = firingSound;
        gunAudioSource.Play();
    }
    public GameObject GetAmmo()
    {
        for(int i = bulletPool.Length; i > 0; i--)
        {
            if (!bulletPool[i - 1].activeInHierarchy)
            {
                bulletPool[i - 1].SetActive(false);
                return bulletPool[i - 1];
            }
        }
        GameObject oldestBullet = bulletPool[bulletPool.Length - 1];
        for (int i = bulletPool.Length; i > 0; i--)
        {
            if(bulletPool[i - 1].GetComponent<GeneralBullet>().duratPassed > oldestBullet.GetComponent<GeneralBullet>().duratPassed)
            {
                oldestBullet = (bulletPool[i - 1]);
            }
        }
        oldestBullet.SetActive(false);
        return oldestBullet;
    }
}
    public enum WeaponType { AR_1, TR_1, SR_1, Shotgun, Pistol}
