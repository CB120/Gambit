using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIChris : AIParticipant
{
    
    /** Debugging **/
    //public Cell[] Path;
    //Cell moveCell = null;
    /** Properties **/


    /** Variables **/
    public int moveCounter = 0;
    //private int CellScore = 2;
    public List<Unit> PlayerUnits = new List<Unit>();
    public int movementTurns = 0;
    //public PriorityType AIState;
    public float AIMoveTime = 1f;
    bool lateStartRun = false;

    /** References **/
    public GameObject Castle;
    CastleAI castleAI;
    

    private void LateStart()
    {
        castleAI = Castle.GetComponent<CastleAI>();
        lateStartRun = true;
    }

    private void Update()
    {
        if (!lateStartRun)
        {
            LateStart();
            //Debug.Log("updating");
        }
    }

    #region Simple AI
    public override void StartTurn()
    {
        base.StartTurn(); //needed for movement to be enabled :)
        //turn logic here!
        moveCounter++;
        GetPlayers();
        foreach(Unit unit in units)
        {
            CheckState(unit);
        }
        InvokeRepeating("MoveTypeInvoke", 1f, AIMoveTime);
        
        //call base.EndTurn() when the turn's finished, doesn't have to be from this method but it'll disable all movement when done
        //Debug.Log(moveCounter);
        
    }

    void MoveTypeInvoke()
    {
        MoveType(units[movementTurns]);
        NewCameraMovement.JumpToCell(units[movementTurns].currentCell);
        movementTurns++;
        if(movementTurns == units.Count)
        {
            CancelInvoke();
            movementTurns = 0;
            base.EndTurn();
        }
    }

    AIProperties GetAIPropertiesOfUnit(Unit unit)
    {
        return unit.GetComponent<AIProperties>();
    }

    public AIProperties.PriorityType GetPriorityState(Unit unit)
    {

        AIProperties aiProperties = GetAIPropertiesOfUnit(unit);
        if (!aiProperties) return AIProperties.PriorityType.Invalid;
        
        return aiProperties.AIState;
    }

    void MoveType(Unit unit)
    {
        
        if (GetPriorityState(unit) == AIProperties.PriorityType.Setup)
        {
            MoveAIRandom(unit);
        }
        else if (GetPriorityState(unit) == AIProperties.PriorityType.Attack)
        {
            if (PlayerInRange(unit) == true)
            {
                MoveAIClosest(unit);
            } else
            {
                MoveAIRandom(unit);
            }
        }
        else if (GetPriorityState(unit) == AIProperties.PriorityType.Defend)
        {
            // MoveDefend();
        }
    }

    private void GetPlayers()
    {
        PlayerUnits = ParticipantManager.Temp_GetPlayer().GetUnits();
    }

    private void CheckState(Unit unit) // Should i just move everything from turn to here?? 
    {
            castleAI.PlayerInCastleRange();
            if (moveCounter < 3)
            {
                GetAIPropertiesOfUnit(unit).AIState = AIProperties.PriorityType.Setup;
            }
            else if (moveCounter >= 3 && castleAI.PlayerInCastleRange() == false)
            {
                GetAIPropertiesOfUnit(unit).AIState = AIProperties.PriorityType.Attack;
            }
            else if (moveCounter >=3 && castleAI.PlayerInCastleRange() == true)
            {
                GetAIPropertiesOfUnit(unit).AIState = AIProperties.PriorityType.Defend;
            }
    }

    public void MoveAIRandom(Unit unit) // Fuck it, do it all in one
    {
        Cell[] Path;
        Cell SimpleMoveCell;
        int RandomUnit = 0;
        for (int n = 0; n < PlayerUnits.Count; n++)
        {
            RandomUnit = Random.Range(0, PlayerUnits.Count - 1);    // Choose a random player unit to move towards
            Unit Temp = PlayerUnits[n];
            PlayerUnits[n] = PlayerUnits[RandomUnit];
            PlayerUnits[RandomUnit] = Temp;
        }
        Path = unit.currentCell.GetPathTo(PlayerUnits[RandomUnit].currentCell);
        SimpleMoveCell = Path[unit.movementRange-1];
                if (Path != null && SimpleMoveCell != null)
                {
                    if (Path.Length > 7)
                    {
                        MoveUnit(unit, SimpleMoveCell); 
                        Debug.Log("Moving!");
                    }
                    else
                    {
                        Debug.Log("Path too short!");
                        
                    }
            }
    }

    public void MoveAIClosest(Unit unit)
    {
        Cell AttackCell;
        Cell MoveCell;
        Cell[] GetAdjacent;
        GetAdjacent = unit.currentCell.GetAdjacentCells();

        int randomAdjacent = Random.Range(0, GetAdjacent.Length);

        MoveCell = GetAdjacent[randomAdjacent];
        AttackCell = PlayerToAttack(unit); // I dont think this is attacking lol    
                MoveUnit(unit, MoveCell);      
                unit.Attack(AttackCell);
    }
 
    public bool PlayerInRange(Unit unit)
    {
        Cell[] attackRange;
        attackRange = unit.currentCell.GetCellsInRange(unit.attackRange);
            foreach (Cell c in attackRange)
            {
                if (c.attackScore > 5)
                {
                return true;
                } else
            {
                return false;
            }
        }
        return false;
    }
   public Cell PlayerToAttack(Unit unit) // This is where having a cell value ON the 
    {
        Cell[] attackRange;
        Cell tempCell = null;

            attackRange = unit.currentCell.GetCellsInRange(unit.attackRange);
            foreach (Cell c in attackRange)
            {
                if (c.attackScore > 5)
                {
                    tempCell = c;
                    return tempCell;
                }
        }
        Debug.Log(tempCell);
        return tempCell;
    }

    #endregion

    #region Minimax preparation
    /*public override void StartTurn()
    {

        base.StartTurn(); //needed for movement to be enabled :)
        //turn logic here!
        GetScore();
        if (moveCell != null)
        {
            Debug.Log(units[0]);
            MoveUnit(units[0], moveCell);
        }
        //call base.EndTurn() when the turn's finished, doesn't have to be from this method but it'll disable all movement when done
        base.EndTurn();
    }*/
    /*
    public void GetScore() // Not using at the moment
    {
        Cell[] ScoreCells;
        PlayerUnits = ParticipantManager.Temp_GetPlayer().GetUnits();
        ScoreCells = PlayerUnits[0].currentCell.GetCellsInRange(8);
        foreach (Cell c in ScoreCells)
        {
            // Ignore cells containing a unit
            if (c.currentUnit) {
                continue;
            }
            if (c.cellScore > CellScore)
            {
                moveCell = c;
            }
        }
    }


    public Cell PlayCell(Node node, int Depth)
    {
        moveCell = null;
        Node root = new Node(node);
        int bestScore = MiniMax(root, Depth);
        foreach (Node child in root.childNodes)
        {
            if (child.UtilityValue == bestScore)
            {
                moveCell = child.moveCell;
            }
        }
        return moveCell;
    }

    private int MiniMax(Node root, int Depth) // No, it doesnt work yet. Need to assign values to the board, as well as get units to get all available cells
    {
        root.AddChild();
        int utilityValue = MaxValue(root, --Depth);

        return utilityValue;
    }

    public int MaxValue(Node Position, int depth)
    {
        if (Position.depth >= depth || Position.childNodes.Count == 0)
        {
            Position.UtilityValue = Position.Utility();
            return Position.UtilityValue;
        }
        int utilityValue = int.MinValue;
        int maxValue;

        for (int i = 0; i < Position.childNodes.Count; i++)
        {
            Node currentNode = Position.childNodes[i];
            maxValue = MinValue(currentNode, --depth);

            utilityValue = Mathf.Max(utilityValue, maxValue);
        }
        Position.UtilityValue = utilityValue;
        return utilityValue;
    }

    public int MinValue(Node Position, int depth)
    {
        if (Position.depth >= depth || Position.childNodes.Count == 0)
        {
            Position.UtilityValue = Position.Utility();
            return Position.UtilityValue;
        }
        int utilityValue = int.MaxValue;
        int minValue;

        for (int i = 0; i < Position.childNodes.Count; i++)
        {
            Node currentNode = Position.childNodes[i];
            minValue = MaxValue(currentNode, --depth);

            utilityValue = Mathf.Min(utilityValue, minValue);
        }
        Position.UtilityValue = utilityValue;
        return utilityValue;
    }


    // Redacted

    /*private int MiniMax(Unit unit, int depth, bool isMaximizing) // No, it doesnt work yet. Need to assign values to the board, as well as get units to get all available cells
    {
        int bestScore = 0;
        int minScore = 0;
        int finalScore = 0;

        Cell MoveCell = unit.currentCell;
        Cell[] ScoreCells;


        // pass the unit in, get its score, get the best etc
        if (isMaximizing)
        {
            ScoreCells = MoveCell.GetCellsInRange(6);
            bestScore = int.MinValue;
            foreach (Cell c in ScoreCells)
            {
                if (c.score > CellScore)
                {
                    CellScore = c.score;
                    bestScore = Mathf.Max(bestScore, CellScore);
                }
            }

            return bestScore;

        } /*else if (!isMaximizing)
        {
            minScore = int.MaxValue;

            for (int i = 0; i < position.Length; i++)
            {
                score = MiniMax(position, depth + 1, true);
                minScore = Mathf.Min(minScore, score);
            }
            return minScore;

        }

        if (depth == 3 /* or the AI dies)
        {
            finalScore = bestScore - minScore;
        }
            return finalScore;

    }
    */
    //check AIParticipant and Participant for methods carried over through inheritence

    #endregion
}

/** Variables **/
/*public List<Unit> PlayerUnits = new();                              // A list of the players units used for AI pathfinding and attacking
public Unit teamTarget = null;                                      // Used within the focus priority state, the AI teams unit shall all target this unit
public int moveCounter = 0;                                        // A move counter, used to setup the AI's position through the state machine
private int AIUnitCounter = 0;                                      // A counter that goes up for each unit the AI has. This is used during turns to slow down the AI's moves
public float TurnTime = 0;
[SerializeField] private int playersInForests = 0;
public Difficulty AIDifficulty;
private bool checkedForests = false;
public static float AITurnTime = 1f;                                // AIMoveTime is the amount of time between each AI Units move. It is used during the invoke call
bool lateStartRun = false;                                          // for some reason, the regular start function isnt behaving like it should. An alternative "late start" function
                                                                    // is being used instead. If it doesnt get called, this bool will still be false and the function will be retried.

public enum Difficulty
{
    Easy,
    Medium,
    Hard
}

// Patrol related variables
[Header("Patrol settings -- STILL IN DEVELOPMENT DO NOT USE")]
public List<Cell> PatrolPath = new();                               // An array to store cells the AI will follow during the patrol state
private int previousNode = 0;
private int nextNode = 1;

[Header("Enable / Disable whether AI can attack higher than them & whether the AI will attack forest cells")]
public bool NoLimits = false;                                  // Enable / Disable AI attacking things above them or hidden things in forests;
public bool ForestLevel = false;

// AI Spawning related variables
[Header("AI Respawning")]
public Cell[] SpawnPoints;                                          // Cells that AI units will spawn on
public Unit[] UnitsToSpawn;                                         // An array of units to spawn
public int SpawnRate = 2;                                           // The amount of turns before spawning occurs
private int spawningCount = 1;
public bool AISpawningInfinite = false;
public bool AISpawningLimited = false;

// AI State related variables
private bool allowSetup = true;
[Header("AI States. Hover over each for more info")]
[Tooltip("The Attack state moves towards a pre-set target to attack them. Will attack other units if they cross paths")]
public bool allowAttack = true;
[Tooltip("The defense state checks if the players / Catapult (if applicable) are in the castles pre-set 'defense range' (Set this in the castleAI script). If true, moves towards the castle and attacks the most vulnerable units OR the catapult")]
public bool allowDefence = true;
[Tooltip("The focus state sets the entire teams target to one player unit based on its Priority")]
public bool allowFocus;
[Tooltip("The Snipers state is strictly for Archers, it tells them to stay back and sit on hills to snipe enemies")]
public bool allowSnipers;
[Header("STILL IN DEVELOPMENT DO NOT USE")]
[Tooltip("***** DO NOT USE ***** The patrol state is still in development and may be buggy. To use it, choose your desired cells around spawn and tick the 'isPatrolNode' bool ")]
public bool allowPatrol;

/** References **//*
public GameObject Castle;                                           // A reference to the levels castle. This is used to call the defence radius function within CastleAI
CastleAI castleAI;                                                  // A reference to the CastleAI Script.
public Unit catapult;                                               // Reference to the catapult unit
public Animator catapultAnim;

private void LateStart()
{
    AIDifficulty = Difficulty.Easy;
    AITurnTime = TurnTime;
    foreach (GameObject unit in GameObject.FindGameObjectsWithTag("AI"))
    {
        Unit troop;                                                // Initialize the AI units list through tags
        troop = unit.GetComponent<Unit>();

        units.Add(troop);
    }
    castleAI = Castle.GetComponent<CastleAI>();
    lateStartRun = true;

    if (allowPatrol == true)
    {
        foreach (GameObject Cell in GameObject.FindGameObjectsWithTag("PatrolNode"))
        {
            Cell cell;                                             // Initialize the patrol node cells into a list through tags
            cell = Cell.GetComponent<Cell>();
            PatrolPath.Add(cell);
        }
    }
}

private void Update()
{
    if (!lateStartRun)
    {
        LateStart();                                              // If late start hasnt run, run it
    }
}

#region Initializing values & running start turn
private void GetPlayerUnits()
{
    PlayerUnits = ParticipantManager.Temp_GetPlayer().GetUnits();
}

AIProperties GetAIPropertiesOfUnit(Unit unit)
{
    return unit.GetComponent<AIProperties>();
}

public AIProperties.PriorityType GetPriorityState(Unit unit)
{

    AIProperties aiProperties = GetAIPropertiesOfUnit(unit);
    if (!aiProperties) return AIProperties.PriorityType.Invalid;

    return aiProperties.AIState;
}

private void SetTeamTarget()
{
    if (catapult != null)
    {
        teamTarget = catapult;
    }
    else if (PlayerUnits != null)
    {
        int maxScore = int.MinValue;
        foreach (Unit playerUnit in PlayerUnits)
        {
            if (playerUnit.currentCell != null)
            {
                if (playerUnit.currentCell.attackScore > maxScore)
                {
                    maxScore = playerUnit.currentCell.attackScore;
                    teamTarget = playerUnit;
                }
            }
        }
    }
}

private void UpdatePriorityState(Unit unit)
{
    if (moveCounter < 2 && allowSetup == true)                                            // For the first 2 moves, all AI Units will be in a 'setup' state.
    {
        GetAIPropertiesOfUnit(unit).AIState = AIProperties.PriorityType.Setup;
        if (allowPatrol == true)
        {
            GetAIPropertiesOfUnit(unit).hasPatrolled = false;                            // Set this to false to allow for the patrol state to come right after Setup
        }

    }
    else if (moveCounter >= 2 && castleAI.inRange == true && allowDefence == true && GetAIPropertiesOfUnit(unit).hasPatrolled == true)        // After 2 moves, if there is a player in the castles defence range, set the units priority to defend
    {
        GetAIPropertiesOfUnit(unit).AIState = AIProperties.PriorityType.Defend;
        GetAIPropertiesOfUnit(unit).targetUnit = setAITarget(unit);
    }
    else if (allowDefence == true && catapult != null && castleAI.CatapultInCastleRange(catapult) == true)                                                        // If the catapult is in the castles range, target it
    {
        GetAIPropertiesOfUnit(unit).AIState = AIProperties.PriorityType.Defend;
        GetAIPropertiesOfUnit(unit).targetUnit = catapult;
    }
    else if (moveCounter >= 2 && castleAI.inRange == false && allowAttack == true && GetAIPropertiesOfUnit(unit).hasPatrolled == true)       // After 2 moves, if there is no one in the castle's defence range, set the units priority to attack
    {
        GetAIPropertiesOfUnit(unit).AIState = AIProperties.PriorityType.Attack;
    }
    else if (castleAI.CatapultInCastleRange(catapult) == true && allowFocus == true && catapult != null)                                      // This state will be used for focusing the AI team on the catapult unit.
    {
        GetAIPropertiesOfUnit(unit).AIState = AIProperties.PriorityType.Focus;
        teamTarget = catapult;
    }
    else if (moveCounter >= 5 && allowFocus == true && teamTarget.currentCell.attackScore > 200 && units.Count <= 3 && allowFocus == true)    // This state will be used for focusing the AI team on one player unit.
    {
        GetAIPropertiesOfUnit(unit).AIState = AIProperties.PriorityType.Focus;
    }
    else if (allowPatrol == true && castleAI.inRange == false && PlayerUnitMoveInRange(unit) == null && GetAIPropertiesOfUnit(unit).hasPatrolled == false)    // Is it spelled Patrolled or patroled idk bruh ive looked at that word for way too long now
    {
        GetAIPropertiesOfUnit(unit).AIState = AIProperties.PriorityType.Patrol;
        GetAIPropertiesOfUnit(unit).hasPatrolled = true;
    }

    if (unit.unitType == Unit.UnitType.Archer && allowSnipers == true && MountainInRange(unit) != null && GetAIPropertiesOfUnit(unit).AIState != AIProperties.PriorityType.Setup)
    {
        GetAIPropertiesOfUnit(unit).AIState = AIProperties.PriorityType.Snipe;
    }

}

public override void StartTurn()
{
    base.StartTurn();                                                          // Starts the AI's turn, enabled AI movement.
    moveCounter++;
    if (AISpawningInfinite == true)
    {
        spawningCount++;
    }
    if (spawningCount >= SpawnRate && AISpawningInfinite == true)
    {
        SpawnAI();
        spawningCount = 0;
    }
    GetPlayerUnits();
    castleAI.PlayerInCastleRange();
    if (moveCounter > 3 && allowFocus == true && teamTarget == null)
    {
        SetTeamTarget();
    }
    if (ForestLevel == true && checkedForests == false)                                                // Make sure not all players are in forests
    {
        foreach (Unit player in PlayerUnits)
        {
            if (player.currentCell != null)
            {
                if (player.currentCell.isForest)
                {
                    playersInForests++;
                }
                checkedForests = true;
            }
        }
    }
    foreach (Unit unit in units)
    {
        GetAIPropertiesOfUnit(unit).hasAttacked = false;
        UpdatePriorityState(unit);                                            // Update the priority state
    }
    // Make it so if there is no playerInRange, move. Otherwise go straight to attack.
    InvokeRepeating("ActionInvoke", AITurnTime, AITurnTime * 2);                // We use an Invoke to space out the time between each AI Units moves. Since parametres cant be passed into
    JumpCameraToCurrentUnit();                                                // an Invoke, it is split into two functions (MoveInvoke(), Move().
}

void JumpCameraToCurrentUnit()
{
    if (AIUnitCounter >= units.Count) return;
    if (units[AIUnitCounter].currentCell != null)
    {
        NewCameraMovement.JumpToCell(units[AIUnitCounter].currentCell);          // Moves the camera to the unit that is being moved
    }
    //Debug.Log("Camera Called" + units[AIUnitCounter]);
}

void ActionInvoke()
{
    foreach (Unit unit in units)
    {
        unit.EnableUIObject(false);
    }
    units[AIUnitCounter].EnableUIObject(true);                              // Adds the UI above the current AI unit
    if (playersInForests == PlayerUnits.Count)
    {
        Debug.Log("All player units are in trees");
        GetAIPropertiesOfUnit(units[AIUnitCounter]).AIState = AIProperties.PriorityType.Setup;
        if (NearPlayerTree(units[AIUnitCounter]) != null)
        {
            units[AIUnitCounter].Attack(NearPlayerTree(units[AIUnitCounter]));
        }
    }
    if (PlayerUnitAttackInRange(units[AIUnitCounter]) != null || NearPlayerTree(units[AIUnitCounter]) != null)                    // Skips movement, just attacks
    {
        //Debug.Log("Movement skipped");
        JumpCameraToCurrentUnit();
        Attack(units[AIUnitCounter]);
        //Debug.Log(AIUnitCounter);
    }
    else
    {
        GetAIPropertiesOfUnit(units[AIUnitCounter]).sameCell = units[AIUnitCounter].currentCell;
        Move(units[AIUnitCounter]);                                             // Executes the units movement
        if (GetAIPropertiesOfUnit(units[AIUnitCounter]).sameCell == units[AIUnitCounter].currentCell)
        {
            //Debug.Log("The targeted unit was out of move range, so we'll try something else " + units[AIUnitCounter]);
            //Debug.Log("The" + units[AIUnitCounter] + " Will either try move to an adjacent cell of a player close to him or just move in a direction");
            if (PlayerUnitMoveInRange(units[AIUnitCounter]) != null)
            {
                //Debug.Log("Testing moving towards adjacent enemies " + units[AIUnitCounter]);
                //GetAIPropertiesOfUnit(units[AIUnitCounter]).targetUnit = PlayerUnitMoveInRange(units[AIUnitCounter]).currentUnit;
                Cell[] Adjacent = GetAIPropertiesOfUnit(units[AIUnitCounter]).targetUnit.currentCell.GetAdjacentCells();
                Cell MoveOneCell = Adjacent[Random.Range(0, Adjacent.Length)];
                MoveUnit(units[AIUnitCounter], MoveOneCell);
                if (GetAIPropertiesOfUnit(units[AIUnitCounter]).sameCell == units[AIUnitCounter].currentCell)        // STILL
                {
                    MoveAdjacent(units[AIUnitCounter]);
                    GetAIPropertiesOfUnit(units[AIUnitCounter]).targetUnit = PlayerUnits[Random.Range(0, PlayerUnits.Count)];
                }
                GetAIPropertiesOfUnit(units[AIUnitCounter]).sameCell = null;
            }
            else
            {
                Debug.Log("The " + units[AIUnitCounter] + " Could not find anything close to him that he could move to");
                MoveAdjacent(units[AIUnitCounter]);
                GetAIPropertiesOfUnit(units[AIUnitCounter]).targetUnit = PlayerUnits[Random.Range(0, PlayerUnits.Count)];
                Move(units[AIUnitCounter]);
                GetAIPropertiesOfUnit(units[AIUnitCounter]).sameCell = null;
            }

        }
        //Debug.Log(PlayerUnitInRange(units[AIUnitCounter]));

        if (PlayerUnitAttackInRange(units[AIUnitCounter]) != null)
        {
            Attack(units[AIUnitCounter]);                                       // Executes the Attack logic
        }
    }

    AIUnitCounter++;
    if (AIUnitCounter == units.Count)
    {
        Invoke("FinishMove", AITurnTime);                                    // Call to end the turn
    }
}

void FinishMove()
{
    units[AIUnitCounter - 1].EnableUIObject(false);
    CancelInvoke();
    AIUnitCounter = 0;
    base.EndTurn();                                                        // Ends the AI's turn, disables AI movement
    checkedForests = false;
    playersInForests = 0;
    if (catapult != null)
    {
        NewCameraMovement.JumpToCell(catapult.currentCell);
        MoveCatapult(catapult);
        catapult.movePoints = 20;                                          // Fuck right off honestly
        catapult.movementRange++;
    }
}
#endregion

#region Movement
void Move(Unit unit)
{
    if (GetAIPropertiesOfUnit(unit).targetUnit == null)              // We set a single target for each AI which they will focus on till its dead. This is held in the AIProperties script
    {
        GetAIPropertiesOfUnit(unit).targetUnit = PlayerUnits[Random.Range(0, PlayerUnits.Count)];
    }
    if (GetPriorityState(unit) == AIProperties.PriorityType.Setup)   // Using if-else for performance over a switch case.
    {
        MoveSetup(unit);
    }
    else if (GetPriorityState(unit) == AIProperties.PriorityType.Attack)
    {
        MoveAttack(unit);
    }
    else if (GetPriorityState(unit) == AIProperties.PriorityType.Defend)
    {
        MoveDefend(unit);
    }
    else if (GetPriorityState(unit) == AIProperties.PriorityType.Focus)
    {
        MoveFocus(unit);
    }
    else if (GetPriorityState(unit) == AIProperties.PriorityType.Patrol)
    {
        MovePatrol(unit);
    }
    else if (GetPriorityState(unit) == AIProperties.PriorityType.Snipe)
    {
        MoveSniper(unit);
    }
    else if (GetPriorityState(unit) == AIProperties.PriorityType.Invalid)
    {
        Debug.Log("Something went wrong with the AIProperties Priority type");
        MoveSetup(unit);
    }
    JumpCameraToCurrentUnit();
    Invoke("JumpCameraToCurrentUnit", AITurnTime);
}

public void MoveSetup(Unit unit)
{
    Cell[] Path;                                                     // Create an array to store a path for this turn
    Cell Destination;
    int RandomUnit = Random.Range(0, PlayerUnits.Count);             // Choose a random player unit to move towards

    Path = unit.currentCell.GetPathTo(PlayerUnits[RandomUnit].currentCell);     // Set the path to the random player
    if (Path.Length != 0)
    {
        if (Path.Length < unit.movementRange)                        // Check if the path distance is less than the possible movement range so that it doesn't overshoot the movement and jump out of array length
        {
            //Debug.Log("This is working");
            Destination = Path[Path.Length - 1];
        }
        else
        {

            Destination = Path[unit.movementRange / 2];              // Set the Units destination to be the Cell that is half of the Units movement range in the Path Array
        }
        if (Path != null && Destination != null)
        {
            MoveUnit(unit, Destination);                             // Move the unit to the destination cell
        }
        else
        {
            MoveAdjacent(unit);
            Debug.Log("Something is set to null");
        }
    }
    else
    {
        MoveAdjacent(unit);
    }
}

public void MoveAttack(Unit unit)
{
    //Debug.Log(unit);
    Cell[] Path;                                                      // Create an array to store a path for this turn
    Cell Destination;

    Path = unit.currentCell.GetPathTo(GetAIPropertiesOfUnit(unit).targetUnit.currentCell);     // Set the path to the targeted player
    if (Path.Length != 0)
    {
        if (Path.Length < unit.movementRange)                         // Check if the path distance is less than the possible movement range so that it doesn't overshoot the movement and jump out of array length
        {
            //Debug.Log("This is working");
            Destination = Path[Path.Length - 1];
        }
        else
        {
            Destination = Path[unit.movementRange - 1];               // Set the Units destination to be the Cell within the units movement range in the Array -- **********This is causing an index out of range***********
        }
        if (Path != null && Destination != null)
        {
            MoveUnit(unit, Destination);                              // Move the unit to the destination cell
        }
        else
        {
            MoveAdjacent(unit);
            Debug.Log("Something is set to null");
        }
    }
    else
    {
        MoveAdjacent(unit);
    }
}

public void MoveFocus(Unit unit)
{
    Cell[] Path;                                                    // Create an array to store a path for this turn
    Cell Destination;
    if (teamTarget == null)                                     // Set a single player unit for the whole AI team to target
    {
        // **---- In future, please set this to use cell scores, find the highest value target OR the lowest value target. Set it based on difficulty. To be done soon -----** //
        teamTarget = PlayerUnits[Random.Range(0, PlayerUnits.Count)];
    }

    Path = unit.currentCell.GetPathTo(teamTarget.currentCell);      // Set the path to the focused player
    if (Path.Length != 0)
    {
        if (Path.Length < unit.movementRange)                       // Check if the path distance is less than the possible movement range so that it doesn't overshoot the movement and jump out of array length
        {
            //Debug.Log("This is working");
            Destination = Path[Path.Length - 1];
        }
        else
        {
            Destination = Path[unit.movementRange - 1];             // Set the Units destination to be the Cell within the units movement range in the Array
        }
        if (Path != null && Destination != null)
        {
            MoveUnit(unit, Destination);                            // Move the unit to the destination cell
                                                                    // Maybe change this to in the range of the unit
            Debug.Log(unit + "Destination.coordinates" + "This works");
        }
        else
        {
            MoveAdjacent(unit);
            Debug.Log("Something is set to null");
        }
    }
    else
    {
        MoveAdjacent(unit);
    }
}

public void MoveDefend(Unit unit)
{
    Cell[] Path;                                                    // Create an array to store a path for this turn
    Cell Destination;
    if (castleAI.DefenceRange != null)                              // CastleAI has an array of its surrounding cells.
    {
        castleAI.DrawDefenceRadius();
    }

    Path = unit.currentCell.GetPathTo(castleAI.DefenceRange[Random.Range(0, castleAI.DefenceRange.Length)]);     // Sets the path back to the castles range
    if (Path.Length != 0)
    {
        if (Path.Length < unit.movementRange)                       // Check if the path distance is less than the possible movement range so that it doesn't overshoot the movement and jump out of array length
        {
            //Debug.Log("This is working");
            Destination = Path[Path.Length - 1];
        }
        else
        {
            Destination = Path[unit.movementRange - 1];             // Set the Units destination to be the Cell within the units movement range in the Array
        }
        if (Path != null && Destination != null)
        {
            MoveUnit(unit, Destination);                            // Move the unit to the destination cell
        }
        else
        {
            MoveAdjacent(unit);
            Debug.Log("Something is set to null");
        }
    }
}

private void MovePatrol(Unit unit)                                   // Currently for testing purposes
{
    Cell[] Path;
    Cell Destination;
    if (PatrolPath.Count != 0)
    {
        if (previousNode >= PatrolPath.Count)
        {
            previousNode = 0;
            nextNode = previousNode + 1;
        }
        if (unit.currentCell != PatrolPath[previousNode] || unit.currentCell != PatrolPath[nextNode])
        {
            Path = unit.currentCell.GetPathTo(PatrolPath[0]);
        }
        else
        {
            Path = PatrolPath[previousNode].GetPathTo(PatrolPath[nextNode]);
        }
        if (Path.Length != 0)
        {
            if (Path.Length < unit.movementRange)                    // Check if the path distance is less than the possible movement range so that it doesn't overshoot the movement and jump out of array length
            {
                Destination = Path[Path.Length - 1];
            }
            else
            {
                Destination = Path[unit.movementRange - 1];          // Set the Units destination to be the Cell within the units movement range in the Array
            }
            if (Path != null && Destination != null)
            {
                MoveUnit(unit, Destination);                         // Move the unit to the destination cell
                previousNode++;
                nextNode++;
            }
        }
    }
}

private void MoveSniper(Unit unit)                                   // Prioritizes archers moving to high points
{
    if (MountainInRange(unit) != null)
    {
        Cell[] possiblePaths = MountainInRange(unit).ToArray();
        Cell Destination;
        Cell[] Path;
        if (possiblePaths.Length != 0)
        {
            Path = unit.currentCell.GetPathTo(possiblePaths[Random.Range(0, possiblePaths.Length)]);
            if (Path.Length != 0)
            {
                if (Path.Length < unit.movementRange)                       // Check if the path distance is less than the possible movement range so that it doesn't overshoot the movement and jump out of array length
                {
                    Destination = Path[Path.Length - 1];
                }
                else
                {
                    Destination = Path[unit.movementRange - 1];             // Set the Units destination to be the Cell within the units movement range in the Array
                }

                if (Path != null && Destination != null)
                {
                    MoveUnit(unit, Destination);                            // Move the unit to the destination cell
                }
                else                                                        // Add a check to see if the units at the highest point? if so do nothing
                {
                    MoveAdjacent(unit);
                    Debug.Log("Something is set to null");
                }
            }
        }
        else
        {
            Debug.Log("Reached a high point");
        }
    }
}
private void MoveAdjacent(Unit unit)
{
    Cell[] AdjacentCells;
    Cell MoveOneCell;
    AdjacentCells = unit.currentCell.GetAdjacentCells();
    MoveOneCell = AdjacentCells[Random.Range(0, AdjacentCells.Length)];

    MoveUnit(unit, MoveOneCell);
}

public void MoveCatapult(Unit Catapult)
{
    Cell[] Path;                                                    // Create an array to store a path for this turn
    Cell Destination;

    if (Catapult.currentCell != null && Catapult.currentCell != castleAI.castleCell)                               // As the catapult is an AI unit but on the players team, we need to reset its cellScores manually.
    {
        foreach (Cell cell in Catapult.currentCell.GetAdjacentCells())
        {
            CellScore.ResetMoveScore(cell);
        }
        CellScore.ResetAttackScore(Catapult.currentCell);
    }

    Path = Catapult.currentCell.GetPathTo(castleAI.castleCell);     // Sets the path back to the castles range

    if (Catapult.currentCell == castleAI.castleCell || Path.Length <= 4)
    {
        Debug.Log("Attack Time");
        Destination = null;
        CatapultAttack(Catapult);                                       // If you're within range of the castle, start attacking it
    }
    else if (Path.Length < Catapult.movementRange)                  // Check if the path distance is less than the possible movement range so that it doesn't overshoot the movement and jump out of array length
    {
        //Debug.Log("This is working");
        Destination = Path[Path.Length - 1];
    }
    else
    {
        Destination = Path[Catapult.movementRange - 1];             // Set the Units destination to be the Cell within the units movement range in the Array
    }
    if (Path != null && Destination != null)
    {
        //Debug.Log(catapult.currentCell.coordinates);
        MoveUnit(Catapult, Destination);                            // Move the unit to the destination cell
        catapult.currentCell.attackScore = 1000;
    }
    else
    {
        Debug.Log("You have reached the castle, no more movement. Or somethings null idk");
    }
}
#endregion

#region inRange Checks
public Cell PlayerUnitAttackInRange(Unit unit)
{
    Cell playerInRange = null;
    Cell[] attackRange;                                            // Cells within the units attack range
    if (unit.currentCell != null)
    {
        attackRange = unit.currentCell.GetCellsInRange(unit.attackRange);
        foreach (Cell c in attackRange)
        {
            if (NoLimits == false)
            {
                if (c.attackScore > 50 && c.height <= unit.currentCell.height && c.isForest == false)// Each cell is given an attack score depending on the unit that is on it
                {                                                         // If the score is greater than 5, a unit is present (Default cellScore = 1, minimum unit cellScore = 100
                    playerInRange = c;
                }
            }
            else if (NoLimits == true)
            {
                if (c.attackScore > 50)                                    // Each cell is given an attack score depending on the unit that is on it
                {                                                         // If the score is greater than 5, a unit is present (Default cellScore = 1, minimum unit cellScore = 100
                    playerInRange = c;
                }
            }
            if (playerInRange == null && c.currentUnit != null && c.height <= unit.currentCell.height)
            {
                foreach (Unit u in PlayerUnits)
                {
                    if (c.currentUnit == u && c.isForest == false)
                    {
                        Debug.Log(u);
                        playerInRange = c;
                    }
                }
            }
        }
    }
    return playerInRange;
}

public Cell PlayerUnitMoveInRange(Unit unit)
{
    Cell playerInRange = null;
    Cell[] moveRange;                                             // Cells within the units move range
    if (unit.currentCell != null)
    {
        moveRange = unit.currentCell.GetCellsInRange(unit.movementRange);
        foreach (Cell c in moveRange)
        {
            if (NoLimits == false)
            {
                if (c.cellScore > 20 && c.isForest == false)              // Each cell is given a score depending on the unit that is on it
                {                                                         // If the score is greater than 5, a unit is present (Default cellScore = 1, minimum unit cellScore = 100
                    playerInRange = c;
                }
            }
            if (NoLimits == true)
            {
                if (c.cellScore > 20)                                     // Each cell is given a score depending on the unit that is on it
                {                                                         // If the score is greater than 5, a unit is present (Default cellScore = 1, minimum unit cellScore = 100
                    playerInRange = c;
                }
            }
        }
    }
    return playerInRange;
}

private Unit setAITarget(Unit unit)
{
    Unit targetUnit = null;
    Cell[] defenseRange;
    if (unit.currentCell != null)
    {
        defenseRange = unit.currentCell.GetCellsInRange(8);
        foreach (Cell c in defenseRange)
        {
            if (c.attackScore > 50)                                    // Each cell is given an attack score depending on the unit that is on it
            {                                                         // If the score is greater than 5, a unit is present (Default cellScore = 1, minimum unit cellScore = 100
                targetUnit = c.currentUnit;
            }
        }
    }
    return targetUnit;
}

public Cell AITargetInRange(Unit unit)
{
    Cell TargetCell = null;
    Cell[] AttackRange;                                           // Cells within the units attack range
    if (unit.currentCell != null)
    {
        AttackRange = unit.currentCell.GetCellsInRange(unit.attackRange);
        foreach (Cell c in AttackRange)
        {
            if (c.currentUnit != null)
            {
                if (NoLimits == false)
                {
                    if (c.attackScore == GetAIPropertiesOfUnit(unit).targetUnit.currentCell.attackScore || c.currentUnit == GetAIPropertiesOfUnit(unit).targetUnit && c.height <= unit.currentCell.height && c.isForest == false)
                    {                                                            // Checks to see if the units individually assigned target is in range.
                        TargetCell = c;                                          // If it is, return the cell to be attacked.
                    }
                }
                else if (NoLimits == true)
                {
                    if (c.attackScore == GetAIPropertiesOfUnit(unit).targetUnit.currentCell.attackScore || c.currentUnit == GetAIPropertiesOfUnit(unit).targetUnit)
                    {                                                            // Checks to see if the units individually assigned target is in range.
                        TargetCell = c;                                          // If it is, return the cell to be attacked.
                    }
                }
            }
            if (TargetCell == null && c.currentUnit != null && c.height <= unit.currentCell.height)
            {
                foreach (Unit u in PlayerUnits)
                {
                    if (c.currentUnit == u)
                    {
                        Debug.Log(u);
                        TargetCell = c;
                    }
                }
            }
        }
        if (TargetCell != null)
        {
            //Debug.Log(TargetCell.coordinates);
        }
    }
    return TargetCell;

}
public Cell AITeamTargetInRange(Unit unit)
{
    Cell TargetCell = null;
    Cell[] AttackRange;
    if (unit.currentCell != null)
    {
        AttackRange = unit.currentCell.GetCellsInRange(unit.attackRange);
        foreach (Cell c in AttackRange)
        {
            if (catapult != null)
            {
                if (castleAI.CatapultInCastleRange(catapult) == true)
                {
                    if (c.currentUnit == catapult)
                    {
                        TargetCell = c;
                    }
                }
            }
            else if (NoLimits == false)
            {
                if (c.attackScore == teamTarget.currentCell.attackScore && c.height <= unit.currentCell.height && c.isForest == false)
                {
                    TargetCell = c;
                }
            }
            else if (NoLimits == true)
            {
                if (c.attackScore == teamTarget.currentCell.attackScore)
                {
                    TargetCell = c;
                }
            }
        }
    }
    return TargetCell;
}

public List<Cell> MountainInRange(Unit unit)
{
    List<Cell> highCells = new List<Cell>();
    if (unit.currentCell != null)
    {
        Cell mountainCell = unit.currentCell;
        Cell maxCell = null;
        Cell[] mapCells;
        mapCells = unit.currentCell.GetCellsInRange(70);

        if (mapCells != null)
        {
            foreach (Cell c in mapCells)
            {
                if (c.height > unit.currentCell.height)
                {
                    maxCell = c;
                    if (maxCell.height > mountainCell.height)
                    {
                        highCells.Add(maxCell);
                        mountainCell = maxCell;
                    }
                }
            }
        }
    }
    //Debug.log(highCells.Count);
    return highCells;
}

public Cell NearPlayerTree(Unit unit)
{
    Cell treeCell = null;
    if (unit.currentCell != null)
    {
        Cell[] surroundingCells = unit.currentCell.GetCellsInRange(2);     // 2 is the range in which players can see in trees

        foreach (Cell c in surroundingCells)
        {
            if (c.attackScore > 50 && c.isForest == true && c.height <= unit.currentCell.height)
            {
                treeCell = c;
            }
        }
    }
    //Debug.Log(treeCell);
    return treeCell;
}
#endregion

#region Attacking logic
public void Attack(Unit unit)                                         // Based on the units state, executes an attack style
{
    if (NearPlayerTree(unit) != null && ForestLevel == true)
    {
        unit.Attack(NearPlayerTree(unit));
        GetAIPropertiesOfUnit(unit).hasAttacked = true;
    }
    if (GetAIPropertiesOfUnit(unit).hasAttacked == false)
    {
        if (GetPriorityState(unit) == AIProperties.PriorityType.Attack)
        {
            if (GetAIPropertiesOfUnit(unit).targetUnit == null)              // We set a single target for each AI which they will focus on till its dead. This is held in the AIProperties script
            {
                GetAIPropertiesOfUnit(unit).targetUnit = PlayerUnits[Random.Range(0, PlayerUnits.Count)];
            }
            if (AITargetInRange(unit) != null)
            {
                unit.Attack(AITargetInRange(unit));
                Debug.Log("Target is in range");
                GetAIPropertiesOfUnit(unit).hasAttacked = true;
            }
            if (PlayerUnitAttackInRange(unit) != null && GetAIPropertiesOfUnit(unit).hasAttacked == false)                     // If the target unit is not in range
            {
                UntargetedAttack(unit);
                GetAIPropertiesOfUnit(unit).hasAttacked = true;
            }
        }
        else if (GetPriorityState(unit) == AIProperties.PriorityType.Defend)
        {
            Debug.Log("Defensive Attack pre attack check");
            if (GetAIPropertiesOfUnit(unit).hasAttacked == false)
            {
                DefensiveAttack(unit);
                Debug.Log("Defensive Attack");
                GetAIPropertiesOfUnit(unit).hasAttacked = true;
            }
            if (PlayerUnitAttackInRange(unit) != null && GetAIPropertiesOfUnit(unit).hasAttacked == false)                     // If the target unit is not in range
            {
                UntargetedAttack(unit);
                GetAIPropertiesOfUnit(unit).hasAttacked = true;
            }

        }
        else if (GetPriorityState(unit) == AIProperties.PriorityType.Focus)
        {
            if (AITeamTargetInRange(unit) != null)
            {
                unit.Attack(AITeamTargetInRange(unit));
                Debug.Log("Team Target is in range");
                GetAIPropertiesOfUnit(unit).hasAttacked = true;
            }
            if (PlayerUnitAttackInRange(unit) != null && GetAIPropertiesOfUnit(unit).hasAttacked == false)                     // If the target unit is not in range
            {
                UntargetedAttack(unit);
                GetAIPropertiesOfUnit(unit).hasAttacked = true;
            }
        }
        else if (GetPriorityState(unit) == AIProperties.PriorityType.Snipe)
        {
            UntargetedAttack(unit);                                                                                     // Add an attack to target the highest priority target
            GetAIPropertiesOfUnit(unit).hasAttacked = true;
        }
        //GetAIPropertiesOfUnit(unit).hasAttacked = true;
    }
}

private void UntargetedAttack(Unit unit)
{
    Debug.Log("Untargeted Attack" + unit);
    Cell CellToAttack = null;
    if (PlayerUnitAttackInRange(unit) != null)
    {
        CellToAttack = PlayerUnitAttackInRange(unit);
    }
    if (CellToAttack != null)
    {
        //GetAIPropertiesOfUnit(unit).targetUnit = CellToAttack.currentUnit;
        unit.Attack(CellToAttack);
        GetAIPropertiesOfUnit(unit).hasAttacked = true;
    }
}
private void DefensiveAttack(Unit unit)
{
    // Pretty much want this to be the player who is most vulnerable to be attacked first
    Cell[] UnitAttackRange;
    Cell CellToAttack = null;
    UnitAttackRange = unit.currentCell.GetCellsInRange(unit.attackRange);
    float lowestHealth = 250;
    foreach (Cell c in UnitAttackRange)
    {
        if (catapult != null)
        {
            if (castleAI.CatapultInCastleRange(catapult) == true)
            {
                if (c.currentUnit == catapult)
                {
                    CellToAttack = c;
                }
            }
        }
        else if (c.attackScore > 50)
        {
            Debug.Log("its doing the else statement");
            if (c.currentUnit.health < lowestHealth)
            {
                CellToAttack = c;
                lowestHealth = c.currentUnit.health;
            }
        }
    }
    if (CellToAttack != null)
    {
        if (catapult != null)
        {
            GetAIPropertiesOfUnit(unit).targetUnit = catapult;
        }
        else
        {
            //GetAIPropertiesOfUnit(unit).targetUnit = CellToAttack.currentUnit;
        }
        unit.Attack(CellToAttack);
        GetAIPropertiesOfUnit(unit).hasAttacked = true;
    }
}

private void CatapultAttack(Unit Catapult)
{
    castleAI.Attack(Catapult.damage);
    if (catapultAnim != null)
    {
        catapultAnim.SetBool("AttackState", true);
    }
}
#endregion

#region Spawning new AI Units
public void SpawnAI()
{
    if (AISpawningInfinite)
    {
        int randomCell = Random.Range(0, SpawnPoints.Length); //Get a random cell in the predefined list
        Unit unitToSpawn = GetRandomUnit();    //Unit to spawn
        int i = 0;
        bool foundAvailableCell = false;

        //Find an available cell
        while (foundAvailableCell && i != 10)
        {
            if (SpawnPoints[randomCell].currentUnit == null) //If the random cell doesnt currently have a unit
                foundAvailableCell = true;//Break the while loop
            else
                randomCell = Random.Range(0, SpawnPoints.Length); //Find a different cell

            i++; // Check to make sure that we don't get stuck in an infinite loop, will never be used in non-testing environment but safe to have eh.
        }
        //Instantiate the prefab
        Unit prefab = Instantiate(unitToSpawn, SpawnPoints[randomCell].transform.position, Quaternion.identity, transform);
        prefab.startCell = SpawnPoints[randomCell];//set the start cell of the unit
        prefab.currentCell = prefab.startCell;
        SpawnPoints[randomCell].currentUnit = prefab;//set the currentUnit of the cell it spawns at
        units.Add(prefab);//add to units list
    }
    //else if (AISpawningLimited)
    //{

    // }
}

private Unit GetRandomUnit()
{
    float randNum = Random.value;
    if (randNum > 0.75) //25%
        return UnitsToSpawn[0];
    else if (randNum > 0.5) //25%
        return UnitsToSpawn[1];
    else if (randNum > 0.25)//15%
        return UnitsToSpawn[2];
    else if (randNum > 0.1)//10%
        return UnitsToSpawn[3];
    return UnitsToSpawn[0];
}
}
#endregion
*/