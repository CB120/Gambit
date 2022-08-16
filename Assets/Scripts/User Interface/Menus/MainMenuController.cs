using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private bool startedGame = false;

    [Header("Menu Container References")]
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject newGameMenu;
    [SerializeField] private GameObject loadGameMenu;
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private GameObject quitMenu;


    //UI Element References
    [Header("UI Element References")]
    [SerializeField] private GameObject gameLogo;
    [SerializeField] private GameObject fivePPLogo;
    [SerializeField] private GameObject pressAnyKey;
    [SerializeField] private GameObject selectIcon;

    //UI Main Menu Buttons
    [Header("Main Menu Button References")]
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button loadGameButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private TextMeshProUGUI swapFilesText;
    [SerializeField] private Image swapFilesButton;


    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown && !startedGame)
        {
            //Transition to main menu
            startedGame = true;
            gameLogo.GetComponent<Animator>().SetBool("startedGame", startedGame);
            fivePPLogo.GetComponent<Animator>().SetBool("startedGame", startedGame);
            pressAnyKey.GetComponent<Animator>().SetBool("startedGame", startedGame);
            //Show main menu if save already exists
            if (SaveSystem.GetAllSaves()[0] != "")
            {
                SaveSystem.LoadSave(PlayerPrefs.GetString("most_recent_save"));
                mainMenu.SetActive(true);
                SwapRecentPlayerInMainMenu();
            }
            //Show new game menu if save doesn't exist yet
            else
                newGameMenu.SetActive(true);
        }
    }

    public void SwapFromMainToNewMenu(string newMenu)
    {
        switch (newMenu)
        {
            case "New Game": mainMenu.SetActive(false); newGameMenu.SetActive(true); break;
            case "Load Game": mainMenu.SetActive(false); loadGameMenu.SetActive(true); break;
            case "Settings": mainMenu.SetActive(false); settingsMenu.SetActive(true); break;
            case "Quit Game": mainMenu.SetActive(false); quitMenu.SetActive(true); break;
            default: Debug.Log("Invalid Option"); break;
        }
    }

    public void SwapBackToMainMenu(string currentMenu)
    {
        switch (currentMenu)
        {
            case "New Game": mainMenu.SetActive(true); newGameMenu.SetActive(false); break;
            case "Load Game": mainMenu.SetActive(true); loadGameMenu.SetActive(false); SwapRecentPlayerInMainMenu(); break;
            case "Settings": mainMenu.SetActive(true); settingsMenu.SetActive(false); break;
            case "Quit Game": mainMenu.SetActive(true); quitMenu.SetActive(false); break;
            default: Debug.Log("Invalid Option"); break;
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void PlaceholderMethod(string echo)
    {
        Debug.Log(echo);
    }

    public void SwapRecentPlayerInMainMenu()
    {
        swapFilesText.text = SaveSystem.loadedSaveKey;
        swapFilesButton.sprite = NewGameMenuController.GetFlagFromRecentPlayer(SaveSystem.loadedFlagIndex);
    }
}
