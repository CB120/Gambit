using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;
    public GameObject[] UIGameObjects;
    private Queue<string> sentences;
    public GameObject DialogueCanvas;

    void Start()
    {
        sentences = new Queue<string>();
        foreach (GameObject UIObject in UIGameObjects)
        {
            UIObject.SetActive(false);
        }
    }

    public void StartTutorial(Dialogue dialogue)
    {
        nameText.text = dialogue.name;

        sentences.Clear();

        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if(sentences.Count == 0)
        {
            EndTutorial();
            return;
        }
        string sentence = sentences.Dequeue();
        dialogueText.text = sentence;
    }

    private void EndTutorial()
    {
        Debug.Log("exit the tutorial");
        foreach(GameObject UIObject in UIGameObjects)
        {
            UIObject.SetActive(true);
        }
        DialogueCanvas.SetActive(false);
    }

}
