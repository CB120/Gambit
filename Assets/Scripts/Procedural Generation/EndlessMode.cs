using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EndlessMode : MonoBehaviour
{
    [SerializeField] private GenerateProcedurally pcgController;
    [SerializeField] private UnitController unitController;
    [SerializeField] private EndlessUI endlessUI;
    List<GameObject> unitItems = new List<GameObject>();
    private int score = 0;
    public int scoreIncrement = 5;
    private int roundCounter = 1;

    [Header("Scoreboards")]
    [SerializeField] private Image flag;
    [SerializeField] private GameObject scoreParent;
    [SerializeField] private GameObject setPlayerStats;
    private List<GameObject> objects = new List<GameObject>();
    private static int previousScore = 0;
    // Start is called before the first frame update
    private void Start()
    {
        if (SaveSystem.GetActivePlayer() != null && SaveSystem.GetActivePlayer() != "")
            flag.sprite = SaveSystem.GetPlayerFlag(SaveSystem.GetActivePlayer()) ;
        SetScores();
    }

    private void SetScores()
    {
        foreach (GameObject obj in objects)
            Destroy(obj);
        objects.Clear();
        string[] profiles = SaveSystem.GetAllSaves();
        for (int i = 0; i < profiles.Length; i++)
        {
            for (int j = 0; j < profiles.Length; j++)
            {
                if (SaveSystem.GetPlayerEndlessHighScore(profiles[i]) > SaveSystem.GetPlayerEndlessHighScore(profiles[j]))
                {
                    string temp = profiles[i];
                    profiles[i] = profiles[j];
                    profiles[j] = temp;
                }
            }
        }

        for (int i = 0; i < profiles.Length; i++)
        {
            GameObject playerStats = Instantiate(setPlayerStats, scoreParent.gameObject.transform);
            objects.Add(playerStats);
            SetPlayerStats components = playerStats.GetComponent<SetPlayerStats>();
            components.positionText.text = (i + 1).ToString();
            components.nameText.text = profiles[i];
            components.scoreText.text = SaveSystem.GetPlayerEndlessHighScore(profiles[i]).ToString();
        }
    }

    public void PopulateSettings()
    {

        //endlessUI.units.text;
        PopulateUnits();
        endlessUI.biome.text = pcgController.currentBiome.biome.ToString();
        endlessUI.mapSize.text = pcgController.mapSize.ToString();
        endlessUI.gameMode.text = pcgController.gameMode.mainTitle;

    }

    public void OnWinRound()
    {
        roundCounter++;
        endlessUI.roundCounterText.text = "Round " + (roundCounter).ToString();
        switch (pcgController.mapSize)
        {
            case MapSize.Small:
                score += scoreIncrement;
                break;
            case MapSize.Medium:
                score += (int)(scoreIncrement * 2);
                break;
            case MapSize.Large:
                score += (int)(scoreIncrement * 3);
                break;
        }

        endlessUI.score.text = "Score " + (score < 10 ? "0" : "") + score.ToString();
        if (SaveSystem.GetActivePlayer() != null)
            if (score > SaveSystem.GetPlayerEndlessHighScore(SaveSystem.GetActivePlayer()))
                SaveSystem.OverwritePlayerStats("endlesshighscore", score);

        GetComponent<Animator>().SetTrigger("NextRound");
        Invoke("Regenerate", 1f);
        Invoke("RestartGame", 5f);

    }
    public void OnLoseRound()
    {
        GetComponent<Animator>().SetTrigger("Dead");
        roundCounter = 0;
        SetScores();
        Invoke("Regenerate", 1f);
        Invoke("RestartGame", 5f);
    }
    public void RestartGame()
    {
        UIManager.transitionAnimator.SetTrigger("RestartGame");
    }
    
    public void Regenerate()
    {
        pcgController.GenerateRandomMap();
    }

    public void PopulateUnits()
    {
        foreach (GameObject obj in unitItems)
            Destroy(obj);
        unitItems.Clear();

        foreach(ChooseUnit unit in unitController.endlessUnits)
        {
            GameObject obj = Instantiate(unit.button, endlessUI.unitParent.transform);
            unitItems.Add(obj); 
        }
    }

    [System.Serializable]
    public struct EndlessUI
    {
        public GameObject unitParent;
        public TextMeshProUGUI biome;
        public TextMeshProUGUI mapSize;
        public TextMeshProUGUI gameMode;
        public TextMeshProUGUI score;
        public TextMeshProUGUI roundCounterText;
    }
}

