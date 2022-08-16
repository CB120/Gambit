using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProceduralSettings : MonoBehaviour
{
    public GenerateProcedurally pcgController;
    [SerializeField] private UnitController unitController;
    [SerializeField] private List<GameObject> UIObjects;
    [SerializeField] private PCGSettings settings;
    [SerializeField] private UnitSelector unitSelector;

    [SerializeField] private int maximumUnits = 100;
    [SerializeField] private AIController aiController;

    [Header("UI")]
    [SerializeField] private GameObject selectedContent;
    [SerializeField] private GameObject selectedAIContent;
    [SerializeField] private TextMeshProUGUI counterText;
    [SerializeField] private TextMeshProUGUI counterAIText;
    [SerializeField] private Button startButton;
    [SerializeField] private Button continueButton;
    [SerializeField] private List<TextMeshProUGUI> selectedBiomeText;
    [SerializeField] private List<TextMeshProUGUI> selectedSizeText;
    [SerializeField] private TextMeshProUGUI selectedGamemodeText;
    [SerializeField] private GameObject popup;
    [SerializeField] private List<Image> mapPreviewImages;
    [SerializeField] private List<Button> gameModeButtons;
    [SerializeField] private List<Button> difficultyButtons;
    [SerializeField] private TextMeshProUGUI sunlightText;
    private int selectedBiome = 0;
    private int selectedSize = 0;
    [Header("Animator")]
    [SerializeField] private Animator animator;

    private void Start()
    {
        SaveSystem.dynamicDifficulty = true;
    }
    public void StartGame()
    {
        pcgController.GenerateRandomMap();
        UIManager.isOtherStateActive = false;
        //SaveSystem.dynamicDifficulty = true;
        //CloseUI();
        //this.gameObject.SetActive(false);
    }

    public void ShowPopup()
    {
        popup.SetActive(!popup.gameObject.activeInHierarchy);
    }

    public void ChooseMapSize(MapSize mapSize)
    {
        pcgController.mapSize = mapSize;
        foreach(TextMeshProUGUI text in selectedSizeText)
            text.text = mapSize.ToString();
    }

    public void ChooseBiome(BiomeType biomeType)
    {
        ChangePreviewImage(GetBiome(biomeType).mapImage);  
        
        foreach (TextMeshProUGUI text in selectedBiomeText)
            text.text = biomeType.ToString();

        pcgController.setBiome = biomeType;
    }

    private Biome GetBiome(BiomeType biomeType)
    {
        switch (biomeType)
        {
            case BiomeType.Forest:
                return pcgController.biomes.Forest;
            case BiomeType.Snow:
                return pcgController.biomes.Snow;
            case BiomeType.Desert:
                return pcgController.biomes.Desert;
            case BiomeType.Mesa:
                return pcgController.biomes.Mesa;
            case BiomeType.Volcano:
                return pcgController.biomes.Volcano;
            case BiomeType.Cherry:
                return pcgController.biomes.Cherry;
            case BiomeType.Waterworld:
                return pcgController.biomes.Waterworld;
            case BiomeType.Swamp:
                return pcgController.biomes.Swamp;
            default: return pcgController.biomes.Forest;
        }
    }

    public void SelectPlayerUnit(int unitType)
    {
        SelectUnit(unitType, true);
    }

    public void SelectAIUnit(int unitType)
    {
        SelectUnit(unitType, false);
    }

    private void SelectUnit(int unitType, bool isPlayer)
    {
        if (isPlayer && unitController.selectedUnits.Count == maximumUnits) return;
        if (!isPlayer && unitController.selectedAIUnits.Count == maximumUnits) return;
        Unit.UnitType selectedUnit = (Unit.UnitType)unitType;
        ChooseUnit unit = new ChooseUnit();
        switch (selectedUnit)
        {
            case Unit.UnitType.Soldier:
                unit = unitSelector.soldier;
                break;
            case Unit.UnitType.Archer:
                unit = unitSelector.archer;
                break;
            case Unit.UnitType.Crossbow:
                unit = unitSelector.crossbow;
                break;
            case Unit.UnitType.Cavalry:
                unit = unitSelector.cavalry;
                break;
            case Unit.UnitType.Mage:
                unit = unitSelector.mage;
                break;
        }
        if(isPlayer)
            unitController.selectedUnits.Add(unit);
        else
            unitController.selectedAIUnits.Add(unit);
        GameObject button = Instantiate(unit.button, isPlayer ? selectedContent.transform : selectedAIContent.transform);
        button.GetComponentInChildren<Button>().onClick.AddListener(delegate { RemoveUnit(unitType, isPlayer); });
        UpdateCounter(isPlayer);
    }

    public void RemoveUnit(int unitType, bool isPlayer)
    {
        Unit.UnitType selectedUnit = (Unit.UnitType)unitType;
        foreach (ChooseUnit unit in isPlayer ? unitController.selectedUnits : unitController.selectedAIUnits) {
            if (unit.unitType == selectedUnit)
            {
                if(isPlayer)
                unitController.selectedUnits.Remove(unit);
                else
                unitController.selectedAIUnits.Remove(unit);
                UpdateCounter(isPlayer);
                return;
            }
        }
    }

    #region Game Mode and Difficulty
    public void SelectGameMode(int modeIndex)
    {
        GameMode selectedGameMode = (GameMode)modeIndex;
        foreach (Button button in gameModeButtons)
        {
            button.GetComponent<Image>().color = new Color(0.17f, 0.17f, 0.17f);
        }
        
        gameModeButtons[modeIndex].GetComponent<Image>().color = new Color(0.63f, 0, 0);

        switch (selectedGameMode)
        {
            case GameMode.DefeatAll:
                pcgController.gameMode = pcgController.selectGameMode.killAllEnemies;
                break;
            case GameMode.Catapult:
                pcgController.gameMode = pcgController.selectGameMode.catapult;
                break;
            case GameMode.Outposts:
                pcgController.gameMode = pcgController.selectGameMode.captureOutPosts;
                break;
        }
        selectedGamemodeText.text = pcgController.gameMode.mainTitle;

    }

    public void SelectDifficulty(int modeIndex)
    {
        foreach (Button button in difficultyButtons)
        {
            button.GetComponent<Image>().color = new Color(0.17f, 0.17f, 0.17f);
        }
        SaveSystem.dynamicDifficulty = false;
        switch (modeIndex)
        {
            case 0: aiController.AIDifficulty = Difficulty.Easy;
                break;
            case 1:
                aiController.AIDifficulty = Difficulty.Medium;
                break;
            case 2:
                aiController.AIDifficulty = Difficulty.Hard;
                break;
            case 3:
                SaveSystem.dynamicDifficulty = true;
                break;
        }

        difficultyButtons[modeIndex].GetComponent<Image>().color = new Color(0.63f, 0, 0);
    }
    #endregion

    #region Gross Stuff
    public void SelectTime()
    {
        pcgController.isDayTime = !pcgController.isDayTime;
        sunlightText.text = pcgController.isDayTime ? "Day Time" : "Night Time";
    }

    public void ChangeBiome(int increment) {
        selectedBiome += increment;
        if (selectedBiome > System.Enum.GetValues(typeof(BiomeType)).Length - 1)
            selectedBiome = 0;
        else if (selectedBiome < 0)
            selectedBiome = System.Enum.GetValues(typeof(BiomeType)).Length - 1;
        BiomeType biomeType = (BiomeType)selectedBiome;
        ChooseBiome(biomeType);
    }

    public void ChangeSize(int increment)
    {
        selectedSize += increment;
        if (selectedSize > System.Enum.GetValues(typeof(MapSize)).Length - 1)
            selectedSize = 0;
        else if (selectedSize < 0)
            selectedSize = System.Enum.GetValues(typeof(MapSize)).Length - 1;
        MapSize mapSize = (MapSize)selectedSize;
        

        switch (mapSize)
        {
            case MapSize.Small:
                gameModeButtons[1].interactable = false;
                gameModeButtons[2].interactable = false;
                SelectGameMode(0);
                break;
            case MapSize.Medium:
                gameModeButtons[1].interactable = true;
                gameModeButtons[2].interactable = false;
                //if gamemode != 1
                SelectGameMode(0);
                break;
            case MapSize.Large:
                gameModeButtons[1].interactable = true;
                gameModeButtons[2].interactable = true;
                break;
        }
        ChooseMapSize(mapSize);
    }

    private void ChangePreviewImage(Sprite img)
    {
        foreach (Image image in mapPreviewImages)
        {
            image.sprite = img;
        }
    }


    private void UpdateCounter(bool isPlayer)
    {
        if (!isPlayer)
        {
            if (unitController.selectedAIUnits.Count == 0)
                startButton.interactable = false;
            else
                startButton.interactable = true;


            if (unitController.selectedAIUnits.Count == maximumUnits)
            {
                counterAIText.text = "Max Units Reached";
                counterAIText.color = new Color(1, 0.33f, 0.33f);
                return;
            }
            counterAIText.color = new Color(255, 255, 255);
            counterAIText.text = unitController.selectedAIUnits.Count.ToString() + " Units Selected";
            return;
        }
        else
        {
            if (unitController.selectedUnits.Count == 0)
                continueButton.interactable = false;
            else
                continueButton.interactable = true;


            if (unitController.selectedUnits.Count == maximumUnits)
            {
                counterText.text = "Max Units Reached";
                counterText.color = new Color(1, 0.33f, 0.33f);
                return;
            }
            counterText.color = new Color(255, 255, 255);
            counterText.text = unitController.selectedUnits.Count.ToString() + " Units Selected";
        }
    }

    public void ChangeScreen(bool swipeRight)
    {
        if(swipeRight)
        animator.SetTrigger("SwipeRight");
        else
            animator.SetTrigger("SwipeLeft");
    }
    #endregion
}

public struct PCGSettings
{
    public Biome selectedBiome;
    public MapSize mapsize;
    public List<Unit> selectedUnits;
}
[System.Serializable]
public struct UnitSelector
{
    public ChooseUnit soldier;
    public ChooseUnit archer;
    public ChooseUnit crossbow;
    public ChooseUnit cavalry;
    public ChooseUnit mage;
}

public enum GameType
{
    Campaign,
    Endless,
    Skirmish,
}