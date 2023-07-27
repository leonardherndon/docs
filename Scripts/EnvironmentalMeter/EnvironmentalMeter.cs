using System;
using System.Collections.Generic;
using System.Linq;
using ChromaShift.Scripts.Managers;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace ChromaShift.Scripts.EnvironmentalMeter
{
    public class EnvironmentalMeter : MonoBehaviour
    {
        [SerializeField] private bool _cooldownActive;
        [SerializeField] private List<UnityEvent> _unityEvents;
        [SerializeField] private List<EventWatcherZone> _zones;
        [SerializeField] private float _currentValue;
        [FormerlySerializedAs("_meterValues")] [SerializeField] private Vector2 _meterMinMax = new Vector2(0,100f);
        [SerializeField] private float _cooldownRate;
        [SerializeField] private bool delayCooldown;
        [SerializeField] private int _delayLoop = 0;
        [SerializeField] private int delay = 0;
        
        
        
        void Awake()
        {
            _zones = GameObject.FindObjectsOfType<EventWatcherZone>().ToList();

            foreach (EventWatcherZone zone in _zones)
            {
                zone.ZoneTriggered += AddToMeter;
            }
        }

        private void OnDisable()
        {
            foreach (EventWatcherZone zone in _zones)
            {
                zone.ZoneTriggered -= AddToMeter;
            }
        }

        private void FixedUpdate()
        {
            if(_cooldownActive)
                CoolDownMeter();
        }
        
        public void CoolDownMeter()
        {
            if(GameStateManager.Instance.CurrentState.StateType != GameStateType.Gameplay)
                return;

            if (
                
                delayCooldown)
            {

                if (_delayLoop < delay)
                {
                    _delayLoop += 1;
                }
                else
                {
                    
                    _delayLoop = 0;
                }

            }
            
            if (_cooldownRate < 0)
            {
                //Debug.LogWarning("Value needs to be positive. Please fix");
                return;
            }
    
            _currentValue -= _cooldownRate;
            _currentValue = Mathf.Clamp(_currentValue, _meterMinMax.x, _meterMinMax.y);
            
            //Min
            if (_currentValue == _meterMinMax.x)
                _unityEvents[0]?.Invoke();
        }
        
        public void AddToMeter(float amount)
        {
            if(GameStateManager.Instance.CurrentState.StateType != GameStateType.Gameplay)
                return;
		
            if (amount < 0)
            {
                //Debug.LogWarning("You can only add HP not remove. Please try UseLife or DrainLife");
                return;
            }
    
            _currentValue += amount;
    

            _currentValue = Mathf.Clamp(_currentValue, _meterMinMax.x, _meterMinMax.y);
            

            //Max
            if (_currentValue == _meterMinMax.y)
                _unityEvents[1]?.Invoke();
        }

        /*public void TestDebugMin()
        {
            Debug.Log("Min Hit");
        }
        public void TestDebugMax()
        {
            Debug.Log("Max Hit");
        }*/
    }
}
