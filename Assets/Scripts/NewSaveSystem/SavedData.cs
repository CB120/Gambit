using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;

namespace Saving{ 
    public static class SavedData
    {
        public static GameData gameData;
        public static string filePath = Application.dataPath + $"/Resources/{"GameData.txt"}";

        public static void LoadGameData()
        {
            if (File.Exists(filePath))
            {
                var json = File.ReadAllText(filePath);
                GameData result = JsonConvert.DeserializeObject<GameData>(json);
                if (result == null)
                    gameData = new GameData();
                else
                    gameData = result;
            }
            else
            {
                gameData = new GameData();
            }

            Debug.Log("Loaded Game Data");
        }

        public static void StoreGameData()
        {
            GameData data = gameData;
            string output = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText(Application.dataPath + $"/Resources/{"GameData.txt"}", output);
            
        }

        public static void CompleteLevel(int levelIndex)
        {
            Debug.Log($"Level Index: {levelIndex}, Levels Completed: {gameData.levelsCompleted}");
            if(levelIndex == gameData.levelsCompleted)
                gameData.levelsCompleted = levelIndex + 1;
            //if(levelIndex >= gameData.levelsCompleted)
            //{
            //    gameData.levelsCompleted = levelIndex;
            //    Debug.Log(gameData.levelsCompleted);
            //}
        }
    }

  
}
