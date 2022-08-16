using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatsContainer : MonoBehaviour
{
    [Header("File Select")]
    [SerializeField] private GameObject scrollView;
    public GameObject fileButton;
    public GameObject addFileButton;
    void OnEnable()
    {
        if (SaveSystem.GetAllSaves()[0] != "")
        {
            foreach (string s in SaveSystem.GetAllSaves())
            {
                if(s != "")
                {
                    GameObject file = Instantiate(fileButton, scrollView.transform);
                    file.GetComponent<SaveFileButton>().assignedSavekey = s;
                }
            }
        }
        GameObject add = Instantiate(addFileButton, scrollView.transform);
    }
}
