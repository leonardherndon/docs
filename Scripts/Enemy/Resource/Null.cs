using System;

namespace ChromaShift.Scripts.Enemy.Resource
{   
    public class Null : IResource
    {
        public GameColor GetColor()
        {
            return GameColor.Grey;
        }

        public string GetPrefabPath()
        {
            return string.Empty;
        }

        public bool IsAttached()
        {
            return false;
        }
    }
}