using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;

namespace Saving{ 
    public static class SavedData
    {
        //Holds The JSON Data Through Runtime
        private static GameData gameData;
        //Accessor For Editor Purposes (Used for testing without going through the menu)
        public static GameData GameData { 
            get
            {
                if(gameData != null)
                    return gameData;

                return LoadGameData();
            }
            set
            {
                gameData = value;
            }
        }
        //Filepath that the JSON File Is Located
        public static string filePath = Application.dataPath + $"/Resources/{"GameData.txt"}";

        //Loads JSON Data if it Exists
        public static GameData LoadGameData()
        {
            GameData data = new GameData();
            if (File.Exists(filePath))
            {
                var json = File.ReadAllText(filePath);
                GameData result = JsonConvert.DeserializeObject<GameData>(json);
                if (result != null)
                    data = result;
            }

            gameData = data;
            return data;
        }

        //Stores Game Data When the Application is Exited (Will Only Be Updated if You Go Through the Main Menu)
        public static void StoreGameData()
        {
            GameData data = gameData;
            string output = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText(Application.dataPath + $"/Resources/{"GameData.txt"}", output);
            
        }

        //When A Level Is Completed
        public static void CompleteLevel(int levelIndex)
        {
            Debug.Log($"Level Index: {levelIndex}, Levels Completed: {gameData.levelsCompleted}");
            if(levelIndex == gameData.levelsCompleted)
                gameData.levelsCompleted = levelIndex + 1;
        }
    }

  
}
