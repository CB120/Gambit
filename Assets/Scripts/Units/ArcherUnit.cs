using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherUnit : Unit
{
    private int defaultRange;
    public override void Start()
    {
        defaultRange = attackRange;
        base.Start();
    }

    public override void MoveToCell(Cell c)
    {
        switch (c.height)
        {
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


}
