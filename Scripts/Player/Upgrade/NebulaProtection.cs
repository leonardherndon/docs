using System;
using UnityEngine;

namespace ChromaShift.Scripts.Player.Upgrade
{
    public class NebulaProtection: MonoBehaviour, IUpgrade<float>
    {
        private PlayerShip _playerShip;
        public bool isProtectedFromNebula;
        public float Modifier { get; set; }
        public void Awake()
        {
            _playerShip = GetComponent<PlayerShip>();
        }

        private void Start()
        {
            isProtectedFromNebula = true;
        }

        public void OnDestroy()
        {
            isProtectedFromNebula = false;
        }
        public void Remove()
        {
            Destroy(this);
        }
    }
}