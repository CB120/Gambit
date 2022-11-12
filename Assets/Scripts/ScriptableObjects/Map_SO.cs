using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Map_SO", order = 2)]
public class Map_SO : ScriptableObject
{
    public string mapName;
    public int unitAmount;
    public int enemyAmount;
    public string levelName;
}
