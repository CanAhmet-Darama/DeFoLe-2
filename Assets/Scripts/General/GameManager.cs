using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("State Bools")]
    public static PlayerState mainState;

    [Header("Settings")]
    public static byte mouseSensitivity = 10;
    public static float generalSoundMultiplier;

    [Header("Instances")]
    public static Transform mainCam;
    public static Transform mainCar;
    public static Transform mainChar;
    public static Terrain mainTerrain;
    public static UI_Manager uiManager;
    public static EnemyManager enemyManager;

    public static GameObject[] weaponPrefabs = new GameObject[6];
    public static GameObject[] enemyCamps;
    [SerializeField] GameObject[] weaponPrefabsAreThese;
    [SerializeField] Transform mainCamIsThis;
    [SerializeField] Transform mainCharIsThis;
    [SerializeField] Transform mainCarIsThis;
    [SerializeField] Terrain mainTerrainIsThis;
    [SerializeField] UI_Manager userInterfaceManagerIsThis;
    [SerializeField] EnemyManager enemyManagerIsThis;
    [SerializeField] GameObject[] enemyCampsAreThese;


    [Header("General Numbers")]
    [HideInInspector] public static byte numberOfCamps = 4;

    void Awake()
    {
        weaponPrefabs = weaponPrefabsAreThese;
        switch (SceneManager.GetActiveScene().name)
        {
            case "Level 1":
                mainState = PlayerState.onFoot;
                mainCam = Camera.main.transform;
                mainChar = mainCharIsThis;
                mainTerrain = mainTerrainIsThis;
                mainCar = mainCarIsThis;
                uiManager = userInterfaceManagerIsThis;
                mainCam.GetComponent<CameraScript>().AdjustCameraPivotOrFollow(PlayerState.onFoot, CamState.follow);
                mainChar.GetComponent<MainCharacter>().RegulateMainChar();

                numberOfCamps = 4;
                enemyCamps = enemyCampsAreThese;
                enemyManager = enemyManagerIsThis;
                enemyManager.EnemyManagerStart();

                TerrainManager.mainTerrain = mainTerrain;
                break;
        }
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            int totalCoverObj = 0;
            for (int i = CoverObjectsManager.coverObjectsOfWorld.Length - 1; i >= 0; i--)
            {
                if (CoverObjectsManager.coverObjectsOfWorld[i] != null)
                    totalCoverObj += CoverObjectsManager.coverObjectsOfWorld[i].Length;
            }
            Debug.Log("Cover Objects : " + totalCoverObj);


            int totalCoverPoints = 0;
            for(int i = CoverObjectsManager.coverPointsOfWorld.Length - 1; i >= 0; i--)
            {
                if(CoverObjectsManager.coverPointsOfWorld[i] != null)
                totalCoverPoints += CoverObjectsManager.coverPointsOfWorld[i].Length;
            }
            Debug.Log("Cover Points : " + totalCoverPoints);
        }
    }

    public static void ChangeState()
    {
        if(mainState == 0)
        {
            mainState = PlayerState.onFoot;
            if (Input.GetMouseButton(1))
            {
                mainCam.GetComponent<CameraScript>().AdjustCameraPivotOrFollow(PlayerState.onFoot, CamState.pivot);
            }
            else
            {
                mainCam.GetComponent<CameraScript>().AdjustCameraPivotOrFollow(PlayerState.onFoot, CamState.follow);
            }
        }
        else
        {
            mainState = PlayerState.inMainCar;
            if (Input.GetMouseButton(1))
            {
                mainCam.GetComponent<CameraScript>().AdjustCameraPivotOrFollow(PlayerState.inMainCar, CamState.pivot);
            }
            else
            {
                mainCam.GetComponent<CameraScript>().AdjustCameraPivotOrFollow(PlayerState.inMainCar, CamState.follow);
            }
        }
    }
    public static void ChangeState(PlayerState state)
    {
        mainState = state;

        if(state == PlayerState.onFoot)
        {
            mainChar.position = mainCar.position - mainCar.right * 3 + new Vector3(0,2,0);
            mainChar.gameObject.SetActive(true);
            uiManager.curAmmoText.gameObject.SetActive(true);
            uiManager.totalAmmoText.gameObject.SetActive(true);

            MainCar mainCarScr = mainCar.GetComponent<MainCar>();
            mainCarScr.vehicleAudioSource.Stop();
            mainCarScr.GetComponent<MainCar>().ResetMotorTorque();

            MainCharacter charComp=mainChar.GetComponent<MainCharacter>();
            charComp.canShoot = true;
            charComp.canReload = true;
            charComp.ResetHandTargets(charComp.currentWeapon);
            AimManager.ResetWeights(charComp);

            if (Input.GetMouseButton(1))
            {
                mainCam.GetComponent<CameraScript>().AdjustCameraPivotOrFollow(PlayerState.onFoot, CamState.pivot);
                uiManager.crosshair.gameObject.SetActive(true);
            }
            else
            {
                mainCam.GetComponent<CameraScript>().AdjustCameraPivotOrFollow(PlayerState.onFoot, CamState.follow);
            }

        }
        else if(state == PlayerState.inMainCar)
        {
            mainChar.gameObject.SetActive(false);
            uiManager.interactionText.gameObject.SetActive(false);
            uiManager.curAmmoText.gameObject.SetActive(false);
            uiManager.totalAmmoText.gameObject.SetActive(false);
            uiManager.crosshair.gameObject.SetActive(false);

            MainCar mainCarScr =mainCar.GetComponent<MainCar>();
            if(mainCarScr.vehicleHealth > 0)
            {
                mainCarScr.vehicleAudioSource.Play();
            }

            if (Input.GetMouseButton(1))
            {
                mainCam.GetComponent<CameraScript>().AdjustCameraPivotOrFollow(PlayerState.inMainCar, CamState.pivot);
            }
            else
            {
                mainCam.GetComponent<CameraScript>().AdjustCameraPivotOrFollow(PlayerState.inMainCar, CamState.follow);
            }
        }

        uiManager.SetVehicleHealthUI();
    }

    #region General Functions
    public static float LerpOrSnap(float valueToChange, float targetValue, float lerpRate)
    {
        if (Mathf.Abs(targetValue - valueToChange) < 0.01)
        {
            return targetValue;
        }
        else
        {
            return Mathf.Lerp(valueToChange, targetValue, lerpRate);
        }
    }
    public static Vector3 LerpOrSnap(Vector3 valueToChange, Vector3 targetValue, float lerpRate)
    {
        if ((targetValue - valueToChange).magnitude < 0.01)
        {
            return targetValue;
        }
        else
        {
            return Vector3.Lerp(valueToChange, targetValue, lerpRate);
        }
    }
    public static float SqrDistance(Vector3 a, Vector3 b)
    {
        return ((a.x - b.x)* (a.x - b.x) + (a.y - b.y) * (a.y - b.y) + (a.z - b.z) * (a.z - b.z));
    }

    public static short[] CopyArray(short[] sourceArray, short[] destinationArray)
    {
        if (sourceArray.Length > destinationArray.Length)
            destinationArray = new short[sourceArray.Length];
        Array.Copy(sourceArray, destinationArray, sourceArray.Length);
        return destinationArray;
    }
    public static bool[] CopyArray(bool[] sourceArray, bool[] destinationArray)
    {
        if (sourceArray.Length > destinationArray.Length)
            destinationArray = new bool[sourceArray.Length];
        Array.Copy(sourceArray, destinationArray, sourceArray.Length);
        return destinationArray;
    }
    public static byte[] CopyArray(byte[] sourceArray, byte[] destinationArray)
    {
        if (sourceArray.Length > destinationArray.Length)
            destinationArray = new byte[sourceArray.Length];
        Array.Copy(sourceArray, destinationArray, sourceArray.Length);
        return destinationArray;
    }
    public static float[] CopyArray(float[] sourceArray, float[] destinationArray)
    {
        if (sourceArray.Length > destinationArray.Length)
            destinationArray = new float[sourceArray.Length];
        Array.Copy(sourceArray, destinationArray, sourceArray.Length);
        return destinationArray;
    }

    public static bool CompareArray(short[] firstArray, short[] secondArray)
    {
        if(firstArray.Length != secondArray.Length)
        {
            return false;
        }
        else
        {
            bool arraysSame = true;
            for(int index = firstArray.Length - 1; index >= 0; index--)
            {
                if (firstArray[index] != secondArray[index])
                {
                    arraysSame = false;
                }
            }
            return arraysSame;
        }
    }
    public static bool CompareArray(float[] firstArray, float[] secondArray)
    {
        if(firstArray.Length != secondArray.Length)
        {
            return false;
        }
        else
        {
            bool arraysSame = true;
            for(int index = firstArray.Length - 1; index >= 0; index--)
            {
                if (firstArray[index] != secondArray[index])
                {
                    arraysSame = false;
                }
            }
            return arraysSame;
        }
    }
    #endregion
}
public enum PlayerState { inMainCar, onFoot, observing , gameOver}
