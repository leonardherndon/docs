using System.Collections;
using System.Collections.Generic;
using ChromaShift.Scripts;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "ChromaShift/DataBlocks/CollisionControllerData")]
public class CollisionControllerData : SerializedScriptableObject 
{
    public bool isPlayerShip;
    public bool isLightNode;
    public bool doesNotReceiveCollisionDamage = false;
    public bool doesVelocityDamage = false;
    
    [Header("COLOR")]
    public ColorMatchType collisionColorMatchType;
    
    [Header ("THREAT")]
    public ObjectTag _objectTag;
    public ThreatGroup threatGroup;
    public FriendlyType friendlyType;
    
    [Header ("DAMAGE")]
    public Vector2 _velocityDeathThreshold = new Vector2(25f,35f);
    public DamageGroup damageGroup;
    public ImpactType impactType;
    public DamageImmunityType damageImmunityType;
    public List<DamageDataBlock> damageDataBlocks;
    
    [Header ("STATUS EFFECTS")]
    public bool statusEffectImmunityAll = false;
    public StatusEffectType statusEffectImmunityType;
    public List<StatusEffectDataBlock> statusEffectBlocks;

    

}
