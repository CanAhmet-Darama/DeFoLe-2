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
            areYouSurePanel.SetActive(false);
        }
        else
        {
            areYouSurePanel.SetActive(true);
            areYouSureText.text = "Are you sure you want to start a new game by deleting the previous save game?";
        }
        question = AreYouSureQuestion.newGame;

    }
    public void ExitGameAreYouSure()
    {
        if (question == AreYouSureQuestion.exitGame && areYouSurePanel.activeInHierarchy)
        {
            areYouSurePanel.SetActive(false);
        }
        else
        {
            areYouSurePanel.SetActive(true);
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

    #endregion
}
public enum AreYouSureQuestion { newGame, exitGame}
