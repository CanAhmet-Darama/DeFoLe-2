using OpenCover.Framework.Model;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [Header("Main Things")]
    [SerializeField] GameObject settingsPanelPrefab;
    public static GameObject settingsPanel;

    [Header("Sub Things")]
    public static Button settingsExitButton;
    public static TMP_Dropdown resolutionDropdown;
    public static TMP_Dropdown qualityDropdown;
    public static Toggle fullScreenToggle;
    public static Slider brightnessSlider;
    public static Slider volumeSlider;
    public static Slider mouseSensitivitySlider;

    [Header("Post Process")]
    public PostProcessProfile brightnessProfile;
    AutoExposure brightnessExposure;


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

        brightnessProfile.TryGetSettings(out brightnessExposure);

        UI_Manager.mainCanvas = FindObjectOfType<Canvas>();
        settingsPanel = Instantiate(settingsPanelPrefab, UI_Manager.mainCanvas.transform);

        settingsExitButton = settingsPanel.transform.Find("Exit Settings Panel").GetComponent<Button>();
        resolutionDropdown = settingsPanel.transform.Find("Resolution Dropdown").GetComponent<TMP_Dropdown>();
        qualityDropdown = settingsPanel.transform.Find("Quality Dropdown").GetComponent<TMP_Dropdown>();
        fullScreenToggle = settingsPanel.transform.Find("FullScreen Toggle").GetComponent<Toggle>();
        brightnessSlider = settingsPanel.transform.Find("Brightness Setter").GetComponentInChildren<Slider>();
        volumeSlider = settingsPanel.transform.Find("Volume Setter").GetComponentInChildren<Slider>();
        mouseSensitivitySlider = settingsPanel.transform.Find("Mouse Sensitivity Setter").GetComponentInChildren<Slider>();

        settingsExitButton.onClick.AddListener(ExitSettingsMenu);
        qualityDropdown.onValueChanged.AddListener(SetQuality);
        fullScreenToggle.onValueChanged.AddListener(SetFullscreen);
        brightnessSlider.onValueChanged.AddListener(SetBrightness);
        volumeSlider.onValueChanged.AddListener(SetVolume);
        mouseSensitivitySlider.onValueChanged.AddListener(SetMouseSensitivity);

        brightnessExposure.keyValue.value = 1;
        brightnessSlider.value = brightnessExposure.keyValue.value;
        SetQuality(2);
        qualityDropdown.value = 2;

    }

    public void ExitSettingsMenu()
    {
        if(SceneManager.GetActiveScene().name == "Main Menu")
        {
            FindAnyObjectByType<MainMenuManager>().JustMainButtons();
        }
        else
        {
            GameManager.uiManager.ChangePanels(PanelType.none);
        }
    }


    void ResolutionStart()
    {
        resolutions = Screen.resolutions;


        resolutionDropdown.ClearOptions();

        byte currentResoIndex = 0;
        List<string> resolutionOptions = new List<string>();
        for (int i = 0, z = resolutions.Length; i < z; i++)
        {
            resolutionOptions.Add(resolutions[i].width + " x " + resolutions[i].height);
            if(resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                currentResoIndex = (byte)i;
            }
        }
        resolutionDropdown.AddOptions(resolutionOptions);
        resolutionDropdown.value = currentResoIndex;

        UnityAction<int> onValueChanged = new UnityAction<int>(SetResolution);
        resolutionDropdown.onValueChanged.RemoveAllListeners();
        resolutionDropdown.onValueChanged.AddListener(onValueChanged);
    }
    public void SetResolution(int resolutionIndex)
    {
        Screen.SetResolution(resolutions[resolutionIndex].width, resolutions[resolutionIndex].height, true);
        //Debug.Log("Resolution set to : " + resolutions[resolutionIndex].width + " x " + resolutions[resolutionIndex].height);
    }
    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }
    public void SetFullscreen(bool fullscreenEnabled)
    {
        Screen.fullScreen = fullscreenEnabled;
    }
    public void SetBrightness(float brightnessValue)
    {
        brightnessExposure.keyValue.value = brightnessValue;
    }
    public void SetVolume(float volumeValue)
    {
        AudioListener.volume = volumeValue;
    }
    public void SetMouseSensitivity(float newSensitivity)
    {
        GameManager.mouseSensitivity = newSensitivity;
    }

}
