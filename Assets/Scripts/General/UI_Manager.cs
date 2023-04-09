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
    public TextMeshProUGUI playerHealthText;

    public Image crosshair;
    public Image sniperZoomScreen;

    public GameObject playerHealthFill;
    
    MainCharacter mainChar;
    float maxHealth;

    void Start()
    {
        mainChar = GameManager.mainChar.GetComponent<MainCharacter>();
        maxHealth = mainChar.health;
    }

    void Update()
    {
        GeneralUISetter();
    }
    public void SetAmmoUI()
    {
        curAmmoText.text = Convert.ToString(mainChar.currentWeapon.currentAmmo);
        totalAmmoText.text = Convert.ToString(mainChar.ammoCounts[(int)mainChar.currentWeapon.weaponType]);
        if(mainChar.weaponState != GeneralCharacter.WeaponState.ranged && (curAmmoText.gameObject.activeInHierarchy || totalAmmoText.gameObject.activeInHierarchy))
        {
            curAmmoText.gameObject.SetActive(false);
            totalAmmoText.gameObject.SetActive(false);
        }
        else if(!(curAmmoText.gameObject.activeInHierarchy || totalAmmoText.gameObject.activeInHierarchy))
        {
            curAmmoText.gameObject.SetActive(true);
            totalAmmoText.gameObject.SetActive(true);
        }
    }
    public void SetHealthUI()
    {
        if(mainChar.health > 0)
        {
            playerHealthFill.transform.localScale = new Vector3(mainChar.health / maxHealth, playerHealthFill.transform.localScale.y, playerHealthFill.transform.localScale.z) ;
            playerHealthText.text = "Health : " + mainChar.health;
        }
        else
        {
            playerHealthFill.transform.localScale = Vector3.zero;
            playerHealthText.text = "DEAD";
        }
    }
    void GeneralUISetter()
    {
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

    public enum InteractableForUI { mainCar, healthPack, ammoPack}
}
