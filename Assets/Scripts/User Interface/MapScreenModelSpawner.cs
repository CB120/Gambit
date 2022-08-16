using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapScreenModelSpawner : MonoBehaviour
{
    public GameObject[] lvlModels;
    private int currentModelIndex = 0;

    private void Start()
    {
        currentModelIndex = 0;
        SpawnObject(currentModelIndex);
    }

    public void ChangeLvlModel(int lvlIndex)
    {
        if(lvlIndex == currentModelIndex)
            return;
        else
        {
            DespawnObject(currentModelIndex);
            currentModelIndex = lvlIndex;
            SpawnObject(currentModelIndex);
        }
    }

    private void SpawnObject(int modelIndex)
    {
        GameObject tempGo = lvlModels[modelIndex];
        Instantiate(tempGo);
    }

    private void DespawnObject(int modelIndex)
    {
        string objName = lvlModels[modelIndex].name;
        GameObject objToDestroy = GameObject.Find(objName + "(Clone)");
        if (objToDestroy)
            Destroy(objToDestroy);
        else
        {
            Debug.Log("Error - couldnt find " + objName + "(Clone)");
        }
    }
}
