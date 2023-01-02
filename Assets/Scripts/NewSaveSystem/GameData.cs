using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Saving
{
    [System.Serializable]
    public class GameData
    {
        [Header("Stats")]
        public int levelsCompleted;
        public float playtime;
        public int wins;
        public int losses;
        public int kills;
        public int deaths;
        public int skirmishHighScore;
        public int endlessHighScore;

        [Header("Difficulty")]
        public Difficulty difficulty = Difficulty.Easy;
        public bool dynamicDifficultyEnabled;

        [Header("Unlocks")]
        public bool hasUnlockedSkirmish;
        public bool hasUnlockedEndless;
    }
}
