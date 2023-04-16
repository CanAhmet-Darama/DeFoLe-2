using System;
using System.Collections;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("State Bools")]
    public static PlayerState mainState;

    [Header("Settings")]
    public static float mouseSensitivity = 10;
    public static float generalSoundMultiplier;
    public static float grassDisappearDistance;
    public static float detailDiminishMultiplier;

    [Header("Instances")]
    public static Transform mainCam;
    public static Transform mainCar;
    public static Transform mainChar;
    public static MainCharacter mainCharScr;
    public static Terrain mainTerrain;
    public static UI_Manager uiManager;
    public static EnemyManager enemyManager;
    public static SettingsManager settingsManager;

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
    public static string saveDataPath;
    public static bool isGamePaused;

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
                mainCharScr = mainChar.GetComponent<MainCharacter>();
                mainCharScr.RegulateMainChar();

                numberOfCamps = 4;
                enemyCamps = enemyCampsAreThese;
                enemyManager = enemyManagerIsThis;
                enemyManager.EnemyManagerStart();

                settingsManager.SettingsManagerAssignments(false);

                TerrainManager.mainTerrain = mainTerrain;
                mainTerrain.detailObjectDistance = SettingsManager.detailDistance;
                uiManager.settingsMenuPanel = SettingsManager.settingsPanel;
                break;
        }
    }

    void Start()
    {
        mainCharScr.MainCharStart();
        if (MainMenuManager.hasLoadedGame)
        {
            LoadGame();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseGame();
            if (uiManager.escMenuPanel.activeInHierarchy || uiManager.settingsMenuPanel.activeInHierarchy || uiManager.areYouSurePanel.activeInHierarchy)
            {
                uiManager.ChangePanels(PanelType.none);
            }
            else
            {
                uiManager.ChangePanels(PanelType.escMenu);
            }
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

            mainCharScr.canShoot = true;
            mainCharScr.canReload = true;
            mainCharScr.ResetHandTargets(mainCharScr.currentWeapon);
            AimManager.ResetWeights(mainCharScr);

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
        EnvObject.SaveDestroyableObjects(gameDataForSave);
        string saveJSON = JsonConvert.SerializeObject(gameDataForSave, Formatting.Indented);
        File.WriteAllText(saveDataPath, saveJSON);
    }
    public static void LoadGameCompletely()
    {
        MainMenuManager.hasLoadedGame = true;
        SceneManager.LoadScene("Level 1");
    }
    public static void LoadGame()
    {
        mainCharScr.StartCoroutine(WaitOneFrameToLoad());
    }
    public static void PauseGame()
    {
        if (Time.timeScale == 1)
        {
            Time.timeScale = 0;
            AudioListener.volume = 0;
            mouseSensitivity = 0;
            isGamePaused = true;
        }
        else if(mainCharScr.health > 0){
            Time.timeScale = 1;
            AudioListener.volume = settingsManager.GetVolume();
            mouseSensitivity = SettingsManager.mouseSensitivityValue;
            isGamePaused = false;
        }
    }
    public static IEnumerator WaitOneFrameToLoad()
    {
        yield return null;
        yield return null;
        string loadJson = File.ReadAllText(saveDataPath);
        GameData gameDataForLoad = JsonConvert.DeserializeObject<GameData>(loadJson);
        EnemyManager.LoadAllEnemies(gameDataForLoad);
        GameData.LoadFieldsOfCharAndVehicle(gameDataForLoad);
        GameData.LoadEnvironmentObjects(gameDataForLoad);

    }
    public static void ExitGame()
    {
        Application.Quit();
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
        if ((targetValue - valueToChange).sqrMagnitude < 0.0001)
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

    public static void AddToArray<T>(T obj, ref T[] arrayToAdd)
    {
        IncreaseArray(ref arrayToAdd);
        arrayToAdd[arrayToAdd.Length - 1] = obj;
    }

    public static void IncreaseArray<T>(ref T[] arrayToInc, int increaseAmmount = 1) {
        if (arrayToInc == null || arrayToInc.Length == 0)
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

    public static void SortObjectArrayByDistance<T>(ref T[] arrayToSort, Transform[] arrayToTakeDistance,Vector3 posToTakeDistance)
    {
        bool hasSwapped = true;
        T holderObj;
        Transform holderTransform;
        while (hasSwapped)
        {
            hasSwapped = false;
            for (short index = 0, limit = (short)(arrayToSort.Length - 1); index < limit; index++)
            {
                if ((arrayToTakeDistance[index].position - posToTakeDistance).sqrMagnitude >
                    (arrayToTakeDistance[index + 1].position - posToTakeDistance).sqrMagnitude)
                {
                    holderObj = arrayToSort[index];
                    arrayToSort[index] = arrayToSort[index + 1];
                    arrayToSort[index + 1] = holderObj;
                    hasSwapped = true;

                    holderTransform = arrayToTakeDistance[index];
                    arrayToTakeDistance[index] = arrayToTakeDistance[index + 1];
                    arrayToTakeDistance[index + 1] = holderTransform;
                }
            }
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
    #region General
    public PlayerState mainState;
    #endregion
    
    #region For Player
    public float[] playerPos;
    public float[] playerRot;
    public short playerHealth;
    public short[] playerAmmoCounts;
    public short[] playerCurrentAmmoCounts;
    public byte currentWeaponIndex;
    #endregion

    #region For Vehicle
    public float[] vehiclePos;
    public float[] vehicleRot;
    public float[] vehicleVelocity;
    public float[] vehicleAngularVelocity;
    public short vehicleHealth;
    #endregion

    #region For Enemies
    public float[][][] enemyPoses;
    public short[][] enemyHealths;
    public short[][][] enemyAmmoCounts;
    public short[][][] enemyCurrentAmmoCounts;
    public bool[] campsAlerted;
    #endregion

    #region Environment
    public bool[] interactablesTaken;
    public float[][] interactablePositions;
    public bool[] envObjectsDestroyed;
    public float[][] envObjPoses;
    public bool[][] envObjectsSubPartsDestroyed;
    #endregion

    public GameData(bool forCreating = false)
    {
        if (forCreating)
        {
            mainState = GameManager.mainState;

            MainCharacter mainCharScr = GameManager.mainChar.GetComponent<MainCharacter>();
            playerPos = new float[3] { GameManager.mainChar.position.x, GameManager.mainChar.position.y, GameManager.mainChar.position.z};
            playerRot = new float[3] { GameManager.mainChar.eulerAngles.x, GameManager.mainChar.eulerAngles.y, GameManager.mainChar.eulerAngles.z };
            playerHealth = mainCharScr.health;
            playerAmmoCounts = new short[5];
            GameManager.CopyArray(mainCharScr.ammoCounts, ref playerAmmoCounts);
            currentWeaponIndex = (byte)mainCharScr.currentWeapon.weaponType;
            playerCurrentAmmoCounts = new short[mainCharScr.weaponScripts.Length];
            for(int i = mainCharScr.weaponScripts.Length- 1; i >= 0; i--)
            {
                playerCurrentAmmoCounts[i] = mainCharScr.weaponScripts[i].currentAmmo;
            }

            MainCar mainCarScr = GameManager.mainCar.GetComponent<MainCar>();
            vehiclePos = new float[3] { GameManager.mainCar.position.x, GameManager.mainCar.position.y, GameManager.mainCar.position.z };
            vehicleRot = new float[3] { GameManager.mainCar.eulerAngles.x, GameManager.mainCar.eulerAngles.y, GameManager.mainCar.eulerAngles.z };
            vehicleVelocity = new float[3] { mainCarScr.vehicleRb.velocity.x, mainCarScr.vehicleRb.velocity.y, mainCarScr.vehicleRb.velocity.z};
            vehicleAngularVelocity = new float[3] { mainCarScr.vehicleRb.angularVelocity.x, mainCarScr.vehicleRb.angularVelocity.y, mainCarScr.vehicleRb.angularVelocity.z };
            vehicleHealth = mainCarScr.vehicleHealth;

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
        for (int i = mainCharScr.weaponScripts.Length - 1; i >= 0; i--)
        {
            mainCharScr.weaponScripts[i].currentAmmo = (byte)gameDataToUse.playerCurrentAmmoCounts[i];
        }
        GameManager.uiManager.SetAmmoUI();

        if(GameManager.mainState != gameDataToUse.mainState)
        {
            GameManager.ChangeState(gameDataToUse.mainState);
        }

        MainCar mainCarScr = GameManager.mainCar.GetComponent<MainCar>();
        mainCarScr.transform.position = new Vector3(gameDataToUse.vehiclePos[0], gameDataToUse.vehiclePos[1], gameDataToUse.vehiclePos[2]);
        mainCarScr.transform.eulerAngles = new Vector3(gameDataToUse.vehicleRot[0], gameDataToUse.vehicleRot[1], gameDataToUse.vehicleRot[2]);
        mainCarScr.vehicleRb.velocity = new Vector3(gameDataToUse.vehicleVelocity[0], gameDataToUse.vehicleVelocity[1], gameDataToUse.vehicleVelocity[2]);
        mainCarScr.vehicleRb.angularVelocity = new Vector3(gameDataToUse.vehicleAngularVelocity[0], gameDataToUse.vehicleAngularVelocity[1], gameDataToUse.vehicleAngularVelocity[2]);
        mainCarScr.vehicleHealth = gameDataToUse.vehicleHealth;
        
    }
    public static void LoadEnvironmentObjects(GameData gameDataToUse)
    {
        InteractableSpecial.LoadInteractableObjects(gameDataToUse);
        EnvObject.LoadDestroyableObjects(gameDataToUse);
    }
}
