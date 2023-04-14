using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [Header("Main Things")]
    [SerializeField] GameObject settingsPanelPrefab;
    public static GameObject settingsPanel;

    [Header("Sub Things")]
    public static Button settingsExitButton;
    public static TMP_Dropdown resolutionDropdown;
    public static Toggle fullScreenToggle;

    Resolution[] resolutions;

    void Start()
    {
        SettingsManagerAssignments();
        ResolutionStart();
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        
    }

    void SettingsManagerAssignments()
    {
        GameManager.settingsManager = this;

        UI_Manager.mainCanvas = FindObjectOfType<Canvas>();
        settingsPanel = Instantiate(settingsPanelPrefab, UI_Manager.mainCanvas.transform);

        settingsExitButton = settingsPanel.transform.Find("Exit Settings Panel").GetComponent<Button>();
        resolutionDropdown = settingsPanel.transform.Find("Resolution Dropdown").GetComponent<TMP_Dropdown>();
        fullScreenToggle = settingsPanel.transform.Find("FullScreen Toggle").GetComponent<Toggle>();

        settingsExitButton.onClick.AddListener(FindAnyObjectByType<MainMenuManager>().JustMainButtons);

        if (resolutionDropdown == null)
        {
            Debug.Log("NULL");
        }
    }

    void ResolutionStart()
    {
        resolutions = Screen.resolutions;


        resolutionDropdown.ClearOptions();

        List<string> resolutionOptions = new List<string>();
        for (int i = 0, z = resolutions.Length; i < z; i++)
        {
            resolutionOptions.Add(resolutions[i].width + " x " + resolutions[i].height);
        }
        resolutionDropdown.AddOptions(resolutionOptions);

    }
}
