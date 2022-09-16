using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveSystemDebug : MonoBehaviour
{
    public Sprite[] flags;

    private void Awake()
    {
        SaveSystem.availableFlags = flags;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
