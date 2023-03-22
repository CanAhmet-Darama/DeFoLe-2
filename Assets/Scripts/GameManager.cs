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

    [Header("Instances")]
    public static Transform mainCam;
    public static Transform mainCar;
    public static Transform mainChar;
    public static Terrain mainTerrain;
    public static UI_Manager uiManager;
    public static GameObject[] weaponPrefabs = new GameObject[6];
    public static GameObject[] enemyCamps;
    [SerializeField] GameObject[] weaponPrefabsAreThese;
    [SerializeField] Transform mainCamIsThis;
    [SerializeField] Transform mainCharIsThis;
    [SerializeField] Transform mainCarIsThis;
    [SerializeField] Terrain mainTerrainIsThis;
    [SerializeField] UI_Manager userInterfaceManagerIsThis;
    [SerializeField] GameObject[] enemyCampsAreThese;


    [Header("General Numbers")]
    [HideInInspector] public static byte numberOfCamps = 3;

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

                numberOfCamps = 3;
                enemyCamps = new GameObject[numberOfCamps];
                enemyCamps = enemyCampsAreThese;
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
            uiManager.crosshair.gameObject.SetActive(true);
            uiManager.curAmmoText.gameObject.SetActive(true);
            uiManager.totalAmmoText.gameObject.SetActive(true);

            mainCar.GetComponent<MainCar>().ResetMotorTorque();

            if (Input.GetMouseButton(1))
            {
                mainCam.GetComponent<CameraScript>().AdjustCameraPivotOrFollow(PlayerState.onFoot, CamState.pivot);
            }
            else
            {
                mainCam.GetComponent<CameraScript>().AdjustCameraPivotOrFollow(PlayerState.onFoot, CamState.follow);
            }

            MainCharacter charComp=mainChar.GetComponent<MainCharacter>();
            charComp.canShoot = true;
            charComp.canReload = true;
        }
        else if(state == PlayerState.inMainCar)
        {
            mainChar.gameObject.SetActive(false);
            uiManager.interactionText.gameObject.SetActive(false);
            uiManager.curAmmoText.gameObject.SetActive(false);
            uiManager.totalAmmoText.gameObject.SetActive(false);
            uiManager.crosshair.gameObject.SetActive(false);

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
    #endregion
}
public enum PlayerState { inMainCar, onFoot, observing }
