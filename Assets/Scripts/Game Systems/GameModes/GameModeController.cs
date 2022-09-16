using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameModeController : MonoBehaviour
{
    [SerializeField] private GameMode localMode;

    public void Start()
    {
        GameManager.gameMode = localMode;
    }

}
