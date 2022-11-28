using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Saving
{
    [System.Serializable]
    public class GameData
    {
        public int levelsCompleted;
        public string playtime = "";
        public int wins;
        public int losses;
        public int kills;
        public int deaths;
        public int skirmishHighScore;
        public int endlessHighScore;
    }
}
