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
    public static Transform mainChar;
    [SerializeField] Transform mainCamIsThis;
    [SerializeField] Transform mainCharIsThis;


    void Awake()
    {
        switch (SceneManager.GetActiveScene().name)
        {
            case "Level 1":
                mainState = PlayerState.onFoot;
                mainCam = Camera.main.transform;
                mainChar = mainCharIsThis;
                mainCam.GetComponent<CameraScript>().AdjustCameraPivotOrFollow(PlayerState.onFoot, CamState.follow);
                mainChar.GetComponent<MainCharacter>().RegulateMainChar();
                break;
        }

    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            ChangeState();
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
        if (Input.GetMouseButton(1))
        {
            mainCam.GetComponent<CameraScript>().AdjustCameraPivotOrFollow(state, CamState.pivot);
        }
        else
        {
            mainCam.GetComponent<CameraScript>().AdjustCameraPivotOrFollow(state, CamState.follow);
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

    #endregion
}
public enum PlayerState { inMainCar, onFoot }
