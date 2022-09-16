using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartOfLevelTutorial : MonoBehaviour
{

    public GameObject nextSlide;

    private void Start()
    {
        Invoke("FreezeTime", 0.7f);    
    }

    public void ShowNextSlide()
    {
        Instantiate(nextSlide);
        Destroy(gameObject);
    }

    public void StartLevel()
    {
        Time.timeScale = 1;
        Destroy(gameObject);
    }

    private void FreezeTime()
    {
        Time.timeScale = 0f;
    }
}
