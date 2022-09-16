using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using TMPro;

public class InteractiveTutorialController : MonoBehaviour
{
    [Header("Canvas References")]
    public Canvas tutorialCanvas;
    public Canvas inGameHUD;
    public Canvas inGameButtons;
    public TextMeshProUGUI instructionsText;
    public TextMeshProUGUI objectiveText;
    public TextMeshProUGUI headerText;
    public Button continueButton;
    public Button endTurnButton;
    public VideoPlayer clipPlayer;

    [Header("Tutorial Variables")]
    [TextArea]
    public string[] instructions;
    public string[] objectives;
    public string[] headers;
    public VideoClip[] tutorialClips;
    public int tutorialProgress = 0;
    public GameObject dummy;

    //Bools that control the stage of the tutorial
    public bool finishedBasics = false;
    private bool hasPanned = false;
    private bool hasRotated = false;
    public bool finishedCamera;
    public bool finishedEnvironment;

    [Header("Participants")]
    public PlayerParticipant playerParticipant;
    private Unit startingUnit;

    void Awake()
    {
        UIManager.isPaused = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        DisplayInstructions();
        startingUnit = playerParticipant.GetCurrentUnit();
    }

    private void Update()
    {
        if (UIManager.isPaused) return; //GUARD to prevent camera movement when the game is paused
        if (dummy == null)
        {
            dummy = transform.GetChild(0).gameObject;
            Invoke("DisplayInstructions", 0.5f);
            endTurnButton.interactable = true;
        }
        if(playerParticipant.turnCounter > 1)
        {
            if(dummy == transform.GetChild(0).gameObject && !finishedBasics)
            {
                finishedBasics = true;
                DisplayInstructions();
            }

            if (!finishedCamera && hasPanned && hasRotated)
            {
                finishedCamera = true;
                Invoke("DisplayInstructions", 2.0f);
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D) || Input.GetMouseButtonDown(0))
                {
                    hasPanned = true;
                }
                if(Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.E))
                {
                    hasRotated = true;
                }
            }
        }
    }

    public void ContinueButtonFunction()
    {
        if(continueButton.GetComponentInChildren<TextMeshProUGUI>().text.ToLower() == "continue")
        {
            HideInstructions();
            Time.timeScale = 1.0f;
            if(tutorialProgress == 0)
            {
                Invoke("DisplayInstructions", 1.0f);
            }
            tutorialProgress++;
        }
        else
        {
            tutorialProgress++;
            DisplayInstructions();
        }

        if(finishedBasics && finishedCamera && finishedEnvironment)
        {
            Invoke("EndTutorial", 0.5f);
        }
    }

    private void FreezeTime()
    {
        Time.timeScale = 0.0f;
    }

    private void EndTutorial()
    {
        Destroy(tutorialCanvas.gameObject);
        Destroy(gameObject);
    }

    public void RotatedThroughUIButton()
    {
        if (playerParticipant.turnCounter > 1 && finishedBasics)
            hasRotated = true;
    }

    public void DisplayInstructions()
    {
        UIManager.isPaused = true;
        tutorialCanvas.enabled = true;
        inGameHUD.enabled = false;
        inGameButtons.enabled = false;
        objectiveText.enabled = false;
        instructionsText.text = instructions[tutorialProgress];
        objectiveText.text = objectives[tutorialProgress];
        headerText.text = headers[tutorialProgress];
        clipPlayer.clip = tutorialClips[tutorialProgress];
        if (tutorialProgress == 5 || tutorialProgress == 6)
        {
            continueButton.GetComponentInChildren<TextMeshProUGUI>().text = "Next";
        }
        else
        {
            continueButton.GetComponentInChildren<TextMeshProUGUI>().text = "continue";
        }
    }

    public void HideInstructions()
    {
        UIManager.isPaused = false;
        tutorialCanvas.enabled = false;
        inGameHUD.enabled = true;
        inGameButtons.enabled = true;
        objectiveText.enabled = true;
    }
}
