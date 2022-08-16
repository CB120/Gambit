using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MapScreenController : MonoBehaviour
{

    [Header("Main UI Element References")]
    [SerializeField] private GameObject mapContainer;
    [SerializeField] private GameObject enemyInformation;
    [SerializeField] private GameObject selectIcon;
    [SerializeField] private GameObject selectLevelButtons;
    [SerializeField] private GameObject header;
    [SerializeField] private TextMeshProUGUI levelNameText;
    [SerializeField] private TextMeshProUGUI levelDescriptionText;


    [Header("Level Information")]
    [SerializeField] private string[] levelNames;
    [SerializeField] private string[] levelDescriptions;
    private string levelToLoad = "";
    [SerializeField] private Button[] levelbuttons;
    [SerializeField] private Image[] levelCompletedFlags;

    private void Start()
    {
        UpdateLevelAvailability();
    }


    private void UpdateLevelAvailability()
    {
        char[] unlockedLevels = SaveSystem.loadedProgress.ToCharArray();
        char[] completedLevels = SaveSystem.loadedLevelsCompleted.ToCharArray();

        for (int i = 0; i < unlockedLevels.Length; i++)
        {
            if (unlockedLevels[i] == '1')
            {
                levelbuttons[i].interactable = true;
                levelbuttons[i].image.enabled = true;
            }
        }

        for (int i = 0; i < completedLevels.Length; i++)
        {
            if (completedLevels[i] == '1')
            {
                levelCompletedFlags[i].enabled = true;
            }
        }
    }

    public void SelectLevel(string selectedLevel)
    {
        levelToLoad = selectedLevel;
        mapContainer.GetComponent<Animator>().SetBool("levelSelected", true);
        enemyInformation.GetComponent<Animator>().SetBool("levelSelected", true);
        selectIcon.GetComponent<Animator>().SetBool("levelSelected", true);
        selectLevelButtons.GetComponent<Animator>().SetBool("levelSelected", true);
        header.GetComponent<Animator>().SetBool("levelSelected", true);
        foreach(Button b in levelbuttons)
        {
            b.image.enabled = false;
            b.interactable = false;
        }
        foreach (Image f in levelCompletedFlags)
            f.enabled = false;
        Invoke("ChangeHeaderText", 0.4f);
    }

    public void CancelSelection()
    {
        levelToLoad = "";
        mapContainer.GetComponent<Animator>().SetBool("levelSelected", false);
        enemyInformation.GetComponent<Animator>().SetBool("levelSelected", false);
        selectIcon.GetComponent<Animator>().SetBool("levelSelected", false);
        selectLevelButtons.GetComponent<Animator>().SetBool("levelSelected", false);
        header.GetComponent<Animator>().SetBool("levelSelected", false);
        UpdateLevelAvailability();
        Invoke("ChangeHeaderText", 0.4f);
    }

    public void ChangeHeaderText()
    {
        if(levelToLoad == "")
        {
            header.GetComponent<TextMeshProUGUI>().text = "Awaiting your Command...";
        }
        else
        {
            header.GetComponent<TextMeshProUGUI>().text = "Ready to Attack?";
        }
    }

    public void LoadSelectedLevel()
    {
        if(levelToLoad != "")
        {
            SceneLoader.LoadScene(levelToLoad);
        }
    }

    public void UpdateLevelInformation(int levelIndex)
    {
        if (levelbuttons[levelIndex].interactable)
        {
            levelDescriptionText.text = levelDescriptions[levelIndex];
            levelNameText.text = levelNames[levelIndex];
        }
    }
}
