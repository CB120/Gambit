using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("HUD")]
    [SerializeField] private TextMeshProUGUI title;
    //[SerializeField] private TextMeshProUGUI health;
    [SerializeField] private TextMeshProUGUI attack;
    [SerializeField] private TextMeshProUGUI range;
    [SerializeField] private TextMeshProUGUI information;
    [SerializeField] private Image portrait;
    [SerializeField] private Button endTurn;
    [SerializeField] private Button attackButton;
    [SerializeField] private HealthBar healthBarObj;
    [SerializeField] private GameObject ObjectiveCanvas;
    [SerializeField] private GameObject settingsScreenUI;
    [SerializeField] private TextMeshProUGUI mainObjectiveText;
    [SerializeField] private Button fastForward;
    public static TextMeshProUGUI HUDtitle;
    //public static TextMeshProUGUI HUDhealth;
    public static TextMeshProUGUI HUDattack;
    public static TextMeshProUGUI HUDrange;
    public static TextMeshProUGUI HUDinformation;
    public static Image HUDportrait;
    public static Button HUDEndTurn;
    public static Button HUDEnterAttackButton;
    public static HealthBar healthBar;
    public static GameObject ObjectiveCanvasObj;
    public static GameObject settingsScreen;
    public static TextMeshProUGUI objectiveTitle;
    public static Button fastForwardButton;


    [Header("Tool Bar")]
    //[SerializeField] private List<Texture2D> images;
    [SerializeField] private List<Sprite> images;
    [SerializeField] private List<GameObject> toolbar;
    [SerializeField] private GameObject slotHolder;
    [SerializeField] private GameObject toolbarHolder;
    private static List<Sprite> imageList;
    private static List<GameObject> toolbarList;
    private static GameObject slot;
    private static GameObject toolbarObject;

    [Header("Menu")]
    [HideInInspector]
    public static bool isPaused = false;
    [SerializeField] private GameObject pauseMenu;
    public static GameObject gameLostMenu;
    [SerializeField] private GameObject statisticsHolder;
    [SerializeField] private GameObject[] canvases;
    public static bool isOtherStateActive = false;

    [Header("Game Modes")]

    public bool isSkirmishMode = false;
    public static bool isSkirmishModeActive = false;
    public bool isEndlessMode = false;
    public static bool isEndlessModeActive = false;
    [SerializeField] private GameObject skirmishSettings;
    private static GameObject skirmishSettingsObject;
    public static List<GameObject> deadUnits = new List<GameObject>();
    [SerializeField] private GameMode gameMode;
    public static GameMode currentGameMode;


    [Header("KillAllEnemies")]
    [SerializeField] private TextMeshProUGUI objectiveText;
    private static TextMeshProUGUI killObjectiveText;

    [Header("Outposts")]
    [SerializeField] private GameObject outpostGridParent;
    private static GameObject outpostParentObj;
    [SerializeField] private GameObject outpostIndicatorObj;
    private static GameObject outpostIndicator;

    [Header("Animations")]
    public static Animator transitionAnimator;

    [Header("Level Information")]
    public int levelIndex;
    private static int thisLevelIndex;


    public void Awake()
    {
        currentGameMode = gameMode;
        outpostIndicator = outpostIndicatorObj;
        outpostParentObj = outpostGridParent;
        skirmishSettingsObject = skirmishSettings;
        isSkirmishModeActive = isSkirmishMode;
        isEndlessModeActive = isEndlessMode;
        //isOtherStateActive = true;
        //TODO: Convert all of these to an object (Singleton)
        HUDtitle = title;
        //HUDhealth = health;
        HUDinformation = information;
        HUDattack = attack;
        HUDrange = range;
        HUDportrait = portrait;
        HUDEndTurn = endTurn;
        HUDEnterAttackButton = attackButton;
        imageList = images;
        toolbarList = toolbar;
        HUDinformation = information;
        slot = slotHolder;
        toolbarObject = toolbarHolder;
        healthBar = healthBarObj;
        ObjectiveCanvasObj = ObjectiveCanvas;
        settingsScreen = settingsScreenUI;
        thisLevelIndex = levelIndex;
        objectiveTitle = mainObjectiveText;
        //isKillAllEnemiesMode = isKillAllEnemies;
        killObjectiveText = objectiveText;
        fastForwardButton = fastForward;
        Time.timeScale = 1;

    }

    public static void SetOtherState(bool value)
    {
        isOtherStateActive = value;
    }

    private void Start()
    {
        transitionAnimator = GameObject.FindGameObjectWithTag("TransitionManager").GetComponent<Animator>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Slash))//We gots to remove dis
        {
            enableGameResultState(false);
        }

        if (Input.GetButtonDown("Pause"))
        {
            if (!isOtherStateActive)
            {
                Pause();
            }
        }
    }

    //To Call this write UIManager.SetHUD(parameter), you do not need a reference to the object this is attached to
    public static void SetHUD(Unit selectedUnit)
    {
        HUDtitle.text = selectedUnit.unitType.ToString();
        //HUDhealth.text = selectedUnit.health.ToString() + "/" + selectedUnit.maxHealth.ToString();
        HUDinformation.text = selectedUnit.informationText;
        healthBar.SetMaxHealth(selectedUnit.maxHealth);
        healthBar.SetHealth(selectedUnit.health);
        HUDattack.text = selectedUnit.damage.ToString();
        HUDrange.text = selectedUnit.attackRange.ToString() + " UNITS";
        HUDportrait.sprite = GetImage(selectedUnit);

    }

    public static void SetEnemyObjectiveUnits(int unitCount)
    {
        killObjectiveText.text = "<color=#FF5D5D>" + unitCount.ToString() + "</color> remaining";
    }

    public static void DamageObjective(float health)
    {
        HealthBar healthBar = ObjectiveCanvasObj.GetComponent<HealthBar>();
        healthBar.SetHealth(health);
    }

    public static void SetObjectiveMaxHealth(float health)
    {
        ObjectiveCanvasObj.GetComponent<HealthBar>().SetMaxHealth(health);
    }


    //=======HUD======
    public static void SetTurnUI(List<Unit> units, int arrPos)
    {

        if (units[arrPos]) SetHUD(units[arrPos]);
        SetHoverPosition(arrPos, units);
        foreach (Unit unit in units) //If we ever
        {
            if (unit != units[arrPos])
            {
                unit.EnableUIObject(false);
            }
            else
            {
                unit.EnableUIObject(true);
            }
        }
    }

    //=======ToolBar======
    public static void SetUnitDead(List<Unit> units, Unit unit)
    {
        GameObject newSlot = Instantiate(slot, toolbarObject.transform);
        Color col = new Color(0.6f, 0, 0);
        newSlot.GetComponent<Image>().color = col;
        newSlot.GetComponent<Button>().enabled = false;
        newSlot.GetComponentsInChildren<Image>()[1].sprite = GetImage(unit);
        newSlot.GetComponentsInChildren<Image>()[1].color = new Color(0.4f, 0.4f, 0.4f); //set the color to white
        newSlot.GetComponentInChildren<TextMeshProUGUI>().text = "";
        deadUnits.Add(newSlot);
        SetToolBarImage(units);
    }

    public static void RemoveDeadUnits()
    {
        foreach (GameObject obj in deadUnits)
            Destroy(obj);

        deadUnits.Clear();
    }

    public static void SetHoverPosition(int toolBarPos, List<Unit> units)
    {
        //Unit.SetLayerRecursively(units[toolBarPos].gameObject, 14, true);
        HUDEnterAttackButton.interactable = true;
        //HUDEnterAttackButton.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "Skip Movement";
        if (toolbarList.Count > 0)//Ensure the list isnt empty
        {
            for (int i = 0; i < units.Count; i++)//For each toolbarSlot
            {
                Color disabledColor = new Color(0.4f, 0.4f, 0.4f);//Set the default disabled color
                if (i != units.Count && units[i].isAttacking)
                {
                    disabledColor = units[i].hasAttacked ? new Color(0.14f, 0.14f, 0.14f) : new Color(0.46f, 0.23f, 0); //if the unit is in attack mode, change the disabled color to orange
                }

                toolbarList[i].GetComponent<Image>().color = disabledColor;//set the color of the toolbar slot
                toolbarList[i].GetComponentsInChildren<Image>()[1].color = new Color(0.4f, 0.4f, 0.4f); //set the color of the image in the toolbar slot
            }
            Color selectedColor = new Color(1, 1, 1);//the default selected color (when a player clicks a number to select a unit)
            if (units[toolBarPos].isAttacking)
            {
                HUDEnterAttackButton.interactable = false;
                //HUDEnterAttackButton.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "Movement Forfeit";
                selectedColor = units[toolBarPos].hasAttacked ? new Color(0.14f, 0.14f, 0.14f) : new Color(1, 0.54f, 0); //if the selected unit is attacking set color to bright orange
            }
            toolbarList[toolBarPos].GetComponent<Image>().color = selectedColor;//set the selected toolbars color
            toolbarList[toolBarPos].GetComponentsInChildren<Image>()[1].color = new Color(1, 1, 1);//set the selected toolbar slots image color
        }
    }

    public static void SetToolBarImage(List<Unit> currentUnits)
    {
        foreach (GameObject oldSlot in toolbarList)
        {
            Destroy(oldSlot);
        }
        toolbarList.Clear();

        if (currentUnits.Count == 0) return;
        PlayerParticipant playerParticipant = currentUnits[0].ownerParticipant.GetComponent<PlayerParticipant>();

        for (int i = 0; i < currentUnits.Count; i++)
        {
            if (i < currentUnits.Count)
            {
                GameObject newSlot = Instantiate(slot, toolbarObject.transform);
                toolbarList.Add(newSlot);
                Color col = new Color(255, 255, 255, 255);
                Unit unit = currentUnits[i];
                newSlot.GetComponentsInChildren<Image>()[1].sprite = GetImage(unit);
                newSlot.GetComponentsInChildren<Image>()[1].color = col; //set the color to white
                newSlot.GetComponentInChildren<TextMeshProUGUI>().text = (i + 1).ToString();
                int unitID = i;//ID Needs to be set otherwise delegate result will always be 5
                newSlot.GetComponent<Button>().onClick.AddListener(delegate { playerParticipant.SelectUnit(unitID); });

            }
        }

        SetHoverPosition(PlayerParticipant.selectedUnit, currentUnits);
    }

    public static Sprite GetImage(Unit unit) //gets the index of the corresponding image
    {
        switch (unit.unitType)
        {
            case Unit.UnitType.Soldier:
                return imageList[0];
            case Unit.UnitType.Archer:
                return imageList[1];
            case Unit.UnitType.Crossbow:
                return imageList[2];
            case Unit.UnitType.Cavalry:
                return imageList[3];
            case Unit.UnitType.Catapult:
                return imageList[4];
            case Unit.UnitType.Mage:
                return imageList[5];
        }
        return imageList[0];
    }



    /*============
        MENUS
     ============*/
    public static void enableGameResultState(bool isLoseState)
    {
        isOtherStateActive = true;
        NewCameraMovement.JumpToCenter();
        if (isLoseState)
        {

            transitionAnimator.SetTrigger("Lose");
            SaveSystem.AddToPlayerStats("losses", 1);
            if (!isSkirmishModeActive && !isEndlessModeActive)
                GameObject.FindObjectOfType<SceneController>().Invoke("ReloadLevel", 4.75f);
            else if (isSkirmishModeActive)
                GameObject.FindObjectOfType<UIManager>().Invoke("OnSkirmishDeath", 4.75f);
            else if (isEndlessModeActive)
            {
                Debug.Log("Died On Endless Mode");
                skirmishSettingsObject.GetComponent<EndlessMode>().Invoke("OnLoseRound", 4.75f);
            }


        }
        else
        {
            transitionAnimator.SetTrigger("Win");
            SaveSystem.FinishedALevel(thisLevelIndex);
            SaveSystem.AddToPlayerStats("wins", 1);
            if (!isSkirmishModeActive && !isEndlessModeActive)
            {
                GameObject.FindObjectOfType<SceneController>().Invoke("LoadNextLevel", 3.5f);
            }
            else if (isSkirmishModeActive)
            {
                GameObject.FindObjectOfType<UIManager>().Invoke("OnSkirmishWin", 2f);
            }
            else if (isEndlessModeActive)
            {
                Debug.Log("Endless Mode Won");
                skirmishSettingsObject.GetComponent<EndlessMode>().Invoke("OnWinRound", 2f);

            }
        }

    }

    public void OnSkirmishDeath()
    {
        skirmishSettingsObject.GetComponent<ProceduralSettings>().pcgController.GenerateRandomMap();
        transitionAnimator.SetTrigger("RestartGame");
        isOtherStateActive = false;
    }

    public void OnSkirmishWin()
    {
        skirmishSettingsObject.SetActive(true);
        transitionAnimator.SetTrigger("RestartGame");
        skirmishSettingsObject.GetComponent<Animator>().SetTrigger("LevelWon");


    }


    public void Pause()
    {
        isPaused = !isPaused;
        if (isPaused) //Disable HUD & Enable Pause Menu
        {
            Camera.main.gameObject.GetComponent<MouseController>().cellSelectionEnabled = false;
            pauseMenu.SetActive(true);
            statisticsHolder.SetActive(false);
            toolbarObject.SetActive(false);
            HUDEndTurn.gameObject.SetActive(false);
            foreach (GameObject canvas in canvases) canvas.SetActive(false);
            Time.timeScale = 0;
        }
        else //Disable Menu & Enable HUD
        {
            settingsScreenUI.SetActive(false);
            Camera.main.gameObject.GetComponent<MouseController>().cellSelectionEnabled = true;
            pauseMenu.SetActive(false);
            statisticsHolder.SetActive(true);
            toolbarObject.SetActive(true);
            HUDEndTurn.gameObject.SetActive(true);
            foreach (GameObject canvas in canvases) canvas.SetActive(true);
            Time.timeScale = 1;
        }
    }

    public static void ToggleTurn(Participant participantType)
    {
        //Debug.Log(participantType);

        if (participantType.properties.participantType == ParticipantType.LocalPlayer)
        {
            HUDEndTurn.interactable = true;
            HUDEndTurn.GetComponentInChildren<TextMeshProUGUI>().text = "END TURN";
        }
        else if (participantType.properties.participantType == ParticipantType.AI)
        {
            HUDEndTurn.interactable = false;
            HUDEndTurn.GetComponentInChildren<TextMeshProUGUI>().text = "ENEMY'S TURN";
        }
    }

    public void SkipMovement()
    {
        Unit unit = ParticipantManager.GetCurrentUnit();

        if (unit)
        {
            HUDEnterAttackButton.interactable = false;
            unit.SkipMovement();
            SetHoverPosition(PlayerParticipant.selectedUnit, unit.ownerParticipant.GetComponent<PlayerParticipant>().units);
        }
    }

    public void ShowAttackCells()
    {
        Unit unit = ParticipantManager.GetCurrentUnit();

        if (unit)
        {
            GridController.ClearGrid();
            GridController.ShowAttackCells(unit);
            unit.currentCell.UpdateMaterial(World.GetPalette().gridCellAttackSelectedMaterial);
        }
    }

    public void ShowMovementCells()
    {
        GridController.UpdateGrid();
    }

    public static void ToggleActive(GameObject toggle)
    {
        if (toggle.activeInHierarchy)
            toggle.SetActive(false);
        else
            toggle.SetActive(true);
    }

    public static List<GameObject> outPostIndicators = new List<GameObject>();
    public static void SetOutPosts(int counter)
    {
        Debug.Log(outPostIndicators.Count);
        foreach (GameObject obj in outPostIndicators)
            Destroy(obj);

        foreach (GameObject obj in outPostIndicatorsActive)
            Destroy(obj);

        outPostIndicators.Clear();
        outPostIndicatorsActive.Clear();

        for (int i = 0; i < counter; i++)
        {
            GameObject indicator = Instantiate(outpostIndicator, outpostParentObj.transform);
            outPostIndicators.Add(indicator);
        }

    }
    public static List<GameObject> outPostIndicatorsActive = new List<GameObject>();
    public static void CaptureOutpost()
    {
        if (outPostIndicators.Count > 0)
        {
            outPostIndicators[0].GetComponent<Image>().color = new Color(0.156f, 0.53f, 0.73f, 1);
            outPostIndicatorsActive.Add(outPostIndicators[0]);
            outPostIndicators.RemoveAt(0);
        }
    }

    public static void FastForward()
    {
        if (Time.timeScale > 5) return;
        Time.timeScale += 1f;
    }

    public static void ToggleFastForward(bool on)
    {
        if (!on)
        {
            Time.timeScale = 1;
        }
        if (fastForwardButton) fastForwardButton.interactable = on;
    }
}
