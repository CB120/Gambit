using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CellState {
    Inactive,
    Active,
    AttackActive,
    Selected,
    Path
};

public enum CellMode {
    Normal,
    AttackOnly
};

public class Cell : MonoBehaviour
{
    [HideInInspector] public CellMode mode = CellMode.Normal;

    [SerializeField] MeshRenderer meshRenderer;

    public Vector2Int coordinates;

    public int height;

    public CellState state = CellState.Active;
    CellState materialState = CellState.Active;

    public Cell[] currentPlayerPath;
    public bool inRange { get; private set; }
    bool mouseInCell;

    /*[HideInInspector]*/
    public Unit currentUnit = null;
    public bool isForest = false;

    MouseController mouseController; //Ethan's cell-disabling bodge

    public int cellScore = 1;
    public int attackScore = 0;

    // AI Patrol Path
    public bool isPatrolNode = false;

    [HideInInspector] public GameObject forestObject;



    private void Awake() {
        if (meshRenderer == null) meshRenderer = GetComponentInChildren<MeshRenderer>();
        inRange = false;
    }

    private void Start() {
        mouseController = GameObject.FindWithTag("MainCamera").GetComponent<MouseController>();

        if (isPatrolNode == true) this.tag = "PatrolNode";
    }

    void Update(){
        if (forestObject) forestObject.transform.position = transform.position;
    }

    public void SetState(CellState newState) {
        state = newState;
        UpdateMaterial(state);
    }

    public void SetState(CellState newState, bool newInRange){
        state = newState;
        inRange = newInRange;
        UpdateMaterial(state);
    }

    public void UpdateMaterial(CellState newState){ //This overflow's intended to replace the other one, but allow the material to be set separate to the Cell's state
        Material newMaterial = null;
        materialState = newState;

        switch (newState) {
            case CellState.Inactive:
                if (mouseInCell) {
                    newMaterial = World.GetPalette().gridCellHoverInvalidMaterial;
                } else {
                    newMaterial = World.GetPalette().gridCellOutOfRangeMaterial;
                }
                break;

            case CellState.Active:
                if (mouseInCell){
                    newMaterial = World.GetPalette().gridCellHoverMaterial;
                } else {
                    newMaterial = World.GetPalette().gridCellMaterial;
                }
                break;

            case CellState.AttackActive:
                if (mouseInCell){
                    newMaterial = World.GetPalette().gridCellAttackHoverMaterial;
                } else {
                    newMaterial = World.GetPalette().gridCellAttackMaterial;
                }
                break;

            case CellState.Selected:
                newMaterial = GetCurrentSelectMaterial();
                break;

            case CellState.Path:
                Debug.Log("Path is deprecated but Cell " + gameObject.name + " at (" + coordinates.x + ", " + coordinates.y + ") is set to path state.");
                break;
        }

        meshRenderer.material = newMaterial;
    }

    public void UpdateMaterial(Material material){
        meshRenderer.material = material;
    }

    public void UpdateMaterial() {
        Debug.Log("Cell.UpdateMaterial() is now deprecated, please use Cell.UpdateMaterial(CellState), or Cell.UpdateMaterial(Material)");

        Material newMaterial = null;

        switch (state) {
            case CellState.Inactive:
                if (mouseInCell)
                    newMaterial = World.GetPalette().gridCellHoverInvalidMaterial;
                else
                    newMaterial = World.GetPalette().gridCellOutOfRangeMaterial;
                break;

            case CellState.Active:
                if (mouseInCell)
                    newMaterial = GetCurrentHoverMaterial();
                else
                    newMaterial = GetCurrentActiveMaterial();
                break;

            case CellState.Selected:
                newMaterial = GetCurrentSelectMaterial();
                break;

            case CellState.Path:
                Debug.Log("Path is deprecated but Cell " + gameObject.name + " at (" + coordinates.x + ", " + coordinates.y + ") is set to path state.");
                if (inRange) newMaterial = World.GetPalette().gridCellPathMaterial;
                else newMaterial = World.GetPalette().gridCellOutOfRangeMaterial;
                break;
        }

        meshRenderer.material = newMaterial;
    }

    public void CursorOnCell() {
        if (mouseController && !mouseController.cellSelectionEnabled) return; //Ethan's cell-disabling bodge

        GridController.CursorOnCell(this);
    }

    public void SetMouseInCell(bool b) {
        mouseInCell = b;
        if (SystemInfo.deviceType == DeviceType.Desktop) UpdateMaterial(materialState);

        if (currentUnit && !isForest){
            currentUnit.SetUI(b);

            int value = 0;
            if (b){
                if (currentUnit.isAIControlled){
                    value = 11;
                } else {
                    value = 10;
                }
            }
            Unit.SetLayerRecursively(currentUnit.gameObject, value, b);
        }
    }


    //Mouse input helpers
    Material GetCurrentHoverMaterial() {
        if (ParticipantManager.GetCurrentUnit().isAttacking || mode == CellMode.AttackOnly) {
            return World.GetPalette().gridCellAttackHoverMaterial;
        } else {
            return World.GetPalette().gridCellHoverMaterial;
        }
    }

    Material GetCurrentSelectMaterial() {
        if (ParticipantManager.GetCurrentUnit().isAttacking || mode == CellMode.AttackOnly) {
            return World.GetPalette().gridCellAttackSelectedMaterial;
        } else {
            return World.GetPalette().gridCellSelectedMaterial;
        }
    }

    Material GetCurrentActiveMaterial() {
        if ((ParticipantManager.GetCurrentUnit() && ParticipantManager.GetCurrentUnit().isAttacking) || mode == CellMode.AttackOnly) {
            return World.GetPalette().gridCellAttackMaterial;
        } else {
            return World.GetPalette().gridCellMaterial;
        }
    }


    //Pathfinding functions
    
    // Gets the path from this cell to the destination cell using depth first search.
    public Cell[] GetPathTo(Cell destination) {
        List<Cell> path = new List<Cell>();

        if (destination == this) return path.ToArray();

        GetPathToRecursive(destination, path);

        return path.ToArray();
    }

    public void GetPathToRecursive(Cell destination, List<Cell> path) {
        GetPathToRecursive(destination, path, new List<Cell>());
    }

    public void GetPathToRecursive(Cell destination, List<Cell> path, List<Cell> visited) {
        // TODO: ?
        if (destination == null) return;
        
        visited.Add(this);

        Cell closestCell = null;
        float minDistance = Mathf.Infinity;

        List<Cell> adjacent = new List<Cell>();
        foreach (Cell cell in GetAdjacentCells()) {
            if (visited.Contains(cell)) continue;
            // if (path.Count > 0 && path[path.Count - 1] == cell) continue;
            adjacent.Add(cell);
        }

        if (adjacent.Count == 0) {
            if (path.Count > 0) {
                // Adjacent cells are blocked, so go back a cell and start again (due to the visited list this will result in a different cell selection)
                Cell lastCell = path[path.Count - 1];
                path.Remove(lastCell);
                lastCell.GetPathToRecursive(destination, path, visited);
            } else {
                // This issue is happening on the first move
                // As a quick and dirty solution we'll just try any adjacent cell that hasn't been visited.
                // This will work perfectly for simple 1-wide corridor situations but might have unexpected results in some cases.
                // note visited[0] targets the initial that GetPathTo was called on.
                Cell unvisitedAdjacent = null;
                foreach (Cell cell in visited[0].GetAdjacentCells()) {
                    if (!visited.Contains(cell)) unvisitedAdjacent = cell;
                }
                if (!unvisitedAdjacent) return;
                unvisitedAdjacent.GetPathToRecursive(destination, path, visited);
            }
            return;
        }

        foreach (Cell c in adjacent) {
            if (!c) continue; //GUARD, skipping to the next element of adjacent if c is null

            if (c == destination) {
                path.Add(c);
                return;
            }

            float distance = c.GetSquareDistanceTo(destination);
            if (distance < minDistance) {
                minDistance = distance;
                closestCell = c;
            }
        }

        path.Add(closestCell);
        closestCell.GetPathToRecursive(destination, path, visited);
    }

    // Deprecated
    // public Cell[] GetPathToFromPlayer() {
    //     return currentPlayerPath;
    // }

    // A super quick moves estimation to be used when you already know you can get there.
    public int FastMovesTo(Cell cell) {
        Vector2Int vector = (coordinates - cell.coordinates);
        return Mathf.Abs(vector.x) + Mathf.Abs(vector.y);
    }

    public int FastMovesTo(Unit unit) { //Overflow method by Ethan, uses a Unit's position to skip any searching
        Vector2Int vector = (coordinates - unit.position);
        return Mathf.Abs(vector.x) + Mathf.Abs(vector.y);
    }

    public Cell[] GetCellsInRange(int range) {
        // Depth-limited breadth-first search

        // Cells that are in range
        List<Cell> cellsInRange = new List<Cell>();

        // Cells being worked on
        Queue<Cell> cells = new Queue<Cell>();
        // Add this cell as route
        cells.Enqueue(this);

        while (cells.Count > 0) {
            // Pop first cell
            Cell cell = cells.Dequeue();
            // If this cell is in range then
            if (FastMovesTo(cell) <= range) {
                cellsInRange.Add(cell);
            } else {
                // NOTE: I am 80% sure this can be break not a continue which means one ring less cells to iterate through (not that there's any computation)
                // If anything blows up then this is probably the issue, just replace break with continue.
                break;
            }

            // For all surrounding cells
            foreach (Cell neighbour in cell.GetAdjacentCells()) {
                // If they are either in the cellsInRange list or are already on the queue to be processed, skip
                if (cellsInRange.Contains(neighbour) || cells.Contains(neighbour)) {
                    continue;
                }
                // Add this neighbour
                cells.Enqueue(neighbour);
            }
        }

        return cellsInRange.ToArray();
    }


    public int GetMovesTo(Cell destination) {
        return GetPathTo(destination).Length;
    }

    public int GetMovesFromPlayer() {
        return currentPlayerPath.Length;
    }

    public Vector2Int GetCoords(){
        return coordinates;
    }

    public float GetSquareDistanceTo(Cell destination) {
        return (destination.transform.position - transform.position).sqrMagnitude;
    }

    public Cell[] GetAdjacentCells() {
        List<Cell> cellList = new List<Cell>();
        Vector2Int[] adjacentCoordinates = {
            coordinates + Vector2Int.up,
            coordinates + Vector2Int.down,
            coordinates + Vector2Int.left,
            coordinates + Vector2Int.right
        };

        foreach (Vector2Int adjacentCoordinate in adjacentCoordinates) {
            Cell c = GridController.GetCellAt(adjacentCoordinate.x, adjacentCoordinate.y);
            if (c) cellList.Add(c);
        }

        return cellList.ToArray();
    }
}
