using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Unit_SO", order = 1)]
public class Unit_SO : ScriptableObject
{
    public Unit.UnitType unitType;
    public string unitName;
    [TextArea]
    public string unitDescription;
    public int health;
    public float damage;
    public int range;
    public int movementRange;

    public Sprite icon;
}
