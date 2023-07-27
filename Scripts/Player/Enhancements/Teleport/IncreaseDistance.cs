using System;
using BehaviorDesigner.Runtime.Tasks.Unity.Timeline;
using UnityEngine;

namespace ChromaShift.Scripts.Player.Enhancements.Teleport
{
    [RequireComponent(typeof(PlayerTeleportInterface))]
    public class IncreaseDistance:MonoBehaviour
    {
        private PlayerTeleport _playerTeleport;
        public float Increase = 1.0f;

        private void Awake()
        {
            _playerTeleport = GetComponent<PlayerTeleport>();
            _playerTeleport.Distance *= 1.0f + Increase;
        }

        private void OnDestroy()
        {
            _playerTeleport.Distance /= 1.0f + Increase;
        }
    }
}