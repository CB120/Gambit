using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIObjects : MonoBehaviour
{
    [Header("HUD")]
    public TextMeshProUGUI HUDtitle;
    public TextMeshProUGUI HUDattack;
    public TextMeshProUGUI HUDrange;
    public TextMeshProUGUI HUDinformation;
    public Image HUDportrait;
    public Button HUDEndTurn;
    public Button HUDEnterAttackButton;
    // public HealthBar healthBar; DEPRECATED
    public HealthSquareBar healthSquareBar;
    public GameObject ObjectiveCanvasObj;
    public GameObject settingsScreen;
    public TextMeshProUGUI objectiveTitle;
    public Button fastForwardButton;
    public Button HUDButton;

    [Header("Tool Bar")]
    public List<Sprite> imageList;
    public List<GameObject> toolbarList;
    public GameObject slot;
    public GameObject toolbarObject;

    [Header("Menu")]
    //[HideInInspector]
    public GameObject pauseMenu;
    public GameObject statisticsHolder;
    public GameObject[] canvases;
    public PauseManager pauseManager;
    public UnitInfoMenu infoMenu;

    [Header("Game Modes")]
    public GameObject skirmishSettingsObject;
    public GameMode currentGameMode;


    [Header("KillAllEnemies")]
    public TextMeshProUGUI killObjectiveText;

    [Header("Outposts")]
    public GameObject outpostParentObj;
    public GameObject outpostIndicator;

    [Header("World")]
    public TextMeshProUGUI mapName;

}
