using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ImpactMarkManager : MonoBehaviour
{
    [Header("Bullet Marks")]
    public static GameObject bulletMarkPrefab;
    [SerializeField] GameObject bMP_isThis;
    float markLife = 10;

    public static GameObject[] bulletMarks;
    public static bool firstOrSecondCycle = false;
    public static bool[] cyclesStates;
    public static short bulletMarksCount = 75;
    public static short lastCalledIndex;
    static ImpactMarkManager impactManagerIns;

    [Header("Bullet Impacts")]
    public static ParticleSystem generalBulletImpactPrefab;
    public static ParticleSystem metalBulletImpactPrefab;
    public static ParticleSystem woodBulletImpactPrefab;
    public static ParticleSystem dirtBulletImpactPrefab;
    public static ParticleSystem bloodBulletImpactPrefab;
    public static ParticleSystem[] impactPrefabs;
    [SerializeField] ParticleSystem _generalBulletImpact;
    [SerializeField] ParticleSystem _metalBulletImpact;
    [SerializeField] ParticleSystem _woodBulletImpact;
    [SerializeField] ParticleSystem _dirtBulletImpact;
    [SerializeField] ParticleSystem _bloodBulletImpact;
    public static ParticleSystem[,] bulletImpacts;
    public static short impactsCount = 10;

    [Header("Sound stuff")]
    public static AudioSource audioSource;
    public static AudioClip generalImpactSound;
    public static AudioClip dirtImpactSound;
    public static AudioClip[] concreteImpactSounds;
    public static AudioClip[] metalImpactSounds;
    public static AudioClip glassImpactSound;
    public static AudioClip woodImpactSound;
    public static AudioClip fleshImpactSound;
    public static AudioClip bulletWhoosh;
    #region Holder Sounds
    public AudioSource _audioSource;
    public AudioClip _generalImpactSound;
    public AudioClip _dirtImpactSound;
    public AudioClip[] _concreteImpactSounds;
    public AudioClip[] _metalImpactSounds;
    public AudioClip _glassImpactSound;
    public AudioClip _woodImpactSound;
    public AudioClip _fleshImpactSound;
    public AudioClip _bulletWhoosh;
    #endregion

    [Header("Melee Impact")]
    public static GameObject[,] bladeMarks;
    [SerializeField] GameObject[] bladeMarkPrefabs;






    void Start()
    {
        impactManagerIns= this;
        bulletMarkPrefab = bMP_isThis;
        bulletMarks = new GameObject[bulletMarksCount];
        cyclesStates = new bool[bulletMarksCount];
        lastCalledIndex = 0;

        SoundsManagerStart();
        ParticlesManagerStart();
        InstantiateBulletMarks();
        InstantiateBulletImpacts();
        InstantiateBladeImpacts();
    }

    public static void CallMark(Vector3 pos, Vector3 rot, EnvObjType objType)
    {
        GameObject mark = GetMarkReady();
        mark.transform.position = pos;
        mark.transform.rotation = Quaternion.LookRotation(rot);
        mark.SetActive(true);
        impactManagerIns.DeleteBulletMark(mark, lastCalledIndex);

        MakeImpactParticle(pos, rot, objType);

        MakeImpactSound(pos, objType);
    }
    public static void CallBladeMark(Vector3 pos, Vector3 rot, EnvObjType objType)
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
        bladeM.transform.eulerAngles = rot;
        bladeM.SetActive(true);

        MakeImpactParticle(pos, rot + new Vector3(0,60,0), objType);
        MakeImpactSound(pos, objType);

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
    void InstantiateBladeImpacts()
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
            if (!bladeMarks[index, i].activeSelf)
            {
                return bladeMarks[index, i];
            }
        }
        return bladeMarks[index, 0];
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

    static void MakeImpactSound(Vector3 pos, EnvObjType objType)
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
            default:
                impactSound = generalImpactSound;
                break;
        }
        audioSource.transform.position = pos;
        audioSource.PlayOneShot(impactSound);
    }
    static void MakeImpactParticle(Vector3 pos, Vector3 rot, EnvObjType objType)
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
            default:
                impact = GetImpactReady(bulletImpacts, 0);
                break;

        }
        impact.transform.position = pos;
        impact.transform.rotation = Quaternion.LookRotation(rot);
        impact.Play();
    }
    public static void MakeBloodImpactAndSound(Vector3 pos, Vector3 rot)
    {
        ParticleSystem impact = GetBloodImpactReady();
        impact.transform.position = pos;
        impact.transform.rotation = Quaternion.LookRotation(rot);
        impact.Play();
        audioSource.transform.position = pos;
        audioSource.PlayOneShot(fleshImpactSound);
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
        bulletWhoosh = _bulletWhoosh;
    }
    void ParticlesManagerStart()
    {
        generalBulletImpactPrefab = _generalBulletImpact;
        metalBulletImpactPrefab = _metalBulletImpact;
        woodBulletImpactPrefab = _woodBulletImpact;
        dirtBulletImpactPrefab = _dirtBulletImpact;
        bloodBulletImpactPrefab = _bloodBulletImpact;
        impactPrefabs = new ParticleSystem[] { generalBulletImpactPrefab, metalBulletImpactPrefab, woodBulletImpactPrefab, dirtBulletImpactPrefab, bloodBulletImpactPrefab };
    }

}
