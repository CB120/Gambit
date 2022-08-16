using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenuController : MonoBehaviour
{
    [SerializeField] GameObject[] tabScreens;
    [SerializeField] Button[] tabButtons;
    [SerializeField] Color unselectedColour;
    [SerializeField] Color selectedColour;
    [SerializeField] int defaultTab = 0;
    [SerializeField] int targetFramerateMax = 300;

    [SerializeField] TMPro.TMP_Dropdown resolutionsDropdown;
    
    List<Resolution> resolutions;

    private void OnEnable() {
        SwitchToTab(defaultTab);
        GetResolutions();
    }

    public void SwitchToTab(int tabNumber) {
        for (int i = 0; i < tabScreens.Length; i++) {
            tabScreens[i].SetActive(i == tabNumber);
            tabButtons[i].image.color = i == tabNumber ? selectedColour : unselectedColour;
        }
    }

    // Graphics settings

    public void GetResolutions (){ 

        resolutionsDropdown.options.Clear();
        
        List<TMPro.TMP_Dropdown.OptionData> resolutionsList = new List<TMPro.TMP_Dropdown.OptionData>();
        resolutions = new List<Resolution>();

        int i = 0;
        int currentResolution = 0;
        foreach (Resolution resolution in Screen.resolutions) {
            TMPro.TMP_Dropdown.OptionData option = new TMPro.TMP_Dropdown.OptionData(resolution.width + "x" + resolution.height + ", " + resolution.refreshRate + "Hz");
            resolutionsList.Add(option);
            resolutions.Add(resolution);
            if (Screen.width == resolution.width && Screen.height == resolution.height && Screen.currentResolution.refreshRate == resolution.refreshRate) {
                currentResolution = i;
            }
            i++;
        }

        resolutionsDropdown.AddOptions(resolutionsList);
        resolutionsDropdown.SetValueWithoutNotify(currentResolution);

    }

    public void SetResolution(int i) {
        Resolution resolution = resolutions[i];
        Screen.SetResolution(resolution.width, resolution.height, FullScreenMode.ExclusiveFullScreen, resolution.refreshRate);
        PlayerPrefs.SetInt("GRAPHICS_ScreenWidth", resolution.width);
        PlayerPrefs.SetInt("GRAPHICS_ScreenHeight", resolution.height);
        PlayerPrefs.SetInt("GRAPHICS_ScreenRefreshRate", resolution.refreshRate);
        GraphicsSettingsLoader.UpdateScreen();
    }

    public void SetResolutionScale(float val) {
        int i = Mathf.RoundToInt(val);
        PlayerPrefs.SetInt("GRAPHICS_ResScale", i);
        GraphicsSettingsLoader.UpdateResolutionScale();
    }

    public void SetFullscreenMode(int i) {
        PlayerPrefs.SetInt("GRAPHICS_FullscreenMode", i);
        GraphicsSettingsLoader.UpdateScreen();

    }

    public void SetVsync(bool b) {
        PlayerPrefs.SetInt("GRAPHICS_Vsync", b ? 1 : 0);
        GraphicsSettingsLoader.UpdateVsync();
    }

    public void SetTargetFramerate(float val) {
        int i = Mathf.RoundToInt(val);
        if (i >= 300) i = -1;
        PlayerPrefs.SetInt("GRAPHICS_TargetFPS", i);
        GraphicsSettingsLoader.UpdateTargetFramerate();
    }


    public void SetSMAA(bool b) {
        PlayerPrefs.SetInt("GRAPHICS_SMAA", b ? 1 : 0);
        GraphicsSettingsLoader.UpdateSMAA();
    }

    public void SetMSAA(int val) {
        PlayerPrefs.SetInt("GRAPHICS_MSAA", val);
        GraphicsSettingsLoader.UpdateMSAA();
        
    }

    public void SetShadows(bool b) {
        PlayerPrefs.SetInt("GRAPHICS_Shadows", b ? 1 : 0);
        GraphicsSettingsLoader.UpdateShadowQuality();
    }

    public void SetSSAO (bool b) {
        PlayerPrefs.SetInt("GRAPHICS_SSAO", b ? 1 : 0);
        GraphicsSettingsLoader.UpdateSSAO();
    }

    public void SetEnvironmentFX (bool b) {
        PlayerPrefs.SetInt("GRAPHICS_EnvironmentFX", b ? 1 : 0);
    }

    public void SetUltraLow (bool b) {
        PlayerPrefs.SetInt("GRAPHICS_UltraLow", b ? 1 : 0);
        GraphicsSettingsLoader.UpdateUltraLow();
    }

    public void SavePrefs () {
        PlayerPrefs.Save();
    }
}
