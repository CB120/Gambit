using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class NewGameMenuController : MonoBehaviour
{
    [SerializeField] private Sprite[] flags;
    private static Sprite[] availableFlags;

    [Header("UI Element References")]
    [SerializeField] private Image flagPreview;
    [SerializeField] private TMP_InputField usernameEnter;
    [SerializeField] private Button siegeButton;
    [SerializeField] private TMP_Text errorText;
    private int flagIndex = 0;
    public Canvas mainCanvas;
    public Canvas tutCanvas;
    public GameObject mainMenu;

    private void Awake()
    {
        SaveSystem.SetAvailableFlags(flags);
        availableFlags = flags;
    }

    private void Start()
    {
        SaveSystem.SetAvailableFlags(flags);
        availableFlags = flags;
    }
    // Update is called once per frame
    void Update()
    {
        siegeButton.interactable = !(usernameEnter.text == "");
        //if (Input.GetKeyDown(KeyCode.Delete))
        //{
        //    PlayerPrefs.DeleteAll();
        //    PlayerPrefs.SetString("existing_saves", null);
        //    Debug.Log("Deleted all player prefs data");
        //}
    }

    public void CycleFlagRight()
    {
        flagIndex++;
        if (flagIndex >= flags.Length)
            flagIndex = 0;

        flagPreview.sprite = flags[flagIndex];
    }

    public void CycleFlagLeft()
    {
        if (flagIndex <= 0)
            flagIndex = flags.Length;

        flagIndex--;
        flagPreview.sprite = flags[flagIndex];
    }

    public void StartNewGame(bool startTut)
    {
        //Ensure that the entered key doesn't already exist
        bool saveKeyAlreadyExists = false;
        string[] existingSaves = SaveSystem.GetAllSaves();
        foreach(string save in existingSaves)
        {
            if (usernameEnter.text.ToLower() == save)
                saveKeyAlreadyExists = true;
        }
        //Create a new save if the key isn't in use
        if (!saveKeyAlreadyExists)
        {
            errorText.text = "";
            SaveSystem.CreateNewSave(usernameEnter.text.ToLower(), flagIndex);
            usernameEnter.text = "";
            if (!startTut)
            {
                SaveSystem.FinishedALevel(0);
                //SceneLoader.LoadScene("LoadingScreen");
                mainCanvas.enabled = false;
                tutCanvas.enabled = false;
                mainMenu.SetActive(true);
            }
            else
                SceneLoader.LoadScene("TutorialLevel");
        }
        else
            errorText.text = "File Already Exists!";
    }

    public void ShowTutPrompt()
    {
        //Ensure that the entered key doesn't already exist
        bool saveKeyAlreadyExists = false;
        string[] existingSaves = SaveSystem.GetAllSaves();
        foreach (string save in existingSaves)
        {
            if (usernameEnter.text.ToLower() == save)
                saveKeyAlreadyExists = true;
        }
        if (!saveKeyAlreadyExists)
        {
            errorText.text = "";
            mainCanvas.enabled = false;
            mainMenu.SetActive(false);
            tutCanvas.enabled = true;
        }
        else
            errorText.text = "File Already Exists!";
    }

    public static Sprite GetFlagFromRecentPlayer(int index)
    {
        return availableFlags[index];
    }
}
