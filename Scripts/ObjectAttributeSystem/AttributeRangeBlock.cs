using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChromaShift.Scripts.ObjectAttributeSystem
{
    [CreateAssetMenu(menuName = "ChromaShift/ShipAttributeRangeBlock")]
    public class AttributeRangeBlock : ScriptableObject
    {
        public float ATTRDEFAULT = 100;
        public float ATTRMIN = 0;
        public float ATTRMAX = 200;
        public float THERMMAX = 150;
        public float THERMDEFAULT = 50;
        public float THERMMIN = -50;
    }
}