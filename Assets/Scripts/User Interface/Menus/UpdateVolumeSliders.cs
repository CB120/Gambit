using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class UpdateVolumeSliders : MonoBehaviour
{
    [SerializeField] string playerPrefName = "MusicVolume";
    [SerializeField] float defaultValue = 1;

    [SerializeField] AudioMixer audioMixer;
    [SerializeField] string audioMixerParameter = "MusicVolume";

    private void OnEnable()
    {
        if (GetComponent<Slider>())
        {
            GetComponent<Slider>().value = PlayerPrefs.GetFloat(playerPrefName, defaultValue);
        }
    }

    public void setPlayerPref(float value) {
        // volume is on a log scale so maths innit
        PlayerPrefs.SetFloat(playerPrefName, value);
        if (audioMixer) {
            audioMixer.SetFloat(audioMixerParameter, Mathf.Log10(value) * 20);
        }
    }
}
