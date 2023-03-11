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
    public static ParticleSystem bulletImpactPrefab;
    [SerializeField] ParticleSystem bImpact_isThis;
    public static ParticleSystem[] bulletImpacts;
    public static short impactsCount = 20;

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
    public AudioSource _audioSource;
    public AudioClip _generalImpactSound;
    public AudioClip _dirtImpactSound;
    public AudioClip[] _concreteImpactSounds;
    public AudioClip[] _metalImpactSounds;
    public AudioClip _glassImpactSound;
    public AudioClip _woodImpactSound;
    public AudioClip _fleshImpactSound;
    public AudioClip _bulletWhoosh;






    void Start()
    {
        impactManagerIns= GetComponent<ImpactMarkManager>();
        bulletMarkPrefab = bMP_isThis;
        bulletMarks = new GameObject[bulletMarksCount];
        cyclesStates = new bool[bulletMarksCount];
        lastCalledIndex = 0;

        bulletImpacts = new ParticleSystem[impactsCount];
        bulletImpactPrefab = bImpact_isThis;
        InstantiateBulletMarks();
        InstantiateBulletImpacts();
        SoundsManagerStart();
    }

    void Update()
    {
        
    }
    public static void CallMark(Vector3 pos, Vector3 rot, EnvObjType objType)
    {
        GameObject mark = GetMarkReady();
        mark.transform.position = pos;
        mark.transform.rotation = Quaternion.LookRotation(rot);
        mark.SetActive(true);
        impactManagerIns.DeleteBulletMark(mark, lastCalledIndex);

        ParticleSystem impact = GetImpactReady();
        impact.transform.position = pos;
        impact.transform.rotation = Quaternion.LookRotation(rot);
        impact.Play();

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
        for (short i = (short)(impactsCount - 1); i >= 0; i--)
        {
            ParticleSystem bImpact = Instantiate(bulletImpactPrefab, transform);
            bulletImpacts[i] = bImpact;
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
    IEnumerator DeleteBulletMark(GameObject mark, short index)
    {
        bool previousValue = cyclesStates[index];
        yield return new WaitForSeconds(markLife);

        if(previousValue == cyclesStates[index])
        {
            mark.SetActive(false);
        }
    }
    static ParticleSystem GetImpactReady()
    {
        for (short i = (short)(bulletImpacts.Length - 1); i >= 0; i--)
        {
            if (!bulletImpacts[i].isPlaying)
            {
                return bulletImpacts[i];
            }
        }
        return bulletImpacts[bulletImpacts.Length - 1];
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
        fleshImpactSound = fleshImpactSound;
        bulletWhoosh = _bulletWhoosh;
    }
}
