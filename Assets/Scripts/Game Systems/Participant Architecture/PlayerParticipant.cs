using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerParticipant : Participant
{
    //Properties


    //Variables
    public static int selectedUnit = 0; //index in Participant.units that the player's currently selected
    public int turnCounter = 0;

    //References
    [SerializeField] AudioClip changeUnitSound;

    public Unit Catapult = null;
    [SerializeField] bool AIGoesFirst;


    //Engine-called
    void Update() {
        // Select next unit when tab pressed (Temporary?)
        GetInput();
    }

    //private void Start()
    //{
    //    foreach (GameObject unit in GameObject.FindGameObjectsWithTag("Player"))
    //    {
    //        Unit troop;
    //        troop = unit.GetComponent<Unit>();

    //        units.Add(troop);
    //    }
    //}

    //Turn-ly methods
    public override void StartTurn(){
        base.StartTurn();
        if (AIGoesFirst)
        {
            Invoke("SkipToAITurn", 2f);
            return;
        }
        mouseController.cellSelectionEnabled = true;
        ResetMovePoints();
        OnSelectNewUnit();
        UIManager.SetToolBarImage(units);
        
        turnCounter++;
    }

    //Public getters
    public Unit GetCurrentUnit(){
        //Debug.Log("SelectedUnit");
        //Debug.Log(selectedUnit);
        //Debug.Log("Units");
        //Debug.Log(units.Count);
        if (selectedUnit == -1 || selectedUnit >= units.Count || !units[selectedUnit]){
            if (debugOn) Debug.LogWarning("PlayerParticipant.GetCurrentUnit(): No selected unit, returning null.");
            return null;
        }

        return units[selectedUnit].GetComponent<Unit>(); //will need to apply similar Character class changes as in ParticipantManager
    }

    public void SelectNextUnit() {
        if (ParticipantManager.GetCurrentParticipant() != this) return; //GUARD to prevent changing Units during the AI's turn

        if (units.Count > 0) {
            selectedUnit++;
            if (selectedUnit >= units.Count) selectedUnit = 0;
            OnSelectDifferentUnit();
            OnSelectNewUnit();
        }
    }

    public void SelectUnit(int unitId) {
        if (ParticipantManager.GetCurrentParticipant() != this) return; //GUARD to prevent changing Units during the AI's turn

        int oldUnit = selectedUnit;
        if (unitId < units.Count) {
            selectedUnit = unitId;
        } else {
            Debug.LogWarning("PlayerParticipant.SelectUnit(): Attempted to select unit outside of units range | " + unitId);
        }
        if (oldUnit != selectedUnit) {
            OnSelectDifferentUnit();
        }

        // always call OnSelectNewUnit to catch minor bugs
        OnSelectNewUnit();
    }

    public void SelectUnit(Unit unit) {
        if (units.Contains(unit)) {
            // Use internal SelectUnit function to avoid differing functionality
            SelectUnit(units.IndexOf(unit));
        } else {
            // Debug.LogWarning("PlayerParticipant.SelectUnit(): Attempted to select unit this participant doesn't own");
        }
    }

    public void OnSelectNewUnit () {
        if(units.Count > 0) UIManager.SetTurnUI(units, selectedUnit);
        GridController.UpdateGrid();

        if (GetCurrentUnit()){
            if (GetCurrentUnit().currentCell){
                if (turnCounter > 0){
                    NewCameraMovement.JumpToCell(GetCurrentUnit().currentCell);
                }
            }
        }
    }

    // Called only when the newly selected unit is different to the previously selected unit
    void OnSelectDifferentUnit() {
        AudioManager.PlaySound(changeUnitSound, AudioType.GameUI);
    }

    public void ResetMovePoints() {
        foreach (Unit unit in units) {
            unit.ResetMovePoints();
        }
    }

    public static void ResetSelectedUnit(int unitsLength){
        if(selectedUnit > unitsLength - 1 && unitsLength != 0){
            selectedUnit = 0;
            GameObject.FindObjectOfType<PlayerParticipant>().SelectUnit(selectedUnit);
        }
    }

    public void GetInput(){
        if (!UIManager.isPaused)
        {
            if (Input.GetKeyDown(KeyCode.Tab)) SelectNextUnit();

            //Lord Please Forgive Me
            //you are forgiven
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                SelectUnit(0);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                SelectUnit(1);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                SelectUnit(2);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                SelectUnit(3);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                SelectUnit(4);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                SelectUnit(5);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha7))
            {
                SelectUnit(6);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha8))
            {
                SelectUnit(7);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha9))
            {
                SelectUnit(8);
            }
        }
    }
    private void SkipToAITurn()
    {
        base.EndTurn();
        turnCounter++;
        AIGoesFirst = false;
    }
}
