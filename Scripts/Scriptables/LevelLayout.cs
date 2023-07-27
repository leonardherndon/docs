using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ChromaShift.Scripts.Scriptables
{
    public class LevelLayout : ScriptableObject
    {
        private Dictionary<string, List<BaseObstacle>> layers = new Dictionary<string, List<BaseObstacle>>();

        public Dictionary<string, List<BaseObstacle>> Layers
        {
            get { return layers; }
            set { layers = value; }
        }
    }
}