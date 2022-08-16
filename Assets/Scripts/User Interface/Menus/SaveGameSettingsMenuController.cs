using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SaveGameSettingsMenuController : MonoBehaviour
{
    // [Header("UI Element References")]
    // [SerializeField] private Scrollbar musicToggle;
    // [SerializeField] private Scrollbar sfxToggle;

    // [Header("Canvas Reference")]
    // [SerializeField] private GameObject settingsCanvas;

    private void OnEnable()
    {
        // musicToggle.value = PlayerPrefs.GetFloat("MusicVolume");
        // sfxToggle.value = PlayerPrefs.GetFloat("SFXVolume");
    }

    public void SaveGameSettings()
    {
        // PlayerPrefs.SetFloat("MusicVolume", musicToggle.value);
        // PlayerPrefs.SetFloat("SFXVolume", sfxToggle.value);
        PlayerPrefs.Save();
    }
}
