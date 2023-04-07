using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Manager : MonoBehaviour
{
    public static bool isLookingInteractable;
    public static InteractableForUI interactionType;

    public TextMeshProUGUI curAmmoText;
    public TextMeshProUGUI totalAmmoText;
    public TextMeshProUGUI interactionText;

    public Image crosshair;
    public Image sniperZoomScreen;

    MainCharacter mainChar;

    void Start()
    {
        mainChar = GameManager.mainChar.GetComponent<MainCharacter>();
    }

    void Update()
    {
        PlayerTexts();
    }
    void PlayerTexts()
    {
        curAmmoText.text = Convert.ToString(mainChar.currentWeapon.currentAmmo);
        totalAmmoText.text = Convert.ToString(mainChar.ammoCounts[(int)mainChar.currentWeapon.weaponType]);
        if (isLookingInteractable)
        {

            string interactText;
            switch(interactionType)
            {
                case InteractableForUI.mainCar:
                    interactText = "Press F to get in car";
                    break;
                default: interactText = ""; break;
            }
            interactionText.text = interactText;
        }
    }

    public enum InteractableForUI { mainCar}
}
