using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralWeapon : MonoBehaviour
{
    [Header("General")]
    public GeneralCharacter owner;
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
<<<<<<< HEAD
    public GeneralBullet[] bulletScripts;
    public LineRenderer[] lineRenderers;
=======
>>>>>>> parent of ef6e371 (Commit 17_3)
    public GameObject bulletPoolHolder;
    public ParticleSystem muzzleFlash;
    public AudioClip firingSound;
    public AudioSource gunAudioSource;
    public AudioClip reloadSound;

    [Header("For Animation etc")]
    public Vector3 leftHandPos;
    public Vector3 bulletLaunchOffset;
    public Vector3 rightHandPosOffset;
    public Vector3 rightHandRotOffset;
    public AnimationClip idleAnim;
    public AnimationClip semiIdleAnim;
    public AnimationClip aimAnim;
    public AnimationClip fireAnim;
    public AnimationClip reloadAnim;
    public byte animOverriderIndex;



    protected void Start()
    {
        if(weaponType == WeaponType.AR_1 || weaponType == WeaponType.TR_1)
        {
            bulletPool = new GameObject[16];
        }
        else if(weaponType == WeaponType.Shotgun)
        {
            bulletPool = new GameObject[24];
        }
        else
        {
            bulletPool = new GameObject[5];
        }
<<<<<<< HEAD
        lineRenderers =  new LineRenderer[bulletPool.Length];
        bulletScripts = new GeneralBullet[bulletPool.Length];
=======
>>>>>>> parent of ef6e371 (Commit 17_3)

        for(int i = bulletPool.Length; i > 0; i--)
        {
            bulletPool[i - 1] = Instantiate(bullet, Vector3.zero, transform.rotation, bulletPoolHolder.transform);
            bulletPool[i - 1].transform.localPosition = Vector3.zero;
<<<<<<< HEAD

            bulletScripts[i - 1] = bulletPool[i - 1].GetComponent<GeneralBullet>();
            lineRenderers[i - 1] = bulletPool[i - 1].GetComponent<LineRenderer>();

            bulletScripts[i - 1].index = (byte)(i - 1);
            bulletScripts[i - 1].itsHolder = bulletPoolHolder;
            bulletScripts[i - 1].itsOwnerWeapon = this;
=======
>>>>>>> parent of ef6e371 (Commit 17_3)
            bulletPool[i - 1].SetActive(false);
            bulletPool[i - 1].GetComponent<GeneralBullet>().itsHolder = bulletPoolHolder;
            bulletPool[i - 1].GetComponent<GeneralBullet>().itsOwnerWeapon = this;

        }
        gunAudioSource.volume = 0.05f;
        currentAmmo = maxAmmo;
    }

    public void Reload()
    {
        owner.StartCoroutine("CanShootAgain", reloadTime);
    }
    public void Fire()
    {
        byte bulletPerShot = 1;
        if (weaponType == WeaponType.Shotgun) { bulletPerShot = 12; }

        for (byte i = bulletPerShot; i >0; i--)
        {
            GameObject bulletToShoot = GetAmmo();

<<<<<<< HEAD
            bulletToShoot.GetComponent<LineRenderer>().enabled = false;
            bulletToShoot.SetActive(false);

=======
>>>>>>> parent of ef6e371 (Commit 17_3)
            bulletToShoot.transform.parent = bulletPoolHolder.transform;
            bulletToShoot.transform.localPosition = bulletLaunchOffset;
            bulletToShoot.transform.SetParent(null);

            bulletToShoot.SetActive(true);
            GeneralBullet gBullet = bulletToShoot.GetComponent<GeneralBullet>();
            gBullet.duratPassed = 0;
            gBullet.firedPos = bulletToShoot.transform.position;


            float inaccX = Random.Range(-inaccuracyDegree / 100, inaccuracyDegree / 100);
            float inaccY = Random.Range(-inaccuracyDegree / 100, inaccuracyDegree / 100);
            float launchSpeed = bulletToShoot.GetComponent<GeneralBullet>().bulletSpeed;
            bulletToShoot.GetComponent<Rigidbody>().velocity = launchSpeed * transform.forward;
            bulletToShoot.GetComponent<Rigidbody>().velocity += (transform.right * inaccX * launchSpeed) + (transform.up * inaccY * launchSpeed);
            StartCoroutine(AFrameThenTrail(bulletToShoot));
        }

        if (gunAudioSource.clip != firingSound) gunAudioSource.clip = firingSound;
        gunAudioSource.Play();
        muzzleFlash.Play();
        owner.StartCoroutine("CanShootAgain", firingTime);

    }
    public GameObject GetAmmo()
    {
        for(int i = bulletPool.Length; i > 0; i--)
        {
            if (!bulletPool[i - 1].activeInHierarchy && bulletPool[i - 1].transform.parent != null)
            {
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
        oldestBullet.GetComponent<TrailRenderer>().enabled = false;
        oldestBullet.SetActive(false);
        return oldestBullet;
    }

    IEnumerator AFrameThenTrail(GameObject bullet)
    {
        yield return null;
<<<<<<< HEAD
        bullet.GetComponent<LineRenderer>().enabled = true;
=======
        yield return null;
        bullet.GetComponent<TrailRenderer>().enabled = true;
>>>>>>> parent of ef6e371 (Commit 17_3)
    }
}
    public enum WeaponType { AR_1, TR_1, Pistol, Shotgun, SR_1}
