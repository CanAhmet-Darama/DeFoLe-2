using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralWeapon : MonoBehaviour
{
    #region Variables
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
    public GeneralBullet[] bulletScripts;
    public GameObject bulletPoolHolder;
    public ParticleSystem muzzleFlash;
    public AudioClip firingSound;
    public AudioSource gunAudioSource;
    public AudioClip reloadSound;

    [Header("For Animation etc")]
    public Vector3 leftHandPos;
    public Vector3 leftHandRot;
    public Vector3 bulletLaunchOffset;
    public Vector3 rightHandPosOffset;
    public Vector3 rightHandRotOffset;
    public AnimationClip idleAnim;
    public AnimationClip semiIdleAnim;
    public AnimationClip aimAnim;
    public AnimationClip fireAnim;
    public AnimationClip reloadAnim;
    public byte animOverriderIndex;
    [SerializeField] Animator subAnimator;

    #endregion


    void Start()
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
        bulletScripts = new GeneralBullet[bulletPool.Length];


        for(int i = bulletPool.Length; i > 0; i--)
        {
            bulletPool[i - 1] = Instantiate(bullet, Vector3.zero, transform.rotation, bulletPoolHolder.transform);
            bulletPool[i - 1].transform.localPosition = Vector3.zero;


            bulletScripts[i - 1] = bulletPool[i - 1].GetComponent<GeneralBullet>();


            bulletScripts[i - 1].index = (byte)(i - 1);
            bulletScripts[i - 1].itsHolder = bulletPoolHolder;
            bulletScripts[i - 1].itsOwnerWeapon = this;

            bulletPool[i - 1].SetActive(false);
            bulletPool[i - 1].GetComponent<GeneralBullet>().itsHolder = bulletPoolHolder;
            bulletPool[i - 1].GetComponent<GeneralBullet>().itsOwnerWeapon = this;

        }
        gunAudioSource.volume = 0.05f;
        currentAmmo = maxAmmo;

    }
    void Update()
    {
        
    }
    public void Reload()
    {
        owner.StartCoroutine("CanShootAgain", reloadTime);
        owner.StartCoroutine("CanReloadAgain", reloadTime);
        owner.animator.SetTrigger("reload");
        owner.isReloading = true;
        StartCoroutine(DisableSecondHandTBIK(reloadTime));
        StartCoroutine(IncreaseBulletWithReload());
        if(weaponType == WeaponType.SR_1 || weaponType == WeaponType.Shotgun)
        {
            WeapSubAnimer.WeapunSubAnimReload(subAnimator);
        }
    }
    public void Fire()
    {
        byte bulletPerShot = 1;
        if (weaponType == WeaponType.Shotgun) { bulletPerShot = 12; }

        for (byte i = bulletPerShot; i >0; i--)
        {
            GameObject bulletToShoot = GetAmmo();

            bulletToShoot.GetComponent<TrailRenderer>().enabled = false;
            bulletToShoot.SetActive(false);


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
        owner.animator.SetTrigger("fire");
        if(weaponType == WeaponType.SR_1)
        {
            StartCoroutine(DisableSecondHandTBIK(firingTime));
            WeapSubAnimer.WeaponSubAnimFire(subAnimator);
        }

        if (gunAudioSource.clip != firingSound) gunAudioSource.clip = firingSound;
        gunAudioSource.Play();
        muzzleFlash.Play();
        owner.StartCoroutine("CanShootAgain", firingTime);
        owner.StartCoroutine("IsInFiring", firingTime);
        currentAmmo--;
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
        bullet.GetComponent<TrailRenderer>().enabled = true;

    }
    IEnumerator DisableSecondHandTBIK(float durat)
    {
        if(weaponType == WeaponType.SR_1)
        {
            owner.rightHandTBIK.weight = 0;
            yield return new WaitForSeconds(durat);
            owner.rightHandTBIK.weight = 1;
        }
        else// if(weaponType == WeaponType.Shotgun)
        {
            owner.leftHandTBIK.weight = 0;
            yield return new WaitForSeconds(durat);
            owner.leftHandTBIK.weight = 1;
        }
    }
    IEnumerator IncreaseBulletWithReload()
    {
        if (weaponType == WeaponType.SR_1 || weaponType == WeaponType.Shotgun)
        {
            WeapSubAnimer.ResetTriggerReload(subAnimator);
        }
        yield return new WaitForSeconds(reloadTime);
        if (owner.ammoCounts[(int)weaponType] >= (maxAmmo - currentAmmo))
        {
            owner.ammoCounts[(int)weaponType] -= (short)(maxAmmo - currentAmmo);
            currentAmmo = maxAmmo;
        }
        else
        {
            owner.ammoCounts[(int)weaponType] = 0;
            currentAmmo += (byte)owner.ammoCounts[(int)weaponType];
        }
    }
}
    public enum WeaponType { AR_1, TR_1, Pistol, Shotgun, SR_1}
