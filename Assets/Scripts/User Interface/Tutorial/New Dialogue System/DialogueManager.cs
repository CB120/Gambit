using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI dialogueText;
    public GameObject[] UIGameObjects;
    private Queue<string> sentences;
    [SerializeField] private GameObject DialogueCanvas;

    [SerializeField] private Image dialogueFade;
    [SerializeField] private Image backgroundFade;

    void Start()
    {
        sentences = new Queue<string>();
        foreach (GameObject UIObject in UIGameObjects)
        {
            UIObject.SetActive(false);
        }
        StartCoroutine(BGFadeIn());
        StartCoroutine(DialogueFadeIn());

    }

    IEnumerator DialogueFadeIn()
    {
        yield return new WaitForSeconds(3.3f);
        Color dialogue = dialogueFade.color;
        Debug.Log(dialogue.a);
        while(dialogue.a < 100)
        {
            dialogue.a += 0.38f * Time.deltaTime;
            dialogueFade.color = dialogue;
            yield return null;
        }
    }

    IEnumerator BGFadeIn()
    {
        yield return new WaitForSeconds(1.1f);
        Color background = backgroundFade.color;
        Debug.Log(background.a);
        while (background.a <= 0.41)
        {
            background.a += 0.29f * Time.deltaTime;
            backgroundFade.color = background;
            yield return null;
        }
    }
    private void Update()
    {
    //    StartCoroutine(DialogueFadeIn());
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
