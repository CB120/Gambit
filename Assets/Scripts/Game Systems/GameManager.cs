using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Saving;
public enum GameMode
{
    DefeatAll,
    Catapult,
    Outposts
}
public class GameManager : MonoBehaviour
{
    //Properties
    static bool applicationStarted;

    //Variables
    public static GameMode gameMode;

    //References
    static GameManager singleton;

    private void Awake()
    {
        if (!applicationStarted)
        {
            SavedData.LoadGameData();
            applicationStarted = true;
        }
    }
    void Start(){
        DontDestroyOnLoad(this.gameObject);
    }

    private void OnApplicationQuit() // Stores Game Data
    {
        SavedData.StoreGameData();
    }

    public static int UpdateAIDifficulty()
    {
        int DifficultyDecider = 0;
        float WinLossRatio;
        float KillDeathRatio;
        
        // Do the AI & Player scores

        if (SaveSystem.GetPlayerStats("wins") > 0 && SaveSystem.GetPlayerStats("losses") > 0 && SaveSystem.GetPlayerStats("deaths") > 0 && SaveSystem.GetPlayerStats("kills") > 0)
        {

            WinLossRatio = (SaveSystem.GetPlayerStats("wins") / SaveSystem.GetPlayerStats("losses"));
            KillDeathRatio = (SaveSystem.GetPlayerStats("kills") / SaveSystem.GetPlayerStats("deaths"));

            if(WinLossRatio < 1.5)
            {
                DifficultyDecider -= 10;
            } else if (WinLossRatio >= 1.5 && WinLossRatio <= 4)
            {
                DifficultyDecider += 10;
            } else if (WinLossRatio > 4)
            {
                DifficultyDecider += 20;
            }

            if (KillDeathRatio <= 0.5)
            {
                DifficultyDecider -= 20;
            }
            else if (KillDeathRatio >= 0.51 && KillDeathRatio <= 0.7)
            {
                DifficultyDecider -= 10;
            }
            else if (KillDeathRatio >= 0.71 && KillDeathRatio <= 2.5)
            {
                DifficultyDecider += 10;
            }
            else if (KillDeathRatio > 2.5)
            {
                DifficultyDecider += 20;
            }
            //Debug.Log("WinLossRatio = " + WinLossRatio + " KD Ratio = " + KillDeathRatio);
            //Debug.Log(DifficultyDecider);
        }

        //if(AIController.AIUnitScores != 0 && AIController.PlayerUnitScores != 0)
        //{
        
            if((AIController.AIUnitScores - AIController.PlayerUnitScores) > 250)
            {
                DifficultyDecider -= 10;
            } 
            else if ((AIController.AIUnitScores - AIController.PlayerUnitScores) <= 250 && (AIController.AIUnitScores - AIController.PlayerUnitScores) <= -100)
            {
                DifficultyDecider += 20;
            }
            else if ((AIController.AIUnitScores - AIController.PlayerUnitScores) <= -101)
            {
                DifficultyDecider += 30;
            }
            //Debug.Log(DifficultyDecider);
        
        //}

        return DifficultyDecider;
    }

    void Update(){
        if (Input.GetKeyDown(KeyCode.F)) { Debug.Log(gameMode); }
        /*if (Input.GetKeyDown(KeyCode.M))
        {
            //SaveSystem.PrintLoadedVariables();
            Debug.Log("AI Score = " + AIController.AIUnitScores);
            Debug.Log("Player Score = " + AIController.PlayerUnitScores);
            Debug.Log("DifficultyDecier = " + UpdateAIDifficulty());
            Debug.Log("Stats = " + SaveSystem.loadedPlayerStats);
        }*/
    }

    //Game States
    public static void GameIsOver(Unit unit)
    {
        Participant parentParticipant = unit.ownerParticipant.GetComponent<Participant>();
        if (parentParticipant.units.Count == 0 && gameMode == GameMode.DefeatAll)
        {
            UIManager.EndGame(!unit.isAIControlled);
            GameObject timer = GameObject.FindWithTag("Timer");
            if(timer) timer.GetComponent<Timer>().AddTimeToPlayerStats();
        }
        else if (!unit.isAIControlled)
        {
            if(parentParticipant.units.Count == 0)
            {
                UIManager.EndGame(!unit.isAIControlled);
                GameObject timer = GameObject.FindWithTag("Timer");
                if (timer) timer.GetComponent<Timer>().AddTimeToPlayerStats();
            }
        }
    }


}
