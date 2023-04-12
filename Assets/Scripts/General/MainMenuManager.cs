using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
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
    public GameObject settingsPanel;

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
    public void NewGameAreYouSure()
    {
        if(question == AreYouSureQuestion.newGame && areYouSurePanel.activeInHierarchy)
        {
            ChangePanels(PanelType.none);
        }
        else
        {
            ChangePanels(PanelType.areYouSure);
            areYouSureText.text = "Are you sure you want to start a new game by deleting the previous save game?";
        }
        question = AreYouSureQuestion.newGame;

    }
    public void ExitGameAreYouSure()
    {
        if (question == AreYouSureQuestion.exitGame && areYouSurePanel.activeInHierarchy)
        {
            ChangePanels(PanelType.none);
        }
        else
        {
            ChangePanels(PanelType.areYouSure);
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
        ChangePanels(PanelType.none);
    }
    public void DeveloperButton(bool onOff)
    {
        if (onOff)
        {
            ChangePanels(PanelType.developer);
        }
        else
        {
            ChangePanels(PanelType.none);
        }
    }

    void ChangePanels(PanelType newPanelState)
    {
        switch (newPanelState)
        {
            case PanelType.none:
                areYouSurePanel.SetActive(false);
                mainButtonsPanel.SetActive(true);
                developerPanel.SetActive(false);
                settingsPanel.SetActive(false);
                break;
            case PanelType.areYouSure:
                areYouSurePanel.SetActive(true);
                mainButtonsPanel.SetActive(false);
                developerPanel.SetActive(false);
                settingsPanel.SetActive(false);
                break;
            case PanelType.settings:
                areYouSurePanel.SetActive(false);
                mainButtonsPanel.SetActive(false);
                developerPanel.SetActive(false);
                settingsPanel.SetActive(true);
                break;
            case PanelType.developer:
                areYouSurePanel.SetActive(false);
                mainButtonsPanel.SetActive(false);
                developerPanel.SetActive(true);
                settingsPanel.SetActive(false);
                break;
        }
    }
    #endregion
}
enum AreYouSureQuestion { newGame, exitGame}
enum PanelType { none, areYouSure, settings, developer}
