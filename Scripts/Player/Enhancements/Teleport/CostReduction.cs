using UnityEngine;

namespace ChromaShift.Scripts.Player.Enhancements.Teleport
{
    [RequireComponent(typeof(PlayerTeleportInterface))]
    public class CostReduction : MonoBehaviour
    {
        private PlayerTeleport _teleport;
        public float Modifier = .90f;
        private void Awake()
        {
            var _teleport = GetComponent<PlayerTeleport>();
            _teleport.BatteryChargeRequired *= Modifier;
        }

        private void OnDestroy()
        {
            _teleport.BatteryChargeRequired /= Modifier;
        }
    }
}