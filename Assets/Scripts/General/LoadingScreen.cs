using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingScreen : MonoBehaviour
{
    public Material flag;
    // Start is called before the first frame update
    void Start()
    {
        flag.mainTexture = SaveSystem.availableFlags[SaveSystem.loadedFlagIndex].texture;
        Invoke("LoadMapScreen", 3.0f);
    }

    private void LoadMapScreen()
    {
        SceneLoader.LoadScene("MapScreen");
    }
}
