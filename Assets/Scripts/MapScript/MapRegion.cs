using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapRegion : MonoBehaviour
{
    // Start is called before the first frame update
    public Animator animator;
    public Button button;

    [Header("Details")]
    public string mapName;
    public int unitAmount;
    public int enemyAmount;
    public bool isUnlocked;
    public string levelName;
    void Start()
    {
        
    }

}
