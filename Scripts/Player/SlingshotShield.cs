using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;

namespace ChromaShift.Scripts.Player
{
    public class SlingshotShield: MonoBehaviour, PlayerAbilityInterface
    {
        public float BatteryChargeRequired { get; }
        public ILifeSystem PlayerLifeSystem { get; }
        private PlayerShip _playerShip;
        public GameObject Shield;
        private GameObject _slingshotShieldInstance;
        
        public delegate void AbilityLeftHandler();
        public event AbilityLeftHandler UsedLeftAbility;
        private float _timeDelay = .5f;

        [ShowInInspector]
        public float TimeDelay
        {
            get => _timeDelay;
            set
            {
                _timeDelay = value;
                
                if (_timeDelay <=0f)
                {
                    _timeDelay = .5f;
                }
            }
        }

        private void Awake()
        {
            _playerShip = GetComponent<PlayerShip>();

            _playerShip.AbilityLeft = this;
        }

        public void DoAbilityPrimary()
        {
            if (_slingshotShieldInstance != null)
            {
                return;
            }
            
            var q = new Quaternion();
            var playerPos = _playerShip.transform.position;
            
            var currentGlobalPos = transform.TransformPoint(playerPos);

            var shield = Instantiate(Shield, currentGlobalPos, q);
            _slingshotShieldInstance = shield;

            var ss = shield.GetComponent<Scripts.SlingshotShield>();
            
            ss.SetPlayerShip(ref _playerShip);
            ss.Delay(_timeDelay);
            ss.TotalMovementTime = 10f;
            UsedLeftAbility?.Invoke();
        }

        public void DoAbilitySecondary()
        {
            throw new System.NotImplementedException();
        }

        public void ExitAbilityPrimary()
        {
            throw new NotImplementedException();
        }

        public void ExitAbilitySecondary()
        {
            throw new NotImplementedException();
        }
    }
}