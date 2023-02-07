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
                mainCam.GetComponent<CameraScript>().AdjustCameraPivot();
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
    void LateUpdate()
    {
        Debug.Log(mainChar.GetComponent<MainCharacter>().animStateSpeed + " " + mainChar.GetComponent<MainCharacter>().animStatePriDir + " " + mainChar.GetComponent<MainCharacter>().animStateSecDir);
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
