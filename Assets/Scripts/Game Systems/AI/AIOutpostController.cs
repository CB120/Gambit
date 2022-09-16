using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SpawnMode {
    Infinite,
    Limited,
    Disabled
}

[System.Serializable]
public class UnitProbability {
    public Unit unit;
    public float probabilityWeight = 1f;

    public UnitProbability(Unit unit, float weight){
        this.unit = unit;
        probabilityWeight = weight;
    }
}

public class AIOutpostController : MonoBehaviour
{
    //Properties
    [Header("AI Respawning Properties")]
    [Tooltip("Infinite: A random Unit from allUnits is selected and spawned each spawnRate turns. Limited: The next Unit from unitsToSpawn is spawned after spawnRate turns, stopping after all have been spawned.")]
    public SpawnMode spawnMode = SpawnMode.Disabled;
    [Tooltip("Does not affect the spawnRate. If True, the first Unit will be spawned on the AI's first turn, consistent with the spawnMode")]
    public bool spawnOnFirstTurn = false;
    [Tooltip("The order of Units to spawn in Limited mode.")]
    public Unit[] unitsToSpawn;
    [Tooltip("The selection of Units available in Infinite mode. The order does not matter.")]
    public UnitProbability[] allUnits;
    [Tooltip("The number of turns before 1 Unit is spawned. Eg. If 4, there will be 3 turns without a spawn and the Unit will be spawned on the 4th. Not affected by spawnOnFirstTurn.")]
    public int spawnRate = 2;

    //Variables
    [Header("Debug variables")]
    [Tooltip("Pulled from GridGenerator on Start(), the available coordinates for a new Unit to spawn. Randomly chosen.")]
    public List<Vector2Int> spawnCoordinates = new List<Vector2Int>();
    [Tooltip("Turns passed since the last spawn. When equal to spawnRate - 1, a spawn will be attempted.")]
    public int turnCounter = 0;

    int spawnLimit;
    int unitsSpawned = 0;
    bool spawnedOnFirstTurn = false;

    //References
    [Header("Asset references")]
    public AudioClip spawnSoundEffect;

    GameObject AIParticipant;
    AIController AIManager;


    //Methods
        //Events
            //Engine-called
    void Start(){
        AIParticipant = GameObject.FindGameObjectWithTag("AIParticipant");
        if(AIParticipant != null) AIManager = AIParticipant.GetComponent<AIController>();
        if(unitsToSpawn != null) spawnLimit = unitsToSpawn.Length;
    }

            //Participant-called
    public void OutpostTurn(){
        if (AIParticipant != null && AIManager != null) {
            if (spawnMode != SpawnMode.Disabled) turnCounter++;

            if (turnCounter >= spawnRate && spawnMode != SpawnMode.Disabled){
                TrySpawn();
                turnCounter = 0;
            }

            if (spawnOnFirstTurn == true && spawnedOnFirstTurn == false){ //allows the spawning of a Unit on the AI's first turn
                TrySpawn();
                spawnedOnFirstTurn = true;
            }
        }
    }


        //Spawning checks & decision-making
    void TrySpawn(){
        if (spawnCoordinates.Count <= 0) return; //GUARD to prevent index-out-of-range errors

        if (spawnMode == SpawnMode.Infinite){
            SpawnAI();
        } else if (spawnMode == SpawnMode.Limited){
            if (unitsSpawned < spawnLimit) {
                SpawnAI();
            }
        }
        UIManager.SetEnemyObjectiveUnits(AIManager.units.Count);
    }

    void SpawnAI(){
        //Select our Unit
        Unit unitToSpawn = null;
        if (spawnMode == SpawnMode.Limited) {
            unitToSpawn = unitsToSpawn[unitsSpawned];
        } else {
            unitToSpawn = GetRandomUnit();
        }

        if (unitToSpawn == null){
            Debug.LogWarning("Could not pick a Unit to spawn! Returning.");
            return;
        }

        //Find an available cell
        int randomCell = Random.Range(0, spawnCoordinates.Count); //Get a random cell in the predefined list
        bool foundAvailableCell = false;
        int i = 0;
        while (!foundAvailableCell && i != 10){
            if (GridController.GetCellAt(spawnCoordinates[randomCell]).currentUnit == null){ //If the random cell doesnt currently have a unit
                foundAvailableCell = true; //Break the while loop
            } else {
                randomCell = Random.Range(0, spawnCoordinates.Count); //Find a different cell
            }

            i++; // Check to make sure that we don't get stuck in an infinite loop
        }

        //Instantiate the prefab
        if (foundAvailableCell){
            Unit prefab = Instantiate(unitToSpawn, GridController.GetCellAt(spawnCoordinates[randomCell]).transform.position, Quaternion.identity, transform);
            prefab.startCell = GridController.GetCellAt(spawnCoordinates[randomCell]);//set the start cell of the unit
            prefab.MoveToCell(prefab.startCell); //Teleport the Unit to their new Cell and play the particle effect
            AIManager.units.Add(prefab);  //add to units list
            unitsSpawned++;

            if (spawnSoundEffect) AudioManager.PlaySound(spawnSoundEffect, AudioType.UnitSounds);
            NewCameraMovement.JumpToCell(spawnCoordinates[randomCell]);
            AIManager.unitSpawnedThisTurn = true;
        }
    }


    //Functions
    Unit GetRandomUnit(){
        //Get the total of all the Units' weighting
        float total = 0f;
        foreach (UnitProbability p in allUnits){
            if (p.probabilityWeight <= 0f){
                Debug.LogWarning("Unit " + p.unit.name + " has a weight <= 0, which is invalid. Please fix or remove. Returning null.");
                return null;
            }
            total += p.probabilityWeight;
        }

        //Guards to prevent invalid values
        if (allUnits.Length <= 0){
            Debug.LogWarning("No UnitProbability pairs set up in allUnits! Please see AIOutpostController in the Inspector. Unable to pick a Unit. Returning null.");
            return null;
        }

        if (total <= 0f) {
            Debug.LogWarning("Unit probability weightings are not > 0, or do not exist. Unable to pick a Unit. Returning null.");
            return null;
        }

        //Pick a random value within the total, and work out which Unit this applies to.
        float randomValue = Random.Range(0f, total);
        float runningTotal = 0f;
        foreach (UnitProbability p in allUnits){
            if (randomValue <= p.probabilityWeight + runningTotal) { //if our value is between runningTotal and the current probability, it's a hit!
                return p.unit;
            } else { //if our value is outside this range, add to the runningTotal
                runningTotal += p.probabilityWeight;
            }
        }
        Debug.LogWarning("Not sure how we got here, potentially an out-of-range randomValue. Unable to pick a Unit. Returning null.");
        return null;
    }
}
