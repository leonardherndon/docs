using System;
using System.Collections;
using UnityEngine;

namespace ChromaShift.Scripts.Player
{
    public class NullTeleport : MonoBehaviour, PlayerTeleportInterface
    {
        public float BatteryChargeRequired { get; private set; }
        public ILifeSystem PlayerLifeSystem { get; private set; }
        public float Distance { get; private set; }
        public float CoolDownTime { get; set; }

        public void DoAbilityPrimary()
        {
            
        }
        
        public void ExitAbilityPrimary()
        {
            
        }
        
        public void DoAbilitySecondary()
        {
            
        }
        
        public void ExitAbilitySecondary()
        {

        }
        
        public void DoTeleport(TeleportDirectionEnum direction)
        {
           return;
        }

        public IEnumerator TeleportCooldown()
        {
            yield return new WaitForSeconds(CoolDownTime);
            
        }
    }
}