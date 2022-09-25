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
    [SerializeField] private TextMeshProUGUI ClickAnywhereFade;
    private bool dialogueStarted = false;
    void Start()
    {
        sentences = new Queue<string>();
        foreach (GameObject UIObject in UIGameObjects)
        {
            UIObject.SetActive(false);
        }
        StartCoroutine(BGFadeIn());
        StartCoroutine(DialogueFadeIn());
        StartCoroutine(continueFade());

    }

    IEnumerator DialogueFadeIn()
    {
        yield return new WaitForSeconds(3.3f);
        Color dialogueText = dialogueFade.color;
        Debug.Log(dialogueText.a);
        while(dialogueText.a < 100)
        {
            dialogueText.a += 0.38f * Time.deltaTime;
            dialogueFade.color = dialogueText;
            yield return null;
        }
    }

    IEnumerator BGFadeIn()
    {
        yield return new WaitForSeconds(1.1f);
        Color backgroundText = backgroundFade.color;
        Debug.Log(backgroundText.a);
        while (backgroundText.a <= 0.41)
        {
            backgroundText.a += 0.29f * Time.deltaTime;
            backgroundFade.color = backgroundText;
            yield return null;
        }
    }

    IEnumerator continueFade()
    {
        Color continueText = ClickAnywhereFade.color;
        yield return new WaitForSeconds(5f);
        Debug.Log(continueText.a);
        while (continueText.a <= 100)
        {
            continueText.a += 0.33f * Time.deltaTime;
            ClickAnywhereFade.color = continueText;
            yield return null;
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
        dialogueStarted = true;
        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if(sentences.Count == 0 && dialogueStarted)
        {
            EndTutorial();
            return;
        }
        if (sentences.Count != 0)
        {
            string sentence = sentences.Dequeue();
            dialogueText.text = sentence;
        }
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
