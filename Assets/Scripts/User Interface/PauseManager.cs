using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public GameObject[] pauseObjects;
    
    public bool PlayerCanPause()
    {
        foreach (GameObject obj in pauseObjects)
            if (obj.activeInHierarchy) return false;

        return true;
    }
}
