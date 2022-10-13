using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialElement : MonoBehaviour
{
    public Dialogue dialogue;
    [SerializeField] private DialogueManager dialogueManager;
    private void Start()
    {
        if (dialogueManager.isCampaign)
        {
            Invoke("TriggerDialogue", 2.5f);
        }
    }
    public void TriggerDialogue()
    {
        dialogueManager.StartTutorial(dialogue);
    }
}
