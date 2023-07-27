using UnityEngine;

namespace ChromaShift.Scripts.Player.Enhancements.Armor
{
    public class Lightwight : MonoBehaviour
    {
        private ChromaShift.Scripts.Armor _armor;
        public float Modifier = 25f;
        private void Awake()
        {
            var _armor = GetComponent<Scripts.Armor>();
            _armor.ThermalIntegrityAdjustment -= Modifier;
        }

        private void OnDestroy()
        {
            _armor.ThermalIntegrityAdjustment += Modifier;
        }
    }
}