using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SaveFileButton : MonoBehaviour
{
    public string assignedSavekey;

    private void Start()
    {
        if(assignedSavekey != "")
            GetComponentInChildren<TextMeshProUGUI>().text = assignedSavekey;
        else
            GetComponentInChildren<TextMeshProUGUI>().text = "New Game";
    }

    public void SelectSaveFile()
    {
        if (GameObject.Find("MenuController"))
        {
            GameObject.Find("MenuController").GetComponent<PlayerStatsScreen>().SelectSaveFile(assignedSavekey);
            
        }
    }

    public void AddNewFile()
    {
        if (GameObject.Find("MenuController"))
        {
            GameObject.Find("MenuController").GetComponent<PlayerStatsScreen>().AddNewSave();
        }

    }

    public void DeleteSave()
    {
        Debug.Log("Delete this save: " + assignedSavekey);
        SaveSystem.DeleteSave(assignedSavekey);
        SaveSystem.PrintAllSaveKeys();
        if (GameObject.Find("MenuController"))
        {
            GameObject.Find("MenuController").GetComponent<PlayerStatsScreen>().RegenerateSavesList();

        }
    }
}
