using System;
using System.IO;
using Newtonsoft.Json;
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
    static string saveDataPath;

    void Awake()
    {
        saveDataPath = Application.dataPath + "/Saves/SaveData.json";
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
            SaveGame();
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadGame();
        }
    }

    public static void ChangeState()
    {
        if (mainState == 0)
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

        if (state == PlayerState.onFoot)
        {
            mainChar.position = mainCar.position - mainCar.right * 3 + new Vector3(0, 2, 0);
            mainChar.gameObject.SetActive(true);
            uiManager.curAmmoText.gameObject.SetActive(true);
            uiManager.totalAmmoText.gameObject.SetActive(true);

            MainCar mainCarScr = mainCar.GetComponent<MainCar>();
            mainCarScr.vehicleAudioSource.Stop();
            mainCarScr.GetComponent<MainCar>().ResetMotorTorque();

            MainCharacter charComp = mainChar.GetComponent<MainCharacter>();
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
        else if (state == PlayerState.inMainCar)
        {
            mainChar.gameObject.SetActive(false);
            uiManager.interactionText.gameObject.SetActive(false);
            uiManager.curAmmoText.gameObject.SetActive(false);
            uiManager.totalAmmoText.gameObject.SetActive(false);
            uiManager.crosshair.gameObject.SetActive(false);

            MainCar mainCarScr = mainCar.GetComponent<MainCar>();
            if (mainCarScr.vehicleHealth > 0)
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

    public static void SaveGame()
    {
        GameData gameDataForSave = new GameData(true);
        EnemyManager.SaveAllEnemies(gameDataForSave);
        InteractableSpecial.SaveInteractableObjects(gameDataForSave);
        string saveJSON = JsonConvert.SerializeObject(gameDataForSave, Formatting.Indented);
        File.WriteAllText(saveDataPath, saveJSON);
    }
    public static void LoadGame()
    {
        string loadJson = File.ReadAllText(saveDataPath);
        GameData gameDataForLoad = JsonConvert.DeserializeObject<GameData>(loadJson);
        EnemyManager.LoadAllEnemies(gameDataForLoad);
        GameData.LoadFieldsOfCharAndVehicle(gameDataForLoad);
        GameData.LoadEnvironmentObjects(gameDataForLoad);
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
        return ((a.x - b.x) * (a.x - b.x) + (a.y - b.y) * (a.y - b.y) + (a.z - b.z) * (a.z - b.z));
    }

    //public static void CopyArray(short[] sourceArray, short[] destinationArray)
    //{
    //    if (sourceArray.Length > destinationArray.Length)
    //        destinationArray = new short[sourceArray.Length];
    //    Array.Copy(sourceArray, destinationArray, sourceArray.Length);
    //}
    //public static void CopyArray(bool[] sourceArray, bool[] destinationArray)
    //{
    //    if (sourceArray.Length > destinationArray.Length)
    //        destinationArray = new bool[sourceArray.Length];
    //    Array.Copy(sourceArray, destinationArray, sourceArray.Length);
    //}
    //public static void CopyArray(byte[] sourceArray, byte[] destinationArray)
    //{
    //    if (sourceArray.Length > destinationArray.Length)
    //        destinationArray = new byte[sourceArray.Length];
    //    Array.Copy(sourceArray, destinationArray, sourceArray.Length);
    //}
    //public static void CopyArray(float[] sourceArray, float[] destinationArray)
    //{
    //    if (sourceArray.Length > destinationArray.Length)
    //        destinationArray = new float[sourceArray.Length];
    //    Array.Copy(sourceArray, destinationArray, sourceArray.Length);
    //}
    public static void CopyArray<T>(T[] sourceArray, ref T[] destinationArray)
    {
        if (sourceArray.Length > destinationArray.Length)
            destinationArray = new T[sourceArray.Length];
        Array.Copy(sourceArray, destinationArray, sourceArray.Length);
    }

    //public static void IncreaseArray<T>(ref T[] arrayToInc, int increaseAmmount = 1){
    //    if(arrayToInc == null || arrayToInc.Length == 0)
    //    {
    //        arrayToInc = new T[1];
    //    }
    //    else
    //    {
    //        T[] holderArray = new T[arrayToInc.Length];
    //        CopyArray(arrayToInc, holderArray);
    //        arrayToInc = new T[arrayToInc.Length + increaseAmmount];
    //        CopyArray(holderArray, arrayToInc);
    //    }
    //}
    public static void IncreaseArray<T>(ref T[] arrayToInc, int increaseAmmount = 1){
        if(arrayToInc == null || arrayToInc.Length == 0)
        {
            arrayToInc = new T[1];
        }
        else
        {
            T[] holderArray = new T[arrayToInc.Length];
            CopyArray(arrayToInc, ref holderArray);
            arrayToInc = new T[arrayToInc.Length + increaseAmmount];
            CopyArray(holderArray, ref arrayToInc);
        }
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

[System.Serializable]
public class GameData
{
    #region For Player
    public float[] playerPos;
    public float[] playerRot;
    public short playerHealth;
    public short[] playerAmmoCounts;
    public byte currentWeaponIndex;
    #endregion

    #region For Vehicle
    public float[] vehiclePos;
    public float[] vehicleRot;
    public short vehicleHealth;
    #endregion

    #region For Enemies
    public float[][][] enemyPoses;
    public short[][] enemyHealths;
    public short[][][] enemyAmmoCounts;
    public bool[] campsAlerted;
    #endregion

    #region Environment
    public bool[] interactablesTaken;
    public bool[] envObjectsDestroyed;
    #endregion

    public GameData(bool forCreating = false)
    {
        if (forCreating)
        {
            MainCharacter mainCharScr = GameManager.mainChar.GetComponent<MainCharacter>();
            playerPos = new float[3] { GameManager.mainChar.position.x, GameManager.mainChar.position.y, GameManager.mainChar.position.z};
            playerRot = new float[3] { GameManager.mainChar.eulerAngles.x, GameManager.mainChar.eulerAngles.y, GameManager.mainChar.eulerAngles.z };
            playerHealth = mainCharScr.health;
            playerAmmoCounts = new short[5];
            GameManager.CopyArray(mainCharScr.ammoCounts, ref playerAmmoCounts);
            currentWeaponIndex = (byte)mainCharScr.currentWeapon.weaponType;

            vehiclePos = new float[3] { GameManager.mainCar.position.x, GameManager.mainCar.position.y, GameManager.mainCar.position.z };
            vehicleRot = new float[3] { GameManager.mainCar.eulerAngles.x, GameManager.mainCar.eulerAngles.y, GameManager.mainCar.eulerAngles.z };
            vehicleHealth = GameManager.mainCar.GetComponent<MainCar>().vehicleHealth;

        }

    }

    public static void LoadFieldsOfCharAndVehicle(GameData gameDataToUse)
    {
        Transform mainChar = GameManager.mainChar;
        mainChar.position = new Vector3(gameDataToUse.playerPos[0], gameDataToUse.playerPos[1], gameDataToUse.playerPos[2]);
        mainChar.eulerAngles = new Vector3(gameDataToUse.playerRot[0], gameDataToUse.playerRot[1], gameDataToUse.playerRot[2]);
        MainCharacter mainCharScr = GameManager.mainChar.GetComponent<MainCharacter>();
        mainCharScr.health = gameDataToUse.playerHealth;
        GameManager.CopyArray(gameDataToUse.playerAmmoCounts, ref mainCharScr.ammoCounts);
        mainCharScr.ChangeWeapon(mainCharScr.weaponScripts[gameDataToUse.currentWeaponIndex]);

        MainCar mainCarScr = GameManager.mainCar.GetComponent<MainCar>();
        mainCarScr.transform.position = new Vector3(gameDataToUse.vehiclePos[0], gameDataToUse.vehiclePos[1], gameDataToUse.vehiclePos[2]);
        mainCarScr.transform.eulerAngles = new Vector3(gameDataToUse.vehicleRot[0], gameDataToUse.vehicleRot[1], gameDataToUse.vehicleRot[2]);
        mainCarScr.vehicleHealth = gameDataToUse.vehicleHealth;
    }
    public static void LoadEnvironmentObjects(GameData gameDataToUse)
    {
        InteractableSpecial.LoadInteractableObjects(gameDataToUse);
    }
}
