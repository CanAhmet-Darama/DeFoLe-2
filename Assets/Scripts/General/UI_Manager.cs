using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UI_Manager : MonoBehaviour
{
    public static bool isLookingInteractable;
    public static InteractableForUI interactionType;

    public static Canvas mainCanvas;

    [Header("Texts")]
    public TextMeshProUGUI curAmmoText;
    public TextMeshProUGUI totalAmmoText;
    public TextMeshProUGUI interactionText;
    public TextMeshProUGUI playerHealthText;
    public TextMeshProUGUI vehicleHealthText;

    [Header("Images")]
    public Image crosshair;
    public Image sniperZoomScreen;
    public Image weaponIcon;

    public Sprite[] weaponIconSprites;

    [Header("Player Health")]
    public GameObject playerHealthFill;
    
    MainCharacter mainChar;
    float maxHealth;

    [Header("Vehicle")]
    public GameObject vehicleHealthFill;
    public GameObject vehicleHealthBarWhole;

    MainCar mainCar;
    float maxVehicleHealth;

    public Coroutine weaponIconOpacityCoroutine;

    void Start()
    {
        mainChar = GameManager.mainChar.GetComponent<MainCharacter>();
        mainCar = GameManager.mainCar.GetComponent<MainCar>();
        maxHealth = mainChar.health;
        maxVehicleHealth = mainCar.vehicleHealth;
    }

    void Update()
    {
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
    public void InteractionUISetter()
    {
            string interactText;
            switch(interactionType)
            {
                case InteractableForUI.mainCar:
                    interactText = "Press F to get in car";
                    break;
                case InteractableForUI.healthPack:
                    interactText = "Press F to get Health";
                    break;
                case InteractableForUI.ammoPack:
                    interactText = "Press F to get Ammo";
                    break;
                default: interactText = ""; 
                    break;
            }
            interactionText.text = interactText;
    }

    public void SetVehicleHealthUI()
    {
        if (mainCar.vehicleHealth > 0)
        {
            vehicleHealthFill.transform.localScale = new Vector3(mainCar.vehicleHealth / maxVehicleHealth, vehicleHealthFill.transform.localScale.y, vehicleHealthFill.transform.localScale.z);
            vehicleHealthText.text = "Vehicle  : " + mainCar.vehicleHealth;
        }
        else
        {
            vehicleHealthFill.transform.localScale = Vector3.zero;
            vehicleHealthText.text = "BROKEN";
        }
        
        if(GameManager.mainState == PlayerState.inMainCar && !vehicleHealthBarWhole.activeInHierarchy)
        {
            vehicleHealthBarWhole.SetActive(true);
        }
        else if(GameManager.mainState != PlayerState.inMainCar && vehicleHealthBarWhole.activeInHierarchy)
        {
            vehicleHealthBarWhole.SetActive(false);
        }
    }

    public void ReduceOpacity(Image opaqueImage, float waitDurat, float reduceDurat)
    {
        opaqueImage.SetNativeSize();
        if(weaponIconOpacityCoroutine != null)
        {
            StopCoroutine(weaponIconOpacityCoroutine);
            Color currentColor = opaqueImage.color;
            currentColor.a = 1;
            opaqueImage.color = currentColor;
        }
        weaponIconOpacityCoroutine = StartCoroutine(ReduceOpacityNumerator(opaqueImage, waitDurat, reduceDurat / 100));
    }
    IEnumerator ReduceOpacityNumerator(Image opaqueImage, float waitDurat, float reduceRate)
    {
        yield return new WaitForSeconds(waitDurat);
        while(opaqueImage.color.a > 0)
        {
            Color currentColor = opaqueImage.color;
            currentColor.a -= reduceRate;
            opaqueImage.color = currentColor;
            yield return null;
        }
    }

    #region Button Functions
    public virtual void SettingsButton()
    {
        ChangePanels(PanelType.settings);
    }

    #endregion

    public virtual void ChangePanels(PanelType pType)
    {
        switch (pType)
        {
            case PanelType.none:
                //settingsPanel.SetActive(false);
                break;
            case PanelType.settings:
                //settingsPanel.SetActive(true);
                break;
        }
    }

    public enum InteractableForUI { mainCar, healthPack, ammoPack}
}
public enum PanelType { none,settings}
