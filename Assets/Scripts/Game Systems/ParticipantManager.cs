using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ParticipantType
{ //Used by ParticipantProperties script for the Manager to work out how to handle different Participant scripts
    LocalPlayer,
    NetworkPlayer, //Probably won't be used for this assignment
    AI //Intent is to add all AI 'types' here, currently leaving it as one
}

public class ParticipantManager : MonoBehaviour
{
    //Singleton reference
    static ParticipantManager Singleton;

    //Properties
    [Header("Properties")]
    public bool debugOn = true;
    public bool isProcGen = false;

    //Variables
    public int turnIndex = 0; //progress through participantReferences list

    //References
    [Space(20)]
    [Header("In-scene GameObject references")]
    [Tooltip("Click and drag Participant children into this list to set the turn order. Participants not in this list will not receive a turn.")]
    public List<GameObject> participantReferences = new List<GameObject>();

    //In-game references, automatically set


    //Engine-called
    void Awake(){
        //Set references
        Singleton = this;

        ParseParticipantReferences();
    }

    void Start(){
        if (!isProcGen)
        {
            StartNextTurn();
        }
    }


    //Turn-ly methods
    public void StartNextTurn(){
        UIManager.ToggleTurn(participantReferences[turnIndex].GetComponent<Participant>());//Disable turn button
        ParticipantProperties properties = participantReferences[turnIndex].GetComponent<ParticipantProperties>();
        //uiManager.SetTurnUI(properties); //UIManager handles any UI enabling/disabling, colour changes, text changes, etc.
        if (debugOn) Debug.Log("Current turn is " + DebugInfo(participantReferences[turnIndex]));

        //TODO: fix this so it doesn't look disgusting
        Participant participant = participantReferences[turnIndex].GetComponent<Participant>();
        participant.StartTurn();

    }

    public void EndTurn(GameObject participant){ //Ends a turn, giving a Warning if the given Participent isn't the current one,
        if (participantReferences[turnIndex] != participant){
            Debug.LogWarning("Received out-of-order ParticipantManager.EndTurn() call from " + DebugInfo(participant) + ". Returning, errors possible.");
            return;
        }

        List<Unit> units = participant.GetComponent<Participant>().units;
        foreach (Unit unit in units){
            unit.EnableUIObject(false);
        }
        
        IncrementTurnIndex();

        //Anything that needs to happen between turns goes here

        StartNextTurn();
    }

    public void EndCurrentTurn(){
        EndTurn(participantReferences[turnIndex]);
    }

    void IncrementTurnIndex(){ //Steps turnIndex by 1, wrapping to maintain 0 <= turnIndex < participantReferences.Count
        turnIndex++;
        if (turnIndex >= participantReferences.Count){
            turnIndex = 0;
        }
    }


    //Start methods
    void ParseParticipantReferences(){ //purely to verify the Participants are all entered and configured correctly.
        //Remove any null references
        foreach (GameObject p in participantReferences){
            if (p == null) participantReferences.Remove(p);
        }

        //check too-few Participants
        if (participantReferences.Count == 0){
            Debug.LogError("No Participants configured! Please check the Participant References list.");
            return;
        } else if (participantReferences.Count == 1){
            Debug.LogWarning("Only 1 Participant configured. Please check the Participant References list.");
            return;
        }

        //Check for duplicates, and store the value for invalid Transform parenting check
        bool hasDuplicates = false;
        if (debugOn){ //broke it down into two if statements as HasDuplicates can be O(n^2)
            hasDuplicates = HasDuplicates(participantReferences);
            if (hasDuplicates){
                Debug.LogWarning("Duplicate Participants added. If this was unintended, please check the Participant References list.");
            }
        }

        //Check Transform parenting
        if (participantReferences.Count > transform.childCount && debugOn && !hasDuplicates){
            Debug.LogWarning("Participants added that are not ParticipantManager's children. Please check the Participant References list.");
        } else if (participantReferences.Count < transform.childCount && debugOn && !hasDuplicates){
            Debug.LogWarning("Not all children of ParticipantManager have been added. If this was unintended, please check the Participant References list.");
        }

        //Check ParticipantProperties scripts
        foreach (GameObject p in participantReferences){
            ParticipantProperties participantProperties = p.GetComponent<ParticipantProperties>();
            if (participantProperties == null){
                Debug.LogError("Participant " + p.name + " has no ParticipantProperties script!");
            }
        }

        //Check Participant controller scripts
        foreach (GameObject p in participantReferences){ //this will need updating if we add more AI types
            PlayerParticipant playerScript = p.GetComponent<PlayerParticipant>();
            AIParticipant aiScript = p.GetComponent<AIParticipant>();
            if (playerScript == null && aiScript == null){
                Debug.LogError("Participant " + p.name + " has no Participant script!");
            }
        }
    }


    //static Getters
    public static Participant GetCurrentParticipant(){ //Returns the Participant currently having their turn
        return Singleton.participantReferences[Singleton.turnIndex].GetComponent<Participant>();
    }

    public static List<Participant> GetOpponentParticipants(Participant currentParticipant){ //Returns a List of all Participants EXCEPT the one given
        List<Participant> output = new List<Participant>();
        GameObject currentGameObject = currentParticipant.gameObject;
        foreach (GameObject p in Singleton.participantReferences){
            if (p != currentGameObject){
                output.Add(p.GetComponent<Participant>());
            }
        }
        return output;
    }

    public static Participant Temp_GetPlayer(){ //'Temporary' as in if we add more Participants it may not function correctly.
                                                //Returns the first Local Player in participantReferences, in this case, THE Player.
        foreach (GameObject p in Singleton.participantReferences){
            if (p.GetComponent<ParticipantProperties>().participantType == ParticipantType.LocalPlayer){
                return p.GetComponent<Participant>();
            }
        }
        return null;
    }

    public static Unit GetCurrentUnit(){ //Gets the Player's current selected unit.
                                         //Will return NULL if it's currently an AI's turn.
        PlayerParticipant playerParticipant = Singleton.participantReferences[Singleton.turnIndex].GetComponent<PlayerParticipant>();
        if (playerParticipant) return playerParticipant.GetCurrentUnit();
        return null;
    }


    public static bool IsCurrentParticipantType(ParticipantType type){ //Returns true if the given ParticipantType matches the current Participant's type
        return GetCurrentParticipant().properties.participantType == type;
    }


    //Debug methods
    string DebugInfo(GameObject participant){ //Gives a Debug readout of the given GameObject, intended for Participant GameObjects.
        ParticipantProperties properties = participant.GetComponent<ParticipantProperties>();

        string output;
        if (properties == null){
            output = "GameObject name: " + participant.name + ", tag: " + participant.tag + "; NOT a Participant!";
        } else {
            output = "GameObject name: " + participant.name + ", Participant name: " + properties.participantName;
        }
        return output;
    }

    bool HasDuplicates(List<GameObject> testList){ //Returns true if the given List has one or more duplicates.
                                                   //Logic should work for any List Type, just copy it and change the Type :)
                                                   //Adapted from http://bit.ly/3JBtLRI
        foreach (GameObject g in testList){
            List<GameObject> allg = testList.FindAll(t => t.Equals(g));
            if (allg.Count > 1) return true;
        }
        return false;
    }
}
