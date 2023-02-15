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


    void Start()
    {
        switch (SceneManager.GetActiveScene().name)
        {
            case "Level 1":
                mainState = PlayerState.onFoot;
                mainCam = Camera.main.transform;
                mainCam.GetComponent<CameraScript>().AdjustCameraPivotOrFollow(PlayerState.onFoot, CamState.follow);
                mainChar = mainCharIsThis;
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
}
public enum PlayerState { inMainCar, onFoot }
