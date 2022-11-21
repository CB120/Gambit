using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Saving;

public class TestGameData : MonoBehaviour
{
    #region Testing
    public GameData gameData = new();

    private void Start()
    {
        gameData = SavedData.gameData;
    }

    public void Load()
    {
        SavedData.LoadGameData();
    }

    public void Store()
    {
        SavedData.StoreGameData();
    }
    #endregion
}
