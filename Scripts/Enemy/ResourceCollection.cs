using Sirenix.OdinInspector;
using System.Collections.Generic;

namespace ChromaShift.Scripts.Enemy
{
    [System.Serializable]
    public class ResourceCollection : SerializedScriptableObject
    {
        public List<IResourceSet> data;
    }
}