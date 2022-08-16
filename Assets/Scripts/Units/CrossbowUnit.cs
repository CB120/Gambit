using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossbowUnit : Unit
{
    //Properties
    private int defaultRange;


    //Methods
        //Engine-called
    public override void Start(){
        defaultRange = attackRange;
        base.Start();
    }

        //Custom
    public override void MoveToCell(Cell c){
        switch (c.height){
            case 1: attackRange = defaultRange;
                break;
            case 2: attackRange = (int)(defaultRange * 1.25f);
                break;
            case 3: attackRange = (int)(defaultRange * 1.5f);
                break;
            case 4: attackRange = (int)(defaultRange * 1.75f);
                break;
        }
        base.MoveToCell(c);
    }

    public override void Attack(Cell targetedCell)
    {
        float defaultDamage = damage;
        if (currentCell.GetPathTo(targetedCell).Length > 7 || targetedCell.height == currentCell.height - 3)
        {
            damage = (int)(defaultDamage * 0.5f); //If the target is more than 7 cells away or 3 cells below, decrease the damage by 50%
        }
        else if (currentCell.GetPathTo(targetedCell).Length > 6 || targetedCell.height == currentCell.height - 2)
        {
            damage = (int)(defaultDamage * 0.7f); //If the target is more than 6 cells away or 2 cells below, decrease the damage by 30%
        }
        else if (currentCell.GetPathTo(targetedCell).Length > 5 || targetedCell.height == currentCell.height - 1)
        {
            damage = (int)(defaultDamage * 0.9f);
        }
        base.Attack(targetedCell);
        damage = defaultDamage;

    }
}
