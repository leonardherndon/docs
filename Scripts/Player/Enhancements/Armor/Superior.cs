using System;
using UnityEngine;

namespace ChromaShift.Scripts.Player.Enhancements.Armor
{
    public class Superior : MonoBehaviour
    {
        private Scripts.Armor _armor;
        public float Modifier = 50f;

        private void Awake()
        {
            _armor = GetComponent<Scripts.Armor>();
            _armor.HullIntegrityAdjustment += Modifier;
        }

        private void OnDestroy()
        {
            _armor.HullIntegrityAdjustment -= Modifier;
        }
    }
}