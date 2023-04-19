using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralWeapon : MonoBehaviour
{
    #region Variables
    [Header("General")]
    public GeneralCharacter owner;
    public short damage;
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

    [Header("Sound Weapon")]
    public AudioClip firingSound;
    public AudioSource gunAudioSource;
    public AudioClip magOutSound;
    public AudioClip magInSound;
    public AudioClip pullBoltSound;
    public AudioClip pushBoltSound;

    [Header("For Animation etc")]
    public Vector3 weaponInternalPos;
    public Vector3 rightTargetPos;
    public Vector3 rightTargetRot;
    public Vector3 leftTargetPos;
    public Vector3 leftTargetRot;
    public Vector3 bulletLaunchOffset;
    public AnimationClip idleAnim;
    public AnimationClip semiIdleAnim;
    public AnimationClip aimAnim;
    public AnimationClip fireAnim;
    public AnimationClip reloadAnim;
    public byte animOverriderIndex;
    [SerializeField] Animator subAnimator;

    [Header("Other Usage")]
    public byte recommendedShotsBeforeCrouch;
    public GameObject[] meshedPartOfWeapon;

    [Header("For Sniper")]
    public bool zoomedAlready;
    public Vector3 zoomCamLocalPos;
    public float zoomFieldOfView;
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
        gunAudioSource.volume = 0.25f;
        currentAmmo = maxAmmo;

    }

    public void Reload()
    {
        owner.StartCoroutine(owner.CanShootAgain(reloadTime));
        owner.StartCoroutine(owner.CanReloadAgain(reloadTime));
        owner.animator.SetTrigger("reload");
        StartCoroutine(DisableSecondHandTBIK(reloadTime));
        StartCoroutine(IncreaseBulletWithReload());
        StartCoroutine(WeaponExtraSound());
        if(weaponType == WeaponType.SR_1 || weaponType == WeaponType.Shotgun)
        {
            WeapSubAnimer.WeapunSubAnimReload(subAnimator);
        }
    }
    public void Fire()
    {
        byte bulletPerShot = 1;
        if (weaponType == WeaponType.Shotgun) { bulletPerShot = 12;}

        for (byte i = bulletPerShot; i >0; i--)
        {
            GameObject bulletToShoot = GetAmmo();
            GeneralBullet gBullet = bulletToShoot.GetComponent<GeneralBullet>();
            
            gBullet.trailRenderer.enabled = false;
            bulletToShoot.SetActive(false);


            bulletToShoot.transform.parent = bulletPoolHolder.transform;
            bulletToShoot.transform.localPosition = bulletLaunchOffset;
            bulletToShoot.transform.SetParent(null);

            bulletToShoot.SetActive(true);
            gBullet.duratPassed = 0;
            gBullet.firedPos = bulletToShoot.transform.position;


            float inaccX = Random.Range(-inaccuracyDegree / 100, inaccuracyDegree / 100);
            float inaccY = Random.Range(-inaccuracyDegree / 100, inaccuracyDegree / 100);
            float launchSpeed = gBullet.bulletSpeed;
            gBullet.bulletRb.velocity = launchSpeed * transform.forward;
            gBullet.bulletRb.velocity += (transform.right * inaccX * launchSpeed) + (transform.up * inaccY * launchSpeed);
            StartCoroutine(AFrameThenTrail(bulletToShoot));
        }
        owner.animator.SetTrigger("fire");
        if(weaponType == WeaponType.SR_1)
        {
            StartCoroutine(DisableSecondHandTBIK(firingTime));
            WeapSubAnimer.WeaponSubAnimFire(subAnimator);
            StartCoroutine(WeaponExtraSound(false));
        }

        if (gunAudioSource.clip != firingSound) gunAudioSource.clip = firingSound;
        gunAudioSource.Play();
        muzzleFlash.Play();
        owner.StartCoroutine(owner.CanShootAgain(firingTime));
        owner.StartCoroutine(owner.IsInFiring(firingTime));
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

    public void SniperZoom(bool activateZoom)
    {
        CameraScript mainCamScr = GameManager.mainCam.GetComponent<CameraScript>();


        if (activateZoom)
        {
            mainCamScr.AdjustCameraPivotOrFollow(PlayerState.onFoot,CamState.zoomScope);
            mainCamScr.transform.SetParent(transform);
            mainCamScr.transform.localPosition = zoomCamLocalPos;
            mainCamScr.transform.localEulerAngles = Vector3.zero;
            mainCamScr.CamScript.fieldOfView = zoomFieldOfView;
            owner.aimTarget.transform.SetParent(mainCamScr.freeLookPivotOnFoot);
            LevelOfDetailManager.detailDistanceMultiplier *= mainCamScr.defaultFOV / zoomFieldOfView;
        }
        else
        {
            mainCamScr.AdjustCameraPivotOrFollow(PlayerState.onFoot, CamState.pivot);
            mainCamScr.CamScript.fieldOfView = mainCamScr.defaultFOV;
            owner.aimTarget.transform.SetParent(mainCamScr.transform);
            LevelOfDetailManager.detailDistanceMultiplier *= zoomFieldOfView / mainCamScr.defaultFOV;
        }
        zoomedAlready = activateZoom;
        GameManager.uiManager.crosshair.gameObject.SetActive(!activateZoom);
        GameManager.uiManager.sniperZoomScreen.gameObject.SetActive(activateZoom);

        if(LevelOfDetailManager.detailDistanceMultiplier < 0.1f)
        {
            switch (SettingsManager.qualityDropdown.value)
            {
                case 0:
                    LevelOfDetailManager.detailDistanceMultiplier = 0.5f;
                    break;
                case 1:
                    LevelOfDetailManager.detailDistanceMultiplier = 0.75f;
                    break;
                case 2:
                    LevelOfDetailManager.detailDistanceMultiplier = 1;
                    break;
            }

        }
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
            owner.rightHandConstraint.weight = 0;
            yield return new WaitForSeconds(durat);
            owner.rightHandTBIK.weight = 1;
            owner.rightHandConstraint.weight = 1;
        }
        else// if(weaponType == WeaponType.Shotgun)
        {
            owner.leftHandTBIK.weight = 0;
            owner.leftHandConstraint.weight = 0;
            yield return new WaitForSeconds(durat);
            owner.leftHandTBIK.weight = 1;
            owner.leftHandConstraint.weight = 1;
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

    IEnumerator WeaponExtraSound(bool forReload = true)
    {
        float weaponExtraSoundVolume = 0.2f;
        yield return null;
        switch (weaponType)
        {
            case WeaponType.AR_1:
                yield return new WaitForSeconds(0.3f);
                gunAudioSource.PlayOneShot(magOutSound, weaponExtraSoundVolume);
                yield return new WaitForSeconds(0.9f);
                gunAudioSource.PlayOneShot(magInSound, weaponExtraSoundVolume);
                break;
            case WeaponType.TR_1:
                yield return new WaitForSeconds(0.3f);
                gunAudioSource.PlayOneShot(magOutSound, weaponExtraSoundVolume);
                yield return new WaitForSeconds(1.1f);
                gunAudioSource.PlayOneShot(magInSound, weaponExtraSoundVolume);
                break;
            case WeaponType.Pistol:
                yield return new WaitForSeconds(0.3f);
                gunAudioSource.PlayOneShot(magOutSound, weaponExtraSoundVolume);
                yield return new WaitForSeconds(1.3f);
                gunAudioSource.PlayOneShot(magInSound, weaponExtraSoundVolume);
                yield return new WaitForSeconds(0.4f);
                gunAudioSource.PlayOneShot(pullBoltSound, weaponExtraSoundVolume);
                break;
            case WeaponType.Shotgun:
                if (!owner.isEnemy)
                {
                    gunAudioSource.PlayOneShot(pullBoltSound, weaponExtraSoundVolume);
                    yield return new WaitForSeconds(0.7f);
                    gunAudioSource.PlayOneShot(magOutSound, weaponExtraSoundVolume);
                    yield return new WaitForSeconds(0.8f);
                    gunAudioSource.PlayOneShot(magInSound, weaponExtraSoundVolume);
                    yield return new WaitForSeconds(0.2f);
                    gunAudioSource.PlayOneShot(magInSound, weaponExtraSoundVolume);
                    yield return new WaitForSeconds(0.2f);
                    gunAudioSource.PlayOneShot(pushBoltSound, weaponExtraSoundVolume);
                }
                break;
            case WeaponType.SR_1:
                if (!owner.isEnemy && forReload)
                {
                    yield return new WaitForSeconds(0.7f);
                    gunAudioSource.PlayOneShot(pullBoltSound, weaponExtraSoundVolume);
                    yield return new WaitForSeconds(1.6f);
                    gunAudioSource.PlayOneShot(magInSound, weaponExtraSoundVolume);
                    yield return new WaitForSeconds(0.3f);
                    gunAudioSource.PlayOneShot(magInSound, weaponExtraSoundVolume);
                    yield return new WaitForSeconds(0.3f);
                    gunAudioSource.PlayOneShot(magInSound, weaponExtraSoundVolume);
                    yield return new WaitForSeconds(0.3f);
                    gunAudioSource.PlayOneShot(magInSound, weaponExtraSoundVolume);
                    yield return new WaitForSeconds(0.3f);
                    gunAudioSource.PlayOneShot(magInSound, weaponExtraSoundVolume);
                    yield return new WaitForSeconds(0.53f);
                    gunAudioSource.PlayOneShot(pushBoltSound, weaponExtraSoundVolume);
                }
                else
                {
                    yield return new WaitForSeconds(0.6f);
                    gunAudioSource.PlayOneShot(pullBoltSound, weaponExtraSoundVolume);
                    yield return new WaitForSeconds(0.4f);
                    gunAudioSource.PlayOneShot(pushBoltSound, weaponExtraSoundVolume);
                }
                break;
        }
    }
}
    public enum WeaponType { AR_1, TR_1, Pistol, Shotgun, SR_1}
