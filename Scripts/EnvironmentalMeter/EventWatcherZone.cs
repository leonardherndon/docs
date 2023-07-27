using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Chronos;
using Sirenix.Serialization;
using UnityEngine.Serialization;

namespace ChromaShift.Scripts.EnvironmentalMeter
{
    public class EventWatcherZone : MonoBehaviour
    {
        [SerializeField] private EnvironmentalTriggers _triggers;
        [FormerlySerializedAs("_damageBuildRate")] [Range(0.01f, 2f)][SerializeField] private float _damageMeterBuildRate = 1f;
        [FormerlySerializedAs("_velocityBuildRate")] [Range(0.0001f, 0.001f)][SerializeField] private float _velocityMeterBuildRate = 1f;
        [FormerlySerializedAs("_timeBuildRate")] [Range(0.001f, 0.1f)][SerializeField] private float _timeMeterBuildRate = 1f;
        [FormerlySerializedAs("_generalBuildRate")] [Range(0.1f, 10f)][SerializeField] private float _generalMeterBuildRate = 1f;
        [SerializeField] private AnimationCurve _sustainedTimeCurve;
        [SerializeField] private float _playerTimeInZone = 0;
        [SerializeField] private GearMoveAdvanced GMA;
        [OdinSerialize] private LifeSystemController oLS;
        [OdinSerialize] private ICollidible oCoC;
        [SerializeField] private PlayerShip PS;
        
        
        
        [SerializeField] private bool _playerOnly = true;

        public Timeline timeline;
        
        public delegate void EffectZoneHandler(float amount);
        public event EffectZoneHandler ZoneTriggered;
        
        public delegate void TimeTriggerHandler (float amount);
        public event TimeTriggerHandler TimeTriggered;
        
        private void OnTriggerEnter(Collider other)
        {
            if (_playerOnly)
            {
                PS = other.GetComponent<PlayerShip>();
                GMA = other.GetComponent<GearMoveAdvanced>();
                if(!PS) return;
            } 
            
            oCoC = other.GetComponent<ICollidible>();
            oLS = other.GetComponent<LifeSystemController>();
            
            foreach (EnvironmentalTriggers _trigger in Enum.GetValues(_triggers.GetType()))
            {
                switch (_trigger)
                {
                    case EnvironmentalTriggers.PlayerDamage:
                        oLS.LostHealth += PlayerLostHealthTrigger;
                        break;
                    case EnvironmentalTriggers.Velocity:
                        if (GMA)
                            GMA.SpeedSet += VelocityTrigger;
                        break;
                    case EnvironmentalTriggers.ShieldUse:
                        if (PS)
                            PS.shieldFixedAbility.UsedShieldAbility += DefaultTrigger;
                        break;
                    case EnvironmentalTriggers.ColorShift:
                        if (PS)
                            InputSystem.OnColorShift += ColorShiftTrigger;
                        break;
                    case EnvironmentalTriggers.LumenSwitch:
                        if (PS)
                            PS.LumenShift.UsedLumenShift += DefaultTrigger;
                        break;
                    case EnvironmentalTriggers.AbilityLeftUse:
                        if (PS)
                            PS.AbilityLeft.UsedLeftAbility += DefaultTrigger;
                        break;
                    case EnvironmentalTriggers.AbilityRightUse:
                        if (PS)
                            PS.AbilityRight.UsedRightAbility += DefaultTrigger;
                        break;
                    case EnvironmentalTriggers.ScannerUse:
                        if (PS)
                            PS.UsedScanAbility += DefaultTrigger;
                        break;
                    case EnvironmentalTriggers.PlayerSustainedTime:
                        _playerTimeInZone = 0;
                        keepCounting = true;
                        TimeTriggered += TimeTrigger;
                        StartCoroutine(CountTriggerUp(other));
                        break;
                    //Disabled for now. These triggers is supposed to go off when you kill or damage an enemy
                    //The event doesn't work since decoupling the LifeSystem
                    //TODO: Rebuild events to support the environmental triggers.
                    // case EnvironmentalTriggers.EnemyDeath:
                    //     oLS.Died += EnemyDeathTrigger;
                    //     break;
                    // case EnvironmentalTriggers.EnemyDamage:
                    //     oLS.Died += EnemyDeathTrigger;
                    //     break;
                }
            }

        }

        private IEnumerator CountTriggerUp(Collider other)
        {

            /*if (_playerOnly && !PS)
                return;*/
            while (keepCounting)
            {
                foreach (EnvironmentalTriggers _trigger in Enum.GetValues(_triggers.GetType()))
                {
                    if (_trigger == EnvironmentalTriggers.PlayerSustainedTime)
                    {
                        _playerTimeInZone += timeline.deltaTime;
                        var amount = _sustainedTimeCurve.Evaluate(_playerTimeInZone) * _timeMeterBuildRate;
                        TimeTriggered?.Invoke(amount);
                    }
                }

                yield return null;
            }
        }

        public bool keepCounting { get; set; }

        private void OnTriggerExit(Collider other)
        {

            if (_playerOnly)
            {
                PS = other.GetComponent<PlayerShip>();
                GMA = other.GetComponent<GearMoveAdvanced>();
                if(!PS) return;
            } 
            
            oCoC = other.GetComponent<ICollidible>();
            oLS = other.GetComponent<LifeSystemController>();

            foreach (EnvironmentalTriggers _trigger in Enum.GetValues(_triggers.GetType()))
            {
                switch (_trigger)
                {
                    case EnvironmentalTriggers.PlayerDamage:
                        oLS.LostHealth -= PlayerLostHealthTrigger;
                        break;
                    case EnvironmentalTriggers.Velocity:
                        if (GMA)
                            GMA.SpeedSet -= VelocityTrigger;
                        break;
                    case EnvironmentalTriggers.ShieldUse:
                        if (PS)
                            PS.shieldFixedAbility.UsedShieldAbility -= DefaultTrigger;
                        break;
                    case EnvironmentalTriggers.ColorShift:
                        if (PS)
                            InputSystem.OnColorShift -= ColorShiftTrigger;
                        break;
                    case EnvironmentalTriggers.LumenSwitch:
                        if (PS)
                            PS.LumenShift.UsedLumenShift -= DefaultTrigger;
                        break;
                    case EnvironmentalTriggers.AbilityLeftUse:
                        if (PS)
                            PS.AbilityLeft.UsedLeftAbility -= DefaultTrigger;
                        break;
                    case EnvironmentalTriggers.AbilityRightUse:
                        if (PS)
                            PS.AbilityRight.UsedRightAbility -= DefaultTrigger;
                        break;
                    case EnvironmentalTriggers.ScannerUse:
                        if (PS)
                            PS.UsedScanAbility -= DefaultTrigger;
                        break;
                    case EnvironmentalTriggers.PlayerSustainedTime:
                        _playerTimeInZone = 0;
                        keepCounting = false;
                        TimeTriggered -= TimeTrigger;
                        break;
                    //Disabled to Support the above disabling of the activations
                    // case EnvironmentalTriggers.EnemyDeath:
                    //     oLS.Died -= EnemyDeathTrigger;
                    //     break;
                    // case EnvironmentalTriggers.EnemyDamage:
                    //     oLS.Died -= EnemyDeathTrigger;
                    //     break;
                }
            }
        }

        private void PlayerLostHealthTrigger(float amount)
        {
            //if (!player) return;
            //var final = amount * _damageMeterBuildRate; 
            ZoneTriggered?.Invoke(amount);
        }

        private void VelocityTrigger(float hori, float vert)
        {
            var amount = ((hori * vert) / 2f) * _velocityMeterBuildRate;
            ZoneTriggered?.Invoke(amount);
        }
        
        private void TimeTrigger(float amount)
        {
            var final = _timeMeterBuildRate * _sustainedTimeCurve.Evaluate(amount);
           
            ZoneTriggered?.Invoke(final);
        }
        
        private void ColorShiftTrigger(GameColor amount)
        {
            ZoneTriggered?.Invoke(_generalMeterBuildRate);
        }
        
        private void EnemyDeathTrigger(int colli)
        {
            ZoneTriggered?.Invoke(_generalMeterBuildRate);
        }

        private void DefaultTrigger()
        {
            ZoneTriggered?.Invoke(_generalMeterBuildRate);
        }

    }
}
