using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DifficultySwitch : MonoBehaviour
{
    private bool difficultySwitchedOn = true;
    public Sprite onSwitch;
    public Sprite offSwitch;
    public Button difficultyButton;
    public TMP_Dropdown dropdown;
    public GameObject difficultyDropdownSelection;

    public void OnEnable()
    {
        dropdown.value = SaveSystem.GetDifficulty();
        if (SaveSystem.dynamicDifficulty == true)
        {
            difficultySwitchedOn = true;
        }
        else
        {
            difficultySwitchedOn = false;
        }

        if (difficultySwitchedOn)
        {
            difficultyButton.image.sprite = onSwitch;
            difficultyDropdownSelection.SetActive(false);
        }
        else
        {
            difficultyButton.image.sprite = offSwitch;
            difficultyDropdownSelection.SetActive(true);
        }
    }

    public void SwitchDynamicDifficulty()
    {
        //Change difficulty
        difficultySwitchedOn = !difficultySwitchedOn;
        SaveSystem.FlipDynamicDifficulty();
        //Change button appearance
        if (difficultySwitchedOn)
        {
            difficultyButton.image.sprite = onSwitch;
            difficultyDropdownSelection.SetActive(false);
        }
        else
        {
            difficultyButton.image.sprite = offSwitch;
            difficultyDropdownSelection.SetActive(true);
        }

        Debug.Log(SaveSystem.dynamicDifficulty);
    }

    public void SetDifficulty()
    {
        SaveSystem.SetDifficulty(dropdown.value);
    }
}
