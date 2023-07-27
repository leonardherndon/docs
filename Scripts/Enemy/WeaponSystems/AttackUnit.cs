using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChromaShift.Enemy.WeaponSystems
{

    public struct AttackUnit
    {
        public string unitDescription;
        public int armamentIndex;
        public int deployPointIndex;
        public GameColor gameColor;
        //public float startupDelay;
        public float cooldownDelay;
        public bool localToParent;
    }
}