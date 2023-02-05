using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        mainState = PlayerState.onFoot;
        mainCam = mainCamIsThis;
        mainCam.GetComponent<CameraScript>().AdjustCameraPivot();
        mainChar = mainCharIsThis;
        mainChar.GetComponent<MainCharacter>().RegulateMainChar();
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
        }
        else
        {
            mainState = PlayerState.inMainCar;
        }
        mainCam.GetComponent<CameraScript>().AdjustCameraPivot();
    }
    public static void ChangeState(PlayerState state)
    {
        mainState = state;
    }
}
public enum PlayerState { inMainCar, onFoot }
