using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerStatsScreen : MonoBehaviour
{
    [Header("File Select")]
    public GameObject playerSaves;
    public GameObject playerStatsMenu;
    private bool savesOpen = false;
    public Button exitButton;

    private GameObject generatedStatsContainer;

    [Header("Player Stats")]
    public TextMeshProUGUI playerName;
    public Image playerFlag;
    public TextMeshProUGUI playTime;
    public TextMeshProUGUI wins;
    public TextMeshProUGUI losses;
    public TextMeshProUGUI kills;
    public TextMeshProUGUI death;
    public TextMeshProUGUI highScore;

    [Header("Misc.")]
    public GameObject newGameMenu;

    private void Start()
    {
        SaveSystem.LoadSave(PlayerPrefs.GetString("most_recent_save"));
        DisplayStats();
    }

    // Update is called once per frame
    void Update()
    {
        if(playerStatsMenu.activeSelf)
        {
            if (!savesOpen)
            {
                savesOpen = true;
                generatedStatsContainer = Instantiate(playerSaves, playerStatsMenu.transform);
                if (SaveSystem.GetAllSaves()[0] == "")
                {
                    exitButton.interactable = false;
                }
                else
                {
                    exitButton.interactable = true;
                    DisplayStats();
                }
            }
        }
        else
        {
            savesOpen = false;
            Destroy(generatedStatsContainer);
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            SaveSystem.PrintAllSaveKeys();
        }
    }

    public void SelectSaveFile(string selectedSaveKey)
    {
        //Load save
        SaveSystem.LoadSave(selectedSaveKey);
        //Display stats
        DisplayStats();
    }

    public void AddNewSave()
    {
        //Disable player stats menu and open the new game menu
        newGameMenu.SetActive(true);
        newGameMenu.GetComponent<Canvas>().enabled = true;
        playerStatsMenu.SetActive(false);
        //Debug.Log("newGameMenu active is " + newGameMenu.activeSelf + ", playerStatsMenu active is " + playerStatsMenu.activeSelf);
    }

    private string GetReadableTime(string timeString)
    {
        var time = TimeSpan.FromSeconds(float.Parse(timeString));
        string formattedTime = string.Format("{0:00}:{1:00}:{1:00}", time.TotalHours, time.TotalMinutes, time.TotalSeconds);
        //string minutes = ((int)time / 60).ToString();
        //string seconds = (time % 60).ToString("f0");
        return formattedTime;
        //return timeString;
    }

    public void DisplayStats()
    {
        playerName.text = SaveSystem.loadedSaveKey;
        playerFlag.sprite = SaveSystem.availableFlags[SaveSystem.loadedFlagIndex];
        playTime.text = "Playtime: " + GetReadableTime(SaveSystem.GetPlayerStats("playtime").ToString());
        wins.text = "Games Won: " + SaveSystem.GetPlayerStats("wins").ToString();
        losses.text = "Games Lost: " + SaveSystem.GetPlayerStats("losses").ToString();
        kills.text = "Kills: " + SaveSystem.GetPlayerStats("kills").ToString();
        death.text = "Deaths: " + SaveSystem.GetPlayerStats("deaths").ToString();
        highScore.text = "High Score: " + SaveSystem.GetPlayerStats("endlesshighscore").ToString();
    }

    public void DisplayPlaceholderStats()
    {
        playerName.text = "No Player Selected!";
        playerFlag.sprite = SaveSystem.availableFlags[0];
        playTime.text = "Playtime: 00:00:00";
        wins.text = "Games Won: 0";
        losses.text = "Games Lost: 0";
        kills.text = "Kills: 0";
        death.text = "Deaths: 0";
        highScore.text = "High Score: 0";
    }

    public void RegenerateSavesList()
    {
        Destroy(generatedStatsContainer);
        generatedStatsContainer = Instantiate(playerSaves, playerStatsMenu.transform);
        if (SaveSystem.GetAllSaves()[0] == "")
        {
            exitButton.interactable = false;
            DisplayPlaceholderStats();
        }
        else
        {
            exitButton.interactable = true;
            PlayerPrefs.SetString("most_recent_save", SaveSystem.GetAllSaves()[0]);
            SaveSystem.LoadSave(PlayerPrefs.GetString("most_recent_save"));
            DisplayStats();
        }
    }
}
