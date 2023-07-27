using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Serialization;

namespace ChromaShift.Scripts.ObjectAttributeSystem
{
    [CreateAssetMenu(menuName = "ChromaShift/DataBlocks/AttributeMod")]
    public class AttributeModDataBlock : ScriptableObject
    {
        public RequestOriginType originType;
        public AttributeType attrType;
        public bool isActive = true;
        public bool notStackable = false;
        public float strength;
        public StatusEffectRequest sourceObject;
        public int sourceRequestId;

    }
}

