using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Saving;
/************************************************************************************************************************
 *****                            F I V E   G U Y S  + +                      G D S 1                               *****
 ************************************************************************************************************************
 *                                                                                                                      *
 *                 Project Name : Gambit                                                                                *
 *                                                                                                                      *
 *                    File Name : AIController.cs                                                                       *
 *                                                                                                                      *
 *                   Programmer : Christian Burgio                                                                      *
 *                                                                                                                      *
 *                   Start Date : April 4, 2022                                                                         *
 *                                                                                                                      *
 *                  Last Update : June 27, 2022                                                                         *
 *                                                                                                                      *
 **--------------------------------------------------------------------------------------------------------------------**
 *  Functions:                                                                                                          *
 *      Init::LateStart() -- Alternative Start function. See function for more details                                  *
 *      Init::GetPlayerUnits() -- Returns an array of the players units                                                 *
 *      Init::GetAIPropertiesOfUnit(Unit) -- References the AI Units properties script                                  *
 *      Init::GetPriorityState(Unit) -- Returns the AI Units AIState (Priority enum)                                    *
 *	    Init::GetAIOutpost(Outpost) -- Returns the outpost controller component		                                    *
 *      Init::SetTeamTarget() -- Based on score, sets the teams focus unit                                              *
 * /                                                                                                                  / *
 *      Difficulty/Priority::UpdateDifficulty(Unit) -- Updates the difficulty & states of the AI                        *
 *	    Difficulty/Priority::EasyPriorityStates(Unit) -- Controls the AI on Easy difficulty                             *
 *	    Difficulty/Priority::MediumPriorityStates(Unit) -- Controls the AI on Medium difficulty                         *
 *  	Difficulty/Priority::HardPriorityStates(Unit) -- Controls the AI on Hard difficulty                             *
 * /                                                                                                                  / *
 *      Checks::PlayerUnitAttackInRange(Unit) -- Returns a cell if a player unit is in attack range                     *
 *      Checks::PlayerUnitMoveInRange(Unit) -- Returns a cell if a player unit is in move range                         *
 *	    Checks::setAITarget(Unit) -- Sets the target unit for the AI based on attack score	                            *
 *	    Checks::AITargetInRange(Unit) -- Returns the cell of the AI Units target if in range                            *
 *      Checks::AITeamTargetInRange(Unit) -- Returns the cell of the AI Teams target if in range                        *
 *      Checks::MountainInRange(Unit) -- Returns the highest cell in a range of 70                                      *
 *      Checks::NearPlayerTree(Unit) -- Returns a unit in a tree (hidden)                                               *
 * 	    Checks::UpdateUnitScores() -- Used in conjunction with dynamic difficulty                                       *
 * 	    Checks::FindClosestTarget(Unit) -- Path finding to the closest player from the player to set its target         *
 * 	    Checks::FindClosestTargetPath(Unit) -- Returns the length of the path to the closest player                     *
 * /                                                                                                                  / *
 *      TurnLogic::StartTurn() -- Handles turn logic for the AI.                                                        *
 * 	    TurnLogic::MoveAndAttackInvoke -- Delegates AI movement / attack functions based on various conditions          *
 *	    TurnLogic::FinishMove() -- Called at the end of the AI's turn, moves the Catapult && resets various variables   *
 *	    TurnLogic::JumpCameraToCurrentUnit() -- Handles camera movement for each AI units move                          *
 * /                                                                                                                  / *
 *      Movement::Move(Unit) -- Handles unit movement based on priority state                                           *
 *      Movement::MoveSetup(Unit) -- Handles unit 'setup' priority movement                                             *
 *      Movement::MovePatrol(Unit) -- Handles unit 'Patrol' priority movement  *** Currently in development ***         *
 *	    Movement::MoveAdjacent(Unit) -- If the AI is stuck in the same spot, move to an adjacent cell                   *
 *	    Movement::MoveCatapult(Catapult) -- Handles the catapults movement                                              *
 *	    Movement::MoveSameCell(Unit) -- Checks various special conditions to try and move the AI stuck in a tough spot  *
 * /                                                                                                                  / *
 *      Attacking::Attack(Unit) -- Handles unit attacks based on priority state                                         *
 *      Attacking::CatapultAttack(Catapult) -- Attacks the castle (As the catapult)                                     *
 * - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */
public enum Difficulty
{
    Tutorial,
    Easy,
    Medium,
    Hard
}

public class AIController : AIParticipant
{
    #region Variables & References
    /** Variables **/
    List<Unit> PlayerUnits = new();                                     // A list of the players units used for AI pathfinding and attacking
    int moveCounter = 0;                                                // A move counter, used to setup the AI's position through the state machine
    int AIUnitCounter = 0;                                              // A counter that goes up for each unit the AI has. This is used during turns to slow down the AI's moves
    int playersInForests = 0;
    int unitsLost = 0;                                                  // Move this to a file that is held on do not destroy on load, with other statistics
    [HideInInspector] public bool unitSpawnedThisTurn = false;          // Added by Ethan - Set to true by AIOutpostController when a Unit's been spawned
    bool checkedForests = false;
    bool lateStartRun = false;                                          // for some reason, the regular start function isnt behaving like it should. An alternative "late start" function
                                                                        // is being used instead. If it doesnt get called, this bool will still be false and the function will be retried.
    public static float AITurnTime = 1f;                                // AIMoveTime is the amount of time between each AI Units move. It is used during the invoke call
    int moveDistance = 1;

    public Unit teamTarget = null;                                      // Used within the focus priority state, the AI teams unit shall all target this unit
    public float TurnTime = 0;                                          // Controls the AITurnTime within the inspector
    public bool ForestLevel = false;                                    // Checks whether the level has forest instances

    // Difficulty
    private Unit TrickleUnit;                                           // For easy mode
    public static int AIUnitScores = 0;
    public static int PlayerUnitScores = 0;
    [Header("Difficulty")]
    public Difficulty AIDifficulty;
    public bool TutorialLevel = false;
    //public bool DynamicDifficulty = true;

    // Patrol related variables
    [Header("Patrol settings -- STILL IN DEVELOPMENT DO NOT USE")]
    public List<Cell> PatrolPath = new();                               // An array to store cells the AI will follow during the patrol state
    //int previousNode = 0;
    //int nextNode = 1;

    // Developer tool variables
    [Header("Enable / Disable whether AI can attack higher than them & whether the AI will attack forest cells")]
    public bool NoLimits = false;                                       // Enable / Disable AI attacking things above them or hidden things in forests

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
    [Tooltip("The Trickle state is used exlusively in the Easy difficulty. One unit at a time engages the player")]
    public bool allowTrickle;
    [Header("STILL IN DEVELOPMENT DO NOT USE")]
    [Tooltip("***** DO NOT USE ***** The patrol state is still in development and may be buggy. To use it, choose your desired cells around spawn and tick the 'isPatrolNode' bool ")]
    public bool allowPatrol = false;

    /** References **/
    [Header("References")]
    public GameObject Castle;                         // A reference to the levels castle. This is used to call the defence radius function within CastleAI
    CastleAI castleAI;                                                  // A reference to the CastleAI Script.
    public GameObject Catapult;                       // Reference to the catapult gameObject
    Unit CatapultUnit;                                                  // Reference to the CatapultUnit Script
    Animator CatapultAnim;                                              // Reference to the catapult Animator
    [SerializeField] private GameObject OutpostController;              // Outpost reference
                                                                        //AIOutpostController outpostController;

    [Header("Procedural Generation")]
    [SerializeField] private bool usingProcGen = false;
    #endregion

    #region Start, Update & Initialization

    public void LateStart()
    {
        AITurnTime = TurnTime;
         if(!usingProcGen)
            Castle = GameObject.FindGameObjectWithTag("Castle");
        Catapult = GameObject.FindGameObjectWithTag("Catapult");
        OutpostController = GameObject.FindGameObjectWithTag("OutpostController");
        foreach (GameObject unit in GameObject.FindGameObjectsWithTag("AI"))
        {
            Unit troop;                                                // Initialize the AI units list through tags
            troop = unit.GetComponent<Unit>();

            units.Add(troop);
        }
        if (Castle != null)                                            // Initialize the Castle script
        {
            castleAI = Castle.GetComponent<CastleAI>();
        }
        if (Catapult != null)                                           // Initialize the catapult components if one exists in the scene
        {
            CatapultUnit = Catapult.GetComponent<Unit>();
            CatapultAnim = Catapult.GetComponent<Animator>();
        }

        if (allowPatrol == true)
        {
            foreach (GameObject Cell in GameObject.FindGameObjectsWithTag("PatrolNode"))
            {
                Cell cell;                                             // Initialize the patrol node cells into a list through tags
                cell = Cell.GetComponent<Cell>();
                PatrolPath.Add(cell);
            }
        }
        UIManager.SetEnemyObjectiveUnits(units.Count);
        lateStartRun = true;                                           // Once start has run, tick it off

    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.F)){ Debug.LogError(AIDifficulty); Debug.Log(ParticipantManager.GetCurrentParticipant()); }
        if (usingProcGen) return;
        if (!lateStartRun)
        {
            LateStart();                                              // If late start hasnt run, run it
        }
    }

    private void GetPlayerUnits()
    {
        PlayerUnits = ParticipantManager.Temp_GetPlayer().GetUnits();
    }

    AIProperties GetAIPropertiesOfUnit(Unit unit)
    {
        return unit.GetComponent<AIProperties>();
    }

    private AIProperties.PriorityType GetPriorityState(Unit unit)
    {

        AIProperties aiProperties = GetAIPropertiesOfUnit(unit);
        if (!aiProperties) return AIProperties.PriorityType.Invalid;

        return aiProperties.AIState;
    }

    AIOutpostController GetAIOutpost(GameObject Outpost)
    {
        return Outpost.GetComponent<AIOutpostController>();
    }

    private void SetTeamTarget()
    {
        if (CatapultUnit != null)
        {
            teamTarget = CatapultUnit;
        }
        else if (PlayerUnits != null)
        {
            if (AIDifficulty == Difficulty.Hard || AIDifficulty == Difficulty.Medium)       // If the AI's difficulty is set to Hard or Medium, the teams target will be the highest value player
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
            else if (AIDifficulty == Difficulty.Easy)                                      // If the AI's difficulty is set to Easy, the teams target will be the lowest value player
            {
                int minScore = int.MaxValue;
                foreach (Unit playerUnit in PlayerUnits)
                {
                    if (playerUnit.currentCell != null)
                    {
                        if (playerUnit.currentCell.attackScore < minScore)
                        {
                            minScore = playerUnit.currentCell.attackScore;
                            teamTarget = playerUnit;
                        }
                    }
                }
            }
        }
    }
    #endregion

    #region Difficulty & Priority States
    private void UpdateDifficulty(Unit unit)
    {
        if (SavedData.GameData.dynamicDifficultyEnabled)
        {
            Debug.Log("ON");
            Debug.Log(GameManager.UpdateAIDifficulty());
            if (GameManager.UpdateAIDifficulty() <= -10)
            {
                AIDifficulty = Difficulty.Easy;
                allowFocus = true;
                allowTrickle = true;
                allowAttack = false;
                allowDefence = false;
                allowSnipers = false;
                Debug.Log("easy");
            }
            else if (GameManager.UpdateAIDifficulty() > -10 && GameManager.UpdateAIDifficulty() <= 10)
            {
                AIDifficulty = Difficulty.Medium;
                allowFocus = true;
                allowTrickle = false;
                allowAttack = true;
                allowDefence = true;
                allowSnipers = false;
                Debug.Log("medium");
            }
            else if (GameManager.UpdateAIDifficulty() >= 11)
            {
                AIDifficulty = Difficulty.Hard;
                allowFocus = true;
                allowTrickle = false;
                allowAttack = true;
                allowDefence = true;
                allowSnipers = true;
                Debug.Log("Hard");
            }
        } else if (SavedData.GameData.dynamicDifficultyEnabled)
        {
            switch (SavedData.GameData.difficulty)
            {
                case Difficulty.Easy: AIDifficulty = Difficulty.Easy;
                    allowFocus = true;
                    allowTrickle = true;
                    allowAttack = false;
                    allowDefence = false;
                    allowSnipers = false;
                    Debug.Log("easy");
                    break;
                case Difficulty.Medium: AIDifficulty = Difficulty.Medium;
                    allowFocus = true;
                    allowTrickle = false;
                    allowAttack = true;
                    allowDefence = true;
                    allowSnipers = false;
                    Debug.Log("medium");
                    break;
                case Difficulty.Hard: AIDifficulty = Difficulty.Hard;
                    allowFocus = true;
                    allowTrickle = false;
                    allowAttack = true;
                    allowDefence = true;
                    allowSnipers = true;
                    Debug.Log("Hard");
                    break;
                default: Debug.Log("ERROR: SaveSystem.GetDifficulty() returned null"); break;
            }
        }

        // Use game statistics to change the difficulty on the fly. The way i want to do this is to either store it on the players save file (things like enemies killed, units lost, wins & losses) or
        // output it to a csv file and read that (still somehow attatched to the players save file) each game. Bottom line is it needs to go from game to game
        switch (AIDifficulty)
        {
            case Difficulty.Easy:
                EasyPriorityStates(unit);
                moveDistance = 2;
                if(OutpostController != null)
                {
                    GetAIOutpost(OutpostController).spawnRate = 6;
                }
                break;
            case Difficulty.Medium:
                MediumPriorityStates(unit);
                moveDistance = 2;
                if (OutpostController != null)
                {
                    GetAIOutpost(OutpostController).spawnRate = 5;
                }
                break;
            case Difficulty.Hard:
                HardPriorityStates(unit);
                moveDistance = 1;
                if (OutpostController != null)
                {
                    GetAIOutpost(OutpostController).spawnRate = 4;
                }
                break;
            case Difficulty.Tutorial:
                GetAIPropertiesOfUnit(unit).AIState = AIProperties.PriorityType.Setup;
                break;
            default:
                print("Something went wrong");
                break;
        }
    }

    private void EasyPriorityStates(Unit unit)
    {
        if (moveCounter < 2 && allowSetup == true)                                            // For the first 2 moves, all AI Units will be in a 'setup' state.
        {
            GetAIPropertiesOfUnit(unit).AIState = AIProperties.PriorityType.Setup;
        }
        else if (allowFocus == true && unitsLost >= 1)    // This state will be used for focusing the AI team on one player unit.
        {
            GetAIPropertiesOfUnit(unit).AIState = AIProperties.PriorityType.Focus;
            if (teamTarget == null)
            {
                SetTeamTarget();
            }
        }
        else if (moveCounter > 2 && unitsLost < 1 && units.Count > 2)   // Trickle state
        {
            GetAIPropertiesOfUnit(unit).AIState = AIProperties.PriorityType.Trickle;
            if (TrickleUnit == null)
            {
                TrickleUnit = units[Random.Range(0, units.Count)];
            }
        }
    }

    private void MediumPriorityStates(Unit unit)
    {
        if (moveCounter < 2 && allowSetup == true)                                            // For the first 2 moves, all AI Units will be in a 'setup' state.
        {
            GetAIPropertiesOfUnit(unit).AIState = AIProperties.PriorityType.Setup;
        }
        else if (moveCounter >= 2 && castleAI.inRange == true && allowDefence == true)        // After 2 moves, if there is a player in the castles defence range, set the units priority to defend
        {
            GetAIPropertiesOfUnit(unit).AIState = AIProperties.PriorityType.Defend;
            if (setAITarget(unit) != null)
            {
                GetAIPropertiesOfUnit(unit).targetUnit = setAITarget(unit);
            }
        }
        else if (allowDefence == true && CatapultUnit != null && castleAI.CatapultInCastleRange(CatapultUnit) == true) // If the catapult is in the castles range, target it
        {
            GetAIPropertiesOfUnit(unit).AIState = AIProperties.PriorityType.Defend;
            GetAIPropertiesOfUnit(unit).targetUnit = CatapultUnit;
        }
        else if (moveCounter >= 2 && castleAI.inRange == false && allowAttack == true)       // After 2 moves, if there is no one in the castle's defence range, set the units priority to attack
        {
            GetAIPropertiesOfUnit(unit).AIState = AIProperties.PriorityType.Attack;
        }
        else if (allowFocus == true && unitsLost >= 3)                                       // This state will be used for focusing the AI team on one player unit.
        {
            GetAIPropertiesOfUnit(unit).AIState = AIProperties.PriorityType.Focus;
            if (teamTarget == null)
            {
                SetTeamTarget();
            }
        }
    }

    private void HardPriorityStates(Unit unit)
    {
        if (moveCounter < 1 && allowSetup == true)                                            // For the first move, all AI Units will be in a 'setup' state.
        {
            GetAIPropertiesOfUnit(unit).AIState = AIProperties.PriorityType.Setup;
            //if (allowPatrol == true)
            //{
            //    GetAIPropertiesOfUnit(unit).hasPatrolled = false;                            // Set this to false to allow for the patrol state to come right after Setup
            //}
        }
        else if (moveCounter >= 2 && castleAI.inRange == true && allowDefence == true)        // After 2 moves, if there is a player in the castles defence range, set the units priority to defend
        {
            GetAIPropertiesOfUnit(unit).AIState = AIProperties.PriorityType.Defend;
            if (setAITarget(unit) != null)
            {
                GetAIPropertiesOfUnit(unit).targetUnit = setAITarget(unit);
            }
        }
        else if (allowDefence == true && CatapultUnit != null && castleAI.CatapultInCastleRange(CatapultUnit) == true)  // If the catapult is in the castles range, target it
        {
            GetAIPropertiesOfUnit(unit).AIState = AIProperties.PriorityType.Defend;
            GetAIPropertiesOfUnit(unit).targetUnit = CatapultUnit;
        }
        else if (moveCounter >= 2 && castleAI.inRange == false && allowAttack == true)       // After 2 moves, if there is no one in the castle's defence range, set the units priority to attack
        {
            GetAIPropertiesOfUnit(unit).AIState = AIProperties.PriorityType.Attack;
        }
        else if (allowFocus == true && unitsLost >= 2)    // This state will be used for focusing the AI team on one player unit.
        {
            GetAIPropertiesOfUnit(unit).AIState = AIProperties.PriorityType.Focus;
            if (teamTarget == null)
            {
                SetTeamTarget();
            }
        }
        if (unit.unitType == Unit.UnitType.Archer && allowSnipers == true && MountainInRange(unit) != null && GetAIPropertiesOfUnit(unit).AIState != AIProperties.PriorityType.Setup && units.Count > 1)
        {
            GetAIPropertiesOfUnit(unit).AIState = AIProperties.PriorityType.Snipe;
        }
        /*
        else if (allowPatrol == true && castleAI.inRange == false && PlayerUnitMoveInRange(unit) == null && GetAIPropertiesOfUnit(unit).hasPatrolled == false)    // Is it spelled Patrolled or patroled idk bruh ive looked at that word for way too long now
        {
            GetAIPropertiesOfUnit(unit).AIState = AIProperties.PriorityType.Patrol;
            GetAIPropertiesOfUnit(unit).hasPatrolled = true;
        }*/
    }
    #endregion

    #region Checks
    private Cell PlayerUnitAttackInRange(Unit unit)
    {
        Cell playerInRange = null;
        Cell[] attackRange;                                            // Cells within the units attack range
        if (unit.currentCell != null)
        {
            if (unit.unitType == Unit.UnitType.Archer) {
                attackRange = GetArcherAttackRange(unit);
            } else {
                attackRange = unit.currentCell.GetCellsInRange(unit.attackRange);
            }

            foreach (Cell c in attackRange)
            {
                if (NoLimits == false)
                {
                    if (c.attackScore > 50 && c.height <= unit.currentCell.height && c.isForest == false ) // Each cell is given an attack score depending on the unit that is on it
                    {                                                         // If the score is greater than 5, a unit is present (Default cellScore = 1, minimum unit cellScore = 100
                        playerInRange = c;
                    }
                }
                else if (NoLimits == true)
                {
                    if (c.attackScore > 50 && c.currentUnit.isAIControlled == false)                                    // Each cell is given an attack score depending on the unit that is on it
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
                            //Debug.Log(u);
                            playerInRange = c;
                        }
                    }
                }
            }
        }
        return playerInRange;
    }

    private Cell PlayerUnitMoveInRange(Unit unit)
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
                    if (c.cellScore > 20 && c.currentUnit.isAIControlled == false) // Each cell is given a score depending on the unit that is on it
                    {                                                         // If the score is greater than 5, a unit is present (Default cellScore = 1, minimum unit cellScore = 100
                        playerInRange = c;
                    }
                }
            }
        }
        if (playerInRange != null)
        {
            //Debug.Log("Player Unit : " + playerInRange + " Is in range");
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
                if (c.attackScore > 50 && c.currentUnit.isAIControlled == false) // Each cell is given an attack score depending on the unit that is on it
                {                                                         // If the score is greater than 5, a unit is present (Default cellScore = 1, minimum unit cellScore = 100
                    targetUnit = c.currentUnit;
                }
            }
        }
        return targetUnit;
    }

    private Cell AITargetInRange(Unit unit)
    {
        Cell TargetCell = null;
        Cell[] AttackRange;                                           // Cells within the units attack range
        if (unit.currentCell != null)
        {
            if (GetAIPropertiesOfUnit(unit).targetUnit != null)
            {
                if (unit.unitType == Unit.UnitType.Archer) {
                    AttackRange = GetArcherAttackRange(unit);
                } else {
                    AttackRange = unit.currentCell.GetCellsInRange(unit.attackRange);
                }

                foreach (Cell c in AttackRange)
                {
                    if (c.currentUnit != null)
                    {
                        if (NoLimits == false)
                        {
                            if ((c.attackScore == GetAIPropertiesOfUnit(unit).targetUnit.currentCell.attackScore || c.currentUnit == GetAIPropertiesOfUnit(unit).targetUnit) && c.height <= unit.currentCell.height && c.isForest == false)
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
                                //Debug.Log(u);
                                TargetCell = c;
                            }
                        }
                    }
                }
            }
        }
        return TargetCell;
    }
    private Cell AITeamTargetInRange(Unit unit)
    {
        Cell TargetCell = null;
        Cell[] AttackRange;
        if (unit.currentCell != null)
        {
            if (unit.unitType == Unit.UnitType.Archer) {
                AttackRange = GetArcherAttackRange(unit);
            } else {
                AttackRange = unit.currentCell.GetCellsInRange(unit.attackRange);
            }

            foreach (Cell c in AttackRange)
            {
                if (CatapultUnit != null)
                {
                    if (castleAI.CatapultInCastleRange(CatapultUnit) == true)
                    {
                        if (c.currentUnit == CatapultUnit)
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

    private List<Cell> MountainInRange(Unit unit)
    {
        List<Cell> highCells = new List<Cell>();
        if (unit.currentCell != null)
        {
            Cell mountainCell = unit.currentCell;
            Cell maxCell = null;
            Cell[] mapCells;
            mapCells = unit.currentCell.GetCellsInRange(6);

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
        return highCells;
    }

    private Cell NearPlayerTree(Unit unit)
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
        return treeCell;
    }

    private void UpdateUnitScores()
    {
        foreach (Unit AIUnit in units)
        {
            switch (AIUnit.unitType)
            {
                case Unit.UnitType.Soldier:
                    AIUnitScores += 100;
                    break;
                case Unit.UnitType.Archer:
                    AIUnitScores += 150;
                    break;
                case Unit.UnitType.Mage:
                    AIUnitScores += 175;
                    break;
                case Unit.UnitType.Crossbow:
                    AIUnitScores += 200;
                    break;
                case Unit.UnitType.Cavalry:
                    AIUnitScores += 250;
                    break;
                default:
                    Debug.Log("Something broke with the AI's unit score");
                    break;

            }
        }
        foreach (Unit PlayerUnit in PlayerUnits)
        {
            switch (PlayerUnit.unitType)
            {
                case Unit.UnitType.Soldier:
                    PlayerUnitScores += 100;
                    break;
                case Unit.UnitType.Archer:
                    PlayerUnitScores += 150;
                    break;
                case Unit.UnitType.Mage:
                    PlayerUnitScores += 175;
                    break;
                case Unit.UnitType.Crossbow:
                    PlayerUnitScores += 200;
                    break;
                case Unit.UnitType.Cavalry:
                    PlayerUnitScores += 250;
                    break;
                default:
                    Debug.Log("Something broke with the Players unit score");
                    break;
            }
        }
        if (debugOn)
        {
            Debug.Log("Player " + PlayerUnitScores);
            Debug.Log("AI " + AIUnitScores);
        }
    }

    private Unit FindClosestTarget(Unit unit)
    {
        Unit ClosestUnit = null;
        int minPathLength = int.MaxValue;
        Cell[] CurrentPath;
        foreach (Unit player in PlayerUnits)
        {
            if (player.currentCell != null)
            {
                CurrentPath = unit.currentCell.GetPathTo(player.currentCell);

                if (CurrentPath.Length < minPathLength && CurrentPath.Length >= 1)
                {
                    minPathLength = CurrentPath.Length;
                    ClosestUnit = player;
                }
            }
        }
        return ClosestUnit;
    }
    private int FindClosestTargetPath(Unit unit)
    {
        int ClosestPath = 1;
        int minPathLength = int.MaxValue;
        int CurrentPath;
        foreach (Unit player in PlayerUnits)
        {
            if (player.currentCell != null)
            {
                CurrentPath = unit.currentCell.FastMovesTo(player.currentCell);

                if (CurrentPath < minPathLength && CurrentPath >= 1)
                {
                    minPathLength = CurrentPath;
                    ClosestPath = minPathLength;
                    //Debug.Log("Length = " + CurrentPath.Length + "And the player is " + player);
                }
            }
        }
        return ClosestPath;
    }


    #endregion

    #region Turn Logic
    public override void StartTurn()
    {
        if (units.Count == 0 && GameManager.gameMode == GameMode.Catapult)
        {
            //Debug.Log("There are no more units");
            base.StartTurn();
            if (OutpostController != null)
            {
                GetAIOutpost(OutpostController).OutpostTurn();
            }
            base.EndTurn();
            if (CatapultUnit != null)                                              // If a catapult exists, execute its movement
            {
                NewCameraMovement.JumpToCell(CatapultUnit.currentCell);
                MoveCatapult(CatapultUnit);
                CatapultUnit.movePoints = 20;
                CatapultUnit.movementRange++;
            }
            if (OutpostController != null)
            {
                GetAIOutpost(OutpostController).spawnRate = 2;
                //Debug.Log("Spawn Rate Changed");
            }
            return;
        }
        else if (units.Count == 0 && GameManager.gameMode == GameMode.Outposts)
        {
            //Debug.Log("There are no more units");
            base.StartTurn();
            if (OutpostController != null)
            {
                GetAIOutpost(OutpostController).OutpostTurn();
            }
            base.EndTurn();
            if (OutpostController != null)
            {
                if (GetAIOutpost(OutpostController).spawnRate != 2)
                {
                    GetAIOutpost(OutpostController).spawnRate = 2;
                    //Debug.Log("Spawn Rate Changed");
                }
            }
            return;
        }
        if (units.Count > 0)
        {
            {
                base.StartTurn();                                                                                  // Starts the AI's turn, enabled AI movement.;
                UIManager.ToggleFastForward(true);
                moveCounter++;
                GetPlayerUnits();
                UpdateUnitScores();
                if (OutpostController != null)
                {
                    GetAIOutpost(OutpostController).OutpostTurn();
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
                    UpdateDifficulty(unit);                                                 // Updates each units difficulty & state according
                    if (GetAIPropertiesOfUnit(unit).targetUnit == null)
                    {
                        if (FindClosestTarget(unit) != null)
                        {
                            GetAIPropertiesOfUnit(unit).targetUnit = FindClosestTarget(unit);
                        }
                        else
                        {
                            GetAIPropertiesOfUnit(unit).targetUnit = PlayerUnits[Random.Range(0, PlayerUnits.Count)];
                        }
                    }
                }

                if (unitSpawnedThisTurn == false)
                {                                          // Added by Ethan, AIOutpostController jumps the Camera to the spawned Unit.
                    JumpCameraToCurrentUnit();                                              // This all prevents it from being overriden by JumpCameraToCurrentUnit() not being invoked.
                }
                else
                {
                    unitSpawnedThisTurn = false;
                }

                InvokeRepeating("MoveAndAttackInvoke", AITurnTime, AITurnTime * 2);         // We use an Invoke to space out the time between each AI Units moves. Since parametres cant be passed into
                                                                                            // an Invoke, it is split into two functions (MoveInvoke(), Move().
            }
        }
    }

    private void MoveAndAttackInvoke()
    {
        // Begin the Moving and attacking phase by enabling/disabling the appropriate UI on the enemy
        foreach (Unit unit in units)
        {
            unit.EnableUIObject(false);
        }
        units[AIUnitCounter].EnableUIObject(true);

        JumpCameraToCurrentUnit();

        // Check to see if all the players are inside forest objects
        if (playersInForests == PlayerUnits.Count)
        {
            if (debugOn == true)
            {
                Debug.Log("All player units are in trees");
            }
            GetAIPropertiesOfUnit(units[AIUnitCounter]).AIState = AIProperties.PriorityType.Setup;
            if (NearPlayerTree(units[AIUnitCounter]) != null)
            {
                units[AIUnitCounter].Attack(NearPlayerTree(units[AIUnitCounter]));
            }
        }

        // Next, we check if the AI Unit has an enemy in its direct attack range. If so, skip moving & just attack
        if (PlayerUnitAttackInRange(units[AIUnitCounter]) != null || NearPlayerTree(units[AIUnitCounter]) != null)
        {
            JumpCameraToCurrentUnit();
            Attack(units[AIUnitCounter]);
        }
        else                                                                                                // If there is no one in the immediate range, lets begin the movement logic
        {
            GetAIPropertiesOfUnit(units[AIUnitCounter]).sameCell = units[AIUnitCounter].currentCell;        // Set the cell the AI is on before moving to the sameCell variable. If by the end of the
                                                                                                            // movement logic the AI is on the same cell, execute the code below to make it move.

            Move(units[AIUnitCounter]);                                                                     // Executes the units movement

            // Same cell movement logic
            if (GetAIPropertiesOfUnit(units[AIUnitCounter]).sameCell == units[AIUnitCounter].currentCell)
            {
                if (debugOn) { Debug.Log(units[AIUnitCounter] + " Was in the same cell"); }
                MoveSameCell(units[AIUnitCounter]);
            }

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

    private void FinishMove()
    {
        units[AIUnitCounter - 1].EnableUIObject(false);
        CancelInvoke();
        AIUnitCounter = 0;                                                     // Reset the unit counter
        base.EndTurn();                                                        // Ends the AI's turn, disables AI movement
        checkedForests = false;                                                // Reset the forest check
        playersInForests = 0;
        AIUnitScores = 0;
        PlayerUnitScores = 0;
        if (CatapultUnit != null)                                              // If a catapult exists, execute its movement
        {
            NewCameraMovement.JumpToCell(CatapultUnit.currentCell);
            MoveCatapult(CatapultUnit);
            CatapultUnit.movePoints = 20;
            CatapultUnit.movementRange++;
        }
        UIManager.ToggleFastForward(false);
    }

    private void JumpCameraToCurrentUnit()
    {
        if (AIUnitCounter >= units.Count) return;
        if (units[AIUnitCounter].currentCell != null)
        {
            NewCameraMovement.JumpToCell(units[AIUnitCounter].currentCell);          // Moves the camera to the unit that is being moved
        }
    }


    #endregion

    #region Movement Logic
    private void Move(Unit unit)
    {
        if (GetPriorityState(unit) == AIProperties.PriorityType.Setup)
        {
            MoveSetup(unit);
        }
        else
        {
            int RandomUnit = Random.Range(0, PlayerUnits.Count);
            Cell[] Path = null;                                                      // Create an array to store a path for this turn
            Cell Destination;
            if(GetAIPropertiesOfUnit(unit).targetUnit == null)
            {
                if (FindClosestTarget(unit) != null)
                {
                    GetAIPropertiesOfUnit(unit).targetUnit = FindClosestTarget(unit);
                }
                else
                {
                    GetAIPropertiesOfUnit(unit).targetUnit = PlayerUnits[RandomUnit];
                }
            }
            // Switch case depending on the priority state
            switch (GetPriorityState(unit))
            {
                case AIProperties.PriorityType.Attack:
                    Path = unit.currentCell.GetPathTo(GetAIPropertiesOfUnit(unit).targetUnit.currentCell);     // Set the path to the targeted player
                    //if(Path.Length > FindClosestTargetPath(unit) && FindClosestTarget(unit) != null)                   // I am concerned about the impact this is having on performance
                    //{
                    //    GetAIPropertiesOfUnit(unit).targetUnit = FindClosestTarget(unit);
                    //    Path = unit.currentCell.GetPathTo(GetAIPropertiesOfUnit(unit).targetUnit.currentCell);
                    //}
                    break;
                case AIProperties.PriorityType.Defend:
                    if (castleAI.DefenceRange != null)                              // CastleAI has an array of its surrounding cells.
                    {
                        castleAI.DrawDefenceRadius();
                    }
                    Path = unit.currentCell.GetPathTo(castleAI.DefenceRange[Random.Range(0, castleAI.DefenceRange.Length)]);     // Sets the path back to the castles range
                    break;
                case AIProperties.PriorityType.Focus:
                    if (teamTarget == null)                                         // Set a single player unit for the whole AI team to target
                    {
                        SetTeamTarget();
                    }
                    Path = unit.currentCell.GetPathTo(teamTarget.currentCell);      // Set the path to the focused player
                    break;
                case AIProperties.PriorityType.Snipe:
                    if (MountainInRange(unit) != null)
                    {
                        Cell[] possiblePaths = MountainInRange(unit).ToArray();
                        if (possiblePaths.Length != 0 && possiblePaths != null)
                        {
                            Path = unit.currentCell.GetPathTo(possiblePaths[Random.Range(0, possiblePaths.Length)]);
                            //Debug.Log("Selecting a mountain path" + Path.Length);
                        }
                        else
                        {
                            Path = unit.currentCell.GetPathTo(GetAIPropertiesOfUnit(unit).targetUnit.currentCell);
                        }
                    }
                    break;
                case AIProperties.PriorityType.Trickle:
                    if (TrickleUnit == null)
                    {
                        TrickleUnit = units[Random.Range(0, units.Count)];
                    }
                    if(unit == TrickleUnit)
                    {
                        Path = unit.currentCell.GetPathTo(GetAIPropertiesOfUnit(unit).targetUnit.currentCell);     // Set the path to the targeted player
                        moveDistance = 1;
                    } else
                    {
                        Path = unit.currentCell.GetPathTo(GetAIPropertiesOfUnit(unit).targetUnit.currentCell);
                        moveDistance = Random.Range(3, 4);
                    }
                    break;
                case AIProperties.PriorityType.Invalid:
                    unit.currentCell.GetPathTo(PlayerUnits[RandomUnit].currentCell);
                    Path = unit.currentCell.GetPathTo(PlayerUnits[RandomUnit].currentCell);
                    Debug.LogError("Something went wrong with the AIProperties Priority type");
                    break;
                default:
                    unit.currentCell.GetPathTo(PlayerUnits[RandomUnit].currentCell);
                    Path = unit.currentCell.GetPathTo(PlayerUnits[RandomUnit].currentCell);
                    Debug.LogError("Something went wrong with the AIProperties Priority type");
                    break;
            }
            if (Path != null)
            {
                if (Path.Length != 0)
                {
                    if (Path.Length > (unit.movementRange - moveDistance))                                    // Check if the path distance is less than the possible movement range so that it doesn't overshoot the movement and jump out of array length
                    {
                        Destination = Path[unit.movementRange - moveDistance];
                        //Debug.Log("The first statement is firing");
                    }
                    else if (Path.Length > unit.movementRange)
                    {
                        Destination = Path[unit.movementRange - 1];
                        //Debug.Log("Path is greater than movement range");
                    }
                    else
                    {
                        Destination = null;
                    }
                    if (Path != null && Destination != null)
                    {
                        MoveUnit(unit, Destination);                                         // Move the unit to the destination cell
                    }
                    else if (GetPriorityState(unit) != AIProperties.PriorityType.Snipe)
                    {
                        MoveSameCell(unit);
                    }
                }
            }
            else if (GetPriorityState(unit) != AIProperties.PriorityType.Snipe)
            {
                GetAIPropertiesOfUnit(unit).targetUnit = FindClosestTarget(unit);
                //Debug.Log("Re-trying move!");
                Move(unit);
            }
        }
        JumpCameraToCurrentUnit();
        Invoke("JumpCameraToCurrentUnit", AITurnTime);
    }

    private void MoveSetup(Unit unit)
    {
        Cell[] Path;                                                     // Create an array to store a path for this turn
        Cell Destination;
        int RandomUnit = Random.Range(0, PlayerUnits.Count);             // Choose a random player unit to move towards

        Path = unit.currentCell.GetPathTo(PlayerUnits[RandomUnit].currentCell);     // Set the path to the random player
        if (Path.Length != 0)
        {
            if (Path.Length < unit.movementRange)                        // Check if the path distance is less than the possible movement range so that it doesn't overshoot the movement and jump out of array length
            {
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
                Debug.LogError("Something is set to null");
            }
        }
        else
        {
            MoveAdjacent(unit);
        }
    }

    /*    private void MovePatrol(Unit unit)                                   // Currently for testing purposes
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
        }*/

    private void MoveAdjacent(Unit unit)
    {
        Cell[] AdjacentCells;
        Cell MoveOneCell;
        AdjacentCells = unit.currentCell.GetAdjacentCells();
        MoveOneCell = AdjacentCells[Random.Range(0, AdjacentCells.Length)];
        MoveUnit(unit, MoveOneCell);
    }

    private void MoveCatapult(Unit Catapult)
    {
        Cell[] Path;                                                    // Create an array to store a path for this turn
        Cell Destination;

        if (Catapult.currentCell != null && Catapult.currentCell != castleAI.castleCell)      // As the catapult is an AI unit but on the players team, we need to reset its cellScores manually.
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
            //Debug.Log("Attack Time");
            Destination = null;
            CatapultAttack(CatapultUnit);                                       // If you're within range of the castle, start attacking it
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
            CatapultUnit.currentCell.attackScore = 1000;
        }
        else
        {
            Debug.Log("You have reached the castle, no more movement. Or somethings null idk");
        }
    }

    private void MoveSameCell(Unit unit)
    {
        if (debugOn == true)
        {
            Debug.Log("The targeted unit was out of move range, so we'll try something else " + units[AIUnitCounter]);
            Debug.Log("The" + units[AIUnitCounter] + " Will either try move to an adjacent cell of a player close to him or just move in a direction");
        }

        if (FindClosestTarget(unit) != null && PlayerUnitMoveInRange(unit) != null)                                    // If a player is within the AI's range
        {
                //Debug.Log("Testing moving towards adjacent enemies " + units[AIUnitCounter]);
                GetAIPropertiesOfUnit(unit).targetUnit = FindClosestTarget(unit);
                if (GetAIPropertiesOfUnit(unit).targetUnit != null)
                {
                //Debug.Log(GetAIPropertiesOfUnit(unit).targetUnit);
                Cell[] Adjacent = GetAIPropertiesOfUnit(unit).targetUnit.currentCell.GetAdjacentCells();
                Cell MoveOneCell = Adjacent[Random.Range(0, Adjacent.Length)];
                MoveUnit(unit, MoveOneCell);
                if (GetAIPropertiesOfUnit(unit).sameCell == unit.currentCell)        // STILL
                {
                    MoveAdjacent(unit);
                    GetAIPropertiesOfUnit(unit).targetUnit = PlayerUnits[Random.Range(0, PlayerUnits.Count)];
                }
                GetAIPropertiesOfUnit(unit).sameCell = null;
            }
        }
        else
        {
            //Debug.Log("The " + unit + " Could not find anything close to him that he could move to");
            MoveAdjacent(unit);
            //GetAIPropertiesOfUnit(unit).targetUnit = PlayerUnits[Random.Range(0, PlayerUnits.Count)];
            //Move(unit);
            GetAIPropertiesOfUnit(unit).sameCell = null;
        }
    }
    #endregion

    #region Attacking logic
    public void Attack(Unit unit)
    {
        Cell CellToAttack = null;
        if (GetAIPropertiesOfUnit(unit).hasAttacked == false)
        {
            if (NearPlayerTree(unit) != null && ForestLevel == true)    // First check if the levels a forest level & a player in a forest is near the AI
            {
                CellToAttack = NearPlayerTree(unit);
            }
            else if (NearPlayerTree(unit) == null || ForestLevel == false)
            {
                switch (GetPriorityState(unit))
                {
                    case AIProperties.PriorityType.Attack:
                        if (AITargetInRange(unit) != null)
                        {
                            CellToAttack = AITargetInRange(unit);
                        }
                        else if (PlayerUnitAttackInRange(unit) != null)
                        {
                            CellToAttack = PlayerUnitAttackInRange(unit);
                        }
                        break;

                    case AIProperties.PriorityType.Defend:
                        Cell[] UnitAttackRange;
                        if (unit.unitType == Unit.UnitType.Archer) {
                            UnitAttackRange = GetArcherAttackRange(unit);
                        } else {
                            UnitAttackRange = unit.currentCell.GetCellsInRange(unit.attackRange);
                        }
                        float lowestHealth = 250;
                        foreach (Cell c in UnitAttackRange)
                        {
                            if (CatapultUnit != null)
                            {
                                if (castleAI.CatapultInCastleRange(CatapultUnit) == true)
                                {
                                    if (c.currentUnit == CatapultUnit)
                                    {
                                        CellToAttack = c;
                                        GetAIPropertiesOfUnit(unit).targetUnit = CatapultUnit;
                                    }
                                }
                            }
                            else if (c.attackScore > 50)
                            {
                                if (c.currentUnit.health < lowestHealth)
                                {
                                    CellToAttack = c;
                                    lowestHealth = c.currentUnit.health;
                                    GetAIPropertiesOfUnit(unit).targetUnit = CellToAttack.currentUnit;
                                }
                            }
                        }
                        break;

                    case AIProperties.PriorityType.Focus:
                        if (AITeamTargetInRange(unit) != null)
                        {
                            CellToAttack = AITeamTargetInRange(unit);
                        }
                        else if (PlayerUnitAttackInRange(unit) != null)
                        {
                            CellToAttack = PlayerUnitAttackInRange(unit);
                        }
                        break;

                    case AIProperties.PriorityType.Snipe:   // Add an attack to target the highest priority target
                        if (PlayerUnitAttackInRange(unit) != null)
                        {
                            CellToAttack = PlayerUnitAttackInRange(unit);
                        }
                        break;
                    case AIProperties.PriorityType.Trickle:
                        if (PlayerUnitAttackInRange(unit) != null)
                        {
                            CellToAttack = PlayerUnitAttackInRange(unit);
                        }
                        break;
                    case AIProperties.PriorityType.Invalid:
                        break;
                }
            }
        }
        if (CellToAttack != null)
        {
            unit.Attack(CellToAttack);
        }
        GetAIPropertiesOfUnit(unit).hasAttacked = true;
    }
    private void CatapultAttack(Unit Catapult)
    {
        castleAI.Attack(CatapultUnit.damage);
        if (CatapultAnim != null)
        {
            CatapultAnim.SetBool("AttackState", true);
        }
    }
    #endregion

}
