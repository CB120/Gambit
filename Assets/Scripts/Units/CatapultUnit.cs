using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatapultUnit : Unit
{
    //public override void Start()
    //{
    //    base.Start(); 
    //}

    // Update is called once per frame
    public override void DestroyUnit()
    {
        UIManager.enableGameResultState(true);
        base.DestroyUnit();
    }
}
