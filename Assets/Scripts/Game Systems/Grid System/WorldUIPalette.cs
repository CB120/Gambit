using UnityEngine;

[CreateAssetMenu(fileName = "Custom", menuName = "ScriptableObjects/WorldUIPalette", order = 1)]
public class WorldUIPalette : ScriptableObject
{
    public Material gridCellMaterial;
    public Material gridCellOutOfRangeMaterial;
    public Material gridCellHoverMaterial;
    public Material gridCellHoverInvalidMaterial;
    public Material gridCellPathMaterial;
    public Material gridCellSelectedMaterial;
    public Material gridCellAttackSelectedMaterial;
    public Material gridCellAttackHoverMaterial;
    public Material gridCellAttackMaterial;
}
