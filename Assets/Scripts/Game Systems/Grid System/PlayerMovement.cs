using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // DEPRECATED - DO NOT USE

    public Cell startCell;
    public Cell currentCell;
    public int turnIndex;
    public ParticipantManager participantMngr;
    [SerializeField] int maxActionPoints;
    int actionPoints;

    private void Awake() {
        // GridController.selectedPlayer = this;
        actionPoints = maxActionPoints;

        Debug.LogWarning(gameObject.name + " using deprecated PlayerMovement component.");
    }

    /*private void Start() {
        if (!startCell) {
            startCell = GridController.GetDefaultStartCell();
        }
        MoveToCell(startCell);
    }

    public int ActionPointsToMoveTo(Cell c) {
        return c.GetMovesFromPlayer();
    }

    public void MoveToCell (Cell c) {
        // TODO: Distance check should be elsewhere so this function guarantees the movement
        int requiredActionPoints = ActionPointsToMoveTo(c);
        if (requiredActionPoints > actionPoints) return;

        actionPoints -= requiredActionPoints; //Disabled for now
        
        transform.position = c.transform.position;
        currentCell = c;
        GridController.OnPlayerChangePosition();
        // Debug.Log(c.coordinates);
        
    }

    public bool CanUseActionPoints (int ap) {
        return ap <= actionPoints;
    }

    public void ResetActionPoints () {
        actionPoints = maxActionPoints;
    }*/

}
