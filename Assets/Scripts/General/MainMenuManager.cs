using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : UI_Manager
{
    [Header("MAIN MENU")]
    [Header("Special")]
    public Light spotLight;
    Vector3 initialLightRotation;
    Quaternion newLightRotation;

    [Header("Buttons")]
    public Button newGameButton;
    public Button continueGameButton;
    public Button settingsButton;
    public Button exitGameButton;
    public Button developerButton;

    public Button yesButton;
    public Button noButton;

    [Header("Objects")]
    public GameObject areYouSurePanelMainMenu;
    public GameObject mainButtonsPanel;
    public GameObject developerPanel;

    [Header("Texts")]
    public TextMeshProUGUI areYouSureTextMainMenu;
    public TextMeshProUGUI continueGameText;

    [Header("Other")]
    AreYouSureQuestion question;
    public bool hasSavedGame;
    public static bool hasLoadedGame;

    void Start()
    {
        initialLightRotation = spotLight.transform.eulerAngles;
        StartCoroutine(RandomAngle());

        GameManager.saveDataPath = Application.dataPath + "/Saves/SaveData.json";
        hasSavedGame = File.Exists(GameManager.saveDataPath);
        if(!hasSavedGame )
        {
            continueGameText.text = "NO SAVED GAME";
            continueGameText.color = Color.red;
        }
        else
        {
            continueGameText.text = "CONTINUE";
            continueGameText.color = Color.white;
        }

    }

    void Update()
    {
        SpotLightRotate();
    }

    #region Button Functions
    public void JustMainButtons()
    {
        ChangePanels(PanelTypeMainMenu.none);
    }
    public void NewGameAreYouSure()
    {
        if(question == AreYouSureQuestion.newGame && areYouSurePanelMainMenu.activeInHierarchy)
        {
            ChangePanels(PanelTypeMainMenu.none);
        }
        else
        {
            ChangePanels(PanelTypeMainMenu.areYouSure);
            areYouSureTextMainMenu.text = "Are you sure you want to start a new game by deleting the previous save game?";
        }
        question = AreYouSureQuestion.newGame;

    }
    public void ExitGameAreYouSure()
    {
        if (question == AreYouSureQuestion.exitGame && areYouSurePanelMainMenu.activeInHierarchy)
        {
            ChangePanels(PanelTypeMainMenu.none);
        }
        else
        {
            ChangePanels(PanelTypeMainMenu.areYouSure);
            areYouSureTextMainMenu.text = "Are you sure you want to exit the game?";
        }
        question = AreYouSureQuestion.exitGame;
    }
    public void NewGameStart()
    {
        hasLoadedGame = false;
        SceneManager.LoadScene("Level 1");

    }
    public void ContinueGame()
    {
        if (hasSavedGame)
        {
            hasLoadedGame = true;
            SceneManager.LoadScene("Level 1");
        }
    }
    public void AreYouSureYES()
    {
        if(question == AreYouSureQuestion.newGame)
        {
            NewGameStart();
        }
        else
        {
            Application.Quit();
        }
    }
    public void AreYouSureNO()
    {
        ChangePanels(PanelTypeMainMenu.none);
    }
    public void DeveloperButton()
    {
        ChangePanels(PanelTypeMainMenu.developer);
    }
    public void OpenDeveloperLink(int code)
    {
        switch (code)
        {
            case 0:
                Application.OpenURL("https://www.youtube.com/@CanAhmetDarama-GDev");
                break;
            case 1:
                Application.OpenURL("https://twitter.com/CanADarama_GDev");
                break;
            case 2:
                Application.OpenURL("https://www.linkedin.com/in/can-ahmet-darama-226742225/");
                break;
        }
    }
    public override void SettingsButton()
    {
        ChangePanels(PanelTypeMainMenu.settings);
    }

    void ChangePanels(PanelTypeMainMenu newPanelState)
    {
        switch (newPanelState)
        {
            case PanelTypeMainMenu.none:
                areYouSurePanelMainMenu.SetActive(false);
                mainButtonsPanel.SetActive(true);
                developerPanel.SetActive(false);
                SettingsManager.settingsPanel.SetActive(false);
                break;
            case PanelTypeMainMenu.areYouSure:
                areYouSurePanelMainMenu.SetActive(true);
                mainButtonsPanel.SetActive(false);
                developerPanel.SetActive(false);
                SettingsManager.settingsPanel.SetActive(false);
                break;
            case PanelTypeMainMenu.settings:
                areYouSurePanelMainMenu.SetActive(false);
                mainButtonsPanel.SetActive(false);
                developerPanel.SetActive(false);
                SettingsManager.settingsPanel.SetActive(true);
                break;
            case PanelTypeMainMenu.developer:
                areYouSurePanelMainMenu.SetActive(false);
                mainButtonsPanel.SetActive(false);
                developerPanel.SetActive(true);
                SettingsManager.settingsPanel.SetActive(false);
                break;
        }
    }
    #endregion

    void SpotLightRotate()
    {
        spotLight.transform.rotation = Quaternion.Slerp(spotLight.transform.rotation, newLightRotation, 2 * Time.deltaTime);
    }
    IEnumerator RandomAngle()
    {
        yield return new WaitForSeconds(2);
        
        newLightRotation = Quaternion.Euler(new Vector3(initialLightRotation.x + Random.Range(-5,5), initialLightRotation.y + Random.Range(-5, 5), initialLightRotation.z));
        StartCoroutine(RandomAngle());
    }
}
enum AreYouSureQuestion { newGame, exitGame}
enum PanelTypeMainMenu { none, areYouSure, settings, developer}
