using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchWallPerspective : MonoBehaviour
{
    [SerializeField] private List<GameObject> walls;
    public int index = 3;

    public void ChangePerspective(bool clockwise)
    {
        if (clockwise)
        {
            index++;
        }
        else
        {
            index--;
        }

        if (index == walls.Count)
        {
            index = 0;
        }
        else if (index == -1)
        {
            index = walls.Count - 1;
        }
        foreach(GameObject wall in walls)
        {
            wall.SetActive(true);
        }
        switch (index)
        {
            case 0:
                walls[index].SetActive(false);
                break;
            case 1:
                walls[index].SetActive(false);
                walls[index - 1].SetActive(false);
                break;
            case 2:
                walls[index].SetActive(false);
                walls[index + 1].SetActive(false);
                break;
            case 3:
                walls[index].SetActive(false);
                break;

        }
    }
}
