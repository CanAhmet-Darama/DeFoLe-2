using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ImpactMarkManager : MonoBehaviour
{
    [Header("Bullet Marks")]
    public static GameObject bulletMarkPrefab;
    [SerializeField] GameObject bMP_isThis;
    float markLife = 60;

    public static GameObject[] bulletMarks;
    public static bool firstOrSecondCycle = false;
    public static bool[] cyclesStates;
    public static bool cycleForBlade;
    public static bool[] cycleArrayForBlade;
    public static short bulletMarksCount = 75;
    public static short lastCalledIndex;
    static ImpactMarkManager impactManagerIns;

    [Header("Bullet Impacts")]
    public static ParticleSystem generalBulletImpactPrefab;
    public static ParticleSystem metalBulletImpactPrefab;
    public static ParticleSystem woodBulletImpactPrefab;
    public static ParticleSystem dirtBulletImpactPrefab;
    public static ParticleSystem bloodBulletImpactPrefab;
    public static ParticleSystem glassBulletImpactPrefab;
    public static ParticleSystem[] impactPrefabs;
    [SerializeField] ParticleSystem _generalBulletImpact;
    [SerializeField] ParticleSystem _metalBulletImpact;
    [SerializeField] ParticleSystem _woodBulletImpact;
    [SerializeField] ParticleSystem _dirtBulletImpact;
    [SerializeField] ParticleSystem _bloodBulletImpact;
    [SerializeField] ParticleSystem _glassBulletImpact;
    public static ParticleSystem[,] bulletImpacts;
    public static short impactsCount = 10;

    [Header("Impact Sounds And General")]
    public static AudioSource audioSource;
    [Range(0,1)]public float volumeOfAudio;
    public static AudioClip generalImpactSound;
    public static AudioClip dirtImpactSound;
    public static AudioClip[] concreteImpactSounds;
    public static AudioClip[] metalImpactSounds;
    public static AudioClip glassImpactSound;
    public static AudioClip woodImpactSound;
    public static AudioClip fleshImpactSound;
    public static AudioClip fleshBladeImpactSound;
    public static AudioClip bulletWhoosh;

    public static AudioClip woodBreaking;
    public static AudioClip glassBreaking;

    #region Holder Sounds
    public AudioSource _audioSource;
    public AudioClip _generalImpactSound;
    public AudioClip _dirtImpactSound;
    public AudioClip[] _concreteImpactSounds;
    public AudioClip[] _metalImpactSounds;
    public AudioClip _glassImpactSound;
    public AudioClip _woodImpactSound;
    public AudioClip _fleshImpactSound;
    public AudioClip _fleshBladeImpactSound;
    public AudioClip _bulletWhoosh;
    [Header("Breaking Sounds")]
    public AudioClip _woodBreaking;
    public AudioClip _glassBreaking;
    #endregion

    [Header("Melee Impact")]
    public static GameObject[,] bladeMarks;
    [SerializeField] GameObject[] bladeMarkPrefabs;
    static short lastCalledBladeIndex;






    void Start()
    {
        impactManagerIns= this;
        bulletMarkPrefab = bMP_isThis;
        bulletMarks = new GameObject[bulletMarksCount];
        cyclesStates = new bool[bulletMarksCount];
        cycleForBlade = false;
        cycleArrayForBlade = new bool[bladeMarkPrefabs.Length * impactsCount];
        lastCalledIndex = 0;

        SoundsManagerStart();
        ParticlesManagerStart();
        InstantiateBulletMarks();
        InstantiateBulletImpacts();
        InstantiateBladeMarks();
    }

    public static void CallMark(Vector3 pos, Vector3 rot, EnvObjType objType, float volume = 1)
    {
        GameObject mark = GetMarkReady();
        mark.transform.position = pos;
        mark.transform.rotation = Quaternion.LookRotation(rot);
        mark.SetActive(true);
        impactManagerIns.StartCoroutine(impactManagerIns.DeleteBulletMark(mark, lastCalledIndex));

        MakeImpactParticle(pos, rot, objType);

        if(volume>0)
        MakeImpactSound(pos, objType);
    }
    public static void CallBladeMark(Vector3 pos, Vector3 rot, EnvObjType objType, float volume = 1)
    {
        GameObject bladeM;
        switch(objType)
        {
            case EnvObjType.wood: bladeM = GetBladeMarkReady(2);
                break;
            case EnvObjType.dirt: bladeM = GetBladeMarkReady(1);
                break;
            default:
                bladeM = GetBladeMarkReady(0);
                break;
        }
        bladeM.transform.position = pos;
        bladeM.transform.forward = rot;
        bladeM.SetActive(true);
        impactManagerIns.StartCoroutine(impactManagerIns.DeleteBladeMark(bladeM, lastCalledBladeIndex));

        MakeImpactParticle(pos, rot + new Vector3(0,0,180), objType);
        if (volume > 0)
            MakeImpactSound(pos, objType, volume);

    }

    void InstantiateBulletMarks()
    {
        for (short i = (short)(bulletMarksCount - 1); i >= 0; i--)
        {
            GameObject bMark = Instantiate(bulletMarkPrefab, transform);
            bulletMarks[i] = bMark;
            bMark.SetActive(false);
        }
    }
    void InstantiateBulletImpacts()
    {
        bulletImpacts = new ParticleSystem[impactPrefabs.Length, impactsCount];

        for (int j = impactPrefabs.Length - 1; j >= 0; j--)
        {
            for (short i = (short)(impactsCount - 1); i >= 0; i--)
            {
                bulletImpacts[j,i] = Instantiate(impactPrefabs[j], transform).GetComponent<ParticleSystem>();
            }

        }
    }
    void InstantiateBladeMarks()
    {
        bladeMarks = new GameObject[bladeMarkPrefabs.Length, impactsCount];
        for (short i = (short)(bladeMarks.GetLength(0) - 1); i >= 0; i--)
        {
            for (short j = (short)(bladeMarks.GetLength(1) - 1); j >= 0; j--)
            {
                bladeMarks[i, j] = Instantiate(bladeMarkPrefabs[i], transform);
                bladeMarks[i, j].SetActive(false);

            }
        }
    }

    static GameObject GetMarkReady()
    {
        for(short i = (short)(bulletMarks.Length - 1); i >= 0; i--) {
            if (!bulletMarks[i].activeInHierarchy)
            {
                cyclesStates[i] = !cyclesStates[i];
                lastCalledIndex= i;
                return bulletMarks[i];
            }
        }
        for (short i = (short)(bulletMarks.Length - 1); i >= 0; i--)
        {
            if (cyclesStates[i] == firstOrSecondCycle)
            {
                cyclesStates[i] = !cyclesStates[i];
                lastCalledIndex = i;
                return bulletMarks[i];
            }
        }
        firstOrSecondCycle = !firstOrSecondCycle;
        return GetMarkReady();
    }
    static GameObject GetBladeMarkReady(int index)
    {
        for (short i = (short)(bladeMarks.GetLength(1) - 1); i >= 0; i--)
        {
            if (!bladeMarks[index, i].activeInHierarchy)
            {
                cycleArrayForBlade[i] = !cycleArrayForBlade[i];
                return bladeMarks[index, i];
            }
        }
        for (short i = (short)(bladeMarks.GetLength(1) - 1); i >= 0; i--)
        {
            if (cycleArrayForBlade[i] == cycleForBlade)
            {
                cycleArrayForBlade[i] = !cycleArrayForBlade[i];
                lastCalledBladeIndex = i;
                return bladeMarks[index, i];
            }
        }
        cycleForBlade = !cycleForBlade;
        return GetBladeMarkReady(index);
    }


    IEnumerator DeleteBulletMark(GameObject mark, short index)
    {
        bool previousValue = cyclesStates[index];
        yield return new WaitForSeconds(markLife);

        if(previousValue == cyclesStates[index])
        {
            mark.SetActive(false);
        }
    }
    IEnumerator DeleteBladeMark(GameObject mark, short index)
    {
        bool previousValue = cycleArrayForBlade[index];
        yield return new WaitForSeconds(markLife);

        if (previousValue == cycleArrayForBlade[index])
        {
            mark.SetActive(false);
        }
    }

    static ParticleSystem GetImpactReady(ParticleSystem[,] bulletImpactsArray, int index)
    {
        for (short i = (short)(bulletImpactsArray.GetLength(1) - 1); i >= 0; i--)
        {
            if (!bulletImpactsArray[index, i].isPlaying)
            {
                return bulletImpactsArray[index, i];
            }
        }
        return bulletImpactsArray[index, 0];
    }
    static ParticleSystem GetBloodImpactReady()
    {
        for (short i = (short)(bulletImpacts.GetLength(0) - 1); i >= 0; i--)
        {
            if (!bulletImpacts[4,i].isPlaying)
            {
                return bulletImpacts[4, i];
            }
        }
        return bulletImpacts[4,0];
    }

    public static void MakeImpactSound(Vector3 pos, EnvObjType objType, float volume = 1)
    {
        AudioClip impactSound;
        int randNum;
        switch (objType)
        {
            case EnvObjType.metal: randNum = Random.Range(0,2);
                impactSound = metalImpactSounds[randNum];
                break;
            case EnvObjType.concrete: randNum = Random.Range(0, 2);
                impactSound = concreteImpactSounds[randNum];
                break;
            case EnvObjType.wood:
                impactSound = woodImpactSound;
                break;
            case EnvObjType.dirt:
                impactSound = dirtImpactSound;
                break;
            case EnvObjType.glass:
                impactSound = glassImpactSound;
                break;
            default:
                impactSound = generalImpactSound;
                break;
        }
        audioSource.transform.position = pos;
        audioSource.PlayOneShot(impactSound, volume);
    }
    public static void MakeImpactParticle(Vector3 pos, Vector3 rot, EnvObjType objType)
    {
        ParticleSystem impact;
        switch (objType)
        {
            case EnvObjType.metal: 
                impact = GetImpactReady(bulletImpacts,1);
                break;
            case EnvObjType.dirt:
                impact = GetImpactReady(bulletImpacts,3);
                break;
            case EnvObjType.wood:
                impact = GetImpactReady(bulletImpacts, 2);
                break;
            case EnvObjType.glass:
                impact = GetImpactReady(bulletImpacts, 5);
                break;
            default:
                impact = GetImpactReady(bulletImpacts, 0);
                break;

        }
        impact.transform.position = pos;
        impact.transform.rotation = Quaternion.LookRotation(rot);
        impact.Play();
    }

    public static void MakeBreakingSound(Vector3 pos, EnvObjType objType, float volume = 1)
    {
        AudioClip impactSound = null;
        switch (objType)
        {
            case EnvObjType.wood:
                impactSound = woodBreaking;
                break;
            case EnvObjType.glass:
                impactSound = glassBreaking;
                break;
        }
        if(impactSound != null)
        {
            audioSource.transform.position = pos;
            audioSource.PlayOneShot(impactSound, volume);
        }

    }

    public static void MakeBulletImpactWithoutMark(Vector3 pos, Vector3 rot, EnvObjType objType, bool isBreaking = false)
    {
        if(!isBreaking)
        {
            MakeImpactSound(pos, objType);
        }
        else
        {
            MakeBreakingSound(pos, objType);
        }
        MakeImpactParticle(pos, rot, objType);
    }



    public static void MakeBloodImpactAndSound(Vector3 pos, Vector3 rot, bool byBullet)
    {
        ParticleSystem impact = GetBloodImpactReady();
        impact.transform.position = pos;
        impact.transform.rotation = Quaternion.LookRotation(rot);
        audioSource.transform.position = pos;
        if (byBullet)
        {
            audioSource.PlayOneShot(fleshImpactSound);
        }
        else
        {
            audioSource.PlayOneShot(fleshBladeImpactSound);
            impact.transform.localEulerAngles += new Vector3(0,-90,0);
        }
        impact.Play();
    }

    public static void PlayOnShotClip(AudioClip clip, Vector3 pos, float volume = 1)
    {
        GameManager.generalAudioSource.transform.position = pos;
        GameManager.generalAudioSource.PlayOneShot(clip, volume);
    }


    void SoundsManagerStart()
    {
        audioSource = _audioSource;
        generalImpactSound = _generalImpactSound;
        dirtImpactSound = _dirtImpactSound;
        concreteImpactSounds = _concreteImpactSounds;
        metalImpactSounds = _metalImpactSounds;
        glassImpactSound = _glassImpactSound;
        woodImpactSound = _woodImpactSound;
        fleshImpactSound = _fleshImpactSound;
        fleshBladeImpactSound = _fleshBladeImpactSound;
        bulletWhoosh = _bulletWhoosh;

        woodBreaking = _woodBreaking;
        glassBreaking = _glassBreaking;

        audioSource.volume = volumeOfAudio;
    }
    void ParticlesManagerStart()
    {
        generalBulletImpactPrefab = _generalBulletImpact;
        metalBulletImpactPrefab = _metalBulletImpact;
        woodBulletImpactPrefab = _woodBulletImpact;
        dirtBulletImpactPrefab = _dirtBulletImpact;
        bloodBulletImpactPrefab = _bloodBulletImpact;
        glassBulletImpactPrefab = _glassBulletImpact;
        impactPrefabs = new ParticleSystem[] { generalBulletImpactPrefab, metalBulletImpactPrefab, woodBulletImpactPrefab,
            dirtBulletImpactPrefab, bloodBulletImpactPrefab, glassBulletImpactPrefab };
    }

}
