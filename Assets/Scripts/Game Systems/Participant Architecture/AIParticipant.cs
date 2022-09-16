using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIParticipant : Participant
{
    //Properties


    //Variables


    //References
    GridController gridController;


    //Engine-called methods
    void Start(){
        gridController = GameObject.FindWithTag("Grid Controller").GetComponent<GridController>();
        SetUnitAIFlag();
    }



    //Turn-ly methods
    public override void StartTurn(){
        base.StartTurn();
        ResetMovePoints();

        mouseController.cellSelectionEnabled = false;
        //AI-agnostic start-turn methods
    }

    protected override void EndTurn(){
        base.EndTurn();
        //AI-agnostic end-turn methods
    }


    //Unit-moving methods, add methods as needed.
    protected void MoveUnit(Unit unit, Cell cell){ //If the given Cell is in the given Unit's range, it will move the Unit.
        unit.PerformMoveTo(cell);

        if (cell.FastMovesTo(unit) > unit.movePoints) {
            Debug.LogWarning("AI " + gameObject.name + " attempted to move Unit " + unit.gameObject.name + " to out-of-range Cell (" + cell.coordinates.x + ", " + cell.coordinates.y + ")");
        }
    }

    protected void MoveUnit(Unit unit, Vector2Int coordinates){ //If the given coordinates have a valid Cell and it's in the Unit's range, it will move the Unit
        Cell cell = gridController.GetCellAtPosition(coordinates); //Find the Cell at coordinates

        if (cell == null) { //If we didn't find a Cell, debug print and return
            Debug.LogWarning("AI " + gameObject.name + " attempted to move Unit to coordinates (" + coordinates.x + ", " + coordinates.y +"). There's no cell there!");
        } else { //If we did find a Cell, pass it and unit to the unit/cell MoveUnit() overflow
            MoveUnit(unit, cell);
        }
    }

    protected void MoveUnit(Vector2Int originCoords, Vector2Int destinationCoords){ //If the given coordinates have a valid Unit and Cell (respectively), it will move the Unit
        Unit unit = GetUnitAtPosition(destinationCoords); //Find the Unit at originCoords

        if (unit == null){ //If we didn't find a Unit, debug print and return
            Debug.LogWarning("AI " + gameObject.name + " attempted to move a Unit at (" + originCoords.x + ", " + originCoords.y + "). There's no Unit there!");
        } else { //If we did find a Unit, pass it and destinationCoords to the unit/coordinates MoveUnit() overflow
            MoveUnit(unit, destinationCoords);
        }
    }

    public void ResetMovePoints() {
        foreach (Unit unit in units) {
            unit.ResetMovePoints();
        }
    }


    //AI Library - add functions as needed, this is to make AI development much easier and more peacemeal.
    protected List<Cell> GetCellsInMoveRange(Unit unit){ //Returns a list of Cells that the given Unit can move to
        return gridController.GetCellsInMoveRange(unit);
    }

    protected Cell[] GetArcherAttackRange(Unit unit){
        if (unit.unitType != Unit.UnitType.Archer){
            Debug.LogWarning("Passed in a Unit that isn't an Archer. Returning GetCellsInRange()");
            return unit.currentCell.GetCellsInRange(unit.attackRange);
        }

        List<CellInfo> infoList = GridController.GetAttackCells(unit);

        for (int i = 0; i < infoList.Count; i++){
            if (!infoList[i].inRange){
                infoList.Remove(infoList[i]);
                i--;
            }
        }

        Cell[] output = new Cell[infoList.Count];

        for (int i = 0; i < infoList.Count; i++){
            output[i] = infoList[i].cell;
        }
        return output;
    }


    //Helper functions for the AI Library, can be moved into that library if needed for AI
    Cell GetCellAtPosition(Vector2Int coordinates){ //Returns the Cell (if one exists) at the given grid-space coordinates
        return gridController.GetCellAtPosition(coordinates); //NULL MAY BE RETURNED
    }

    Unit GetUnitAtPosition(Vector2Int coordinates){ //Returns the Unit (if one exists) at the given grid-space coordinates
         foreach (Unit u in units){
             if (u.position == coordinates){
                 return u;
             }
         }
         return null;
    }


    //Other methods
    void SetUnitAIFlag(){ //Sets all of this Participant's Units' isAIControlled flag to true.
        foreach (Unit u in units){
            u.isAIControlled = true;
        }
    }
}
