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

        for(int i = bulletPool.Length; i > 0; i--)
        {
            bulletPool[i - 1] = Instantiate(bullet, Vector3.zero, transform.rotation, bulletPoolHolder.transform);
            bulletPool[i - 1].transform.localPosition = Vector3.zero;
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
        if (weaponType == WeaponType.Shotgun) bulletPerShot = 12;

        for (byte i = bulletPerShot; i >0; i--)
        {
            GameObject bulletToShoot = GetAmmo();
            StartCoroutine(AFrameThenTrail(bulletToShoot));
            GeneralBullet gBullet = bulletToShoot.GetComponent<GeneralBullet>();
            bulletToShoot.transform.parent = bulletPoolHolder.transform;
            bulletToShoot.transform.localPosition = bulletLaunchOffset;
            gBullet.itsHolder = bulletPoolHolder;
            gBullet.duratPassed = 0;
            bulletToShoot.transform.parent = null;
            bulletToShoot.SetActive(true);

            float inaccX = Random.Range(-inaccuracyDegree/10,inaccuracyDegree/10);
            float inaccY = Random.Range(-inaccuracyDegree / 10, inaccuracyDegree / 10);
            bulletToShoot.GetComponent<Rigidbody>().velocity = bulletToShoot.GetComponent<GeneralBullet>().bulletSpeed * transform.forward;
            bulletToShoot.GetComponent<Rigidbody>().velocity += new Vector3(inaccX, inaccY, 0);
        }
        
        if (gunAudioSource.clip != firingSound) gunAudioSource.clip = firingSound;
        gunAudioSource.Play();
        owner.StartCoroutine("CanShootAgain", firingTime);

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
    public void DisableAndParentBullet(Transform bullet, Transform parent)
    {
        StartCoroutine(AFrameThenParent(bullet, parent));
    }

    IEnumerator AFrameThenParent(Transform bullet, Transform parent)
    {
        yield return null;
        bullet.parent = parent;
        bullet.GetComponent<TrailRenderer>().enabled = false;
    }
    IEnumerator AFrameThenTrail(GameObject bullet)
    {
        yield return null;
        bullet.GetComponent<TrailRenderer>().enabled = true;
    }

}
    public enum WeaponType { AR_1, TR_1, Pistol, Shotgun, SR_1}
