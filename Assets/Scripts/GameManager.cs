using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("State Bools")]
    public static PlayerState mainState;

    [Header("Settings")]
    public static byte mouseSensitivity = 10;

    void Start()
    {
        mainState = PlayerState.onFoot;
    }


    void Update()
    {
        
    }
}
public enum PlayerState { inMainCar, onFoot }
