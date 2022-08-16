using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrefsSetter : MonoBehaviour
{
    [SerializeField] string key = "";

    public void SetPref (int i) {
        PlayerPrefs.SetInt(key, i);
    }

    public void SetPref (float f) {
        PlayerPrefs.SetFloat(key, f);
    }
    
    public void SetPref (bool b) {
        PlayerPrefs.SetInt(key, b ? 1 : 0);
    }

    public void SetPrefRounded (float f) {
        PlayerPrefs.SetInt(key, Mathf.RoundToInt(f));
    }


}
