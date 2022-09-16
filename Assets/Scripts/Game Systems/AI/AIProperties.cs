using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIProperties : MonoBehaviour
{
    /** Variables **/
    public PriorityType AIState;
    public Unit targetUnit = null;
    public bool hasAttacked = false;
    public Cell sameCell = null;
    public bool hasPatrolled = true;

    public enum PriorityType // This enum is used to change the AI's priorities.
    {
        Setup, // For x amount of moves (maybe 2?) space out troops onto the map
        Attack, // Give higher value towards player units, getting close to them. Highest value would be Catapult.
        Focus, // Focus on one unit
        Defend, // Switch to this priority if a player unit is within x amount of cells from the castle. give priority towards moving to
        Patrol, // The AI will patrol along pre-set nodes (set in what cells you want) till a player comes into range 
        Snipe, // Used for archers to sit back and shoot from far away. Use only if you have heights on your level
        Trickle, // For easy mode
        Invalid
    }

    private void Start()
    {
        AIState = PriorityType.Setup;
        
    }

}
