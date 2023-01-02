using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Saving;

public class Unit : MonoBehaviour
{
    //========PROPERTIES========//
    [Header("Object Reference")]
    public Unit_SO unitData;

    [Header("Health Settings")]
    public float maxHealth = 100;
    public float health;

    [Header("Damage Settings")]
    public float damage;
    public int attackRange;
    // TODO: make sounds a randomised list
    [SerializeField] protected AudioClip attackSound;
    public bool isRanged;
    //The range in which a player can attack an enemy

    [Header("Movement Settings")]
    public int movementRange;
    //Pass the range into the Cell Manager when canMove = true

    [HideInInspector] public int movePoints;
    [SerializeField] AudioClip moveSound;

    // Data
    // Cell the unit should start in (temporary)
    [Tooltip("Default coordinates for this Unit. If the startCell reference is set, then the coordinates will be overridden.")]
    public Vector2Int defaultStartCoordinates;
    [Tooltip("Default starting Cell for this Unit. If this is null, the Unit will default back to the defaultStartCoordinates.")]
    public Cell startCell;
    // Cell the unit is curently in
    public Cell currentCell;

    //TODO: if we keep the rule that Units must move before attacking, convert these two bools to a state enum
    [HideInInspector] public bool isAttacking = false;
    [HideInInspector] public bool hasAttacked = false;

    bool movedToCellOnStart = false;

    [HideInInspector] public Vector2Int position;
    //Get the current Grid Position of this player

    public enum UnitType {
        Soldier,
        Archer,
        Crossbow,
        Cavalry,
        Mage,
        Catapult,
        King
    }

    [Header("Type Settings")]
    public UnitType unitType;
    public bool isAIControlled = false;

    [Header("Aesthetic")]
    public GameObject selectedUI;
    private HealthBar healthBar;
    [SerializeField] HealthSquareBar healthSquareBar;
    [SerializeField] private GameObject UICanvas;
    [SerializeField] private ParticleSystem spawnPS;
    UnitAesthetics unitAesthetics;
    public string informationText;

    /*[HideInInspector]*/ public GameObject ownerParticipant;

    [SerializeField] protected Animator animator;


    /*==========================
                EVENTS
     =========================*/
    //Engine-called
    void Awake() {
        // Initialise movement points to movement range.
        movePoints = movementRange;
        health = unitData.health;
        maxHealth = unitData.health;
        damage = unitData.damage;
        movementRange = unitData.movementRange;
        attackRange = unitData.range;
        informationText = unitData.unitDescription;

        AssignOwnerParticipant();
    }

    public virtual void Start() {
        unitAesthetics = GetComponent<UnitAesthetics>();
        // healthBar = gameObject.GetComponent<HealthBar>();
        if (!healthSquareBar) healthSquareBar = GetComponentInChildren<HealthSquareBar>();
        if (healthSquareBar) healthSquareBar.Initialise(unitData.health);

        // Temporary system for placing units in their start cell.
        // Long-term this will ideally be replaced with something less janky.
        // e.g. some overseeing automated system to place units in logical positions
        //      OR player starts game by setting unit formation

        if (!startCell){
            startCell = GridController.GetCellAt(defaultStartCoordinates);

            if (!startCell) { // If there is no startCell or defaultStartCoordinates set, use GridController default start
                startCell = GridController.GetDefaultStartCell();
            }
        }
        // Move to start cell
        MoveToCell(startCell);
    }

    public void AssignOwnerParticipant(){
        GameObject participant;
        if (isAIControlled){
            participant = GameObject.FindGameObjectWithTag("AIParticipant");
        } else {
            participant = GameObject.FindGameObjectWithTag("PlayerParticipant");
        }

        if (participant != null && !ownerParticipant) ownerParticipant = participant;
    }

    void OnTriggerEnter(Collider other){
        if (other.gameObject.tag == "Forest"){
            OnEnterForest(other.gameObject);
        }
    }

    void OnTriggerExit(Collider other){
        if (other.gameObject.tag == "Forest"){
            OnExitForest(other.gameObject);
        }
    }

        //Script-called
    public void OnMouseClick(){ //Used by Player Participants to Move and Attack with the same controls
                                //Can be skipped, with Attack() and PerformMoveTo() called directly
                                //Currently called from GridController.Update()
        Cell cell = GridController.GetHoveredCell();

        if (isAttacking){
            Attack(cell);
        } else {
            if (cell.currentUnit){
                if (cell.currentUnit.isAIControlled){
                    if (cell.FastMovesTo(this) > attackRange) return; //GUARD to prevent attacking from outside the attack range

                    SkipMovement();
                    Attack(cell);
                }
            } else if (cell.mode == CellMode.AttackOnly) {
                if (cell.FastMovesTo(this) > attackRange) return; //GUARD to prevent attacking from outside the attack range

                SkipMovement();
                Attack(cell);
            } else {
                PerformMoveTo(cell);
            }
        }
    }

    void OnEnterForest(GameObject forest){
        if (isAIControlled){
            SetLayerRecursively(gameObject, 0, true);
        } else {
            SetLayerRecursively(gameObject, 10, true);
        }
        forest.GetComponent<Animator>().SetTrigger("EnterForest");
    }

    void OnExitForest(GameObject forest){
        SetLayerRecursively(gameObject, 0, false);
    }


    /*============================
        ATTACKING/TAKING DAMAGE
    ============================*/
    public virtual void Attack(Cell targetedCell){ //Performs an Attack on the given Cell
                                           //Currently allows false entry, we can change this as needed
        if (!targetedCell) { //GUARD, logs a warning and returns if the given Cell was null
            Debug.LogWarning("Unit " + gameObject.name + " told to attack a null Cell!");
            return;
        }

        if (targetedCell.currentUnit){
            if (targetedCell.currentUnit.ownerParticipant != ownerParticipant){
                transform.LookAt(new Vector3(targetedCell.transform.position.x, transform.position.y, targetedCell.transform.position.z));
                AttackEnemy(targetedCell.currentUnit);
                hasAttacked = true;
                if(!isAIControlled) UIManager.SetHoverPosition(PlayerParticipant.selectedUnit, ownerParticipant.GetComponent<PlayerParticipant>().units);
                GridController.UpdateGrid();
            }
        } else {
            if (targetedCell.mode == CellMode.AttackOnly){
                //NOTE for future devs, if we want attack-only Cells for mechanics other than the Outposts, adapt the code here
                if (!OutpostObjective.IsOutpostDead(targetedCell)){
                    OutpostObjective.AttackOutpost(targetedCell.coordinates, (int)damage);
                    hasAttacked = true;
                    transform.LookAt(new Vector3(targetedCell.transform.position.x, transform.position.y, targetedCell.transform.position.z));
                    GridController.UpdateGrid();
                    if (animator) animator.SetBool("AttackState", true);
                    AudioManager.PlaySound(attackSound, AudioType.UnitSounds);
                }
            }
        }
    }

    public virtual void AttackEnemy(Unit enemyToAttack){
        //This method needs the range check to happen before it's called!

        enemyToAttack.DecreaseHealth(damage);
        if (animator) animator.SetBool("AttackState", true);
        AudioManager.PlaySound(attackSound, AudioType.UnitSounds);
    }

    public void DecreaseHealth(float healthDecrement){
        unitAesthetics.SpawnDamageText(healthDecrement);
        health -= healthDecrement;
        // if (healthBar) {
        //     EnableUI();
        //     healthBar.SetHealth(health);
        //     Invoke("DisableUI", 5.0f);
        // }
        if (healthSquareBar) {
            EnableUI();
            healthSquareBar.SetHealth((int)health);
            Invoke("DisableUI", 5.0f);
        }
        if (health <= 0) DestroyUnit();
    }

    public virtual void DestroyUnit(){
        GameObject angel = Instantiate(unitAesthetics.Angel, gameObject.transform);
        angel.transform.parent = null;
        Participant owningParticipant = ownerParticipant.GetComponent<Participant>();
        CellScore.ResetAttackScore(currentCell);
        currentCell.currentUnit = null;

        foreach (Cell cell in currentCell.GetAdjacentCells()){
            CellScore.ResetMoveScore(cell);
        }

        health = 0;
        owningParticipant.units.Remove(this); //Remove the unit from the participant units list
        PlayerParticipant.ResetSelectedUnit(owningParticipant.units.Count);

        if (!isAIControlled){
            UIManager.SetUnitDead(owningParticipant.units, this); // Set the toolbar image to dead
            SavedData.GameData.deaths++;
        } else {
            UIManager.SetEnemyObjectiveUnits(owningParticipant.units.Count);
            SavedData.GameData.kills++;
            //owningParticipant.GetComponent<AIController>().SpawnAI();//CHRIS COMMENT THIS OUT
        }

        GameManager.GameIsOver(this); //Check if all Units in this participant are dead

        Destroy(gameObject);
    }


    /*============================
            MOVEMENT
   ============================*/
    public void PerformMoveTo (Cell c) { // PerformMove - to be used for making a movement action
                                         // i.e. will factor in movement points

        if (c.currentUnit != null) return; //GUARD to prevent moving to occupied Cells
        if (c.mode == CellMode.AttackOnly) return; //GUARD to prevent moving to Attack-Only Cells

        // Calculate movement point effect and act accordingly
        int requiredMovePoints = 0;
        if (currentCell) {
            requiredMovePoints = currentCell.FastMovesTo(c);
            if (requiredMovePoints > movePoints) return;
        }

        if (movePoints > 0 && World.GetRules().canOnlyMoveOnce) {
            movePoints = 0; // Immediately end movement
        } else {
            movePoints -= requiredMovePoints; //Disabled for now
        }

        AudioManager.PlaySound(moveSound, AudioType.UnitSounds);
        isAttacking = true;
        MoveToCell(c);
    }

    public virtual void MoveToCell(Cell c) { // MoveToCell - instantly jumps to a cell
        if (movedToCellOnStart) {
            Instantiate(spawnPS, c.transform);
        }

        movedToCellOnStart = true;
        transform.position = c.transform.position;

        if (currentCell != null){
            foreach (Cell cell in currentCell.GetAdjacentCells()){
                CellScore.ResetMoveScore(cell);
            }
            CellScore.ResetAttackScore(currentCell);
        }

        // Clear old cell's unit
        if (currentCell) currentCell.currentUnit = null;

        // Switch cells in data
        currentCell = c;
        // Set new cells unit
        currentCell.currentUnit = this;
        
        foreach (Cell cell in c.GetAdjacentCells()){
            CellScore.updateCellScore(unitType, cell, isAIControlled);
        }
        CellScore.updateAttackScore(unitType, c, isAIControlled);

        position = c.coordinates;

        GridController.UpdateGrid();
    }

    public void SkipMovement(){
        movePoints = 0;
        isAttacking = true;
        GridController.UpdateGrid();
    }

    public Cell[] GetCurrentRange() {
        if (currentCell == null) currentCell = GridController.GetDefaultStartCell();

        if (hasAttacked || !currentCell) return new Cell[0];
        if (isAttacking) return currentCell.GetCellsInRange(attackRange);
        return currentCell.GetCellsInRange(movePoints);
    }

    public Cell[] GetMovementRange(){
        if (currentCell == null) currentCell = GridController.GetDefaultStartCell();
        if (currentCell == null) return new Cell[0];

        return currentCell.GetCellsInRange(movePoints);
    }

    public Cell[] GetAttackRange(){
        if (currentCell == null) currentCell = GridController.GetDefaultStartCell();
        if (currentCell == null) return new Cell[0];

        return currentCell.GetCellsInRange(attackRange);
    }

    public virtual List<CellInfo> GetAttackCells()
    {
        return new List<CellInfo>(); //DON'T RELY ON THIS, OVERRIDE
    }

    public bool CanUseMovePoints (int ap) { //Returns true if the given value is <= this Unit's movePoints
        return ap <= movePoints;
    }

    public bool IsInForest(){
        return currentCell.isForest;
    }

    public void ResetMovePoints () {
        movePoints = movementRange;
        isAttacking = false;
        hasAttacked = false;
    }

    /*============================
           AESTHETIC
  ============================*/
    public void EnableUI(){
        UICanvas.SetActive(true);
    }

    public void DisableUI(){
        UICanvas.SetActive(false);
    }

    public void SetUI(bool enabled){
        UICanvas.SetActive(enabled);
    }

    public void EnableUIObject(bool isEnabled){
        if (!selectedUI) return;
        selectedUI.SetActive(isEnabled);
    }

    public static void SetLayerRecursively(GameObject obj, int newLayer, bool isEnter){ //Extract to aesthetic script
        if (null == obj){
            return;
        }

        Unit unit = obj.GetComponent<Unit>();
        if(unit && unit.currentCell && unit.currentCell.isForest && !unit.isAIControlled)
        {
            newLayer = 10;
        }

        obj.layer = newLayer;
        if (obj.tag == "IconUI") obj.layer = isEnter ? 12 : 0;
        if (obj.tag == "UI") obj.layer = 0;

        foreach (Transform child in obj.transform){
            if (null == child){
                continue;
            }
            SetLayerRecursively(child.gameObject, newLayer, isEnter);
        }
    }
}
