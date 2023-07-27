using System.Collections;
using System.Collections.Generic;
using ChromaShift.Scripts;
using UnityEngine;

public interface ILifeSystem
{
    float CurrentHP { get; set; }
    float StartingHP { get; set; }
    float TotalHP { get; set; }
    float[] RegenRate { get; set; }
    bool IsDead { get; set; }
    
    delegate void DeathHandler();
    delegate void LostHealthHandler(float amount = 0);
       
    void InitLifeSystem();
    void ResetLifeSystem();
    void RestoreLife(float amount);
    void SpendLife(float amount);
    void DrainLife(float amount, ImpactType impactType, bool isPlayerShip = false);
    IEnumerator DrainLifeDOT(float amount, int limit, ImpactType impactType, float tickRate = 1f);
    
    IEnumerator DrainLifeDelayed(float amount, int limit, ImpactType impactType, float delayTime = 1f);

    void ReceiveDamage(ImpactType impactType, List<DamageDataBlock> damageBlockList = null, ICollidible otherCoC = null,
        DamageSeverity severity = DamageSeverity.Normal);
    
    
    void ReceiveVelocityDamage(Collision other, ImpactType impactType, List<DamageDataBlock> damageBlockList = null, ICollidible otherCoC = null, float VelocityMultiplier = 0, DamageSeverity severity = DamageSeverity.Normal);
    void KillSelf();
    

}
