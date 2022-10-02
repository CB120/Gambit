using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CellInfo {
    public Cell cell;
    public CellState state = CellState.Inactive;
    public bool inRange = false;
    public bool losFailed = false;

    public CellInfo(Cell cell, CellState state, bool inRange){
        this.cell = cell;
        this.state = state;
        this.inRange = inRange;
    }

    public CellInfo(Cell cell, CellState state, bool inRange, bool losFailed){
        this.cell = cell;
        this.state = state;
        this.inRange = inRange;
        this.losFailed = losFailed;
    }

    public CellInfo(Cell cell){
        this.cell = cell;
    }

    public void SetState(CellState state, bool inRange){
        this.state = state;
        this.inRange = inRange;
    }
}

public class GridController : MonoBehaviour
{
    //Properties
    [SerializeField] bool drawHoverPath;
    public Vector2Int defaultCellCoordinates;
    public Cell defaultPlayerCell;

    //Variables
        //Static
            //Public
    [HideInInspector] public static List<Cell> currentHoverPath = new List<Cell>();
    [HideInInspector] public static Cell clickHoveredCell; //Used by Ethan to determine if the Mouse-Down Cell is the same as the Mouse-Up Cell

    public static List<Cell> cells;
    public static Cell[,] grid;

            //Private
    static Cell HoveredCell;


    //References
    static GridController Singleton;
    static OutpostObjective outpostObjective;

    public MapGenerator mapGenerator;
    public GridCursor gridCursor;

    public bool isProcedural = false;

    private void Awake () {
        if(!isProcedural) OnAwake();
    }

    public void OnAwake()
    {
        Singleton = this;
        cells = new List<Cell>();
        GridGenerator generator = GetComponent<GridGenerator>();
        grid = new Cell[generator.width, generator.height];

        foreach (Transform c in transform)
        {
            Cell cell = c.gameObject.GetComponent<Cell>();
            if (cell)
            {
                RegisterCell(cell, cell.coordinates.x, cell.coordinates.y);
            }
            else
            {
                Debug.Log("GridController has child GameObjects that aren't Cells. Check if this was intentional");
            }
        }

        if (defaultPlayerCell == null)
        {
            defaultPlayerCell = GetCellAt(defaultCellCoordinates);
            if (defaultPlayerCell == null)
            {
                Debug.LogWarning("No default Cell set, or it cannot be found. This will probably cause errors.");
            }
        }

        SetForestCells(mapGenerator.forestCoordinatesInternal);
    }

    void Start(){
        GameObject outpostController = GameObject.FindWithTag("OutpostController");
        if (outpostController) outpostObjective = outpostController.GetComponent<OutpostObjective>();
    }

    #region Grid data and Systems


    public static void RegisterCell (Cell c, int x, int y) {
        grid[x, y] = c;
    }

    public static void SetForestCells(List<Vector2Int> coordinates){
        for (int i = 0; i < coordinates.Count; i++){
            Vector2Int v = coordinates[i];
            if (v.x >= 0 && v.y >= 0 && grid.GetLength(0) > v.x && grid.GetLength(1) > v.y)
            {
                if (grid[v.x, v.y])
                {
                    grid[v.x, v.y].isForest = true;
                    grid[v.x, v.y].forestObject = Singleton.mapGenerator.forestObjects[i]; // If this is coming up with an error, just Generate forests & then ungenerate them (delete the tree prefabs too) ~ Christian
                }
                else
                {
                    Debug.LogWarning("Cell at (" + v.x + ", " + v.y + ") has gone missing!");
                }
            }
                else
                {
                    Debug.LogWarning("Received int coordinates (" + v.x + ", " + v.y + ") which are outside the Grid's range.");
                }
        }
    }

    public static Cell GetDefaultStartCell() {
        return Singleton.defaultPlayerCell;
    }

    public static Cell GetCellAt(int x, int y) {
        if (x >= 0 && y >= 0 && grid.GetLength(0) > x && grid.GetLength(1) > y) {
            return grid[x,y];
        } else {
            return null;
        }
    }

    public static Cell GetCellAt(Vector2Int coordinates){
        return GetCellAt(coordinates.x, coordinates.y);
    }

    public static Cell GetHoveredCell() {
        return HoveredCell;
    }

    #endregion

    #region Mouse input

    public static void OnMouseDown(){ //Called from MouseController when the mouse isn't over a UI element and the mouse button is pressed
        clickHoveredCell = GetHoveredCell(); //Store the cell we selected when we pressed the mouse
    }

    public static void OnMouseHeld(){
        if (clickHoveredCell == null) return; //If the input's already been invalidated, just keep returning
        if (GetHoveredCell() != clickHoveredCell){ //If the mouse has moved cells in its hold time, invalidate the input
            clickHoveredCell = null;
        }
    }

    public static void OnMouseUp(){ //Called from MouseController when the mouse isn't over a UI element and the mouse button is released
        if (GetHoveredCell() && GetHoveredCell() == clickHoveredCell){ //if we mouse-upped on a Cell and it was the same Cell we moused-down on (if it is, this isn't a drag)
            if (ParticipantManager.GetCurrentParticipant().properties.participantType == ParticipantType.LocalPlayer) { //Check our current Participant is the Player
                if (GetHoveredCell().inRange){ //if this Cell is a valid move or attack space, proceed in the context of the current selected Unit
                    Unit currentUnit = ParticipantManager.GetCurrentUnit();
                    if (currentUnit) currentUnit.OnMouseClick(); //pass the Click off to the Unit to Move or Attack
                }

                if (GetHoveredCell().currentUnit) { //if we clicked on a Unit...
                    if (!GetHoveredCell().currentUnit.isAIControlled) { //...and if the Unit is the player's Unit...
                        //...select that unit.
                        PlayerParticipant playerParticipant = ParticipantManager.GetCurrentParticipant().GetComponent<PlayerParticipant>();
                        if (playerParticipant) playerParticipant.SelectUnit(GetHoveredCell().currentUnit);
                    }
                }
            }
        }
    }

    // Called every update that the cursor is hovered over Cell c
    public static void CursorOnCell(Cell c) {
        // Ensure that this method is only run once when the cursor enters a specific cell.
        // e.g. null -> Cell1: true, Cell1 -> Cell1: false, Cell1 -> Cell2: true
        // Note: For simplicity, most mouse code is called every Update (from MouseController)
        if (c != HoveredCell) {
            // Run logic for when the cursor first enters a cell.
            CursorEnterCell(c);
            if (Singleton.gridCursor) Singleton.gridCursor.SetVisible(true);
        }
    }

    // Called every update that the cursor isn't hovering on a cell.
    public static void CursorNotOnCell() {
        // If there is a cell marked as hovered, leave it
        if (HoveredCell) {
            CursorLeaveCell(HoveredCell);
            if (Singleton.gridCursor) Singleton.gridCursor.SetVisible(false);
        }
    }

    // Called once when the curosr enters a new cell.
    public static void CursorEnterCell(Cell c) {
        // HoveredCell will be null when we're entering a cell after the mouse has been completely off the map.
        if (HoveredCell != null) {
            // Run leaving code on the cell we're coming from
            if (c.currentUnit && !c.isForest){
                if (c.currentUnit.isAIControlled){
                    Unit.SetLayerRecursively(c.currentUnit.gameObject, 11, true);
                } else {
                    Unit.SetLayerRecursively(c.currentUnit.gameObject, 10, true);
                }

                c.currentUnit.EnableUI();
            }
            if (outpostObjective) outpostObjective.CheckOutpostHover(c);
            CursorLeaveCell(HoveredCell);
        }

        // Update HoveredCell
        HoveredCell = c;
        // Tell the cell that it is the active cell.
        // Note this is controlled from GridController to ensure you cannot possibly hover over two cells at once.
        // The old cell is set to false in CursorLeaveCell.
        c.SetMouseInCell(true);

        // Move the GridCursor to this cell
        if (Singleton.gridCursor) if (Singleton.gridCursor.enabled) Singleton.gridCursor.MoveCursor(c);
    }

    // Called once on a cell being left
    public static void CursorLeaveCell(Cell c) {
        //TODO: I think this is causing the bug where the Outline & UI don't always show when a Unit's Cell is hovered - Ethan

        // Ensure this is the currently hovered cell.
        // Note that this is not like CursorOnCell where this if statement is a vital part of the logic
        // Here it is just a safety net, and actually doesn't matter that much.
        if (c == HoveredCell) {
            if (c.currentUnit){
                if (!c.isForest){
                    Unit.SetLayerRecursively(c.currentUnit.gameObject, 0, false);
                }
                c.currentUnit.DisableUI();
            }
            // Clear the hovered cell
            HoveredCell = null;
            // Tell the hovered cell that it is no longer hovered.
            c.SetMouseInCell(false);
        }
    }

    #endregion

    #region Player controls


    public static void ClearGrid(){
        foreach(Cell cell in grid) {
            if (cell){
                cell.SetState(CellState.Inactive, false);
            }
        }
    }

    public static void UpdateGrid() {
        ClearGrid();

        // For player turns
        if (ParticipantManager.IsCurrentParticipantType(ParticipantType.LocalPlayer)){
            Unit currentUnit = ParticipantManager.GetCurrentUnit();

            if (currentUnit){
                //important to note that I (Ethan) messed up where these methods are divided, so they both do Cell and Forest Outline stuff. Sorry :/
                UpdateCells(currentUnit);
                UpdateForestOutlines(currentUnit);
            }
        }
        // Hard set selected tile to selected
        // TODO: this will probably look funky with AI
        if (ParticipantManager.IsCurrentParticipantType(ParticipantType.LocalPlayer)){
            Unit currentUnit = ParticipantManager.GetCurrentUnit();
            if (currentUnit){
                if (ParticipantManager.GetCurrentUnit().currentCell){
                    ParticipantManager.GetCurrentUnit().currentCell.SetState(CellState.Selected);
                }
            }
        }
    }

    public static void UpdateCells(Unit currentUnit){
        if (currentUnit.hasAttacked){
            currentUnit.currentCell.SetState(CellState.Selected);
            return;
        }

        if (currentUnit.isAttacking){
            ShowAttackCells(currentUnit);
        } else { //Movement mode
            ShowMovementCells(currentUnit);
        }
    }

    public static void ShowMovementCells(Unit currentUnit){
        Cell[] range = currentUnit.GetMovementRange();
        List<CellInfo> attackCells = GetAttackCells(currentUnit);

        List<Cell> attackRange = new List<Cell>();
        foreach (CellInfo i in attackCells){
            if (i.cell) {
                if (i.state == CellState.AttackActive && i.inRange) attackRange.Add(i.cell);
            }
        }

        foreach (Cell cell in range){
            if (!cell.currentUnit){ // Ignore occupied cells for movement.
                if (cell.mode == CellMode.AttackOnly) {
                    if (cell.FastMovesTo(currentUnit) <= currentUnit.attackRange && !OutpostObjective.IsOutpostDead(cell)){
                        cell.SetState(CellState.AttackActive, true);
                    }
                } else {
                    cell.SetState(CellState.Active, true);
                }
            } else if (cell.currentUnit.isAIControlled){
                if (attackRange.Contains(cell)){
                    if (cell.isForest){
                        if (cell.FastMovesTo(currentUnit) <= World.GetRules().maxDistanceFromForestToAttack && cell.height <= currentUnit.currentCell.height){
                            cell.SetState(CellState.AttackActive, true);
                        } else {
                            cell.SetState(CellState.Inactive, false);
                        }
                    } else if (cell.height <= currentUnit.currentCell.height){
                        cell.SetState(CellState.AttackActive, true);
                    } else {
                        cell.SetState(CellState.Inactive, false);
                    }
                } else {
                    cell.SetState(CellState.Inactive, false);
                }
            }
        }
    }

    public static void ShowAttackCells(Unit currentUnit){
        List<CellInfo> cellList = GetAttackCells(currentUnit);

        foreach (CellInfo i in cellList){
            if (i != null){
                if (i.cell != null){
                    i.cell.SetState(i.state, i.inRange);
                } else {
                    Debug.LogWarning("i's cell reference is null");
                }
            }
        }
    }

    public static List<CellInfo> GetAttackCells(Unit currentUnit){
        if (currentUnit.unitType == Unit.UnitType.Archer) return GetArcherAttackCells(currentUnit);

        //Create our array for storing all the theoretical Attack states for the given Unit
        List<CellInfo> infoList = new List<CellInfo>();

        //Load every Cell into the output array
        for (int x = 0; x < grid.GetLength(0); x++){
            for (int y = 0; y < grid.GetLength(1); y++){
                if (grid[x,y]) infoList.Add(new CellInfo(grid[x, y]));
            }
        }

        Cell[] range = currentUnit.GetAttackRange();

        List<Cell> attackCells = new List<Cell>(); //A reference to the valid attack cells
        foreach (Cell cell in range){

            if (cell != currentUnit.currentCell){
                bool isValidRange = true;//By default each cell in the range is true
                Cell[] path = currentUnit.currentCell.GetPathTo(cell);  //Gets the path from the current cell to every other cell in range

                for (int i = 0; i < path.Length; i++){//Loops through the path
                    if (path[i].height > currentUnit.currentCell.height){
                        isValidRange = false;//If a cell on the path is higher, set it to an invalid attack cell
                    } else {
                        attackCells.Add(path[i]);//If a cell is valid add it to the attackCells list
                    }

                    //Below: Ethan's long-winded version to account for Outposts
                    if (isValidRange){
                        if (path[i].mode == CellMode.AttackOnly && OutpostObjective.IsOutpostDead(path[i])){
                            GetCellInfo(infoList, path[i]).SetState(CellState.Inactive, false);
                        } else {
                            GetCellInfo(infoList, path[i]).SetState(CellState.AttackActive, true);
                        }
                    } else {
                        GetCellInfo(infoList, path[i]).SetState(CellState.Inactive, false);
                    }

                    if (path[i].currentUnit){
                        if (path[i].currentUnit.ownerParticipant == currentUnit.ownerParticipant){
                            GetCellInfo(infoList, path[i]).SetState(CellState.Inactive, false); //doesn't affect the internal range/state of the Cell, just changes its appearance
                        }
                    }
                }
            }
        }

        foreach (Cell attackCell in attackCells) {//Loop through the attack cells to check if the forest is in range, if it is not then set the forest cell to non-attackable
            if (attackCell.isForest){
                if (attackCell.FastMovesTo(currentUnit) > World.GetRules().maxDistanceFromForestToAttack){
                    GetCellInfo(infoList, attackCell).SetState(CellState.Inactive, false);
                    if (attackCell.currentUnit) Unit.SetLayerRecursively(attackCell.currentUnit.gameObject, 0, true);
                }
            }
        }

        return infoList;
    }

    static List<CellInfo> GetArcherAttackCells(Unit u)
    {
        return u.GetAttackCells();
    }

    public static void UpdateForestOutlines(Unit currentUnit){
        if (currentUnit.isAttacking){//For forest Layer detection
            //Get the movement cell of the unit
            Cell[] forestCellsInRange = currentUnit.currentCell ? currentUnit.currentCell.GetCellsInRange(currentUnit.movementRange + 2) : currentUnit.GetCurrentRange();

            foreach(Cell cell in forestCellsInRange){
                if (cell.isForest && cell.currentUnit){//If a cell is a forest and has a unit in it
                    if (cell.currentUnit.isAIControlled && cell.FastMovesTo(currentUnit) <= World.GetRules().maxDistanceFromForestToAttack){//Cell is AI controlled and is in Attack Distance (Depending on forest distance to attack)
                        Unit.SetLayerRecursively(cell.currentUnit.gameObject, 11, true);//Set the layer to enabled
                    } else if (cell.currentUnit.isAIControlled){//If the currentUnit is AI and out of range
                        Cell[] adjCells = cell.GetAdjacentCells();//Get its adjacent cells --- Will break if we increase maxDistanceFromForestToAttack
                        bool disableOutline = true;

                        foreach (Cell adjCell in adjCells){
                            if (adjCell.currentUnit && !adjCell.currentUnit.isAIControlled){//If any of its adjacent cells have a player unit in it
                                disableOutline = false;//Dont disable outline
                            }
                        }
                        Unit.SetLayerRecursively(cell.currentUnit.gameObject, disableOutline ? 0 : 11, true);//If disable outline is true disable, otherwise reset back to red
                    }
                }
            }
        }
    }


    #region Hover path


    public static void ClearHoverPath() {
        foreach(Cell cell in currentHoverPath) {
            cell.SetState(CellState.Active);
        }
        currentHoverPath.Clear();
    }

    #endregion

    #endregion

    // Start AI code setup

    public List<Cell> GetCellsInMoveRange(Unit unit){ //Returns a list of Cells that the given Unit can move to
                                                      //If we decide to make cells publicly readable, move this to AIParticipant
        List<Cell> output = new List<Cell>();

        foreach (Cell c in cells){
            //TODO: Ethan: move the range check to the Cell function
            if (CellInRange(unit, c)) output.Add(c);
        }
        return output;
    }

    public Cell GetCellAtPosition(Vector2Int coordinates){ //Returns the Cell (if one exists) at the given grid-space coordinates
                                                           //If we decide to make cells publicly readable, move this to AIParticipant
        foreach (Cell c in cells){
            if (c.coordinates == coordinates) return c;
        }
        return null;
    }

    bool CellInRange(Unit unit, Cell cell){ //Returns true if the given Cell is in range of the given Unit
        int xDifference = Mathf.Abs(cell.coordinates.x - unit.position.x);
        int yDifference = Mathf.Abs(cell.coordinates.y - unit.position.y);
        return xDifference + yDifference < unit.movePoints; //TODO: If this works correctly, make it a single line bc memory usage
    }

    public static bool Contains(Cell[] array, Cell element){
        foreach (Cell c in array){
            if (c == element) return true;
        }
        return false;
    }

    public static Cell GetRandomCell(){
        int x = Random.Range(0, grid.GetLength(0));
        int y = Random.Range(0, grid.GetLength(1));
        return grid[x,y];
    }

    public static CellInfo GetCellInfo(List<CellInfo> list, Cell cell){
        foreach (CellInfo i in list){
            if (i.cell == cell) return i;
        }
        return null;
    }

    public static int FastMovesBetween(int x, int y, Unit u){
        Vector2Int v = (new Vector2Int(x, y) - u.position);
        return Mathf.Abs(v.x) + Mathf.Abs(v.y);
    }
}
