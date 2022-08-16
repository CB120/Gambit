using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class GraphicsSettingsLoader : MonoBehaviour
{
    static GraphicsSettingsLoader Singleton;
    [SerializeField] UniversalRenderPipelineAsset urpRenderPipelineAsset = null;
    [SerializeField] UniversalRendererData urpRendererData = null;

    private void Awake() {
        Singleton = this;
    }

    private void Start() {
        InitialiseGraphicsSettings();
    }

    public static void InitialiseGraphicsSettings() {
        UpdateUltraLow();
        UpdateScreen();
        UpdateVsync();
        UpdateTargetFramerate();
        UpdateSMAA();
        UpdateMSAA();
        UpdateShadowQuality();
        UpdateSSAO();
    }

    public static void UpdateScreen () {
        int width = PlayerPrefs.GetInt("GRAPHICS_ScreenWidth", Screen.width);
        int height = PlayerPrefs.GetInt("GRAPHICS_ScreenHeight", Screen.height);
        int fullScreenValue = PlayerPrefs.GetInt("GRAPHICS_FullscreenMode", 1);
        FullScreenMode fullScreenMode = FullScreenMode.Windowed;
        switch (fullScreenValue) {
            case 0: fullScreenMode = FullScreenMode.ExclusiveFullScreen; break;
            case 1: fullScreenMode = FullScreenMode.FullScreenWindow; break;
            case 2: fullScreenMode = FullScreenMode.Windowed; break;
            default: fullScreenMode = FullScreenMode.MaximizedWindow; break;
        }
        int refreshRate = PlayerPrefs.GetInt("GRAPHICS_ScreenRefreshRate", Screen.currentResolution.refreshRate);

        if (Screen.width == width && Screen.height == height && Screen.fullScreenMode == fullScreenMode && Screen.currentResolution.refreshRate == refreshRate) {
            return;
        }

        Screen.SetResolution(width, height, fullScreenMode, refreshRate);
    }

    public static void UpdateResolutionScale() {
        float renderScale = PlayerPrefs.GetInt("GRAPHICS_ResScale", 100);
        Singleton.urpRenderPipelineAsset.renderScale = (float)renderScale / 100.0f;
    }

    public static void UpdateVsync() {
        QualitySettings.vSyncCount = PlayerPrefs.GetInt("GRAPHICS_Vsync", 0);
    }

    public static void UpdateTargetFramerate () {
        int val = PlayerPrefs.GetInt("GRAPHICS_TargetFPS", -1);
        Application.targetFrameRate = val;
    }

    public static void UpdateSMAA () {
        int val = PlayerPrefs.GetInt("GRAPHICS_SMAA", 1);
        UniversalAdditionalCameraData cameraData = Camera.main.GetComponent<UniversalAdditionalCameraData>();

        cameraData.antialiasing = val == 1 ? AntialiasingMode.SubpixelMorphologicalAntiAliasing : AntialiasingMode.None;

    }

    public static void UpdateMSAA () {
        int val = PlayerPrefs.GetInt("GRAPHICS_MSAA", 0);
        int samples = 0;
        switch (val) {
            case 1 : samples = 2; break;
            case 2 : samples = 4; break;
            case 3 : samples = 8; break;
            default: samples = 0; break;
        }
        QualitySettings.antiAliasing = samples;
    }

    public static void UpdateShadowQuality () {
        // this is kinda fucky cause i didn't realise URP doesn't support runtime shadow resolution changes
        // so most of this actually doesn't do anything smh
        int val = PlayerPrefs.GetInt("GRAPHICS_Shadows", 1);
        UnityEngine.ShadowResolution resolution = UnityEngine.ShadowResolution.High;
        UnityEngine.ShadowQuality quality = UnityEngine.ShadowQuality.All;
        UniversalAdditionalCameraData cameraData = Camera.main.GetComponent<UniversalAdditionalCameraData>();
        cameraData.renderShadows = true;
        switch (val) {
            case 0:
                resolution = UnityEngine.ShadowResolution.Low;
                quality = UnityEngine.ShadowQuality.Disable;
                cameraData.renderShadows = false;
                break;/*
            case 1: resolution = UnityEngine.ShadowResolution.Low; break;
            case 2: resolution = UnityEngine.ShadowResolution.Medium; break;
            case 3: resolution = UnityEngine.ShadowResolution.High; break;
            case 4: resolution = UnityEngine.ShadowResolution.VeryHigh; break;*/
            default: resolution = UnityEngine.ShadowResolution.High; break;
        }

        QualitySettings.shadowResolution = resolution;
        QualitySettings.shadows = quality;

    }

    public static void UpdateSSAO() {
        int val = PlayerPrefs.GetInt("GRAPHICS_SSAO", 2);
        Singleton.urpRendererData.rendererFeatures[0].SetActive(val != 0);
    }

    public static void UpdateUltraLow() {
        QualitySettings.SetQualityLevel(1);
        /*int val = PlayerPrefs.GetInt("GRAPHICS_UltraLow", 0);
        if (val == 1) {
            QualitySettings.SetQualityLevel(0);
        } else {
            QualitySettings.SetQualityLevel(1);
        }*/
    }
}
