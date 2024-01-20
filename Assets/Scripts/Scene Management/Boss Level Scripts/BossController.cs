using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : MonoBehaviour
{
    public int remainingUnits;
    [SerializeField] private Unit kingUnit;

    public void activateKing(int remainingUnits, List<Unit> units) {
        if (remainingUnits <= 2 && kingUnit != null)
        {
            units.Add(kingUnit);
            Debug.Log("We did this");
            kingUnit.enabled = true;
            //kingUnit.GetComponent<KingUnit>().
            kingUnit = null;
            this.enabled = false;
        }
    }
}
