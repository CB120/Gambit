using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialElement : MonoBehaviour
{
    public Dialogue dialogue;
    [SerializeField] private DialogueManager dialogueManager;
    private void Start()
    {
        Invoke("TriggerDialogue", 3.4f);
    }
    public void TriggerDialogue()
    {
        dialogueManager.StartTutorial(dialogue);
    }
}