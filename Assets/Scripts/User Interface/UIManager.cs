using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Saving;
public class UIManager : MonoBehaviour
{
    [Header("Objects")]
    [SerializeField] private UIObjects interfaceObjects;
    public static UIObjects UIObj;

    [Header("Menu")]
    [HideInInspector]
    public static bool isPaused = false;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject statisticsHolder;

    [Header("Game Modes")]

    public bool isSkirmishMode = false;
    public static bool isSkirmishModeActive = false;
    public bool isEndlessMode = false;
    public static bool isEndlessModeActive = false;
    public static List<GameObject> deadUnits = new List<GameObject>();

    [Header("Animations")]
    public static Animator transitionAnimator;

    [Header("Level Information")]
    public int levelIndex;
    private static int thisLevelIndex;
    [SerializeField] private Map_SO[] mapInfo;


    public void Awake()
    {
        thisLevelIndex = levelIndex;
        UIObj = interfaceObjects;
        isSkirmishModeActive = isSkirmishMode;
        isEndlessModeActive = isEndlessMode;
        interfaceObjects.mapName.text = mapInfo[levelIndex].mapName;

        Time.timeScale = 1;

    }

    private void Start()
    {
        transitionAnimator = GameObject.FindGameObjectWithTag("TransitionManager").GetComponent<Animator>();

        // Ensure pause menu is disabled in case someone accidentally leaves it enabled in inspector after working on it.
        if (pauseMenu.activeInHierarchy) {
            isPaused = true; // Logically, we're starting from a paused state
            Pause();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Slash))//We gots to remove dis
        {
            EndGame(false);
        }

        if (Input.GetButtonDown("Pause"))
        {
            TryPause();
        }
    }

    //To Call this write UIManager.SetHUD(parameter), you do not need a reference to the object this is attached to
    public static void SetHUD(Unit selectedUnit)
    {
        UIObj.HUDtitle.text = selectedUnit.unitType.ToString();
        UIObj.HUDinformation.text = selectedUnit.informationText;
        // DEPRECATED
        // UIObj.healthBar.SetMaxHealth(selectedUnit.maxHealth);
        UIObj.healthSquareBar.Initialise(selectedUnit.unitData.health);
        UIObj.healthSquareBar.SetHealth((int)selectedUnit.health);
        UIObj.HUDattack.text = selectedUnit.damage.ToString();
        UIObj.HUDrange.text = selectedUnit.attackRange.ToString() + " UNITS";
        UIObj.HUDportrait.sprite = GetImage(selectedUnit);
        UIObj.HUDButton.onClick.AddListener(delegate { UIObj.infoMenu.SetOnEnum(selectedUnit.unitType); });

    }

    public static void SetEnemyObjectiveUnits(int unitCount)
    {
        UIObj.killObjectiveText.text = "<color=#FF5D5D>" + unitCount.ToString() + "</color> remaining";
    }

    public static void DamageObjective(float health)
    {
        HealthBar healthBar = UIObj.ObjectiveCanvasObj.GetComponent<HealthBar>();
        healthBar.SetHealth(health);
    }

    public static void SetObjectiveMaxHealth(float health)
    {
        UIObj.ObjectiveCanvasObj.GetComponent<HealthBar>().SetMaxHealth(health);
    }


    //=======HUD======
#region HUD
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
    #endregion

    #region ToolBar
    //=======ToolBar======
    public static void SetUnitDead(List<Unit> units, Unit unit)
    {
        GameObject newSlot = Instantiate(UIObj.slot, UIObj.toolbarObject.transform);
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
        UIObj.HUDEnterAttackButton.interactable = true;
        if (UIObj.toolbarList.Count > 0)//Ensure the list isnt empty
        {
            for (int i = 0; i < units.Count; i++)//For each toolbarSlot
            {
                Color disabledColor = new Color(0.4f, 0.4f, 0.4f);//Set the default disabled color
                if (i != units.Count && units[i].isAttacking)
                {
                    disabledColor = units[i].hasAttacked ? new Color(0.14f, 0.14f, 0.14f) : new Color(0.46f, 0.23f, 0); //if the unit is in attack mode, change the disabled color to orange
                }

                UIObj.toolbarList[i].GetComponent<Image>().color = disabledColor;//set the color of the toolbar slot
                UIObj.toolbarList[i].GetComponentsInChildren<Image>()[1].color = new Color(0.4f, 0.4f, 0.4f); //set the color of the image in the toolbar slot
            }
            Color selectedColor = new Color(1, 1, 1);//the default selected color (when a player clicks a number to select a unit)
            if (units[toolBarPos].isAttacking)
            {
                UIObj.HUDEnterAttackButton.interactable = false;
                //HUDEnterAttackButton.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "Movement Forfeit";
                selectedColor = units[toolBarPos].hasAttacked ? new Color(0.14f, 0.14f, 0.14f) : new Color(1, 0.54f, 0); //if the selected unit is attacking set color to bright orange
            }
            UIObj.toolbarList[toolBarPos].GetComponent<Image>().color = selectedColor;//set the selected toolbars color
            UIObj.toolbarList[toolBarPos].GetComponentsInChildren<Image>()[1].color = new Color(1, 1, 1);//set the selected toolbar slots image color
        }
    }

    public static void SetToolBarImage(List<Unit> currentUnits)
    {
        foreach (GameObject oldSlot in UIObj.toolbarList)
        {
            Destroy(oldSlot);
        }
        UIObj.toolbarList.Clear();

        if (currentUnits.Count == 0) return;
        PlayerParticipant playerParticipant = currentUnits[0].ownerParticipant.GetComponent<PlayerParticipant>();

        for (int i = 0; i < currentUnits.Count; i++)
        {
            if (i < currentUnits.Count)
            {
                GameObject newSlot = Instantiate(UIObj.slot, UIObj.toolbarObject.transform);
                UIObj.toolbarList.Add(newSlot);
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
                return UIObj.imageList[0];
            case Unit.UnitType.Archer:
                return UIObj.imageList[1];
            case Unit.UnitType.Crossbow:
                return UIObj.imageList[2];
            case Unit.UnitType.Cavalry:
                return UIObj.imageList[3];
            case Unit.UnitType.Catapult:
                return UIObj.imageList[4];
            case Unit.UnitType.Mage:
                return UIObj.imageList[5];
        }
        return UIObj.imageList[0];
    }

    #endregion
    #region Menus
    /*============
        MENUS
     ============*/
    public static void EndGame(bool isLoseState)
    {
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
                UIObj.skirmishSettingsObject.GetComponent<EndlessMode>().Invoke("OnLoseRound", 4.75f);
            }


        }
        else
        {
            transitionAnimator.SetTrigger("Win");
            SavedData.CompleteLevel(thisLevelIndex);
            //SaveSystem.FinishedALevel(thisLevelIndex);
            //SaveSystem.AddToPlayerStats("wins", 1);
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
                UIObj.skirmishSettingsObject.GetComponent<EndlessMode>().Invoke("OnWinRound", 2f);

            }
        }

    }

    public void OnSkirmishDeath()
    {
        UIObj.skirmishSettingsObject.GetComponent<ProceduralSettings>().pcgController.GenerateRandomMap();
        transitionAnimator.SetTrigger("RestartGame");
    }

    public void OnSkirmishWin()
    {
        UIObj.skirmishSettingsObject.SetActive(true);
        transitionAnimator.SetTrigger("RestartGame");
        UIObj.skirmishSettingsObject.GetComponent<Animator>().SetTrigger("LevelWon");
    }

    public void TryPause () {
        if(UIObj.pauseManager.PlayerCanPause()) Pause();
    }
    public void Pause()
    {
        isPaused = !isPaused;
        if (isPaused) //Disable HUD & Enable Pause Menu
        {
            Camera.main.gameObject.GetComponent<MouseController>().cellSelectionEnabled = false;
            pauseMenu.SetActive(true);
            statisticsHolder.SetActive(false);
            UIObj.toolbarObject.SetActive(false);
            UIObj.HUDEndTurn.gameObject.SetActive(false);
            foreach (GameObject canvas in UIObj.canvases) canvas.SetActive(false);
            //Time.timeScale = 0;
        }
        else //Disable Menu & Enable HUD
        {
            UIObj.settingsScreen.SetActive(false);
            Camera.main.gameObject.GetComponent<MouseController>().cellSelectionEnabled = true;
            pauseMenu.SetActive(false);
            statisticsHolder.SetActive(true);
            UIObj.toolbarObject.SetActive(true);
            UIObj.HUDEndTurn.gameObject.SetActive(true);
            foreach (GameObject canvas in UIObj.canvases) canvas.SetActive(true);
            Time.timeScale = 1;
        }
    }

    public static void ToggleTurn(Participant participantType)
    {
        if (participantType.properties.participantType == ParticipantType.LocalPlayer)
        {
            UIObj.HUDEndTurn.interactable = true;
            UIObj.HUDEndTurn.GetComponentInChildren<TextMeshProUGUI>().text = "END TURN";
        }
        else if (participantType.properties.participantType == ParticipantType.AI)
        {
            UIObj.HUDEndTurn.interactable = false;
            UIObj.HUDEndTurn.GetComponentInChildren<TextMeshProUGUI>().text = "ENEMY'S TURN";
        }
    }

    public void SkipMovement()
    {
        Unit unit = ParticipantManager.GetCurrentUnit();

        if (unit)
        {
            UIObj.HUDEnterAttackButton.interactable = false;
            unit.SkipMovement();
            SetHoverPosition(PlayerParticipant.selectedUnit, unit.ownerParticipant.GetComponent<PlayerParticipant>().units);
        }
    }
    #endregion

    #region Cells

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

    #endregion
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
        foreach (GameObject obj in outPostIndicators)
            Destroy(obj);

        foreach (GameObject obj in outPostIndicatorsActive)
            Destroy(obj);

        outPostIndicators.Clear();
        outPostIndicatorsActive.Clear();

        for (int i = 0; i < counter; i++)
        {
            GameObject indicator = Instantiate(UIObj.outpostIndicator, UIObj.outpostParentObj.transform);
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
        if (UIObj.fastForwardButton) UIObj.fastForwardButton.interactable = on;
    }
}
