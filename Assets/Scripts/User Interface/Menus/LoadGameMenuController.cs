using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LoadGameMenuController : MonoBehaviour
{
    //Reference to container
    [SerializeField] private GameObject loadGameMenu;
    

    private void Update()
    {
        
    }

    public void LoadSelectedGame()
    {
        //Get the save key from the drop down
        //int selectedSaveIndex = fileSelect.value;
        //string selectedSave = fileSelect.options[selectedSaveIndex].text;
        //Load the selected save key
        SaveSystem.LoadSave(PlayerPrefs.GetString("most_recent_save"));
        SceneLoader.LoadScene("LoadingScreen");
    }

    public void LoadScene(string scene)
    {
        SaveSystem.LoadSave(PlayerPrefs.GetString("most_recent_save"));
        SceneLoader.LoadScene(scene);
    }

    private void DebugRandomizeLevelsUnlocked()
    {
        int randLevel = Mathf.FloorToInt(Random.Range(1, 3));
        Debug.Log("Activating = " + randLevel);
        SaveSystem.AddProgress(randLevel);
        SaveSystem.AddCompletedLevel(0);
    }
}
