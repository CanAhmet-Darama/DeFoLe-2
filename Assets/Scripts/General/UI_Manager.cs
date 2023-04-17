using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UI_Manager : MonoBehaviour
{
    public static bool isLookingInteractable;
    public static InteractableForUI interactionType;

    public static Canvas mainCanvas;
    [Header("ESC Menu")]
    public GameObject escMenuPanel;
    public GameObject settingsMenuPanel;
    public GameObject normalUIPanel;
    public GameObject areYouSurePanel;

    [Header("Other UI Elements")]
    public TextMeshProUGUI notificationText;
    public TextMeshProUGUI[] campTexts = new TextMeshProUGUI[4];
    public GameObject startingPoint;
    public GameObject[] startingTexts;
    public Image endgamePanel;


    [Header("Texts")]
    public TextMeshProUGUI curAmmoText;
    public TextMeshProUGUI totalAmmoText;
    public TextMeshProUGUI interactionText;
    public TextMeshProUGUI playerHealthText;
    public TextMeshProUGUI vehicleHealthText;
    public TextMeshProUGUI areYouSureText;
    public TextMeshProUGUI loadGameButtonText;
    

    [Header("Images")]
    public Image crosshair;
    public Image sniperZoomScreen;
    public Image weaponIcon;

    public Sprite[] weaponIconSprites;

    [Header("Player Health")]
    public GameObject playerHealthFill;
    

    [Header("Vehicle")]
    public GameObject vehicleHealthFill;
    public GameObject vehicleHealthBarWhole;

    MainCar mainCar;
    float maxVehicleHealth;

    [Header("Different Stuff")]
    public Coroutine weaponIconOpacityCoroutine;
    public Coroutine notificationOpacityCoroutine;
    bool askingForSave;
    bool atStartingPoint;

    [Header("Music Stuff")]
    public AudioSource musicSource;
    public AudioClip[] musics; // First one is win, second one is death

    void Start()
    {
        mainCar = GameManager.mainCar.GetComponent<MainCar>();
        MainCharacter.maxHealth = GameManager.mainCharScr.health;
        maxVehicleHealth = mainCar.vehicleHealth;

        atStartingPoint = true;
        for (int i = campTexts.Length - 1; i >= 0; i--)
        {
            campTexts[i].transform.LookAt(GameManager.mainCam);
            campTexts[i].transform.localEulerAngles += new Vector3(0, 180, 0);
        }
    }

    void Update()
    {
        if (atStartingPoint)
        {
            UI3DSetter();
        }
    }

    public void SetAmmoUI()
    {
        if(GameManager.mainCharScr == null)
        {
            Debug.Log("NULL");
        }
        curAmmoText.text = "" + GameManager.mainCharScr.currentWeapon.currentAmmo;
        totalAmmoText.text = "" + GameManager.mainCharScr.ammoCounts[(int)GameManager.mainCharScr.currentWeapon.weaponType];
        if(GameManager.mainCharScr.weaponState != GeneralCharacter.WeaponState.ranged && (curAmmoText.gameObject.activeInHierarchy || totalAmmoText.gameObject.activeInHierarchy))
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
        if(GameManager.mainCharScr.health > 0)
        {
            playerHealthFill.transform.localScale = new Vector3(MainCharacter.healthRate, 1, 1) ;
            playerHealthText.text = "Health : " + GameManager.mainCharScr.health;
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

    public void ReduceOpacity(MaskableGraphic opaqueImage, float waitDurat, float reduceDurat, bool forWeaponIcon)
    {
        opaqueImage.SetNativeSize();
        if (forWeaponIcon)
        {
            if(weaponIconOpacityCoroutine != null)
            {
                StopCoroutine(weaponIconOpacityCoroutine);
                Color currentColor = opaqueImage.color;
                currentColor.a = 1;
                opaqueImage.color = currentColor;
            }
            weaponIconOpacityCoroutine = StartCoroutine(ReduceOpacityNumerator(opaqueImage, waitDurat, reduceDurat / 100));
        }
        else
        {
            if (notificationOpacityCoroutine != null)
            {
                StopCoroutine(notificationOpacityCoroutine);
                Color currentColor = opaqueImage.color;
                currentColor.a = 1;
                opaqueImage.color = currentColor;
            }
            notificationOpacityCoroutine = StartCoroutine(ReduceOpacityNumerator(opaqueImage, waitDurat, reduceDurat / 100));
        }
    }
    IEnumerator ReduceOpacityNumerator(MaskableGraphic opaqueImage, float waitDurat, float reduceRate)
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
    public void SaveGameButton()
    {
        if(File.Exists(GameManager.saveDataPath))
        {
            askingForSave = true;
            ChangePanels(PanelType.areYouSure);
        }
        else
        {
            GameManager.SaveGame();
        }
    }
    public void LoadGameButton()
    {
        if (File.Exists(GameManager.saveDataPath))
        {
            askingForSave = false;
            ChangePanels(PanelType.areYouSure);
        }

    }
    public void AreYouSureYes()
    {
        if (askingForSave)
        {
            GameManager.SaveGame();
        }
        else
        {
            GameManager.LoadGameCompletely();
        }
    }
    public void AreYouSureNo()
    {
        ChangePanels(PanelType.escMenu);
    }

    #endregion

    void UI3DSetter()
    {
        float sqrDistance = GameManager.SqrDistance(startingPoint.transform.position, GameManager.mainCam.position);
        if (sqrDistance > 100*100)
        {
            atStartingPoint = false;
            campTexts[0].transform.parent.gameObject.SetActive(false);
            for (int i = startingTexts.Length - 1; i >= 0; i--)
            {
                startingTexts[i].gameObject.SetActive(false);
            }
        }
        for (int i = startingTexts.Length-1; i >= 0; i--)
        {
            startingTexts[i].transform.forward = -(GameManager.mainCam.position + new Vector3(0, 1, 0) - startingTexts[i].transform.position).normalized;
        }
    }
    public void CampClearedText(byte campNumber)
    {
        if(!notificationText.gameObject.activeInHierarchy)
        {
            notificationText.gameObject.SetActive(true);
        }
        notificationText.text = "Enemy camp " + campNumber + " is completely cleared";
        ReduceOpacity(notificationText, 4, 2, false);        
    }
    public void GameFinishedText()
    {
        if(!endgamePanel.gameObject.activeInHierarchy)
        {
            endgamePanel.gameObject.SetActive(true);
        }
        musicSource.PlayOneShot(musics[0]);
        ReduceOpacity(endgamePanel, 5, 3, false);        
    }
    public void GameOverDeathText()
    {
        if (!endgamePanel.gameObject.activeInHierarchy)
        {
            endgamePanel.transform.Find("Endgame Text").GetComponent<TextMeshProUGUI>().text = "You are dead. You lost !!!";
            endgamePanel.gameObject.SetActive(true);
        }
        crosshair.gameObject.SetActive(false);
        sniperZoomScreen.gameObject.SetActive(false);

        musicSource.clip = musics[1];
        musicSource.loop = true;
        musicSource.Play();
        
        ReduceOpacity(endgamePanel, 5, 3, false);
    }

    public virtual void ChangePanels(PanelType pType)
    {
        switch (pType)
        {
            case PanelType.none:
                settingsMenuPanel.SetActive(false);
                escMenuPanel.SetActive(false);
                normalUIPanel.SetActive(true);
                areYouSurePanel.SetActive(false);
                break;
            case PanelType.settings:
                settingsMenuPanel.SetActive(true);
                escMenuPanel.SetActive(false);
                normalUIPanel.SetActive(false);
                areYouSurePanel.SetActive(false);
                break;
            case PanelType.escMenu:
                if (File.Exists(GameManager.saveDataPath))
                {
                    loadGameButtonText.text = "Load Game";
                    loadGameButtonText.color = Color.black;
                }
                else
                {
                    loadGameButtonText.text = "NO SAVED GAME";
                    loadGameButtonText.color = Color.red;
                }
                settingsMenuPanel.SetActive(false);
                escMenuPanel.SetActive(true);
                normalUIPanel.SetActive(false);
                areYouSurePanel.SetActive(false);
                break;
            case PanelType.areYouSure:
                settingsMenuPanel.SetActive(false);
                escMenuPanel.SetActive(false);
                normalUIPanel.SetActive(false);
                ChangeQuestionText();
                areYouSurePanel.SetActive(true);
                break;
        }
    }
    void ChangeQuestionText()
    {
        if (askingForSave)
        {
            areYouSureText.text = "Are you sure to overrite the existing save file?";
        }
        else
        {
            areYouSureText.text = "Are you sure to load the save game? Any unsaved progress will be lost";
        }
    }

    public enum InteractableForUI { mainCar, healthPack, ammoPack}
}
public enum PanelType { none,settings, escMenu, areYouSure}
