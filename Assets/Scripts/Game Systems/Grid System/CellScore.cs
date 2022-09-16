using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellScore : MonoBehaviour
{

    public static void updateCellScore(Unit.UnitType unit, Cell cell, bool isAiControlled)
    {
        int defaultScore = 1;
        if (isAiControlled == false)
        {
            switch (unit)
            {
                case Unit.UnitType.Soldier:
                    cell.cellScore = 100;
                    break;
                case Unit.UnitType.Archer:
                    cell.cellScore = 150;
                    break;
                case Unit.UnitType.Mage:
                    cell.cellScore = 175;
                    break;
                case Unit.UnitType.Crossbow:
                    cell.cellScore = 200;
                    break;
                case Unit.UnitType.Cavalry:
                    cell.cellScore = 250;
                    break;
                case Unit.UnitType.Catapult:
                    cell.cellScore = 1000;
                    break;
                default:
                    cell.cellScore = defaultScore;
                    break;
            }
        }
    }


    public static void ResetMoveScore(Cell cell)
    {
        cell.cellScore = 1;
    }
    public static void ResetAttackScore(Cell cell)
    {
        cell.attackScore = 1;
    }

    public static void updateAttackScore(Unit.UnitType unit, Cell cell, bool isAiControlled)
    {
        int defaultScore = 1;
        if (isAiControlled == false)
        {
            switch (unit)
            {
                case Unit.UnitType.Soldier:
                    cell.attackScore = 100;
                    break;
                case Unit.UnitType.Archer:
                    cell.attackScore = 150;
                    break;
                case Unit.UnitType.Mage:
                    cell.attackScore = 175;
                    break;
                case Unit.UnitType.Crossbow:
                    cell.attackScore = 200;
                    break;
                case Unit.UnitType.Cavalry:
                    cell.attackScore = 250;
                    break;
                case Unit.UnitType.Catapult:
                    cell.attackScore = 1000;
                    break;
                default:
                    cell.attackScore = defaultScore;
                    break;
            }
        } 
    }
}
