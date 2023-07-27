using UnityEngine;

namespace ChromaShift.Scripts.Scriptables
{
    public class BaseObstacle : ScriptableObject
    {
        private Vector3 _position;
        private Vector3 _y;
        private float _z;
        private string _type;

        public string Type
        {
            get { return _type; }
            set { _type = value; }
        }
    }
}