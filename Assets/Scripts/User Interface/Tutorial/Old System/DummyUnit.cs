using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyUnit : MonoBehaviour
{
    public bool playerMovedAllUnits = false;
    [SerializeField] private Unit dummyControls;
    public Unit[] playerUnits;
    public GridController gridController;

    void Update()
    {
        if (playerUnits[0].isAttacking && playerUnits[1].isAttacking && !playerMovedAllUnits)
        {
            playerMovedAllUnits = true;
            Cell[] adjacentCells = playerUnits[0].currentCell.GetAdjacentCells();
            if (adjacentCells[0] != null)
            {
                dummyControls.MoveToCell(adjacentCells[0]);
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<Unit>() != null && GameObject.Find("TutorialManager").GetComponent<InteractiveTutorialController>().tutorialProgress > 0)
        {
            if (GameObject.Find("TutorialManager").GetComponent<InteractiveTutorialController>().finishedBasics != true)
                Invoke("OpenNextInstructions", 0.5f);
        }
    }

    private void OpenNextInstructions()
    {
        GameObject.Find("TutorialManager").GetComponent<InteractiveTutorialController>().DisplayInstructions();
    }
}
