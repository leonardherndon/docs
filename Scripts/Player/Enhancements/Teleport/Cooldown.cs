using System;
using BehaviorDesigner.Runtime.Tasks.Unity.Timeline;
using Language.Lua;
using UnityEngine;

namespace ChromaShift.Scripts.Player.Enhancements.Teleport
{
    [RequireComponent(typeof(PlayerTeleportInterface))]
    public class Cooldown : MonoBehaviour
    {
        private PlayerTeleport _teleport;
        public float Modifier = .90f;
        private void Awake()
        {
            var _teleport = GetComponent<PlayerTeleport>();
            _teleport.CoolDownTime *= Modifier;
        }

        private void OnDestroy()
        {
            _teleport.CoolDownTime /= Modifier;
        }
    }
}