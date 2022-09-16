using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeTutorial : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Unit>() != null && GameObject.Find("TutorialManager") != null)
        {
            if (GameObject.Find("TutorialManager").GetComponent<InteractiveTutorialController>().tutorialProgress > 5 && GameObject.Find("TutorialManager").GetComponent<InteractiveTutorialController>().finishedEnvironment != true)
            {
                GameObject.Find("TutorialManager").GetComponent<InteractiveTutorialController>().finishedEnvironment = true;
                Invoke("OpenNextInstructions", 0.5f);
            }   
        }
    }

    private void OpenNextInstructions()
    {
        GameObject.Find("TutorialManager").GetComponent<InteractiveTutorialController>().DisplayInstructions();
    }
}
