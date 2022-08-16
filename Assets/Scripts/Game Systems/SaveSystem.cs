using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveSystem : MonoBehaviour
{
    //Loaded save variables
    public static string loadedSaveKey;
    public static string loadedProgress;
    public static string loadedLevelsCompleted;
    public static int loadedFlagIndex;
    public static string loadedPlayerStats;                     //Player stats are - Playtime_Wins_Losses_Kills_Deaths_highscore_difficulty

    public static Sprite[] availableFlags;
    public static bool dynamicDifficulty = true;

    //Call when creating a new save game from the main menu
    public static void CreateNewSave(string saveKey, int flagIndex)
    {
        string newSaveKey = saveKey.ToLower();
        //Initialise game progress
        PlayerPrefs.SetString(newSaveKey + "_progress", "100000");
        PlayerPrefs.SetString(newSaveKey + "_levels_completed", "000000");
        //Player stats are - Playtime_Wins_Losses_Kills_Deaths_SkirmishHS_EndlessHS_Difficulty
        PlayerPrefs.SetString(newSaveKey + "_player_stats", "0_0_0_0_0_0_0_0");
        //Save the player's flag choice
        PlayerPrefs.SetInt(saveKey, flagIndex);
        //Add player to save file list
        if (PlayerPrefs.GetString("existing_saves") == "")
        {
            PlayerPrefs.SetString("existing_saves", newSaveKey);
        }
        else
        {
            PlayerPrefs.SetString("existing_saves", PlayerPrefs.GetString("existing_saves") + "____" + newSaveKey);
        }
        PlayerPrefs.SetString("most_recent_save", newSaveKey);
        PlayerPrefs.Save();
        OverrideLoadedVariables(newSaveKey);
        //PrintLoadedVariables();
    }

    public static void SetDifficulty(int levelOfDifficulty)
    {
        //Grab the player's stats and separate them into an array of strings
        if (loadedPlayerStats != null && loadedPlayerStats != "")
        {
            string[] playerStats = loadedPlayerStats.Split('_');
            //Record the new difficulty in the player stats
            playerStats[playerStats.Length - 1] = levelOfDifficulty.ToString();
            //Rejoin the player stats into one string and save it
            PlayerPrefs.SetString(loadedSaveKey + "_player_stats", string.Join("_", playerStats));
            OverrideLoadedVariables(loadedSaveKey);
        }
    }

    public static int GetDifficulty()
    {
        //Grab the player's stats and separate them into an array of strings
        if (loadedPlayerStats != null && loadedPlayerStats != "")
        {
            string[] playerStats = loadedPlayerStats.Split('_');
            //Grab the difficulty and return it
            int difficulty = int.Parse(playerStats[playerStats.Length-1]);
            return difficulty;
        }
        else
        {
            Debug.LogWarning("Error: loadedPlayerStats is currently null");
            return 0;
        }

    }

    public static string GetActivePlayer()
    {
        if (loadedSaveKey != null || loadedSaveKey != "")
            return loadedSaveKey;
        else return null;
    }

    public static void FlipDynamicDifficulty()
    {
        dynamicDifficulty = !dynamicDifficulty;
    }

    //Call when deleting save files
    public static void DeleteSave(string saveKey)
    {
        //Stop the function if there are no save file
        if (GetAllSaves()[0] == "")
        {
            Debug.LogWarning("Error: No existing save files");
        }
        else
        {
            //Temporarily store all of the saves in a list
            List<string> remainingSaves = new List<string>();
            foreach(string save in GetAllSaves())
            {
                remainingSaves.Add(save);
            }
            //Remove the given key
            remainingSaves.Remove(saveKey);
            //Replace existing_saves with the new list
            string[] newSaves = remainingSaves.ToArray();
            string finalList = string.Join("____", newSaves);
            //Replace the existing saves string
            PlayerPrefs.SetString("existing_saves", finalList);
            Debug.Log("New existing saves: " + finalList);
            //Delete all player data associated with the key
            PlayerPrefs.DeleteKey(saveKey + "_progress");
            PlayerPrefs.DeleteKey(saveKey + "_levels_completed");
            PlayerPrefs.DeleteKey(saveKey + "_player_stats");
            PlayerPrefs.DeleteKey(saveKey);

        }
    }

    public static void SetAvailableFlags(Sprite[] flags)
    {
        availableFlags = flags;
    }

    //Call when loading a save game from the menu
    public static void LoadSave(string saveKey)
    {
        saveKey = saveKey.ToLower();
        PlayerPrefs.SetString("most_recent_save", saveKey);
        OverrideLoadedVariables(saveKey);
    }

    //Call when a player has completed a level
    public static void AddCompletedLevel(int completedLevelIndex)
    {
        //Get each char in the levels completed key and change the value at the given index
        char[] completedLevels = PlayerPrefs.GetString(loadedSaveKey + "_levels_completed").ToCharArray();
        if (completedLevels.Length > completedLevelIndex){
            completedLevels[completedLevelIndex] = '1';
            //Update the levels completed key
            string updatedLevels = new string(completedLevels);
            PlayerPrefs.SetString(loadedSaveKey + "_levels_completed", updatedLevels);
            PlayerPrefs.Save();
            OverrideLoadedVariables(loadedSaveKey);
        }
    }

    //Call when a player unlocks the next level
    public static void AddProgress(int newLevelIndex)
    {
        //Get each char in the progress key and change the value at the given index
        char[] unlockedLevels = PlayerPrefs.GetString(loadedSaveKey + "_progress").ToCharArray();
        if (unlockedLevels.Length > newLevelIndex){
            unlockedLevels[newLevelIndex] = '1';
            //Update the progress key
            string updatedLevels = new string(unlockedLevels);
            PlayerPrefs.SetString(loadedSaveKey + "_progress", updatedLevels);
            PlayerPrefs.Save();
            OverrideLoadedVariables(loadedSaveKey);
        }
    }

    //Adds to the recorded player's stats
    public static void AddToPlayerStats(string statIndex, float statToAdd)
    {
        int indexToAddTo = 0;
        switch (statIndex.ToLower())
        {
            case "playtime": indexToAddTo = 0; break;
            case "wins": indexToAddTo = 1; break;
            case "losses": indexToAddTo = 2; break;
            case "kills": indexToAddTo = 3; break;
            case "deaths": indexToAddTo = 4; break;
            case "skirmishhighscore": indexToAddTo = 5; break;
            case "endlesshighscore": indexToAddTo= 6; break;
            default: Debug.Log("Invalid string given to AddToPlayerStats"); break;
        }

        //Grab the player's stats and separate them into an array of strings
        if (loadedPlayerStats != null && loadedPlayerStats != ""){
            string[] playerStats = loadedPlayerStats.Split('_');
            //Add statToAdd value to the specified stat index
            float addedStat = float.Parse(playerStats[indexToAddTo]) + statToAdd;
            //Record the new number in the player stats
            playerStats[indexToAddTo] = addedStat.ToString();
            //Rejoin the player stats into one string and save it
            PlayerPrefs.SetString(loadedSaveKey + "_player_stats", string.Join("_", playerStats));
            OverrideLoadedVariables(loadedSaveKey);
        }
    }

    //Adds to the recorded player's stats
    public static void OverwritePlayerStats(string statIndex, float statToChange)
    {
        int indexToAddTo = 0;
        switch (statIndex.ToLower())
        {
            case "playtime": indexToAddTo = 0; break;
            case "wins": indexToAddTo = 1; break;
            case "losses": indexToAddTo = 2; break;
            case "kills": indexToAddTo = 3; break;
            case "deaths": indexToAddTo = 4; break;
            case "skirmishhighscore": indexToAddTo = 5; break;
            case "endlesshighscore": indexToAddTo = 6; break;
            default: Debug.Log(statIndex + " is an invalid string given to AddToPlayerStats"); break;
        }

        //Grab the player's stats and separate them into an array of strings
        if (loadedPlayerStats != null && loadedPlayerStats != "")
        {
            string[] playerStats = loadedPlayerStats.Split('_');
            //Record the new number in the player stats
            playerStats[indexToAddTo] = statToChange.ToString();
            //Rejoin the player stats into one string and save it
            PlayerPrefs.SetString(loadedSaveKey + "_player_stats", string.Join("_", playerStats));
            OverrideLoadedVariables(loadedSaveKey);
        }
    }

    //Gets a requested player stat - for AI purposes
    public static float GetPlayerStats(string statIndex)
    {
        int indexToGet = 0;
        float retrievedStat = 0;
        switch (statIndex.ToLower())
        {
            case "playtime": indexToGet = 0; break;
            case "wins": indexToGet = 1; break;
            case "losses": indexToGet = 2; break;
            case "kills": indexToGet = 3; break;
            case "deaths": indexToGet = 4; break;
            case "skirmishhighscore": indexToGet = 5; break;
            case "endlesshighscore": indexToGet = 6; break;
            default: Debug.Log(statIndex + " is an invalid string given to GetPlayerStats"); break;
        }

        //Grab the player's stats and separate them into an array of strings
        if (loadedPlayerStats != "" && loadedPlayerStats != null && indexToGet >= 0){
            string[] playerStats = loadedPlayerStats.Split('_');

            if (indexToGet >= playerStats.Length) return 0f; //GUARD to prevent IndexOOBounds errors

            //Retrieve the requested stat
            retrievedStat = float.Parse(playerStats[indexToGet]);
            return retrievedStat;
        } else {
            return 0f;
        }
    }

    //Call when wanting to get a specific endless high score
    public static float GetPlayerEndlessHighScore(string saveKey)
    {
        string[] playerStats = PlayerPrefs.GetString(saveKey + "_player_stats").Split("_");
        if (playerStats != null && playerStats[6] != "")
        {
            return float.Parse(playerStats[6]);
        }
        else
        {
            Debug.Log("Error: " + saveKey + " Endless High Score is invalid!");
            return 0;
        }
    }

    //Call when wanting to get a specific player's flag
    public static Sprite GetPlayerFlag(string saveKey)
    {
        return availableFlags[PlayerPrefs.GetInt(saveKey)];
    }

    //Call when wanting to get a specific skirmish high score
    public static float GetPlayerSkirmishHighScore(string saveKey)
    {
        string[] playerStats = PlayerPrefs.GetString(saveKey + "_player_stats").Split("_");
        if (playerStats[5] != "")
        {
            return float.Parse(playerStats[5]);
        }
        else
        {
            Debug.Log("Error: " + saveKey + " Skirmish High Score is invalid!");
            return 0;
        }
    }

    //Getter for retrieving all save files
    public static string[] GetAllSaves()
    {
        return PlayerPrefs.GetString("existing_saves").Split("____");
    }

    //Overrides the static variables on loading of a save game
    private static void OverrideLoadedVariables(string chosenSaveKey)
    {
        loadedSaveKey = chosenSaveKey;
        loadedProgress = PlayerPrefs.GetString(chosenSaveKey + "_progress");
        loadedLevelsCompleted = PlayerPrefs.GetString(chosenSaveKey + "_levels_completed");
        loadedPlayerStats = PlayerPrefs.GetString(chosenSaveKey + "_player_stats");
        loadedFlagIndex = PlayerPrefs.GetInt(chosenSaveKey);

    }

    public static void FinishedALevel(int levelIndex)
    {
        //Add the finished level to the array
        AddCompletedLevel(levelIndex);
        //Unlock the next level for the player
        if (levelIndex + 1 <= 5)
        {
            AddProgress(levelIndex + 1);
        }
        //Mark any previously finished levels as unlocked - precautionary
        for (int i = 0; i < levelIndex; i++)
        {
            AddProgress(i+1);
        }
        //PrintLoadedVariables();
    }

    public static void PrintLoadedVariables()
    {
        Debug.LogWarning("loadedSaveKey = " + loadedSaveKey);
        Debug.LogWarning("loadedProgress = " + loadedProgress);
        Debug.LogWarning("loadedLevelsCompleted = " + loadedLevelsCompleted);
        Debug.LogWarning("loadedFlagIndex = " + loadedFlagIndex);
        Debug.LogWarning("loadedPlayerStats = " + loadedPlayerStats);
    }

    public static void PrintAllSaveKeys()
    {
        int total = 0;
        Debug.Log("Save files are: ");
        foreach(string s in GetAllSaves())
        {
            Debug.Log(s);
            total++;
        }
        Debug.Log("Save file total: " + total);
    }
}
