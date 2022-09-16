using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Participant : MonoBehaviour
{
    //Properties
    public bool debugOn = true;

    //Variables


    //References
    //In-game GameObject references
    //[HideInInspector]
    [SerializeField] public List<Unit> units = new List<Unit>(); //Ethan: we may need to swap this back to GameObjects later
    [HideInInspector] public ParticipantProperties properties;
    protected MouseController mouseController;

    ParticipantManager participantManager;


    //Engine-called
    void Awake(){
        participantManager = GameObject.FindWithTag("Participant Manager").GetComponent<ParticipantManager>();
        mouseController = GameObject.FindWithTag("MainCamera").GetComponent<MouseController>();
        properties = GetComponent<ParticipantProperties>();
    }

    void Start(){
        AssignUnitOwnerReferences();
    }


    //Turn-ly methods
    public virtual void StartTurn(){
        //On-start-turn logic that applies to all Participants
    }

    protected virtual void EndTurn(){
        //On-end-turn logic that applies to all Participants

        participantManager.EndTurn(this.gameObject);
    }


    //Getters
    public List<Unit> GetUnits(){ //returns this Participant's units array, minus any currently in Forest.
                                  //Intended for AI to know where 'visible' Player Units are
        List<Unit> output = new List<Unit>();
        foreach (Unit u in units){
            if (u.currentCell == null){
                Debug.LogWarning(u);
            }
            output.Add(u); 
        }
        return output;
    }


    //Helpers
    void AssignUnitOwnerReferences(){
        foreach (Unit u in units){
            u.ownerParticipant = this.gameObject;
        }
    }
}
