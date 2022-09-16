using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    [SerializeField] WorldUIPalette palette;
    [SerializeField] WorldRules rules;

    private static World Singleton;

    private void Awake () {
        Singleton = this;
    }

    public static WorldUIPalette GetPalette() {
        return Singleton.palette;
    }

    public static WorldRules GetRules() {
        return Singleton.rules;
    }
}
