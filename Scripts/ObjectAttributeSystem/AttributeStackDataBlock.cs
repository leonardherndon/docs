using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttributeStackDataBlock : MonoBehaviour
{
    public class AttributeDataBlock : ScriptableObject
    {
        public AttributeType type;
        public List<Vector3> value;
    }
}
