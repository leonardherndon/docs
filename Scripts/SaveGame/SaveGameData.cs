using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChromaShift.Scripts.SaveGame
{
    public class SaveGameData : ScriptableObject
    {
        public int ID;
        public string currentLevelID;
        public int currentSceneIndex;
        public int currentCheckPointIndex;
        public int currentDeathCount;
        public int currentTimePlayed;

    }
}