using UnityEngine;

[CreateAssetMenu(fileName = "Custom", menuName = "ScriptableObjects/WorldRules", order = 1)]
public class WorldRules : ScriptableObject
{
    public bool canOnlyMoveOnce = true;
    public int maxDistanceFromForestToAttack = 2;
}
