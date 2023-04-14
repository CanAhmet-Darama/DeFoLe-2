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
    [Header("Buttons")]
    public Button newGameButton;
    public Button continueGameButton;
    public Button settingsButton;
    public Button exitGameButton;
    public Button developerButton;

    public Button yesButton;
    public Button noButton;

    [Header("Objects")]
    public GameObject areYouSurePanel;
    public GameObject mainButtonsPanel;
    public GameObject developerPanel;

    [Header("Texts")]
    public TextMeshProUGUI areYouSureText;
    public TextMeshProUGUI continueGameText;

    [Header("Other")]
    AreYouSureQuestion question;
    public bool hasSavedGame;
    public static bool hasLoadedGame;

    void Start()
    {

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

    #region Button Functions
    public void JustMainButtons()
    {
        ChangePanels(PanelTypeMainMenu.none);
    }
    public void NewGameAreYouSure()
    {
        if(question == AreYouSureQuestion.newGame && areYouSurePanel.activeInHierarchy)
        {
            ChangePanels(PanelTypeMainMenu.none);
        }
        else
        {
            ChangePanels(PanelTypeMainMenu.areYouSure);
            areYouSureText.text = "Are you sure you want to start a new game by deleting the previous save game?";
        }
        question = AreYouSureQuestion.newGame;

    }
    public void ExitGameAreYouSure()
    {
        if (question == AreYouSureQuestion.exitGame && areYouSurePanel.activeInHierarchy)
        {
            ChangePanels(PanelTypeMainMenu.none);
        }
        else
        {
            ChangePanels(PanelTypeMainMenu.areYouSure);
            areYouSureText.text = "Are you sure you want to exit the game?";
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
        hasLoadedGame = true;
        SceneManager.LoadScene("Level 1");
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
    public override void SettingsButton()
    {
        ChangePanels(PanelTypeMainMenu.settings);
    }

    void ChangePanels(PanelTypeMainMenu newPanelState)
    {
        switch (newPanelState)
        {
            case PanelTypeMainMenu.none:
                areYouSurePanel.SetActive(false);
                mainButtonsPanel.SetActive(true);
                developerPanel.SetActive(false);
                SettingsManager.settingsPanel.SetActive(false);
                break;
            case PanelTypeMainMenu.areYouSure:
                areYouSurePanel.SetActive(true);
                mainButtonsPanel.SetActive(false);
                developerPanel.SetActive(false);
                SettingsManager.settingsPanel.SetActive(false);
                break;
            case PanelTypeMainMenu.settings:
                areYouSurePanel.SetActive(false);
                mainButtonsPanel.SetActive(false);
                developerPanel.SetActive(false);
                SettingsManager.settingsPanel.SetActive(true);
                break;
            case PanelTypeMainMenu.developer:
                areYouSurePanel.SetActive(false);
                mainButtonsPanel.SetActive(false);
                developerPanel.SetActive(true);
                SettingsManager.settingsPanel.SetActive(false);
                break;
        }
    }
    #endregion
}
enum AreYouSureQuestion { newGame, exitGame}
enum PanelTypeMainMenu { none, areYouSure, settings, developer}
