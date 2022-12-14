using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    public static void LoadMainMenu()
    {
        Time.timeScale = 1;
        UIManager.isPaused = false;
        SceneLoader.LoadScene("MainMenu");
    }

    public void ReloadLevel()
    {
        Time.timeScale = 1;
        UIManager.isPaused = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadNextLevel()
    {
        if(SceneManager.GetActiveScene().name == "Level5")
        {
            SceneLoader.LoadScene("VictoryScene");
        }
        else if (SceneManager.GetActiveScene().buildIndex + 1 < SceneManager.sceneCountInBuildSettings)
        {
          
            SceneLoader.LoadScene(NameOfSceneByBuildIndex(SceneManager.GetActiveScene().buildIndex + 1));
            //SceneLoader.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    public string NameOfSceneByBuildIndex(int buildIndex)
    {
        string path = SceneUtility.GetScenePathByBuildIndex(buildIndex);
        int slash = path.LastIndexOf('/');
        string name = path.Substring(slash + 1);
        int dot = name.LastIndexOf('.');
        return name.Substring(0, dot);
    }

    public static void LoadLevel(string levelToLoad)
    {
        if(SceneManager.GetActiveScene() != SceneManager.GetSceneByName(levelToLoad))
        {
            SceneLoader.LoadScene(levelToLoad);
        }
    }

}
