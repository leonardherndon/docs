using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ChromaShift.Scripts.Player.Upgrade
{
    public class Teleport : MonoBehaviour, IUpgrade<float>
    {
        [ShowInInspector] private float _modifier;
        [ShowInInspector] private float _originalDistance;
        private PlayerShip _playerShip;

        public float Modifier
        {
            get => (float) (_modifier - 1.0);
            set
            {
                ResetModifier();
                _modifier = (float) 1.0 + value;
                _playerShip.teleportAbility.teleportDistance *= _modifier;
            }
        }

        public void Awake()
        {
            _playerShip = GetComponent<PlayerShip>();
            
            _originalDistance = _playerShip.teleportAbility.teleportDistance;
        }
        
        public void Start()
        {
            ResetModifier();
        }

        public void OnDestroy()
        {
            ResetModifier();
        }

        private void ResetModifier()
        {
            if (Math.Abs(_modifier) < 0.00001)
            {
                return;
            }
            
            if (Math.Abs(_modifier - 1.0) < 0.00001)
            {
                _playerShip.teleportAbility.teleportDistance = _originalDistance;
                return;
            }
            
            _playerShip.teleportAbility.teleportDistance /= _modifier;    
        }
        public void Remove()
        {
            Destroy(this);
        }
    }
}