using System;
using System.Collections;
using System.Collections.Generic;
using ChromaShift.Scripts.Player;
using UnityEngine;

namespace ChromaShift.Scripts
{
    public delegate void OnEventLumenTransferLight();
    public delegate void OnEventLumenTransferDark();
    
    public delegate void OnEventLostRedCore();
    public delegate void OnEventLostGreenCore();
    public delegate void OnEventLostBlueCore();
    public delegate void OnEventGainedRedCore();
    public delegate void OnEventGainedGreenCore();
    public delegate void OnEventGainedBlueCore();
    public delegate void OnEventGainedCore();
    public delegate void OnEventLostCore();
 
    
    public interface ICollidible
    {
        int CollisionObjectId { get; set; }
        Collider ObjectCollider { get; set; }
        ChromaShiftManager CSM { get; set; }
        List<DamageDataBlock> DamageDataBlocks { get; set; }
        List<StatusEffectDataBlock> StatusEffectDataBlocks { get; set; }

        ImpactType ImpactType { get; set; }
        List<DamageRequest> DamageRequests { get; set; }
        List<StatusEffectRequest> StatusEffectRequests { get; set; }
        ILifeSystem LifeSystem { get; set; }
        
        ThreatGroup ThreatGroup { get; set; }
        FriendlyType FriendlyType { get; set; }
        DamageGroup DamageGroup { get; set; }
        ColorMatchType CollisionColorMatchType { get; set; }
        GameObject GObject { get; set; }
        bool DoesVelocityDamage { get; set; }
        bool IsPlayerShip { get; set; }
        bool IsLightNode { get; set; }
        void InitCollisionController();
        void CalcCollision(ICollidible colliderObject, Collision otherCollision = null);
        void HandleCollision(GameObject other, List<DamageDataBlock> dA = null, List<StatusEffectDataBlock> sEA = null);
        void ReceiveStatusEffect(List<StatusEffectDataBlock> StatusBlockList, ICollidible otherCoC, string sourceName = null);
        bool CompareStatusEffectsToList(StatusEffectRequest newRequest);
        bool CompareStatusEffectsToList(StatusEffectDataBlock block);
        event OnEventLostRedCore OnLostRedCore;
        event OnEventLostGreenCore OnLostGreenCore;
        event OnEventLostBlueCore OnLostBlueCore;
        event OnEventGainedRedCore OnGainedRedCore;
        event OnEventGainedGreenCore OnGainedGreenCore;
        event OnEventGainedBlueCore OnGainedBlueCore;
        
        delegate void DamageHandler(ImpactType impactType, List<DamageDataBlock> damageBlockList = null, ICollidible otherCoC = null, DamageSeverity severity = DamageSeverity.Normal);
        event DamageHandler GotDamaged;

        delegate void VelocityDamageHandler(Collision other, ImpactType impactType, List<DamageDataBlock> damageBlockList = null, ICollidible otherCoC = null, float VelocityMultiplier = 0, DamageSeverity severity = DamageSeverity.Normal);
        event VelocityDamageHandler GotVelocityDamaged;
        delegate void WreckedHandler();
        
        event WreckedHandler Wrecked;
        public void GetWrecked();
    }
    
}